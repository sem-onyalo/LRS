using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LRS.Lib;

namespace LRS.Repository
{
    public class FulfilledRepository : IFulfilledRepository
    {
        private static Mutex mutex = new Mutex();

        private static List<FulfilledSpot> _fulfilledSpots = new List<FulfilledSpot>();

        public void Lock()
        {
            mutex.WaitOne();
        }

        public void Unlock()
        {
            mutex.ReleaseMutex();
        }

        public FulfilledSpot GetFulfilledSpot(int id)
        {
            return _fulfilledSpots.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<FulfilledSpot> GetFulfilledSpotsByStatus(FulfillSpotStatus status)
        {
            return _fulfilledSpots.Where(x => x.Status == status);
        }

        public void Add(FulfilledSpot spot)
        {
            spot.Id = _fulfilledSpots.Count == 0 ? 1 : _fulfilledSpots.Max(x => x.Id) + 1;

            _fulfilledSpots.Add(spot);
        }

        public void AddRange(IEnumerable<FulfilledSpot> spots)
        {
            foreach (var spot in spots)
            {
                this.Add(spot);
            }
        }

        public void Update(FulfilledSpot spot)
        {
            int i = _fulfilledSpots.FindIndex(x => x.Id.Equals(spot.Id));

            _fulfilledSpots[i] = spot;
        }
        
    }
}
