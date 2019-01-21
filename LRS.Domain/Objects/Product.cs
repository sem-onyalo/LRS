using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRS.Lib
{
    public class Product
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Product)) return false;

            Product other = obj as Product;

            return Code == other.Code;
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }
    }
}
