using Independence;

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
        /// <param name="boundaryName">Global name of terget node</param>
        /// <param name="value">Potential to be set</param>
        void SetPotential(string boundaryName, double value);

        /// <summary>
        /// Set gradient on the node specified.
        /// This method implements the Neumman type boundary condition.
        /// </summary>
        /// <param name="boundaryName">Global name of target node</param>
        /// <param name="value">Gradient to be set</param>
        void SetGradient(string boundaryName, double value);

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
