using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRS.Lib
{
    public interface IRobotManager
    {
        Queue<FulfilledItem> FulfilledItemQueue { get; }

        Queue<FulfillmentItem> FulfillmentItemQueue { get; }

        void ProcessFulfilledItemQueue();

        void ProcessFulfillmentItemQueue();

        IEnumerable<Robot> GetRobots();

        void AddRobot(Robot robot);

        void AddRobots(IEnumerable<Robot> robots);
    }
}
