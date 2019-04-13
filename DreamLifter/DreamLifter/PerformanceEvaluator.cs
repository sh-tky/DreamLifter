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
        private IGEVPSolver _gevpSolver;
        /// <summary>
        /// Lifter solver, e.g., drift diffusion solver.
        /// </summary>
        private IDDESolver _lifterSolver;

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

        public bool? Update(int id, ref double potential)
        {
            _lifterSolver.SetPotential(id, potential);
            var eigPair = _gevpSolver.Solve(_lifterSolver.SensitivityMatrix, _lifterSolver.JacobianMatrix, "SM");
            var omega = eigPair.EigenValue.Real;
            bool? IsNeutral = null;
            if (Math.Abs(omega) < _tol)
            {
                IsNeutral = IsStable(eigPair.RightEigenMatrix.Real());
            }
            else
            {
                potential -= omega;
            }
            if (IsNeutral.HasValue)
            {
                if (!IsNeutral.Value)
                {
                    potential *= _dropFactor;
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
