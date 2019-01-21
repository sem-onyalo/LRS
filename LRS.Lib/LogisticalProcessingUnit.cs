using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using LRS.Repository;

namespace LRS.Lib
{
    public class LogisticalProcessingUnit : ILogisticalProcessingUnit
    {
        #region Private Fields

        private IWorld _world;
        private readonly IRobotManager _robotManager;
        private readonly IPathRepository _pathRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IFulfilledRepository _fulfilledRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IFulfillmentRepository _fulfillmentRepository;
        private readonly ILoadingDockRepository _loadingDockRepository;

        private readonly int _taskFrequency = 1000;
        private readonly string _pickupMarker = "P5";

        private List<Task> _tasks;
        private CancellationTokenSource _taskTokenSource;
        
        #endregion

        #region Constructors

        public LogisticalProcessingUnit(IWorld world, IRobotManager robotManager, IPathRepository pathRepository, IOrderRepository orderRepository, IProductRepository productRepository, IFulfilledRepository fulfilledRepository, IInventoryRepository inventoryRepository, IFulfillmentRepository fulfillmentRepository, ILoadingDockRepository loadingDockRepository)
        {
            if (world == null) throw new ArgumentNullException("world");
            if (robotManager == null) throw new ArgumentNullException("robotManager");
            if (pathRepository == null) throw new ArgumentNullException("pathRepository");
            if (orderRepository == null) throw new ArgumentNullException("orderRepository");
            if (productRepository == null) throw new ArgumentNullException("productRepository");
            if (fulfilledRepository == null) throw new ArgumentNullException("fulfilledRepository");
            if (inventoryRepository == null) throw new ArgumentNullException("inventoryRepository");
            if (fulfillmentRepository == null) throw new ArgumentNullException("fulfillmentRepository");
            if (loadingDockRepository == null) throw new ArgumentNullException("loadingDockRepository");

            _world = world;
            _robotManager = robotManager;
            _pathRepository = pathRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _fulfilledRepository = fulfilledRepository;
            _inventoryRepository = inventoryRepository;
            _fulfillmentRepository = fulfillmentRepository;
            _loadingDockRepository = loadingDockRepository;

            _tasks = new List<Task>();

            Initialize();
        }

        #endregion

        #region Event Handlers

        protected void FulfilledItemRetrieved(object sender, FulfilledItemRetrievedEventArgs args)
        {
            View.FulfilledItemRetrieved(args.FulfilledItem);
        }

        protected void FulfilledItemCompleted(object sender, FulfilledItemCompletedEventArgs args)
        {
            args.FulfilledItem.Status = FulfillItemStatus.DelieverItem;
            CompleteFulfilledItemQueue.Enqueue(args.FulfilledItem);
        }

        protected void FulfilledItemStored(object sender, FulfilledItemStoredEventArgs args)
        {
            _fulfilledRepository.Lock();
            FulfilledSpot spot = _fulfilledRepository.GetFulfilledSpot(args.FulfilledItem.FulfilledSpot.Id);
            spot.Status = FulfillSpotStatus.Available;
            _fulfilledRepository.Update(spot);
            _fulfilledRepository.Unlock();

            View.RemovePayload(args.FulfilledItem.Id);
        }

        protected void FulfillmentItemRetrieved(object sender, FulfillmentItemRetrievedEventArgs args)
        {
            View.FulfillmentItemRetrieved(args.FulfillmentItem);
        }

        protected void FulfillmentItemCompleted(object sender, FulfillmentItemCompletedEventArgs args)
        {
            args.FulfillmentItem.Status = FulfillItemStatus.DelieverItem;
            _robotManager.FulfillmentItemQueue.Enqueue(args.FulfillmentItem);
        }

        protected void FulfillmentItemStored(object sender, FulfillmentItemStoredEventArgs args)
        {
            _fulfillmentRepository.Lock();
            FulfillmentSpot spot = _fulfillmentRepository.GetFulfillmentSpot(args.FulfillmentItem.FulfillmentSpot.Id);
            spot.Status = FulfillSpotStatus.Available;
            _fulfillmentRepository.Update(spot);
            _fulfillmentRepository.Unlock();

            View.RemovePayload(args.FulfillmentItem.Id);
        }

