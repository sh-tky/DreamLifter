using Independence;
using Independence.Ex;

namespace DreamLifter.FR.Tests
{
    /// <summary>
    /// Implementation of a solver for 1D advection equation.
    /// </summary>
    public sealed class Advection1D
    {
        private readonly DoubleDenseMatrix _dcoef;
        private readonly DoubleDenseMatrix _correctionFuncLeft;
        private readonly DoubleDenseMatrix _correctionFuncRight;
        private readonly int _numberOfElements;
        private readonly int _numberOfSolutionPoints;

        public Advection1D(MatrixProvider provider, int numberOfElements, int numberOfSolutionPoints)
        {
            _dcoef = provider.GetLocalAdvectionMatrix();
            _correctionFuncLeft = provider.GetLeftCorrectionFunc();
            _correctionFuncRight = provider.GetRightCorrectionFunc();
            _numberOfElements = numberOfElements;
            _numberOfSolutionPoints = numberOfSolutionPoints;
        }

        public DoubleDenseMatrix EvaluateRHS(DoubleDenseMatrix u)
        {
            var yield = new DoubleDenseMatrix(_numberOfSolutionPoints, _numberOfElements);
            for (var k = 0; k < _numberOfElements; k++)
            {
                // compute discontinuous derivative.
                var dfdx = _dcoef * u.SubMatrix(0, k, _numberOfSolutionPoints - 1, k);
                {
                    // left flux correction procedure.
                    var minusElementIndex = k == 0 ? _numberOfElements - 1 : k - 1;
                    var commonFluxLeftBoundary = 0.5 * (u[_numberOfSolutionPoints - 1, minusElementIndex] + u[0, k]);
                    var corrctionFluxLeft = commonFluxLeftBoundary - u[0, k];
                    for (var i = 0; i < _numberOfSolutionPoints; i++)
                    {
                        dfdx[i, 0] += _correctionFuncLeft[i, 0] * corrctionFluxLeft;
                    }
                }
                {
                    // right flux correction procedure.
                    var plusElementIndex = k == _numberOfElements - 1 ? 0 : k + 1;
                    var commonFluxRightBoundary = 0.5 * (u[0, plusElementIndex] + u[_numberOfSolutionPoints - 1, k]);
                    var corrctionFluxRight = commonFluxRightBoundary - u[_numberOfSolutionPoints - 1, k];
                    for (var i = 0; i < _numberOfSolutionPoints; i++)
                    {
                        dfdx[i, 0] += _correctionFuncRight[i, 0] * corrctionFluxRight;
                    }
                }
                yield.SubMatrix(dfdx, 0, k, _numberOfSolutionPoints - 1, k);
            }
            return -yield;
        }
    }
}
