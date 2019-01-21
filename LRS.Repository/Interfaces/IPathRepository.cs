using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRS.Repository
{
    public interface IPathRepository
    {
        IEnumerable<int> A { get; }

        IDictionary<string, int> a { get; }

        IEnumerable<int> B { get; }

        IDictionary<string, int> b { get; }

        void Add_A(int i);

        void Add_B(int i);

        void Add_a(string key, int value);

        void Add_b(string key, int value);
    }
}
