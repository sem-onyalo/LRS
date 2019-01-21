using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRS.Lib
{
    public class FulfillmentSpot
    {
        public int Id { get; set; }

        public FulfillSpotStatus Status { get; set; }

        public PathMarker DestinationMarker { get; set; }
    }
}
