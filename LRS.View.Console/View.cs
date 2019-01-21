using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LRS.Lib;

// ToDo: Create model objects for the view so that it doesn't need to reference the LRS.Domain project and have a 
// dependancy on Domain objects.
namespace LRS.ConsoleApp
{
    public class View //: IView
    {
        public event FulfillmentItemCompletedEventHandler FulfillmentItemCompleted;

        public event FulfilledItemCompletedEventHandler FulfilledItemCompleted;

        protected virtual void OnFufillmentItemCompleted(FulfillmentItemCompletedEventArgs e)
        {
            FulfillmentItemCompletedEventHandler handler = FulfillmentItemCompleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnFulfilledItemCompleted(FulfilledItemCompletedEventArgs e)
        {
            FulfilledItemCompletedEventHandler handler = FulfilledItemCompleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void RobotPositionChanged(object sender, RobotPositionChangedEventArgs args)
        {
            throw new NotImplementedException();
        }

        public void NewRobot(string name, Coordinates coords, double width, double height)
        {
            throw new NotImplementedException();
        }

        public void RemovePayload(Guid payloadId)
        {
            throw new NotImplementedException();
        }

        public void FulfilledItemRetrieved(FulfilledItem item)
        {
            Console.WriteLine("Fulfilled item retrieved");
        }

        public void FulfillmentItemRetrieved(FulfillmentItem item)
        {
            Console.WriteLine("Fulfillment item retrieved");
        }

        public void Message(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
