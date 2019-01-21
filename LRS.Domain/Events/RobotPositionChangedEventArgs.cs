using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRS.Lib
{
    public class RobotPositionChangedEventArgs
    {
        public int TurnStep { get; set; }

        public Direction Direction { get; set; }

        public Orientation Orientation { get; set; }

        public Coordinates Coordinates { get; set; }

        public Coordinates OldCoordinates { get; set; }
    }
}
