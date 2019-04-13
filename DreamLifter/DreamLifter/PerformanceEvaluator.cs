using Independence;
using Independence.Ex;
using Independence.EVP;
using System.Collections.Generic;
using System;
using System.Linq;

namespace DreamLifter
{
    /// <summary>
    /// Evaluate the performance of given EAD thruster(s).
    /// </summary>
    public sealed class PerformanceEvaluator
    {
        private readonly IGEVPSolver _evpSolver;

        /// <summary>
        /// A drift diffusion solver.
        /// </summary>
        private readonly IDDESolver _ddeSolver;

        /// <summary>
        /// Represent whether the electric potential is converged or not.
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

        public bool? Update(Dictionary<string, double> potentials, string perturbationTarget)
        {
            const double eps = 1.0e-4;
            foreach (var p in potentials)
            {
                _ddeSolver.SetPotential(p.Key, p.Value);
            }
            var nominalJacobian = _ddeSolver.GetJacobianMatrix();
            foreach (var p in potentials)
            {
                if (p.Key == perturbationTarget)
                {
                    _ddeSolver.SetPotential(p.Key, p.Value + eps);
                }
            }
            var perturbedJacobian = _ddeSolver.GetJacobianMatrix();
            var sensitivity = (perturbedJacobian - nominalJacobian) * (1.0 / eps);
            _eigenPair = _evpSolver.Solve(sensitivity, nominalJacobian, "SM");
            var omega = _eigenPair.EigenValue.Real;
            _hasConvergedEigenMode = Math.Abs(omega) < _tol;
            bool? IsNeutral = null;
            if (_hasConvergedEigenMode.Value)
            {
                IsNeutral = IsStable(_eigenPair.RightEigenMatrix.Real());
            }
            else
            {
                potentials[perturbationTarget] -= omega;
            }
            if (IsNeutral.HasValue)
            {
                if (!IsNeutral.Value)
                {
                    potentials[perturbationTarget] *= _dropFactor;
                }
            }
            return IsNeutral;
        }

        /// <summary>
        /// Judge the stability of the given eigenmode.
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
