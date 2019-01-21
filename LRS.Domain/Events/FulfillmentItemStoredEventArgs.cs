using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRS.Lib
{
    public class FulfillmentItemStoredEventArgs : EventArgs
    {
        public FulfillmentItem FulfillmentItem { get; set; }

        public FulfillmentItemStoredEventArgs(FulfillmentItem item)
        {
            FulfillmentItem = item;
        }
    }
}
