using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LRS.Lib;

namespace LRS.Repository
{
    public class FulfillmentRepository : IFulfillmentRepository
    {
        private static Mutex mutex = new Mutex();

        private static List<FulfillmentSpot> _fulfillmentSpots = new List<FulfillmentSpot>();

        public FulfillmentRepository()
        {
            NewFulfillmentItemQueue = new Queue<FulfillmentItem>();
        }

        public Queue<FulfillmentItem> NewFulfillmentItemQueue { get; private set; }

        public void Lock()
        {
            mutex.WaitOne();
        }

        public void Unlock()
        {
            mutex.ReleaseMutex();
        }

        public FulfillmentSpot GetFulfillmentSpot(int id)
        {
            return _fulfillmentSpots.FirstOrDefault(x => x.Id == id);
        }
        
        public IEnumerable<FulfillmentSpot> GetFulfillmentSpotsByStatus(FulfillSpotStatus status)
        {
            return _fulfillmentSpots.Where(x => x.Status == status);
        }

        public void Add(FulfillmentSpot spot)
        {
            spot.Id = _fulfillmentSpots.Count == 0 ? 1 : _fulfillmentSpots.Max(x => x.Id) + 1;

            _fulfillmentSpots.Add(spot);
        }

        public void AddRange(IEnumerable<FulfillmentSpot> spots)
        {
            foreach (var spot in spots)
            {
                this.Add(spot);
            }
        }

        public void Update(FulfillmentSpot spot)
        {
            int i = _fulfillmentSpots.FindIndex(x => x.Id.Equals(spot.Id));

            _fulfillmentSpots[i] = spot;
        }
    }
}
