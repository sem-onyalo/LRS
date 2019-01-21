using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LRS.Lib;

namespace LRS.Repository
{
    public class InventoryRepository : IInventoryRepository
    {
        private static IList<InventoryItem> _items = new List<InventoryItem>();

        public InventoryItem GetItemByProductCode(string code)
        {
            return _items.FirstOrDefault(x => x.Product.Code == code);
        }

        public void Add(InventoryItem item)
        {
            item.Id = _items.Count == 0 ? 1 : _items.Max(x => x.Id) + 1;

            _items.Add(item);
        }

        public void AddRange(IEnumerable<InventoryItem> items)
        {
            foreach (var i in items)
            {
                this.Add(i);
            }
        }
    }
}
