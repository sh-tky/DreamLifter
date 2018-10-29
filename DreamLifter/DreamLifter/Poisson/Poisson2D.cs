using Independence;
using System;
using System.Collections.Generic;

namespace DreamLifter.Poisson
{

    /// <summary>
    /// A solver for the Poisson equation.
    /// </summary>
    public sealed class Poisson2D : IPoissonSolver
    {
        /// <summary>
        /// An assembler for matrices.
        /// </summary>
        private readonly IMatrixAssembler _assembler = null;

        private readonly ILinearSolver _solverJ;

        private readonly ILinearSolver _solverM;

        private readonly IBoundaryOperable _boundary;

        private Dictionary<string, double> _potentials = new Dictionary<string, double>();

        /// <summary>
        /// Mass matrix.
        /// </summary>
        private DoubleSparseMatrix _M = null;

        /// <summary>
        /// An operator matrix to derive the electric field in X-direction from the scalar potential.
        /// </summary>
        private DoubleSparseMatrix _HxT = null;

        /// <summary>
        /// An operator matrix to derive the electric field in Y-direction from the scalar potential.
        /// </summary>
        private DoubleSparseMatrix _HyT = null;

        /// <summary>
        /// Electric scalar potential.
        /// </summary>
        private DoubleDenseMatrix _phi = null;

        /// <summary>
        /// The right hand side vector.
        /// </summary>
        private DoubleDenseMatrix _rhs = null;

        /// <summary>
        /// Relative permittivity.
        /// </summary>
        private DoubleDenseMatrix _epsilon_r = null;

        public Poisson2D(IMatrixAssembler assembler, IBoundaryOperable boundary, ILinearSolver solverForMassMatrix, ILinearSolver solverForStiffnessMatrix)
        {
            _assembler = assembler;
            _boundary = boundary;
            _solverM = solverForMassMatrix;
            _solverJ = solverForStiffnessMatrix;
        }

        public void Solve()
        {
            if (_solverJ == null)
            {
                var A = _assembler.GetStiffnessMatrix(_epsilon_r);
                foreach (var boundary in _potentials)
                {
                    _boundary.ImposeFirstKindBoundaryCondition(A, boundary.Key);
                }
                _solverJ.SetMatrix(A);
            }
            if (_solverM == null)
            {
                _M = _assembler.GetMassMatrix();
                _solverM.SetMatrix(_M);
            }
            var rhsVector = _M * _rhs;
            foreach (var boundary in _potentials)
            {
                _boundary.ImposeFirstKindBoundaryCondition(rhsVector, boundary.Key, boundary.Value);
            }
            _phi = _solverJ.Solve(rhsVector);
        }

        public DoubleDenseMatrix Solution
        {
            get { return _phi; }
        }

        public DoubleDenseMatrix GetVectorField(Axis axis)
        {
            if (_HxT == null)
            {
                _HxT = _assembler.GetDifferentialMatrix(Axis.X);
            }
            if (_HyT == null)
            {
                _HyT = _assembler.GetDifferentialMatrix(Axis.Y);
            }
            switch (axis)
            {
                case Axis.X:
                    return -(_solverM.Solve(_HxT * _phi));
                case Axis.Y:
                    return -(_solverM.Solve(_HyT * _phi));
                default:
                    throw new ArgumentOutOfRangeException("axis");
            }
        }

        public void SetPotential(string boundaryName, double value)
        {
            if (_potentials.ContainsKey(boundaryName))
            {
                _potentials[boundaryName] = value;
            }
            else
            {
                _potentials.Add(boundaryName, value);
            }
        }

        public void SetGradient(string boundaryName, double value)
        {
            throw new NotImplementedException();
        }

        public void SetRightHandSideVector(DoubleDenseMatrix value)
        {
            _rhs = value;
        }

        public void SetLeftHandSideVector(DoubleDenseMatrix value)
        {
            _epsilon_r = value;
        }

        public DoubleDenseMatrix GetScalarPotential()
        {
            return _phi;
        }
    }
}
