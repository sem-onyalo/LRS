using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRS.Lib
{
    /// <summary>
    /// Not a Domain object
    /// </summary>
    public class FulfillmentItem
    {
        public Guid Id { get; set; }

        public Order Order { get; set; }

        public FulfillItemStatus Status { get; set; }

        public InventoryItem InventoryItem { get; set; }

        public FulfillmentSpot FulfillmentSpot { get; set; }
    }
}
