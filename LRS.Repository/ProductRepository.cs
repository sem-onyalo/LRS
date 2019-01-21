using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LRS.Lib;

namespace LRS.Repository
{
    public class ProductRepository : IProductRepository
    {
        private static IList<Product> _products = new List<Product>();

        public Product GetByCode(string code)
        {
            return _products.FirstOrDefault(x => x.Code == code);
        }

        public void Add(Product product)
        {
            if (product == null) throw new Exception("Cannot add empty product");
            if (string.IsNullOrWhiteSpace(product.Code)) throw new Exception("Cannot add product with empty code");

            _products.Add(product);
        }

        public void AddRange(IEnumerable<Product> products)
        {
            foreach (var p in products)
            {
                this.Add(p);
            }
        }
    }
}
