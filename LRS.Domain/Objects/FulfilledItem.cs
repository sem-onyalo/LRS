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
    public class FulfilledItem
    {
        public Guid Id { get; set; }

        public Order Order { get; set; }

        public FulfillItemStatus Status { get; set; }

        public FulfilledSpot FulfilledSpot { get; set; }

        public LoadingDock LoadingDock { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is FulfilledItem)) return false;

            FulfilledItem other = obj as FulfilledItem;

            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
