using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRS.Lib
{
    public class Coordinates
    {
        public double MarginTop { get; set; }

        public double MarginLeft { get; set; }

        public Coordinates()
        {
        }

        public Coordinates(double marginLeft, double marginTop)
        {
            MarginLeft = marginLeft;
            MarginTop = marginTop;
        }

        public Coordinates Clone()
        {
            Coordinates clone = new Coordinates();
            clone.MarginLeft = this.MarginLeft;
            clone.MarginTop = this.MarginTop;

            return clone;
        }

        public override string ToString()
        {
            return string.Format("({0},{1})", this.MarginLeft, this.MarginTop);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Coordinates)) return false;

            Coordinates other = obj as Coordinates;

            return this.MarginLeft == other.MarginLeft && this.MarginTop == other.MarginTop;
        }

        public override int GetHashCode()
        {
            return MarginLeft.GetHashCode() ^ MarginTop.GetHashCode();
        }

        public static bool operator ==(Coordinates a, Coordinates b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.MarginLeft == b.MarginLeft && a.MarginTop == b.MarginTop;
        }

        public static bool operator !=(Coordinates a, Coordinates b)
        {
            return !(a == b);
        }
    }

}
