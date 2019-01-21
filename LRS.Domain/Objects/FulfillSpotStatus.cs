using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRS.Lib
{
    public enum FulfillSpotStatus
    {
        Available, // available to be used
        Reserved,  // reserved for an item
        Utilized,  // currently being used
        Complete   // usage has completed
    }
}
