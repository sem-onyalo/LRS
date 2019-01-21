using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRS.Lib
{
    public class FulfilledItemRetrievedEventArgs : EventArgs
    {
        public FulfilledItem FulfilledItem { get; set; }

        public FulfilledItemRetrievedEventArgs(FulfilledItem item)
        {
            FulfilledItem = item;
        }
    }
}
