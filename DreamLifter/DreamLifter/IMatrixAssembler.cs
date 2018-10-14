using Independence;

namespace DreamLifter
{
    /// <summary>
    /// 高次の行列演算子を公開します。
    /// </summary>
    public interface IMatrixAssembler
    {

        /// <summary>
        /// Acquire the Jacobian matrix of the system.
        /// </summary>
        DoubleSparseMatrix JacobianMatrix
        {
            get;
        }

        /// <summary>
        /// Acquire the mass matrix of the system.
        /// </summary>
        DoubleSparseMatrix MassMatrix
        {
            get;
        }

        /// <summary>
        /// Acquire the sensitivity matrix of the system.
        /// </summary>
        DoubleSparseMatrix SensitivityMatrix
        {
            get;
        }
    }
}
