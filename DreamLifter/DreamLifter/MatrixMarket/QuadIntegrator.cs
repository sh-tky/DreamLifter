using DreamLifter.Elements;
using Independence;
using System;

namespace DreamLifter.MatrixMarket
{
    /// <summary>
    /// Provide methods to integrate quad elements
    /// </summary>
    internal static class QuadIntegrator
    {
        public static DoubleDenseMatrix Integrate(Func<Element, double, double, double, double, DoubleDenseMatrix> function, INumericalIntegration integrator, Element element)
        {
            var yield = new DoubleDenseMatrix(element.NumberOfNodesInAnElement, element.NumberOfNodesInAnElement);
            for (var p = 0; p < integrator.NumberOfIntegrationPoints; p++)
            {
                for (var q = 0; q < integrator.NumberOfIntegrationPoints; q++)
                {
                    yield += function(element,
                    integrator.IntegrationPoint(p),
                    integrator.Weight(p),
                    integrator.IntegrationPoint(q),
                    integrator.Weight(q));
                }
            }
            return yield;
        }
    }
}
