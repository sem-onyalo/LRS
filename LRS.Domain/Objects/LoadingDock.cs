using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRS.Lib
{
    public class LoadingDock
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public LoadingDockStatus Status { get; set; }

        public PathMarker DestinationMarker { get; set; }
    }
}
