using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRS.Lib
{
    public class Order
    {
        public Guid Id { get; set; }

        public Region Region { get; set; }

        public DateTime OrderDate { get; set; }

        public OrderStatus Status { get; set; }

        public List<OrderItem> OrderItems { get; set; }

        public Order(List<OrderItem> o, Region r)
        {
            Id = Guid.NewGuid();

            Region = r;

            Status = OrderStatus.New;

            OrderDate = DateTime.Now;

            OrderItems = new List<OrderItem>();
            OrderItems.AddRange(o);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Order)) return false;

            Order other = obj as Order;

            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
