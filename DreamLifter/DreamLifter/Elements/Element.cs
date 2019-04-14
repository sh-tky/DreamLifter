using System.Collections.Generic;
using Independence;
using DreamLifter.MatrixMarket;

namespace DreamLifter.Elements
{
    /// <summary>
    /// Represent elements
    /// </summary>
    public abstract class Element
        : IEqualityComparer<Element>
    {

        protected abstract void Initialize();

        protected Element(int number, IEnumerable<Node> vertices)
        {
            Number = number;
            Nodes = new List<Node>(vertices);
            Initialize();
        }

        protected Element(int number, params Node[] vertices)
        {
            Number = number;
            Nodes = new List<Node>();
            Nodes.AddRange(vertices);
            Initialize();
        }

        public bool Equals(Element x, Element y)
        {
            return x.Number == y.Number;
        }

        public int GetHashCode(Element obj)
        {
            return obj.Number;
        }

        public ShapeFunctionBase Shape
        {
            get;
            protected set;
        }

        public int Number { get; private set; }

        public void SetNumber(int number)
        {
            Number = number;
        }

        public int NumberOfNodesInAnElement
        {
            get { return Nodes.Count; }
        }

        public List<Node> Nodes
        {
            get;
            private set;
        }

        public DoubleDenseMatrix NormalVector
        {
            get;
        }

        public virtual void SetReferenceValue(double value)
        {
            ReferenceValue = value;
        }

        public double ReferenceValue
        {
            private set;
            get;
        }

        public virtual List<Element> BoundaryElements
        {
            get;
            set;
        }
    }
}