        #endregion

        #region Public Properties

        public IView View { get; set; }

        public IRobotManager RobotManager
        {
            get { return _robotManager; }
        }

        public ConcurrentQueue<FulfilledItem> NewFulfilledItemQueue { get; private set; }

        public ConcurrentQueue<FulfilledItem> CompleteFulfilledItemQueue { get; private set; }

        public Dictionary<Region, LoadingDock> DockRegionMapping { get; private set; }

        #endregion

        #region Public Methods

        public void InitializeView(IView view)
        {
            View = view;

            IEnumerable<Robot> robots = _robotManager.GetRobots();
            foreach (Robot r in robots)
            {
                View.AddRobot(r);

                r.RobotPositionChanged += View.RobotPositionChanged;
                r.RobotStateChanged += View.RobotStateChanged;
                r.RobotError += View.RobotError;
            }

            foreach (PathMarker m in _world.PathMarkers)
            {
                View.AddPathMarker(m.Name, m.Coordinates);
            }

            View.FulfilledItemCompleted += this.FulfilledItemCompleted;
            View.FulfillmentItemCompleted += this.FulfillmentItemCompleted;

            View.Message("System initialized");
        }

        public void InitializeTasks()
        {
            _taskTokenSource = new CancellationTokenSource();

            Task processNewOrders = Task.Factory.StartNew(() =>
            {
                while (!_taskTokenSource.IsCancellationRequested)
                {
                    this.ProcessNewOrders();

                    Thread.Sleep(_taskFrequency);
                }
            }, TaskCreationOptions.LongRunning);

            Task processNewFulfilledItems = Task.Factory.StartNew(() =>
            {
                while (!_taskTokenSource.IsCancellationRequested)
                {
                    this.ProcessNewFulfilledItemQueue();

                    Thread.Sleep(_taskFrequency);
                }
            }, TaskCreationOptions.LongRunning);

            Task processNewFulfillmentItems = Task.Factory.StartNew(() =>
            {
                while (!_taskTokenSource.IsCancellationRequested)
                {
                    this.ProcessNewFulfillmentItemQueue();

                    Thread.Sleep(_taskFrequency);
                }
            }, TaskCreationOptions.LongRunning);

            Task processCompleteFulfilledItems = Task.Factory.StartNew(() =>
            {
                while (!_taskTokenSource.IsCancellationRequested)
                {
                    this.ProcessCompleteFulfilledItemQueue();

                    Thread.Sleep(_taskFrequency);
                }
            }, TaskCreationOptions.LongRunning);

            Task processFulfilledItems = Task.Factory.StartNew(() =>
            {
                while (!_taskTokenSource.IsCancellationRequested)
                {
                    this.RobotManager.ProcessFulfilledItemQueue();

                    Thread.Sleep(_taskFrequency);
                }
            }, TaskCreationOptions.LongRunning);

            Task processFulfillmentItems = Task.Factory.StartNew(() =>
            {
                while (!_taskTokenSource.IsCancellationRequested)
                {
                    this.RobotManager.ProcessFulfillmentItemQueue();

                    Thread.Sleep(_taskFrequency);
                }
            }, TaskCreationOptions.LongRunning);

            _tasks.Add(processNewOrders);
            _tasks.Add(processNewFulfilledItems);
            _tasks.Add(processNewFulfillmentItems);
            _tasks.Add(processCompleteFulfilledItems);
            _tasks.Add(processFulfilledItems);
            _tasks.Add(processFulfillmentItems);
        }

        public void ShutdownTasks()
        {
            _taskTokenSource.Cancel();

            Task.WaitAll(_tasks.ToArray());
        }

        public Guid NewOrder(List<OrderItem> orderItems, Region region)
        {
            Order order = new Order(orderItems, region);

            _orderRepository.Add(order);

            return order.Id;
        }

        public void ProcessNewOrders()
        {
            _orderRepository.Lock();
            List<Order> orders = _orderRepository.GetByStatus(OrderStatus.New).ToList();
            _orderRepository.Unlock();

            foreach (Order o in orders)
            {
                ProcessNewOrder(o);
            }
        }

