using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRS.Lib
{
    public class InventoryItem
    {
        public int Id { get; set; }

        public Product Product { get; set; }

        public int Quantity { get; set; }

        public PathMarker DestinationMarker { get; set; }
    }
}
