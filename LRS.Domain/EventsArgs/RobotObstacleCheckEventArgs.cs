using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRS.Lib
{
    public class RobotObstacleCheckEventArgs
    {
        public Coordinates Coordinates { get; set; }

        public Orientation Orientation { get; set; }

        public double Range { get; set; }

        public RobotObstacleCheckEventArgs(Coordinates coordinates, Orientation orientation, double range)
        {
            Coordinates = coordinates;
            Orientation = orientation;
            Range = range;
        }
    }
}
