
namespace DreamLifter.MatrixMarket
{
    /// <summary>
    /// Publish methods for numerical integration
    /// </summary>
    public interface INumericalIntegration
    {
        int NumberOfIntegrationPoints { get; }

        double Weight(int i);

        double IntegrationPoint(int i);
    }
}
