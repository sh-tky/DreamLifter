using Independence;
using Independence.EVP;

namespace DreamLifter
{
    /// <summary>
    /// Represent a generalized eigen value problem solver.
    /// </summary>
    public interface IGEVPSolver
    {
        EigenPair Solve(DoubleSparseMatrix A, DoubleSparseMatrix M, string target);
    }
}
