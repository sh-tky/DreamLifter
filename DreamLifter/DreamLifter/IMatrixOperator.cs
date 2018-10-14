using Independence;

namespace DreamLifter
{
    /// <summary>
    /// 離散化された行列演算子を公開します。
    /// </summary>
    public interface IMatrixOperator
    {
        /// <summary>
        /// 質量行列を取得します。
        /// </summary>
        /// <returns></returns>
        DoubleSparseMatrix MassMatrix();
       
        /// <summary>
        /// 剛性行列を取得します。
        /// </summary>
        /// <param name="diffusivity">拡散係数</param>
        /// <returns></returns>
        DoubleSparseMatrix StiffnessMatrix(DoubleDenseMatrix diffusivity);

        /// <summary>
        /// 微分行列を取得します。
        /// </summary>
        /// <param name="axis">微分軸</param>
        /// <returns></returns>
        DoubleSparseMatrix DerivativeMatrix(Axis axis);

    }
}
