using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRS.Lib
{
    public interface ILogisticalProcessingUnit
    {
        IView View { get; set; }

        void InitializeView(IView view);

        void InitializeTasks();

        void ShutdownTasks();

        Guid NewOrder(List<OrderItem> orderItems, Region region);

        void ProcessNewOrders();

        void ProcessNewFulfilledItemQueue();

        void ProcessNewFulfillmentItemQueue();

        void ProcessCompleteFulfilledItemQueue();
    }
}
