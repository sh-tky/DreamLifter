namespace DreamLifter
{
    /// <summary>
    /// Insist that the electric field is operable. 
    /// </summary>
    public interface IFieldOperable
    {
        void SetPotential(string boundaryName, double potential);
    }
}
