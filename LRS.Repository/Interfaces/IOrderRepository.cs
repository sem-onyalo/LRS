using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LRS.Lib;

namespace LRS.Repository
{
    public interface IOrderRepository : IRepository
    {
        Order Get(Guid id);

        IEnumerable<Order> GetByStatus(OrderStatus status);

        void Add(Order o);

        void Update(Order o);
    }
}
