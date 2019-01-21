using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LRS.Lib;

namespace LRS.Repository
{
    public interface ILoadingDockRepository : IRepository
    {
        LoadingDock GetByName(string name);

        IEnumerable<LoadingDock> GetByStatus(LoadingDockStatus status);

        void Add(LoadingDock dock);

        void AddRange(IEnumerable<LoadingDock> docks);

        void Update(LoadingDock dock);
    }
}
