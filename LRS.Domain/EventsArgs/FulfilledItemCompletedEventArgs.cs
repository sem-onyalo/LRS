using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRS.Lib
{
    public class FulfilledItemCompletedEventArgs : EventArgs
    {
        public FulfilledItem FulfilledItem { get; set; }

        public FulfilledItemCompletedEventArgs(FulfilledItem item)
        {
            FulfilledItem = item;
        }
    }
}
