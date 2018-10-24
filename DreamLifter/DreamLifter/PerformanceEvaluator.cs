using Independence;
using Independence.Ex;
using Independence.EVP;
using System;
using System.Linq;

namespace DreamLifter
{
    /// <summary>
    /// Evaluate the performance of given lifters.
    /// </summary>
    public sealed class PerformanceEvaluator
    {
        private readonly IGEVPSolver _evpSolver;

        /// <summary>
        /// A drift diffusion solver.
        /// </summary>
        private readonly IDDESolver _ddeSolver;

        /// <summary>
        /// Electric potential on a corona active electrode. 
        /// </summary>
        private double? _currentPotential = null;

        /// <summary>
        /// Represent whether an electric potential is converged or not.
        /// </summary>
        private bool? _hasConvergedEigenMode = null;

        private readonly double _tol;
        private readonly double _dropFactor;

        private EigenPair _eigenPair = null;

        public DoubleDenseMatrix GetNormalMode(Species species)
        {
            var rightEigenVector = _eigenPair.RightEigenMatrix;
            switch (species)
            {
                case Species.Electrons:
                case Species.PositiveIons:
                case Species.NegativeIons:
                default:
                    throw new NotImplementedException("species");
            }
        }

        /// <summary>
        /// Initialize this evaluator with the given solvers.
        /// </summary>
        /// <param name="ddeSolver">A drift-diffusion-reaction equation solver</param>
        /// <param name="evpSolver">A solver for generalized eigenvalue problems</param>
        /// <param name="tol"></param>
        /// <param name="dropFactor"></param>
        public PerformanceEvaluator(IDDESolver ddeSolver, IGEVPSolver evpSolver, double tol = 1.0e-4, double dropFactor = 0.8)
        {
            _ddeSolver = ddeSolver;
            _evpSolver = evpSolver;
            _tol = tol;
            _dropFactor = dropFactor;
        }

        public bool? Update(int potentialBoundaryId)
        {
            var potential = _currentPotential.Value;
            _ddeSolver.SetPotential(potentialBoundaryId, potential);
            var J = _ddeSolver.GetJacobianMatrix();
            var S = _ddeSolver.GetSensitivityMatrix();
            _eigenPair = _evpSolver.Solve(S, J, "SM");
            var omega = _eigenPair.EigenValue.Real;
            _hasConvergedEigenMode = Math.Abs(omega) < _tol;
            bool? IsNeutral = null;
            if (_hasConvergedEigenMode.Value)
            {
                IsNeutral = IsStable(_eigenPair.RightEigenMatrix.Real());
            }
            else
            {
                _currentPotential = potential - omega;
            }
            if (IsNeutral.HasValue)
            {
                if (!IsNeutral.Value)
                {
                    _currentPotential = _dropFactor * potential;
                }
            }
            return IsNeutral;
        }

        /// <summary>
        /// Judge the stability of the given mode.
        /// </summary>
        /// <param name="v">The eigenvector corresponding to the eigenmode.</param>
        /// <returns>Return false if the mode includes oscillation.</returns>
        private static bool IsStable(DoubleDenseMatrix v)
        {
            var max = v.Max();
            var min = v.Min();
            return max * min > 0;
        }
    }
}
