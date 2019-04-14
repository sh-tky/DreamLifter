using Independence;
using DreamLifter.Elements;

namespace DreamLifter.MatrixMarket
{
    internal static class QuadIntegralFunction
    {
        internal static double Interpolate(Element element, DoubleDenseMatrix src)
        {
            element.Shape.SetCoordinate(Axis.X, 0.0);
            element.Shape.SetCoordinate(Axis.Y, 0.0);
            var Phi = element.Shape.Phi;
            return Interpolate(element, src, Phi);
        }

        internal static double Interpolate(Element element, DoubleDenseMatrix src, DoubleDenseMatrix Phi)
        {
            var yield = 0.0;
            foreach (var node in element.Nodes)
            {
                yield += src[node.GlobalIndex] * Phi[node.LocalIndex];
            }
            return yield;
        }

        internal static class Vector
        {

        }

        internal static class Matrix
        {
            internal static DoubleDenseMatrix Stiffness(Element element,
                           double xi, double xi_weight,
                           double eta, double eta_weight)
            {
                element.Shape.SetCoordinate(Axis.X, xi);
                element.Shape.SetCoordinate(Axis.Y, eta);
                return IntegralFunction2D.Stiffness(element) * (xi_weight * eta_weight);
            }
        }
    }
}
