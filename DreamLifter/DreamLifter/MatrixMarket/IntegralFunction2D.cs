using DreamLifter.Elements;
using Independence;

namespace DreamLifter.MatrixMarket
{

    internal static class IntegralFunction2D
    {
        internal static DoubleDenseMatrix Stiffness(Element element)
        {
            var yield = new DoubleDenseMatrix(element.NumberOfNodesInAnElement, element.NumberOfNodesInAnElement);

            var dPhi = element.Shape.dPhi;
            var InvJ = element.Shape.InverseOfJacobianMatrix;
            var detJ = element.Shape.Determinant;

            for (var i = 0; i < element.NumberOfNodesInAnElement; i++)
            {
                var dPhiidX = InvJ[0, 0] * dPhi[i, 0] + InvJ[0, 1] * dPhi[i, 1];
                var dPhiidY = InvJ[1, 0] * dPhi[i, 0] + InvJ[1, 1] * dPhi[i, 1];
                for (var j = 0; j < element.NumberOfNodesInAnElement; j++)
                {
                    var dPhijdX = InvJ[0, 0] * dPhi[j, 0] + InvJ[0, 1] * dPhi[j, 1];
                    var dPhijdY = InvJ[1, 0] * dPhi[j, 0] + InvJ[1, 1] * dPhi[j, 1];
                    yield[i, j] = dPhiidX * dPhijdX + dPhiidY * dPhijdY;
                }
            }
            yield *= detJ;
            return yield;
        }
    }
}
