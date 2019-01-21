using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRS.Lib
{
    public enum Direction { None, Left, Forward, Right, Reverse }

    public enum Orientation { North, East, South, West }

    public enum PathMarkerType { Route, Destination }

    public enum RobotState { Idle, Retrieving, Delivering, GoingHome }
}
