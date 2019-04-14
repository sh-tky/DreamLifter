using Independence;
using System;
using DreamLifter.Elements;

namespace DreamLifter.MatrixMarket
{
    public abstract class ShapeFunctionBase
    {
        protected readonly Element _element;
        protected DoubleDenseMatrix _phi;
        protected DoubleDenseMatrix _dphi;
        protected DoubleDenseMatrix _J;

        protected ShapeFunctionBase(Element element, int dimension)
        {
            _element = element;
            _phi = new DoubleDenseMatrix(_element.NumberOfNodesInAnElement, 1);
            _dphi = new DoubleDenseMatrix(_element.NumberOfNodesInAnElement, dimension);
            _J = new DoubleDenseMatrix(dimension, dimension);
        }

        public abstract void SetCoordinate(Axis axis, double value);

        public DoubleDenseMatrix dPhi
        {
            get
            {
                return _dphi;
            }
        }

        public DoubleDenseMatrix Phi
        {
            get
            {
                return _phi;
            }
        }

        public DoubleDenseMatrix JacobianMatrix
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public DoubleDenseMatrix InverseOfJacobianMatrix
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public double Determinant
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
