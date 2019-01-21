using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRS.Lib
{
    public class Obstacle
    {
        public Coordinates Coordinates { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public Obstacle(Coordinates coords, double width, double height)
        {
            Coordinates = coords;
            Width = width;
            Height = height;
        }
    }
}
