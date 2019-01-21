using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LRS.Lib;

namespace LRS.Repository
{
    public class LoadingDockRepository : ILoadingDockRepository
    {
        private static Mutex mutex = new Mutex();

        private static List<LoadingDock> _docks = new List<LoadingDock>();

        public void Lock()
        {
            mutex.WaitOne();
        }

        public void Unlock()
        {
            mutex.ReleaseMutex();
        }

        public LoadingDock GetByName(string name)
        {
            return _docks.FirstOrDefault(x => x.Name == name);
        }

        public IEnumerable<LoadingDock> GetByStatus(LoadingDockStatus status)
        {
            return _docks.Where(x => x.Status == status);
        }

        public void Add(LoadingDock dock)
        {
            dock.Id = _docks.Count == 0 ? 1 : _docks.Max(x => x.Id) + 1;

            _docks.Add(dock);
        }

        public void AddRange(IEnumerable<LoadingDock> docks)
        {
            foreach (var dock in docks)
            {
                this.Add(dock);
            }
        }

        public void Update(LoadingDock dock)
        {
            int i = _docks.FindIndex(x => x.Id == dock.Id);

            _docks[i] = dock;
        }
    }
}
