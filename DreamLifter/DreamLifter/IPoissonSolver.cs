using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Independence;
using Independence.Ex;

namespace DreamLifter
{
    /// <summary>
    /// Publish methods for the Poisson equation solver.
    /// </summary>
    public interface IPoissonSolver
    {
        /// <summary>
        /// Set potential on the node specified. 
        /// </summary>
        /// <param name="gId">Global index of terget node</param>
        /// <param name="value">Potential to be set</param>
        void SetPotential(int gId, double value);

        /// <summary>
        /// Set gradient on the node specified.
        /// This method implements the Neumman type boundary condition.
        /// </summary>
        /// <param name="gId">Global index of target node</param>
        /// <param name="value">Gradient to be set</param>
        void SetGradient(int gId, double value);

        void SetRightHandSideVector(DoubleDenseMatrix value);

        void SetLeftHandSideVector(DoubleDenseMatrix value);

        /// <summary>
        /// Get scalar potential distribution.
        /// </summary>
        /// <returns></returns>
        DoubleDenseMatrix GetScalarPotential();

        /// <summary>
        /// Get vector field regarding the direction specified with <paramref name="axis"/>.
        /// </summary>
        /// <param name="axis">Direction of the vector component.</param>
        /// <returns></returns>
        DoubleDenseMatrix GetVectorField(Axis axis);

    }
}
