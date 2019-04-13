using Independence;
using System;

namespace DreamLifter.FDM.TwoDimension
{

    /// <summary>
    /// Matrix assembler with finite difference scheme.
    /// </summary>
    /// <remarks>
    /// This class is specialized for uniform grids. 
    /// </remarks>
    public sealed class FDMUniformMatrixAssembler : IMatrixAssembler
    {
        private readonly double _delta;
        private readonly int _nx, _ny;

        /// <summary>
        /// Storage for phase information; if fluid then 1, and if solid then 0. 
        /// </summary>
        private readonly Int32DenseMatrix _phase;

        public FDMUniformMatrixAssembler(int nx, int ny, double delta, Int32DenseMatrix phase)
        {
            _nx = nx;
            _ny = ny;
            _delta = delta;
            _phase = phase;
        }

        public DoubleSparseMatrix GetAdvectionMatrix(Func<DoubleDenseMatrix, Axis> velocityHandler)
        {
            throw new NotImplementedException();
        }

        public DoubleSparseMatrix GetDifferentialMatrix(Axis axis)
        {
            throw new NotImplementedException();
        }

        public DoubleSparseMatrix GetMassMatrix()
        {
            throw new NotImplementedException();
        }

        public DoubleSparseMatrix GetStiffnessMatrix(DoubleDenseMatrix diffusivity)
        {
            const int capacitySecondOrder = 5;
            var yield = new DoubleSparseMatrix(_nx * _ny, _nx * _ny, capacitySecondOrder);
            {
                var i = 0;
                // grad = 0.
                {
                    var j = 0;
                    var index = i * _ny + j;
                    yield[index, index + 1] = 1.0;
                    yield[index, index + _ny] = 1.0;
                    yield[index, index] = -2.0;
                }
                for (var j = 1; j < _ny - 1; j++)
                {
                    var index = i * _ny + j;
                    yield[index, index - _ny] = 1.0;
                    yield[index, index + 1] = 1.0;
                    yield[index, index + _ny] = 1.0;
                    yield[index, index] = -3.0;
                }
                {
                    var j = _ny - 1;
                    var index = i * _ny + j;
                    yield[index, index + 1] = 1.0;
                    yield[index, index - _ny] = 1.0;
                    yield[index, index] = -2.0;
                }
            }
            for (var i = 1; i < _nx - 1; i++)
            {
                {
                    var j = 0;
                    // grad = 0.
                    var index = i * _ny + j;
                    yield[index, index - 1] = 1.0;
                    yield[index, index + 1] = 1.0;
                    yield[index, index + _ny] = 1.0;
                    yield[index, index] = -3.0;
                }
                for (var j = 1; j < _ny - 1; j++)
                {
                    var phaseC = _phase[i, j];
                    var phaseE = _phase[i - 1, j];
                    var phaseW = _phase[i + 1, j];
                    var phaseS = _phase[i, j - 1];
                    var phaseN = _phase[i, j + 1];
                    var index = i * _ny + j;
                    yield[index, index - _ny] = phaseS;
                    yield[index, index - 1] = phaseE;
                    yield[index, index + 1] = phaseW;
                    yield[index, index + _ny] = phaseN;
                    var coefS = phaseS == 1 ? 1.0 : 2.0;
                    var coefE = phaseE == 1 ? 1.0 : 2.0;
                    var coefW = phaseW == 1 ? 1.0 : 2.0;
                    var coefN = phaseN == 1 ? 1.0 : 2.0;
                    yield[index, index] = -(coefS + coefE + coefW + coefN);
                }
                {
                    var j = _ny - 1;
                    // grad = 0.
                    var index = i * _ny + j;
                    yield[index, index - 1] = 1.0;
                    yield[index, index + 1] = 1.0;
                    yield[index, index - _ny] = 1.0;
                    yield[index, index] = -3.0;
                }
            }
            {
                var i = _nx - 1;
                // grad = 0.
                {
                    var j = 0;
                    var index = i * _ny + j;
                    yield[index, index - 1] = 1.0;
                    yield[index, index + _ny] = 1.0;
                    yield[index, index] = -2.0;
                }
                for (var j = 1; j < _ny - 1; j++)
                {
                    var index = i * _ny + j;
                    yield[index, index - _ny] = 1.0;
                    yield[index, index - 1] = 1.0;
                    yield[index, index + _ny] = 1.0;
                    yield[index, index] = -3.0;
                }
                {
                    var j = _ny - 1;
                    var index = i * _ny + j;
                    yield[index, index - 1] = 1.0;
                    yield[index, index - _ny] = 1.0;
                    yield[index, index] = -2.0;
                }
            }
            return yield * (1.0 / (_delta * _delta));
        }
    }
}
