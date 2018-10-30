using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Independence;

namespace DreamLifter.FR.TwoDimension
{
    /// <summary>
    /// Matrix assembler with flux reconstruction using kinetic energy preservation scheme.
    /// </summary>
    /// <remarks>
    /// This class is specialized for uniform grids. 
    /// </remarks>
    public sealed class FRUniformMatrixAssembler : IMatrixAssembler
    {
        private readonly int _numberOfElements;
        private readonly int _numberOfSolutionPoints;
        private readonly MatrixProvider _matrixProvider;
        private readonly DoubleDenseMatrix _dcoef;

        public FRUniformMatrixAssembler(int numberOfElements, int polynomialOrder)
        {
            _numberOfElements = numberOfElements;
            _numberOfSolutionPoints = polynomialOrder + 1;
            _matrixProvider = new MatrixProvider(polynomialOrder);
            _dcoef = _matrixProvider.GetLocalAdvectionMatrix();
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
            throw new NotImplementedException();
        }
    }
}
