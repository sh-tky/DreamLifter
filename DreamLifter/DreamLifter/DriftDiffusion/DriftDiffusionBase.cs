using System;
using System.Linq;
using Independence;

namespace DreamLifter.DriftDiffusion
{

    /// <summary>
    /// Drift Diffusion 方程式ソルバの実装です。
    /// </summary>
    public abstract class DriftDiffusionBase
    {

        #region FIELDS
        /// <summary>
        /// Represent FEM matrices.
        /// </summary>
        protected DoubleSparseMatrix _A, _K, _D, _N, _M, _S;

        /// <summary>
        /// Represent Poisson equation solver.
        /// </summary>
        protected PoissonBase _poisson;
        
        /// <summary>
        /// 粒子を管理します。
        /// </summary>
        protected ParticleManager _manager;
        
        /// <summary>
        /// 現在のガス種類です。
        /// </summary>
        protected IGasComponent _gas;

        #endregion

        #region ABSTRACT METHODS

        protected void AssembleBoundaryIntegralMatrix(DoubleSparseMatrix K, Func<Axis, DoubleDenseMatrix> vhandle)
        {
            BoundaryMatrixHelper(K, vhandle);
        }

        /// <summary>
        /// フラックスに関する境界条件を埋め込みます。
        /// </summary>
        protected abstract void BoundaryMatrixHelper(DoubleSparseMatrix K, Func<Axis, DoubleDenseMatrix> vhandle);

        /// <summary>
        /// 移流行列を計算します。
        /// </summary>
        protected abstract void AssembleAdvectionMatrix(DoubleSparseMatrix A, Func<Axis, DoubleDenseMatrix> vhandle, DoubleDenseMatrix diffusion);

        /// <summary>
        /// 二次電子放出に関する境界積分マトリクスを構築します。
        /// </summary>
        protected abstract void AssembleBoundaryIntegralSecondaryEmission(DoubleSparseMatrix K2, Func<Axis, DoubleDenseMatrix> vhandle);

        /// <summary>
        /// 拡散行列を返します。
        /// </summary>
        protected abstract void AssembleDiffusionMatrix(DoubleSparseMatrix D, Func<Axis, DoubleDenseMatrix> vhandle, DoubleDenseMatrix diffusivity);

        /// <summary>
        /// 生成行列を返します。
        /// </summary>
        protected abstract void AssembleSourceMatrix(DoubleSparseMatrix S, Func<Axis, DoubleDenseMatrix> vhandle, DoubleDenseMatrix diffusion, DoubleDenseMatrix source);

        /// <summary>
        /// 質量行列を返します。
        /// </summary>
        /// <param name="M"></param>
        protected abstract void AssembleMassMatrix(DoubleSparseMatrix M, Func<Axis, DoubleDenseMatrix> vhandle, DoubleDenseMatrix diffusion);

        #endregion

        /// <summary>
        /// ドリフト拡散反応方程式ソルバの基底クラスを初期化します。
        /// </summary>
        protected DriftDiffusionBase(int n, int capacity)
        {
            _A = new DoubleSparseMatrix(n, n, capacity);
            _K = new DoubleSparseMatrix(n, n, capacity);
            _S = new DoubleSparseMatrix(n, n, capacity);
            _D = new DoubleSparseMatrix(n, n, capacity);
            _M = new DoubleSparseMatrix(n, n, capacity);
            _N = new DoubleSparseMatrix(n, n, capacity);
        }

        /// <summary>
        /// 質量行列を返します。
        /// </summary>
        public DoubleSparseMatrix GetM()
        {
            var n = _fem.NumberOfNodes;
            var m = _fem.MaxNodesPerRow;
            var M = new DoubleSparseMatrix(2 * n, 2 * m);
            var Mg = new DoubleSparseMatrix(n, m);
            var e = _manager.GetParticle(Species.Electron);
            AssembleMassMatrix(Mg, d => GetVelocity(e, d), e.GetDiffusion());
            M.Insert(Mg, 0 * n, 0 * n);
            Mg.Clear();
            var p = p_manager.GetParticle(Species.Positive);
            AssembleMassMatrix(Mg, d => GetVelocity(p, d), p.GetDiffusion());
            M.Insert(Mg, 1 * n, 1 * n);
            return M;
        }

        /// <summary>
        /// グローバル行列を組み立てます。
        /// </summary>
        private DoubleSparseMatrix Assembler(Particle part)
        {
            _S.Clear();
            _K.Clear();
            _A.Clear();
            _D.Clear();
            Func<Axis, DoubleDenseMatrix> func = d => GetVelocity(part, d);
            var elements = _fem.Elements;
            var diff = part.GetDiffusion();
            AssembleDiffusionMatrix(_D, func, diff);
            AssembleAdvectionMatrix(_A, func, diff);
            AssembleBoundaryIntegralMatrix(_K, func);
            AssembleSourceMatrix(_S, func, diff, part.GetSourceDerivative(part.Spcs) - NablaVelocityFactory(part));
            var J = -_D + _S - _A + _K;
            return J;
        }

        /// <summary>
        /// ヤコビ行列を返します。
        /// </summary>
        /// <returns></returns>
        public DoubleSparseMatrix GetA()
        {
            var sizeOfBlock = _fem.NumberOfNodes;
            var capacity = _fem.MaxNodesPerRow;
            const int numberOfBlock = 2;
            var J = new DoubleSparseMatrix(numberOfBlock * sizeOfBlock, numberOfBlock * sizeOfBlock, numberOfBlock * capacity);
            var electron = _manager.GetParticle(Species.Electron);
            var positive = _manager.GetParticle(Species.Positive);
            var elements = _fem.Elements;

            // J11...
            var J11 = Assembler(electron);
            var rhs = _manager.GetRHSPoissonEqn();
            J.Insert(J11, 0 * sizeOfBlock, 0 * sizeOfBlock);
            // J12
            var J12r = new DoubleSparseMatrix(sizeOfBlock, sizeOfBlock, capacity);
            var J12s = new DoubleSparseMatrix(sizeOfBlock, sizeOfBlock, capacity);
            AssembleBoundaryIntegralSecondaryEmission(J12r, d => GetVelocity(positive, d));
            J.Insert(J12s - J12r, 0 * sizeOfBlock, 1 * sizeOfBlock);
            // J21
            var J21s = new DoubleSparseMatrix(sizeOfBlock, capacity);
            AssembleSourceMatrix(J21s, d => GetVelocity(positive, d), positive.GetDiffusion(), positive.GetSourceDerivative(Species.Electron));
            var J21r = new DoubleSparseMatrix(sizeOfBlock, capacity);
            J.Insert(J21s - J21r, 1 * sizeOfBlock, 0 * sizeOfBlock);
            // J22
            var J22 = Assembler(positive);
            J.Insert(J22, 1 * sizeOfBlock, 1 * sizeOfBlock);
            return J;
        }

        /// <summary>
        /// \nabla \cdot \bm{v} を計算します。
        /// </summary>
        /// <param name="part">粒子です。</param>
        /// <returns></returns>
        protected DoubleDenseMatrix NablaVelocityFactory(Particle part)
        {
            //var res = part.GetMobility();
            return part.GetNablaVelocity(part.GetVelocity);
        }
    }
}
