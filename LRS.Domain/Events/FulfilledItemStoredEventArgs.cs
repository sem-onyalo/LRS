using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRS.Lib
{
    public class FulfilledItemStoredEventArgs : EventArgs
    {
        public FulfilledItem FulfilledItem { get; set; }

        public FulfilledItemStoredEventArgs(FulfilledItem item)
        {
            FulfilledItem = item;
        }
    }
}
