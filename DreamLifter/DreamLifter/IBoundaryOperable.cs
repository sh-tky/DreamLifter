using Independence;

namespace DreamLifter
{
    /// <summary>
    /// Provide methods which can impose boundary conditions.
    /// </summary>
    public interface IBoundaryOperable
    {
        /// <summary>
        /// Impose Dirichlet boundary condition to the given matrix A. 
        /// </summary>
        void ImposeFirstKindBoundaryCondition(DoubleSparseMatrix A, string type);
        void ImposeFirstKindBoundaryCondition(DoubleDenseMatrix rhs, string type, double value);
    }
}