        public void ProcessNewOrder(Order o)
        {
            FulfilledItem fulfilledItem = new FulfilledItem();
            fulfilledItem.Id = Guid.NewGuid();
            fulfilledItem.Order = o;
            NewFulfilledItemQueue.Enqueue(fulfilledItem);

            foreach (string code in o.OrderItems.Select(x => x.ProductCode))
            {
                InventoryItem item = _inventoryRepository.GetItemByProductCode(code);

                FulfillmentItem fulfillmentItem = new FulfillmentItem();
                fulfillmentItem.Id = Guid.NewGuid();
                fulfillmentItem.Order = o;
                fulfillmentItem.InventoryItem = item;
                _fulfillmentRepository.NewFulfillmentItemQueue.Enqueue(fulfillmentItem);
            }

            _orderRepository.Lock();
            o.Status = OrderStatus.Queued;
            _orderRepository.Update(o);
            _orderRepository.Unlock();
        }

        public void ProcessNewFulfilledItemQueue()
        {
            while (NewFulfilledItemQueue.Any())
            {
                _fulfilledRepository.Lock();
                
                FulfilledSpot spot = _fulfilledRepository
                    .GetFulfilledSpotsByStatus(FulfillSpotStatus.Available)
                    .FirstOrDefault();

                if (spot != null)
                {
                    spot.Status = FulfillSpotStatus.Reserved;
                    _fulfilledRepository.Update(spot);
                    _fulfilledRepository.Unlock();
                }
                else
                {
                    _fulfilledRepository.Unlock();
                    break;
                }

                FulfilledItem item;
                if (NewFulfilledItemQueue.TryDequeue(out item))
                {
                    item.FulfilledSpot = spot;
                    item.Status = FulfillItemStatus.RetrieveItem;
                    _robotManager.FulfilledItemQueue.Enqueue(item);

                    _orderRepository.Lock();
                    Order order = _orderRepository.Get(item.Order.Id);
                    order.Status = OrderStatus.Fulfilling;
                    _orderRepository.Update(order);
                    _orderRepository.Unlock();
                }
            }
        }

        public void ProcessNewFulfillmentItemQueue()
        {
            while (_fulfillmentRepository.NewFulfillmentItemQueue.Any())
            {
                FulfillmentItem item = _fulfillmentRepository.NewFulfillmentItemQueue.Peek();
                Order order = _orderRepository.Get(item.Order.Id);

                if (order.Status != OrderStatus.Fulfilling) break;

                _fulfillmentRepository.Lock();
                
                FulfillmentSpot spot = _fulfillmentRepository
                    .GetFulfillmentSpotsByStatus(FulfillSpotStatus.Available)
                    .FirstOrDefault();

                if (spot != null)
                {
                    spot.Status = FulfillSpotStatus.Reserved;
                    _fulfillmentRepository.Update(spot);
                    _fulfillmentRepository.Unlock();
                }
                else
                {
                    _fulfillmentRepository.Unlock();
                    break;
                }

                item = _fulfillmentRepository.NewFulfillmentItemQueue.Dequeue();

                item.FulfillmentSpot = spot;
                item.Status = FulfillItemStatus.RetrieveItem;
                _robotManager.FulfillmentItemQueue.Enqueue(item);
            }
        }

        public void ProcessCompleteFulfilledItemQueue()
        {
            while (CompleteFulfilledItemQueue.Any())
            {
                FulfilledItem item;
                if (CompleteFulfilledItemQueue.TryDequeue(out item))
                {
                    item.LoadingDock = DockRegionMapping[item.Order.Region];
                    item.Status = FulfillItemStatus.DelieverItem;
                    _robotManager.FulfilledItemQueue.Enqueue(item);
                }
            }
        }

        #endregion

        #region Private Methods

