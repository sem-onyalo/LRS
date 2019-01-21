using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRS.Lib
{
    public delegate void FulfillmentItemCompletedEventHandler(object sender, FulfillmentItemCompletedEventArgs e);

    public delegate void FulfilledItemCompletedEventHandler(object sender, FulfilledItemCompletedEventArgs e);

    public interface IView
    {
        event FulfillmentItemCompletedEventHandler FulfillmentItemCompleted;

        event FulfilledItemCompletedEventHandler FulfilledItemCompleted;

        void RobotError(object sender, EventArgs e);

        void RobotStateChanged(object sender, RobotStateChangedEventArgs e);

        void RobotPositionChanged(object sender, RobotPositionChangedEventArgs args);

        void FulfilledItemRetrieved(FulfilledItem item);

        void FulfillmentItemRetrieved(FulfillmentItem item);

        void AddRobot(Robot robot);

        void AddPathMarker(string name, Coordinates coords);

        void RemovePayload(Guid payloadId);

        void Message(string msg);
    }
}
