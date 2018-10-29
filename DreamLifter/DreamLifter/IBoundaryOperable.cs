using Independence;

namespace DreamLifter
{
    public interface IBoundaryOperable
    {
        void ImposeFirstKindBoundaryCondition(DoubleSparseMatrix A, string type);
        void ImposeFirstKindBoundaryCondition(DoubleDenseMatrix rhs, string type, double value);
    }
}
