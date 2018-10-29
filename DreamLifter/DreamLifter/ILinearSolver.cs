using Independence;

namespace DreamLifter
{
    /// <summary>
    /// Insist that the class implements linear solver for matrices.
    /// </summary>
    public interface ILinearSolver
    {
        void SetMatrix(DoubleSparseMatrix matrix);

        DoubleDenseMatrix Solve(DoubleDenseMatrix rhs);
    }
}
