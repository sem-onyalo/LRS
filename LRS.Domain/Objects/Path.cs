using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRS.Lib
{
    public class Path
    {
        public int Priority { get; set; }

        public double Length { get; set; }

        public Coordinates Coordinates { get; set; }

        public Orientation PathDirection { get; set; }

        public Path(Coordinates coordinates, double length, Orientation pathDirection, int priority)
        {
            Length = length;
            Priority = priority;
            Coordinates = coordinates;
            PathDirection = pathDirection;
        }
    }
}
