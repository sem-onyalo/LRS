using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LRS.Lib;

namespace LRS.Repository
{
    public interface IInventoryRepository
    {
        InventoryItem GetItemByProductCode(string code);

        void Add(InventoryItem item);

        void AddRange(IEnumerable<InventoryItem> items);
    }
}
