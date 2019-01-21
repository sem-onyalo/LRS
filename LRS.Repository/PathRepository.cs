using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LRS.Lib;

namespace LRS.Repository
{
    public class PathRepository : IPathRepository
    {
        private static IList<int> _path_A = new List<int>();
        private static IList<int> _path_B = new List<int>();
        private static IDictionary<string, int> _path_a = new Dictionary<string, int>();
        private static IDictionary<string, int> _path_b = new Dictionary<string, int>();

        public IEnumerable<int> A
        {
            get { return _path_A; }
        }

        public IDictionary<string, int> a
        {
            get { return _path_a; }
        }

        public IEnumerable<int> B
        {
            get { return _path_B; }
        }

        public IDictionary<string, int> b
        {
            get { return _path_b; }
        }

        public void Add_A(int i)
        {
            _path_A.Add(i);
        }

        public void Add_B(int i)
        {
            _path_B.Add(i);
        }

        public void Add_a(string key, int value)
        {
            _path_a.Add(key, value);
        }

        public void Add_b(string key, int value)
        {
            _path_b.Add(key, value);
        }
    }
}
