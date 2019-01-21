using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRS.Lib
{
    public interface IUnityFactory
    {
        void Initialize();

        T Resolve<T>();
    }
}
