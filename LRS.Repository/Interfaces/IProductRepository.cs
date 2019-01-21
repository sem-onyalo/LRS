using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LRS.Lib;

namespace LRS.Repository
{
    public interface IProductRepository
    {
        Product GetByCode(string code);

        void Add(Product product);

        void AddRange(IEnumerable<Product> products);
    }
}
