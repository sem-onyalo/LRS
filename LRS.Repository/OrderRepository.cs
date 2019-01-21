using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LRS.Lib;

namespace LRS.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private static Mutex mutex = new Mutex();

        private static List<Order> _orders = new List<Order>();

        public void Lock()
        {
            mutex.WaitOne();
        }

        public void Unlock()
        {
            mutex.ReleaseMutex();
        }

        public Order Get(Guid id)
        {
            return _orders.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Order> GetByStatus(OrderStatus status)
        {
            return _orders.Where(x => x.Status == status);
        }

        public void Add(Order o)
        {
            _orders.Add(o);
        }

        public void Update(Order o)
        {
            int i = _orders.FindIndex(x => x.Id.Equals(o.Id));

            _orders[i] = o;
        }
    }
}
