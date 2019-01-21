using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LRS.Lib;

namespace LRS.Repository
{
    public interface IFulfilledRepository : IRepository
    {
        FulfilledSpot GetFulfilledSpot(int id);

        IEnumerable<FulfilledSpot> GetFulfilledSpotsByStatus(FulfillSpotStatus status);

        void Add(FulfilledSpot spot);

        void AddRange(IEnumerable<FulfilledSpot> spots);

        void Update(FulfilledSpot spot);
    }
}
