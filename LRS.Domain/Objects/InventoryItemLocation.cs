using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRS.Lib
{
    public class InventoryItemLocation
    {
        public InventoryItem Item { get; set; }

        public IList<PathMarker> PathMarkers { get; set; }

        //public int Path_a { get; set; }

        //public int Path_b { get; set; }
    }
}
