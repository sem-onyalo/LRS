using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRS.Lib
{
    public enum LoadingDockStatus
    {
        Available,     // available to be used
        Reserved,      // reserved for an item
        Utilized,      // currently being used
        NotAvailable   // not availabel to be used
    }
}
