using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRS.Lib
{
    public class FulfillmentItemRetrievedEventArgs : EventArgs
    {
        public FulfillmentItem FulfillmentItem { get; set; }

        public FulfillmentItemRetrievedEventArgs(FulfillmentItem item)
        {
            FulfillmentItem = item;
        }
    }
}
