using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Practices.Unity;
using LRS.Lib;

namespace LRS.ViewAnimation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IView
    {
        #region Private Fields

        private readonly IUnityFactory _unityFactory;
        private readonly ILogisticalProcessingUnit _lpu;

        private const string _payloadFormat = "Payload_{0}";

        private List<Rectangle> _rectangles;

        #endregion

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();

            _rectangles = new List<Rectangle>();

            RobotDetails = new ConcurrentDictionary<Robot, Tuple<RobotState>>();
            PendingOrders = new ConcurrentDictionary<Order, List<Product>>();
            PendingFulfilledItems = new ConcurrentDictionary<Order, FulfilledItem>();

            _unityFactory = new UnityFactory(new UnityContainer());
            _lpu = _unityFactory.Resolve<ILogisticalProcessingUnit>();

            _lpu.InitializeView(this);

            _lpu.InitializeTasks();
        }

        #endregion

        #region Private Properties

        private ConcurrentDictionary<Robot, Tuple<RobotState>> RobotDetails { get; set; }

        private ConcurrentDictionary<Order, List<Product>> PendingOrders { get; set; }

        private ConcurrentDictionary<Order, FulfilledItem> PendingFulfilledItems { get; set; }

        #endregion

        #region IView Members

        public event FulfillmentItemCompletedEventHandler FulfillmentItemCompleted;

        public event FulfilledItemCompletedEventHandler FulfilledItemCompleted;

        protected virtual void OnFulfillmentItemCompleted(FulfillmentItemCompletedEventArgs e)
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

        public void FulfilledItemRetrieved(FulfilledItem item)
        {
            PendingFulfilledItems.AddOrUpdate(item.Order, item, (k, v) => { return item; });

            PendingOrders.AddOrUpdate(item.Order, new List<Product>(), (k, v) => { return new List<Product>(); });
        }

        public void FulfillmentItemRetrieved(FulfillmentItem item)
        {
            Task.Run(() =>
            {
                System.Threading.Thread.Sleep(3000);

                PendingOrders[item.Order].Add(item.InventoryItem.Product);

                bool isOrderFulfilled = true;
                foreach (OrderItem i in item.Order.OrderItems)
                {
                    if (!PendingOrders[item.Order].Any(x => x.Code == i.ProductCode))
                    {
                        isOrderFulfilled = false;
                        break;
                    }
                }

                if (isOrderFulfilled)
                {
                    FulfilledItem fulfilledItem;
                    PendingFulfilledItems.TryRemove(item.Order, out fulfilledItem);

                    OnFulfilledItemCompleted(new FulfilledItemCompletedEventArgs(fulfilledItem));

                    List<Product> p;
                    PendingOrders.TryRemove(item.Order, out p);
                }

                OnFulfillmentItemCompleted(new FulfillmentItemCompletedEventArgs(item));
            });
        }

        public void AddRobot(Robot robot)
        {
            AddRobotRectangle(robot.Name, robot.Coordinates, robot.RelativeWidth, robot.RelativeHeight);

            UpdateRobotDetails(robot);
        }

        public void AddPathMarker(string name, Coordinates coords)
        {
            Rectangle r = new Rectangle();

            r.Name = name;
            r.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0x5C, 0x64, 0x67));
            r.Width = 10;
            r.Height = 10;
            r.ToolTip = name;
            r.Visibility = System.Windows.Visibility.Hidden;

            Canvas.SetTop(r, coords.MarginTop);
            Canvas.SetLeft(r, coords.MarginLeft);

            //this.MainCanvas.Children.Add(r);
            this.PathMarkersCanvas.Children.Add(r);
        }

        public void RemovePayload(Guid payloadId)
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                RemovePayloadRectangle(payloadId);
            });
        }

        public void Message(string msg)
        {
            //MessageBox.Show(msg);
        }

        #endregion

        #region Event Handlers

        public void RobotError(object sender, EventArgs e)
        {
            Robot robot = sender as Robot;

            this.Dispatcher.InvokeAsync(() =>
            {
                Rectangle r = _rectangles.First(x => x.Name == robot.Name);

                r.Fill = new SolidColorBrush(Colors.Red);
            });
        }

        public void RobotStateChanged(object sender, RobotStateChangedEventArgs e)
        {
            Robot robot = sender as Robot;

            //UpdateRobotDetails(robot);
        }

        public void RobotObstacleCheck(object sender, RobotObstacleCheckEventArgs e)
        {
            Robot robot = sender as Robot;

            this.Dispatcher.InvokeAsync(() =>
            {
                Rectangle sensorRectangle;
                string name = string.Format("{0}_ObsChk", robot.Name);

                if (!_rectangles.Any(x => x.Name == name))
                {
                    sensorRectangle = new Rectangle();

                    sensorRectangle.Name = name;
                    sensorRectangle.Fill = new SolidColorBrush(Colors.White);

                    _rectangles.Add(sensorRectangle);

                    this.MainCanvas.Children.Add(sensorRectangle);
                }
                else
                {
                    sensorRectangle = _rectangles.First(x => x.Name == name);
                }

                sensorRectangle.Width = e.Orientation == Lib.Orientation.North || e.Orientation == Lib.Orientation.South ? 10 : e.Range;
                sensorRectangle.Height = e.Orientation == Lib.Orientation.East || e.Orientation == Lib.Orientation.West ? 10 : e.Range;

                if (e.Orientation == Lib.Orientation.North)
                {
                    Canvas.SetTop(sensorRectangle, e.Coordinates.MarginTop - e.Range);
                    Canvas.SetLeft(sensorRectangle, e.Coordinates.MarginLeft);
                }
                else if (e.Orientation == Lib.Orientation.East)
                {
                    Canvas.SetTop(sensorRectangle, e.Coordinates.MarginTop);
                    Canvas.SetLeft(sensorRectangle, e.Coordinates.MarginLeft);
                }
                else if (e.Orientation == Lib.Orientation.South)
                {
                    Canvas.SetTop(sensorRectangle, e.Coordinates.MarginTop);
                    Canvas.SetLeft(sensorRectangle, e.Coordinates.MarginLeft);
                }
                else if (e.Orientation == Lib.Orientation.West)
                {
                    Canvas.SetTop(sensorRectangle, e.Coordinates.MarginTop);
                    Canvas.SetLeft(sensorRectangle, e.Coordinates.MarginLeft - e.Range);
                }
            });
        }

        public void RobotPositionChanged(object sender, RobotPositionChangedEventArgs args)
        {
            Robot robot = sender as Robot;

            this.Dispatcher.InvokeAsync(() =>
            {
                //// Show sensors 

                //Rectangle sensorRectangle;
                //string sensorRectangleName = string.Format("{0}_sensor", robot.Name);

                //if (!_rectangles.Any(x => x.Name == sensorRectangleName))
                //{
                //    sensorRectangle = new Rectangle();

                //    sensorRectangle.Name = sensorRectangleName;
                //    sensorRectangle.Fill = new SolidColorBrush(Colors.White);

                //    _rectangles.Add(sensorRectangle);

                //    this.MainCanvas.Children.Add(sensorRectangle);
                //}
                //else
                //{
                //    sensorRectangle = _rectangles.First(x => x.Name == sensorRectangleName);
                //}

                //sensorRectangle.Width = robot.Orientation == Lib.Orientation.North || robot.Orientation == Lib.Orientation.South ? 10 : robot.SensingDistance;
                //sensorRectangle.Height = robot.Orientation == Lib.Orientation.East || robot.Orientation == Lib.Orientation.West ? 10 : robot.SensingDistance;

                //if ((robot.RunDirection == Direction.Forward && robot.Orientation == Lib.Orientation.North)
                //    || (robot.RunDirection == Direction.Reverse && robot.Orientation == Lib.Orientation.South))
                //{
                //    Canvas.SetTop(sensorRectangle, robot.SensingCoordinates.MarginTop - robot.SensingDistance);
                //    Canvas.SetLeft(sensorRectangle, robot.SensingCoordinates.MarginLeft);
                //}
                //else if ((robot.RunDirection == Direction.Forward && robot.Orientation == Lib.Orientation.East)
                //    || (robot.RunDirection == Direction.Reverse && robot.Orientation == Lib.Orientation.West))
                //{
                //    Canvas.SetTop(sensorRectangle, robot.SensingCoordinates.MarginTop);
                //    Canvas.SetLeft(sensorRectangle, robot.SensingCoordinates.MarginLeft);
                //}
                //else if ((robot.RunDirection == Direction.Forward && robot.Orientation == Lib.Orientation.South)
                //    || (robot.RunDirection == Direction.Reverse && robot.Orientation == Lib.Orientation.North))
                //{
                //    Canvas.SetTop(sensorRectangle, robot.SensingCoordinates.MarginTop);
                //    Canvas.SetLeft(sensorRectangle, robot.SensingCoordinates.MarginLeft);
                //}
                //else if ((robot.RunDirection == Direction.Forward && robot.Orientation == Lib.Orientation.West)
                //    || (robot.RunDirection == Direction.Reverse && robot.Orientation == Lib.Orientation.East))
                //{
                //    Canvas.SetTop(sensorRectangle, robot.SensingCoordinates.MarginTop);
                //    Canvas.SetLeft(sensorRectangle, robot.SensingCoordinates.MarginLeft - robot.SensingDistance);
                //}

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////////////////////////////////////////

                Rectangle robotRectangle = _rectangles.First(x => x.Name == robot.Name);

                if (args.Direction == Direction.Forward || args.Direction == Direction.Reverse)
                {
                    Canvas.SetTop(robotRectangle, args.Coordinates.MarginTop);
                    Canvas.SetLeft(robotRectangle, args.Coordinates.MarginLeft);

                    if (robot.HasPayload)
                    {
                        Rectangle payload = AddPayloadRectangle(robot.PayloadId, robot.PayloadCoordinates);

                        Canvas.SetTop(payload, robot.PayloadCoordinates.MarginTop);
                        Canvas.SetLeft(payload, robot.PayloadCoordinates.MarginLeft);
                    }
                }
                else if (args.Direction == Direction.Left || args.Direction == Direction.Right)
                {
                    RemoveRobotRectangle(robot.Name);
                    AddRobotRectangle(robot.Name, args.Coordinates, robot.RelativeWidth, robot.RelativeHeight);

                    if (robot.HasPayload)
                    {
                        RemovePayloadRectangle(robot.PayloadId);

                        AddPayloadRectangle(robot.PayloadId, robot.PayloadCoordinates);
                    }
                }

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////////////////////////////////////////

                UpdateRobotDetails(robot);
            });
        }

        protected override void OnClosed(EventArgs e)
        {
            _lpu.ShutdownTasks();

            base.OnClosed(e);
        }

        private void NewOrder_Click(object sender, RoutedEventArgs e)
        {
            NewOrders();
        }

        private void Shutdown_Click(object sender, RoutedEventArgs e)
        {
            _lpu.ShutdownTasks();
        }

        private void ShowPathMarkers_Checked(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                this.Dispatcher.InvokeAsync(() =>
                {
                    foreach (var r in PathMarkersCanvas.Children)
                    {
                        if (r is Rectangle) (r as Rectangle).Visibility = Visibility.Visible;
                    }
                });
            });
        }

        private void ShowPathMarkers_Unchecked(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                this.Dispatcher.InvokeAsync(() =>
                {
                    foreach (var r in PathMarkersCanvas.Children)
                    {
                        if (r is Rectangle) (r as Rectangle).Visibility = Visibility.Collapsed;
                    }
                });
            });
        }

        #endregion

        #region Private Methods

        private void AddRobotRectangle(string name, Coordinates coords, double width, double height)
        {
            Rectangle r = new Rectangle();
            
            r.Name = name;
            r.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0xEC, 0x76, 0x00));
            r.Width = width;
            r.Height = height;
            r.ToolTip = name;

            _rectangles.Add(r);

            Canvas.SetTop(r, coords.MarginTop);
            Canvas.SetLeft(r, coords.MarginLeft);

            this.MainCanvas.Children.Add(r);
        }

        private void RemoveRobotRectangle(string name)
        {
            Rectangle r = _rectangles.First(x => x.Name == name);
            int i = this.MainCanvas.Children.IndexOf(r);
            this.MainCanvas.Children.RemoveAt(i);

            _rectangles = _rectangles.Where(x => x.Name != name).ToList();
        }

        private Rectangle AddPayloadRectangle(Guid payloadId, Coordinates coords)
        {
            string name = string.Format(_payloadFormat, payloadId.GetHashCode().ToString().Replace('-', 'N'));

            if (this.MainCanvas.FindName(name) == null)
            {
                Rectangle r = new Rectangle();

                r.Name = name;
                r.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0x8B, 0x5C, 0x35));
                r.Stroke = new SolidColorBrush(Colors.Black);
                r.Width = r.Height = 10;

                Canvas.SetTop(r, coords.MarginTop);
                Canvas.SetLeft(r, coords.MarginLeft);

                this.RegisterName(name, r);

                this.MainCanvas.Children.Add(r);

                return r;
            }
            else
            {
                return this.MainCanvas.FindName(name) as Rectangle;
            }
        }

        private void RemovePayloadRectangle(Guid payloadId)
        {
            string name = string.Format(_payloadFormat, payloadId.GetHashCode().ToString().Replace('-', 'N'));

            if (this.MainCanvas.FindName(name) != null)
            {
                Rectangle r = this.MainCanvas.FindName(name) as Rectangle;

                int i = this.MainCanvas.Children.IndexOf(r);
                
                this.MainCanvas.Children.RemoveAt(i);

                this.UnregisterName(name);
            }
        }

        private void UpdateRobotDetails(Robot robot)
        {
            var details = new Tuple<RobotState>(robot.State);

            RobotDetails.AddOrUpdate(robot, details, (k, v) => details);

            this.Dispatcher.InvokeAsync(() =>
            {
                this.RobotDetailsCanvas.Children.Clear();

                foreach (var d in RobotDetails)
                {
                    TextBlock b = new TextBlock();
                    b.Height = 20;
                    b.Width = this.RobotDetailsCanvas.Width;
                    b.Padding = new Thickness(5, 0, 0, 0);
                    b.Foreground = new SolidColorBrush(Colors.White);
                    b.Text = string.Format("{0} {1} {2} {3} {4}", d.Key.Name, d.Key.State, d.Key.SensingDistance, d.Key.NoObstacles, d.Key.RunDirection);

                    Canvas.SetLeft(b, 0);
                    Canvas.SetTop(b, 20 * this.RobotDetailsCanvas.Children.Count);

                    this.RobotDetailsCanvas.Children.Add(b);
                }
            });
        }

        #endregion

        private void NewOrders()
        {
            Region r1 = Region.Region1;
            Region r2 = Region.Region2;
            Region r3 = Region.Region3;

            List<OrderItem> o1 = new List<OrderItem>();
            o1.Add(new OrderItem { ProductCode = "PTR-RICE", Quantity = 5 });
            //o1.Add(new OrderItem { ProductCode = "PTR-BEAN", Quantity = 10 });
            //o1.Add(new OrderItem { ProductCode = "PTR-CSPC", Quantity = 20 });

            List<OrderItem> o2 = new List<OrderItem>();
            o2.Add(new OrderItem { ProductCode = "PTR-SCBL", Quantity = 10 });
            o2.Add(new OrderItem { ProductCode = "PTR-FTBL", Quantity = 10 });
            o2.Add(new OrderItem { ProductCode = "PTR-BSBL", Quantity = 10 });
            o2.Add(new OrderItem { ProductCode = "PTR-BKBL", Quantity = 10 });
            o2.Add(new OrderItem { ProductCode = "PTR-TNBL", Quantity = 10 });

            List<OrderItem> o3 = new List<OrderItem>();
            o3.Add(new OrderItem { ProductCode = "PTR-CHRS", Quantity = 10 });
            o3.Add(new OrderItem { ProductCode = "PTR-TBLS", Quantity = 10 });
            o3.Add(new OrderItem { ProductCode = "PTR-LMPS", Quantity = 10 });
            o3.Add(new OrderItem { ProductCode = "PTR-DSKS", Quantity = 10 });

            Task.Run(() =>
            {
                _lpu.NewOrder(o1, r1);

                //Thread.Sleep(1000);

                //_lpu.NewOrder(o2, r1);

                //Thread.Sleep(1000);

                //_lpu.NewOrder(o3, r2);

                //Thread.Sleep(1000);
            });
        }
    }
}
