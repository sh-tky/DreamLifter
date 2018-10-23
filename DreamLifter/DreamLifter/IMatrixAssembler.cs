using Independence;

namespace DreamLifter
{
    /// <summary>
    /// Insist that this class can generate some matrices.
    /// </summary>
    public interface IMatrixAssembler
    {

        /// <summary>
        /// Acquire the Jacobian matrix of the system.
        /// </summary>
        /// <remarks>
        /// This method can have heavy computational cost. 
        /// </remarks>
        DoubleSparseMatrix GetJacobianMatrix();

        /// <summary>
        /// Acquire the mass matrix of the system.
        /// </summary>
        /// <remarks>
        /// This method can have heavy computational cost. 
        /// </remarks>
        DoubleSparseMatrix GetMassMatrix();

        /// <summary>
        /// Acquire the sensitivity matrix of the system.
        /// </summary>
        /// <remarks>
        /// This method can have heavy computational cost.
        /// </remarks>
        DoubleSparseMatrix GetSensitivityMatrix();
    }
}