        private void Initialize()
        {
            DockRegionMapping = new Dictionary<Region, LoadingDock>();
            NewFulfilledItemQueue = new ConcurrentQueue<FulfilledItem>();
            CompleteFulfilledItemQueue = new ConcurrentQueue<FulfilledItem>();

            XmlDocument config = new XmlDocument();
            config.Load(string.Format("{0}\\{1}", System.IO.Directory.GetCurrentDirectory(), "config.xml"));
            foreach (XmlNode node in config["configuration"].ChildNodes)
            {
                switch (node.Name)
                {
                    case "pathMarkers":
                        _world.PathMarkers.AddRange(GetPathMarkers(node));
                        break;

                    case "fulfillmentSpots":
                        _fulfillmentRepository.AddRange(GetFulfillmentSpots(node));
                        break;

                    case "fulfilledSpots":
                        _fulfilledRepository.AddRange(GetFulfilledSpots(node));
                        break;

                    case "loadingDocks":
                        _loadingDockRepository.AddRange(GetLoadingDocks(node));
                        break;

                    case "robots":
                        _robotManager.AddRobots(GetRobots(node));
                        break;

                    case "products":
                        _productRepository.AddRange(GetProducts(node));
                        break;

                    case "inventoryItems":
                        _inventoryRepository.AddRange(GetInventoryItems(node));
                        break;

                    case"loadingDockRegionMappings":
                        AddLoadingDockRegionMappings(node);
                        break;
                }
            }
        }

        private IEnumerable<PathMarker> GetPathMarkers(XmlNode pathMarkerRootNode)
        {
            List<PathMarker> markers = new List<PathMarker>();

            foreach (XmlNode pathMarkerNode in pathMarkerRootNode.ChildNodes)
            {
                if (pathMarkerNode.Name == "pathMarker")
                {
                    var name = pathMarkerNode["name"].InnerText;
                    var type = (PathMarkerType)Enum.Parse(typeof(PathMarkerType), pathMarkerNode["type"].InnerText);
                    var marginLeft = pathMarkerNode["coordinates"]["marginLeft"].InnerText;
                    var marginTop = pathMarkerNode["coordinates"]["marginTop"].InnerText;
                    var coords = new Coordinates(Convert.ToDouble(marginLeft), Convert.ToDouble(marginTop));

                    PathMarker marker = new PathMarker(name, type, coords);

                    foreach (XmlNode northPathMarkersNode in pathMarkerNode["northPathMarkers"].ChildNodes)
                    {
                        string markerName = northPathMarkersNode.InnerText;
                        marker.NorthPathMarkers.Add(markerName);
                    }

                    foreach (XmlNode eastPathMarkersNode in pathMarkerNode["eastPathMarkers"].ChildNodes)
                    {
                        string markerName = eastPathMarkersNode.InnerText;
                        marker.EastPathMarkers.Add(markerName);
                    }

                    foreach (XmlNode southPathMarkersNode in pathMarkerNode["southPathMarkers"].ChildNodes)
                    {
                        string markerName = southPathMarkersNode.InnerText;
                        marker.SouthPathMarkers.Add(markerName);
                    }

                    foreach (XmlNode westPathMarkersNode in pathMarkerNode["westPathMarkers"].ChildNodes)
                    {
                        string markerName = westPathMarkersNode.InnerText;
                        marker.WestPathMarkers.Add(markerName);
                    }

                    markers.Add(marker);
                }
            }

            return markers;
        }

        private IEnumerable<FulfillmentSpot> GetFulfillmentSpots(XmlNode fulfillmentSpotRootNode)
        {
            List<FulfillmentSpot> spots = new List<FulfillmentSpot>();

            foreach (XmlNode fulfillmentSpotNode in fulfillmentSpotRootNode.ChildNodes)
            {
                if (fulfillmentSpotNode.Name == "fulfillmentSpot")
                {
                    FulfillmentSpot spot = new FulfillmentSpot();

                    spot.Status = (FulfillSpotStatus)Enum.Parse(typeof(FulfillSpotStatus), fulfillmentSpotNode["status"].InnerText);
                    spot.DestinationMarker = _world.PathMarkers.First(x => x.Name == fulfillmentSpotNode["destinationMarker"].InnerText);

                    spots.Add(spot);
                }
            }

            return spots;
        }

        private IEnumerable<FulfilledSpot> GetFulfilledSpots(XmlNode fulfilledSpotRootNode)
        {
            List<FulfilledSpot> spots = new List<FulfilledSpot>();

            foreach (XmlNode fulfilledSpotNode in fulfilledSpotRootNode)
            {
                if (fulfilledSpotNode.Name == "fulfilledSpot")
                {
                    FulfilledSpot spot = new FulfilledSpot();

                    spot.Status = (FulfillSpotStatus)Enum.Parse(typeof(FulfillSpotStatus), fulfilledSpotNode["status"].InnerText);
                    spot.DestinationMarker = _world.PathMarkers.First(x => x.Name == fulfilledSpotNode["destinationMarker"].InnerText);

                    spots.Add(spot);
                }
            }

            return spots;
        }

