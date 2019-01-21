using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRS.Lib
{
    public class PathMarker
    {
        public string Name { get; set; }

        public PathMarkerType Type { get; set; }

        public Coordinates Coordinates { get; set; }

        public List<string> NorthPathMarkers { get; set; }

        public List<string> EastPathMarkers { get; set; }

        public List<string> SouthPathMarkers { get; set; }

        public List<string> WestPathMarkers { get; set; }

        public PathMarker(string name, PathMarkerType type, Coordinates coordinates)
        {
            Name = name;
            Type = type;
            Coordinates = coordinates;
            NorthPathMarkers = new List<string>();
            EastPathMarkers = new List<string>();
            SouthPathMarkers = new List<string>();
            WestPathMarkers = new List<string>();
        }

        public override string ToString()
        {
            return this.Name;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is PathMarker)) return false;

            PathMarker other = obj as PathMarker;

            return Type == other.Type && Name == other.Name && Coordinates == other.Coordinates;
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode() ^ Name.GetHashCode() ^ Coordinates.GetHashCode();
        }

        public static bool operator ==(PathMarker a, PathMarker b)
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
            return a.Type == b.Type && a.Name == b.Name && a.Coordinates == b.Coordinates;
        }

        public static bool operator !=(PathMarker a, PathMarker b)
        {
            return !(a == b);
        }
    }
}
