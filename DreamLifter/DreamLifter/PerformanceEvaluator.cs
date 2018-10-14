using Independence;
using Independence.Ex;
using System;
using System.Linq;

namespace DreamLifter
{
    /// <summary>
    /// Evaluate the performance of given lifters.
    /// </summary>
    public sealed class PerformanceEvaluator
    {
        private IGEVPSolver _evpSolver;
        /// <summary>
        /// Lifter solver, e.g., drift diffusion solver.
        /// </summary>
        private IDDESolver _lifterSolver;

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

        /// <summary>
        /// Initialize this evaluator with the given solver.
        /// </summary>
        /// <param name="lifterSolver">Lifter solver, e.g., drift diffusion solver.</param>
        /// <param name="tol"></param>
        /// <param name="dropFactor"></param>
        public PerformanceEvaluator(IDDESolver lifterSolver, IGEVPSolver evpSolver, double tol = 1.0e-4, double dropFactor = 0.8)
        {
            _lifterSolver = lifterSolver;
            _tol = tol;
            _dropFactor = dropFactor;
        }

        public bool? Update(int id)
        {
            var potential = _currentPotential.Value;
            _lifterSolver.SetPotential(id, potential);
            var J = _lifterSolver.JacobianMatrix;
            var S = _lifterSolver.SensitivityMatrix;
            var eigPair = _evpSolver.Solve(S, J, "SM");
            var omega = eigPair.EigenValue.Real;
            _hasConvergedEigenMode = Math.Abs(omega) < _tol;
            bool? IsNeutral = null;
            if (_hasConvergedEigenMode.Value)
            {
                IsNeutral = IsStable(eigPair.RightEigenMatrix.Real());
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
        /// <returns>Is the mode stable?</returns>
        private static bool IsStable(DoubleDenseMatrix v)
        {
            var max = v.Max();
            var min = v.Min();
            return max * min > 0;
        }
    }
}
