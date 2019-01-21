using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRS.Lib
{
    public class RobotException : Exception
    {
        public RobotException() : base() { }

        public RobotException(string message) : base(message) { }

        public RobotException(string message, Exception innerException) : base(message, innerException) { }

        public RobotException(string message, params object[] args) : base(string.Format(message, args)) { }
    }
}
