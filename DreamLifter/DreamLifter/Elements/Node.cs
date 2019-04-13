using System;

namespace DreamLifter.Elements
{
    /// <summary>
    /// Represent nodes
    /// </summary>
    public abstract class Node 
        : IEquatable<Node>
    {
        /// <summary>
        /// Invoke deep copy of the current instance
        /// </summary>
        /// <returns></returns>
        public abstract Node Clone();

        public Element ParentElement
        {
            get;
            set;
        }

        public int GlobalIndex
        {
            get;
            set;
        }

        public int LocalIndex
        {
            get;
            set;
        }

        public abstract double GetCoordinate(Axis axis);
        
        public abstract void SetCoordinate(Axis axis, double val);

        public bool Equals(Node other)
        {
            return other.GlobalIndex == this.GlobalIndex;
        }
    }
}
