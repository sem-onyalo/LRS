using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LRS.Lib;

namespace LRS.Lib
{
    public class RobotManager : IRobotManager
    {
        #region Private Fields

        private static List<Robot> _robots = new List<Robot>();

        #endregion

        #region Constructors

        public RobotManager()
        {
            FulfilledItemQueue = new Queue<FulfilledItem>();
            FulfillmentItemQueue = new Queue<FulfillmentItem>();
        }

        #endregion

        #region Public Properties

        public Queue<FulfilledItem> FulfilledItemQueue { get; private set; }

        public Queue<FulfillmentItem> FulfillmentItemQueue { get; private set; }

        #endregion

        #region Public Methods

        public void ProcessFulfilledItemQueue()
        {
            while (FulfilledItemQueue.Any())
            {
                FulfilledItem item = FulfilledItemQueue.Peek();

                if (item.Status == FulfillItemStatus.RetrieveItem)
                {
                    Robot robot = GetAvailableRobot();
                    
                    // No available robot so break from the loop and wait until one is available
                    if (robot == null) break;

                    item = FulfilledItemQueue.Dequeue();

                    // Launch robot as a thread
                    Task.Run(() => robot.RetrieveItem(item));
                }
                else if (item.Status == FulfillItemStatus.DelieverItem)
                {
                    Robot robot = GetAvailableRobot();

                    // No available robot so break from the loop and wait until one is available
                    if (robot == null) break;

                    item = FulfilledItemQueue.Dequeue();

                    // Launch robot as a thread
                    Task.Run(() => robot.DeliverItem(item));
                }
                else
                {
                    throw new Exception("Unsupported fulfilled item status found");
                }
            }
        }

        public void ProcessFulfillmentItemQueue()
        {
            while (FulfillmentItemQueue.Any())
            {
                FulfillmentItem item = FulfillmentItemQueue.Peek();

                if (item.Status == FulfillItemStatus.RetrieveItem)
                {
                    Robot robot = GetAvailableRobot();

                    // No available robot so break from the loop and wait until one is available
                    if (robot == null) break;

                    item = FulfillmentItemQueue.Dequeue();

                    // Launch robot as a thread
                    Task.Run(() => robot.RetrieveItem(item));
                }
                else if (item.Status == FulfillItemStatus.DelieverItem)
                {
                    Robot robot = null;

                    if (_robots.Any(x => x.PayloadId == item.Id))
                    {
                        robot = _robots.First(x => x.PayloadId == item.Id);
                    }
                    else
                    {
                        robot = GetAvailableRobot();
                    }

                    // No available robot so break from the loop and wait until one is available
                    if (robot == null) break;

                    item = FulfillmentItemQueue.Dequeue();

                    // Launch robot as a thread
                    Task.Run(() => robot.DeliverItem(item));
                }
            }
        }

        public void AddRobot(Robot robot)
        {
            _robots.Add(robot);
        }

        public void AddRobots(IEnumerable<Robot> robots)
        {
            _robots.AddRange(robots);
        }
        

        public Robot GetAvailableRobot()
        {
            Robot robot = null;
            if (_robots.Any(x => x.IsAvailable))
            {
                robot = _robots.First(x => x.IsAvailable);

                //int i = _robots.FindIndex(x => x.Id == robot.Id);
                int i = _robots.IndexOf(robot);

                _robots[i].IsAvailable = false;
            }

            return robot;
        }

        public IEnumerable<Robot> GetRobots()
        {
            return _robots;
        }

        #endregion
    }
}
