using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRS.Lib
{
    public class RobotStateChangedEventArgs
    {
        public RobotState OldState { get; set; }

        public RobotState NewState { get; set; }

        public RobotStateChangedEventArgs(RobotState oldState, RobotState newState)
        {
            OldState = oldState;
            NewState = newState;
        }
    }
}