        private IEnumerable<LoadingDock> GetLoadingDocks(XmlNode loadingDockRootNode)
        {
            List<LoadingDock> docks = new List<LoadingDock>();

            foreach (XmlNode loadingDockNode in loadingDockRootNode)
            {
                if (loadingDockNode.Name == "loadingDock")
                {
                    LoadingDock dock = new LoadingDock();

                    dock.Name = loadingDockNode["name"].InnerText;
                    dock.Status = (LoadingDockStatus)Enum.Parse(typeof(LoadingDockStatus), loadingDockNode["status"].InnerText);
                    dock.DestinationMarker = _world.PathMarkers.First(x => x.Name == loadingDockNode["destinationMarker"].InnerText);

                    docks.Add(dock);
                }
            }

            return docks;
        }

        private IEnumerable<Robot> GetRobots(XmlNode robotRootNode)
        {
            List<Robot> robots = new List<Robot>();

            foreach (XmlNode robotNode in robotRootNode)
            {
                if (robotNode.Name == "robot")
                {
                    Robot robot = new Robot(ref _world);

                    robot.Name = robotNode["name"].InnerText;
                    robot.PickupMarker = _world.PathMarkers.First(x => x.Name == _pickupMarker);
                    robot.HomeMarker = _world.PathMarkers.First(x => x.Name == robotNode["homeMarker"].InnerText);
                    robot.Orientation = (Orientation)Enum.Parse(typeof(Orientation), robotNode["orientation"].InnerText);

                    var marginLeft = robotNode["coordinates"]["marginLeft"].InnerText;
                    var marginTop = robotNode["coordinates"]["marginTop"].InnerText;
                    robot.Coordinates = new Coordinates(Convert.ToDouble(marginLeft), Convert.ToDouble(marginTop));

                    robot.RobotPositionChanged += _world.RobotPositionChanged;

                    robot.FulfilledItemStored += this.FulfilledItemStored;
                    robot.FulfillmentItemStored += this.FulfillmentItemStored;
                    robot.FulfilledItemRetrieved += this.FulfilledItemRetrieved;
                    robot.FulfillmentItemRetrieved += this.FulfillmentItemRetrieved;

                    robots.Add(robot);
                }
            }

            return robots;
        }

        private IEnumerable<Product> GetProducts(XmlNode productRootNode)
        {
            List<Product> products = new List<Product>();

            foreach (XmlNode productNode in productRootNode)
            {
                if (productNode.Name == "product")
                {
                    Product product = new Product();

                    product.Code = productNode["code"].InnerText;
                    product.Name = productNode["name"].InnerText;

                    products.Add(product);
                }
            }

            return products;
        }

        private IEnumerable<InventoryItem> GetInventoryItems(XmlNode inventoryItemRootNode)
        {
            List<InventoryItem> items = new List<InventoryItem>();

            foreach (XmlNode itemNode in inventoryItemRootNode)
            {
                if (itemNode.Name == "inventoryItem")
                {
                    InventoryItem item = new InventoryItem();
                    item.Product = _productRepository.GetByCode(itemNode["productCode"].InnerText);
                    item.Quantity = Convert.ToInt32(itemNode["quantity"].InnerText);
                    item.DestinationMarker = _world.PathMarkers.First(x => x.Name == itemNode["destinationMarker"].InnerText);

                    items.Add(item);
                }
            }

            return items;
        }

        private void AddLoadingDockRegionMappings(XmlNode mappingRootNode)
        {
            foreach (XmlNode mappingNode in mappingRootNode)
            {
                if (mappingNode.Name == "mapping")
                {
                    Region region = (Region)Enum.Parse(typeof(Region), mappingNode["region"].InnerText);
                    LoadingDock dock = _loadingDockRepository.GetByName(mappingNode["loadingDock"].InnerText);

                    DockRegionMapping.Add(region, dock);
                }
            }
        }

        #endregion
    }
}
