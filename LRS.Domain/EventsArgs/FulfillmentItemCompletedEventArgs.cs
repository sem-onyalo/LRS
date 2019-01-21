using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRS.Lib
{
    public class FulfillmentItemCompletedEventArgs : EventArgs
    {
        public FulfillmentItem FulfillmentItem { get; set; }

        public FulfillmentItemCompletedEventArgs(FulfillmentItem item)
        {
            FulfillmentItem = item;
        }
    }
}
