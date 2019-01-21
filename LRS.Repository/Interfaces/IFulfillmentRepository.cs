using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LRS.Lib;

namespace LRS.Repository
{
    public interface IFulfillmentRepository : IRepository
    {
        Queue<FulfillmentItem> NewFulfillmentItemQueue { get; }

        FulfillmentSpot GetFulfillmentSpot(int id);
        
        IEnumerable<FulfillmentSpot> GetFulfillmentSpotsByStatus(FulfillSpotStatus status);

        void Add(FulfillmentSpot spot);

        void AddRange(IEnumerable<FulfillmentSpot> spots);

        void Update(FulfillmentSpot spot);
    }
}
