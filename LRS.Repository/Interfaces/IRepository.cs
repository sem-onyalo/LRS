using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRS.Repository
{
    public interface IRepository
    {
        void Lock();

        void Unlock();
    }
}
