using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LRS.Lib
{
    public class Robot
    {
        #region Fields

        private readonly int _runForwardStep;
        private readonly int _runReverseStep;
        private readonly int _turnStep;

        private readonly int _pathMarkerSize;
        private readonly double _robotLength;
        private readonly double _robotWidth;
        private readonly double _pathWidth;

        private readonly IWorld _world;

        private double _payloadSize;

        private PathMarker _lastMarker;
        private List<PathMarker> _lastPath;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="Robot"/> instance.
        /// </summary>
        /// <param name="world"></param>
        public Robot(ref IWorld world)
        {
            if (world == null) throw new ArgumentNullException("world");

            _world = world;

            _runForwardStep = 5;
            _runReverseStep = 50;
            _turnStep = 10;

            _pathMarkerSize = 10;
            _robotLength = 20;
            _robotWidth = 10;
            _pathWidth = 10;
            _payloadSize = 0; // intially not carrying a payload

            _lastPath = new List<PathMarker>();

            Id = Guid.NewGuid();

            SensorRange = 30;
            PeripheralSensorRange = 10;
            
            IsAvailable = true;

            State = RobotState.Idle;

            PayloadId = Guid.Empty;

            NoObstacles = true;

            SensingDistance = SensorRange;
        }

        #endregion

        #region Delegates

        public delegate void RobotErrorHandler(object sender, EventArgs e);

        public delegate void RobotStateChangedHandler(object sender, RobotStateChangedEventArgs e);

        public delegate void RobotPositionChangedHandler(object sender, RobotPositionChangedEventArgs e);

        public delegate void FulfilledItemStoredEventHandler(object sender, FulfilledItemStoredEventArgs e);

        public delegate void FulfilledItemRetrievedEventHandler(object sender, FulfilledItemRetrievedEventArgs e);

        public delegate void FulfillmentItemStoredEventHandler(object sender, FulfillmentItemStoredEventArgs e);

        public delegate void FulfillmentItemRetrievedEventHandler(object sender, FulfillmentItemRetrievedEventArgs e);

        #endregion

        #region Events

        public event RobotErrorHandler RobotError;

        public event RobotStateChangedHandler RobotStateChanged;

        public event RobotPositionChangedHandler RobotPositionChanged;

        public event FulfilledItemStoredEventHandler FulfilledItemStored;

        public event FulfilledItemRetrievedEventHandler FulfilledItemRetrieved;

        public event FulfillmentItemStoredEventHandler FulfillmentItemStored;

        public event FulfillmentItemRetrievedEventHandler FulfillmentItemRetrieved;

        #endregion

        #region Event Raisers

        protected virtual void OnRobotError(EventArgs e)
        {
            RobotErrorHandler handler = RobotError;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnRobotStateChanged(RobotStateChangedEventArgs e)
        {
            RobotStateChangedHandler handler = RobotStateChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnRobotPositionChanged(RobotPositionChangedEventArgs e)
        {
            RobotPositionChangedHandler handler = RobotPositionChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnFulfilledItemStored(FulfilledItemStoredEventArgs e)
        {
            FulfilledItemStoredEventHandler handler = FulfilledItemStored;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnFulfilledItemRetrieved(FulfilledItemRetrievedEventArgs e)
        {
            FulfilledItemRetrievedEventHandler handler = FulfilledItemRetrieved;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnFulfillmentItemStored(FulfillmentItemStoredEventArgs e)
        {
            FulfillmentItemStoredEventHandler handler = FulfillmentItemStored;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnFulfillmentItemRetrieved(FulfillmentItemRetrievedEventArgs e)
        {
            FulfillmentItemRetrievedEventHandler handler = FulfillmentItemRetrieved;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// The unique identifier of the robot.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The name of the robot.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The current state of the robot.
        /// </summary>
        public RobotState State { get; set; }

        /// <summary>
        /// The width of the robot relative to its orientation.
        /// </summary>
        public double RelativeWidth
        {
            get { return Orientation == Lib.Orientation.South || Orientation == Lib.Orientation.North ? _robotWidth : _robotLength; }
        }

        /// <summary>
        /// The height of the robot relative to its orientation.
        /// </summary>
        public double RelativeHeight
        {
            get { return Orientation == Lib.Orientation.South || Orientation == Lib.Orientation.North ? _robotLength : _robotWidth; }
        }

        /// <summary>
        /// Indicates a <see cref="Robot"/> that is available to perform a task.
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// The robots home path marker.
        /// </summary>
        public PathMarker HomeMarker { get; set; }

        /// <summary>
        /// The path marker location where empty palettes are stored.
        /// </summary>
        public PathMarker PickupMarker { get; set; }

        /// <summary>
        /// The <see cref="Coordinates"/> of the robot.
        /// </summary>
        public Coordinates Coordinates { get; set; }

        /// <summary>
        /// The orientation of the robot.
        /// </summary>
        public Orientation Orientation { get; set; }
        
        /// <summary>
        /// The unique identifier of the payload being carried. Guid.Empty if no payload.
        /// </summary>
        public Guid PayloadId { get; set; }

        /// <summary>
        /// The <see cref="Coordinates"/> of this robot's payload.
        /// </summary>
        public Coordinates PayloadCoordinates { get; set; }

        /// <summary>
        /// Whether or not this <see cref="Robot"/> has a payload.
        /// </summary>
        public bool HasPayload { get { return _payloadSize > 0; } }

        /// <summary>
        /// The maximum distance which this robot can detect obstacles.
        /// </summary>
        public double SensorRange { get; set; }

        /// <summary>
        /// The maximum distance which this robot can detect obstacles in its periphery (i.e. perpendicular paths)
        /// </summary>
        public double PeripheralSensorRange { get; set; }

        public bool NoObstacles { get; set; }

        public Direction RunDirection { get; set; }

        public Direction TurnDirection { get; set; }

        public Coordinates SensingCoordinates { get; set; }

        public double SensingDistance { get; set; }

        #endregion

        #region Inherited Methods

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Robot)) return false;

            Robot other = obj as Robot;

            return this.Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets an item from the specified location and puts it at the specified spot.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="action"></param>
        public void RetrieveItem<T>(T item)
        {
            if (item is FulfilledItem)
            {
                try
                {
                    FulfilledItem fulfilledItem = item as FulfilledItem;

                    PayloadId = fulfilledItem.Id;

                    ChangeState(RobotState.Retrieving);

                    while (this.State != RobotState.Idle)
                    {
                        switch (this.State)
                        {
                            case RobotState.Retrieving:

                                Run(HomeMarker, PickupMarker, false);

                                // Add payload to robot
                                RetrievePayload(fulfilledItem.Id);

                                ChangeState(RobotState.Delivering);
                                break;

                            case RobotState.Delivering:

                                Run(_lastPath.Last(), fulfilledItem.FulfilledSpot.DestinationMarker, false);

                                ReleasePayload();

                                fulfilledItem.Status = FulfillItemStatus.ItemRetrieved;

                                FulfilledItemRetrievedEventArgs e = new FulfilledItemRetrievedEventArgs(fulfilledItem);
                                OnFulfilledItemRetrieved(e);

                                ChangeState(RobotState.GoingHome);
                                break;

                            case RobotState.GoingHome:

                                Home(_lastPath.Last(x => x.Type == PathMarkerType.Route), HomeMarker, true);

                                ChangeState(RobotState.Idle);
                                break;
                        }
                    }
                }
                catch (RobotException)
                {
                    // ToDo: handle RobotException object which indicates robot cannot move
                    OnRobotError(new EventArgs());
                }
            }
            else if (item is FulfillmentItem)
            {
                try
                {
                    FulfillmentItem fulfillmentItem = item as FulfillmentItem;

                    PayloadId = fulfillmentItem.Id;

                    ChangeState(RobotState.Retrieving);

                    while (this.State != RobotState.Idle)
                    {
                        switch (this.State)
                        {
                            case RobotState.Retrieving:

                                Run(HomeMarker, fulfillmentItem.InventoryItem.DestinationMarker, false);

                                // Add payload to robot
                                RetrievePayload(fulfillmentItem.Id);

                                ChangeState(RobotState.Delivering);
                                break;

                            case RobotState.Delivering:

                                Run(_lastPath.Last(x => x.Type == PathMarkerType.Route), fulfillmentItem.FulfillmentSpot.DestinationMarker, true);

                                fulfillmentItem.Status = FulfillItemStatus.ItemRetrieved;

                                FulfillmentItemRetrievedEventArgs e = new FulfillmentItemRetrievedEventArgs(fulfillmentItem);
                                OnFulfillmentItemRetrieved(e);

                                ChangeState(RobotState.GoingHome);
                                break;

                            case RobotState.GoingHome:

                                ChangeState(RobotState.Idle);
                                break;
                        }
                    }
                }
                catch (RobotException)
                {
                    // ToDo: handle RobotException object which indicates robot cannot move
                    OnRobotError(new EventArgs());
                }
            }
            else
            {
                throw new Exception("Unsupported item to retrieve");
            }
        }

        /// <summary>
        /// Gets an item from the specified spot and returns it to the specified location.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="action"></param>
        public void DeliverItem<T>(T item)
        {
            if (item is FulfilledItem)
            {
                try
                {
                    FulfilledItem fulfilledItem = item as FulfilledItem;

                    PayloadId = fulfilledItem.Id;

                    ChangeState(RobotState.Retrieving);

                    while (this.State != RobotState.Idle)
                    {
                        switch (this.State)
                        {
                            case RobotState.Retrieving:

                                Run(HomeMarker, fulfilledItem.FulfilledSpot.DestinationMarker, false);

                                RetrievePayload(fulfilledItem.Id);

                                ChangeState(RobotState.Delivering);
                                break;

                            case RobotState.Delivering:

                                Run(_lastPath.Last(x => x.Type == PathMarkerType.Route), fulfilledItem.LoadingDock.DestinationMarker, true);

                                ReleasePayload();

                                fulfilledItem.Status = FulfillItemStatus.ItemDelivered;

                                FulfilledItemStoredEventArgs e = new FulfilledItemStoredEventArgs(fulfilledItem);
                                OnFulfilledItemStored(e);

                                ChangeState(RobotState.GoingHome);
                                break;

                            case RobotState.GoingHome:

                                Home(_lastPath.Last(), HomeMarker, false);

                                ChangeState(RobotState.Idle);
                                break;
                        }
                    }
                }
                catch (RobotException)
                {
                    // ToDo: handle RobotException object which indicates robot cannot move
                    OnRobotError(new EventArgs());
                }
            }
            else if (item is FulfillmentItem)
            {
                try
                {
                    FulfillmentItem fulfillmentItem = item as FulfillmentItem;

                    // Already have payload
                    //PayloadId = fulfillmentItem.Id;

                    ChangeState(RobotState.Retrieving);

                    while (this.State != RobotState.Idle)
                    {
                        switch (this.State)
                        {
                            case RobotState.Retrieving:

                                ChangeState(RobotState.Delivering);
                                break;

                            case RobotState.Delivering:

                                Run(_lastPath.Last(x => x.Type == PathMarkerType.Route), fulfillmentItem.InventoryItem.DestinationMarker, true);

                                // Remove payload from robot
                                ReleasePayload();

                                fulfillmentItem.Status = FulfillItemStatus.ItemDelivered;

                                FulfillmentItemStoredEventArgs e = new FulfillmentItemStoredEventArgs(fulfillmentItem);
                                OnFulfillmentItemStored(e);

                                ChangeState(RobotState.GoingHome);
                                break;

                            case RobotState.GoingHome:

                                Home(_lastPath.Last(x => x.Type == PathMarkerType.Route), HomeMarker, true);

                                ChangeState(RobotState.Idle);
                                break;
                        }
                    }
                }
                catch (RobotException)
                {
                    // ToDo: handle RobotException object which indicates robot cannot move
                    OnRobotError(new EventArgs());
                }
            }
            else
            {
                throw new Exception("Unsupported item to store");
            }
        }

        #endregion

        #region Private Methods

        private void Run(PathMarker start, PathMarker end, bool reverseOut)
        {
            if (reverseOut)
            {
                Run(Direction.Reverse, new List<PathMarker> { _lastPath.Last(x => x.Type == PathMarkerType.Route) });
            }

            _lastPath = BuildPath(start, end);

            Run(Direction.Forward, _lastPath);
        }

        private void Run(Direction runDirection, List<PathMarker> pathMarkers)
        {
            int distance = 1;
            int idleTime = 0;
            int maxIdleTime = 2000;
            int pathMarkerIndex = 0;
            PathMarker currentPathMarker = pathMarkers[pathMarkerIndex];

            // Set initial no obstalces to true so that we always check next direction on first pass
            NoObstacles = true;

            // Set initial next direction to None which will go straight on first run
            Direction nextDirection = Direction.None;
            
            //while (!CancelToken.IsCancellationRequested)
            while (true)
            {
                // If there are no obstacles in the way then get next direction
                if (NoObstacles)
                {
                    // Reset idle time variable
                    if (idleTime > 0) idleTime = 0;

                    if (IsPathMarker(currentPathMarker))
                    {
                        _lastMarker = currentPathMarker;

                        PathMarker nextPathMarker = pathMarkerIndex + 1 < pathMarkers.Count ? pathMarkers[++pathMarkerIndex] : null;

                        if (nextPathMarker != null)
                        {
                            nextDirection = GetNextDirection(runDirection, currentPathMarker, nextPathMarker);
                        }
                        else if (currentPathMarker.Type == PathMarkerType.Destination || currentPathMarker == pathMarkers.Last())
                        {
                            return; // reached destination, exit
                        }
                        else
                        {
                            nextDirection = GetNextDirection(runDirection, distance, true);
                        }

                        currentPathMarker = nextPathMarker;
                    }
                    else
                    {
                        nextDirection = GetNextDirection(runDirection, distance, false);
                    }
                }

                // Check for obstacles 
                NoObstacles = !IsObstacle(runDirection, nextDirection);

                // If there are no obstacles in the way then move
                if (NoObstacles)
                {
                    switch (nextDirection)
                    {
                        case Direction.Forward:
                        case Direction.Reverse:
                            Move(runDirection);
                            break;

                        case Direction.Left:
                        case Direction.Right:
                            Turn(runDirection, nextDirection);
                            break;
                    }
                }
                else
                {
                    // Sleep, wait for obstacle to pass
                    Thread.Sleep(100);

                    if ((idleTime += 100) >= maxIdleTime) throw new RobotException();
                }

                RunDirection = runDirection;
                TurnDirection = nextDirection == Direction.Left || nextDirection == Direction.Right ? nextDirection : Direction.None;
            }
        }

        private void Home(PathMarker start, PathMarker end, bool reverseOut)
        {
            Run(start, end, reverseOut);

            Park();

            _lastMarker = null;

            this.IsAvailable = true;
        }

        private void Park()
        {
            int distance = 1;

            for (int i = 0; i < _robotLength - _pathMarkerSize; i++)
            {
                Move(Direction.Forward);
            }

            Turn(Direction.Reverse, Direction.Right);

            Coordinates straightPathInReverseWhenFacingSouth = new Coordinates(this.Coordinates.MarginLeft, this.Coordinates.MarginTop - distance);
            bool isPath = GetPath(straightPathInReverseWhenFacingSouth, this.Orientation) != null;

            while (isPath)
            {
                Move(Direction.Reverse);

                straightPathInReverseWhenFacingSouth = new Coordinates(this.Coordinates.MarginLeft, this.Coordinates.MarginTop - distance);
                isPath = GetPath(straightPathInReverseWhenFacingSouth, this.Orientation) != null;
            }
        }

        /// <summary>
        /// Calculates new coordinates relative to the specified information
        /// </summary>
        /// <param name="distance">The distance of the new coordinates.</param>
        /// <param name="orientation">The orientation relative to the new coordinates.</param>
        /// <param name="runDirection">The direction the robot is running (Forward or Reverse).</param>
        /// <param name="turnDirection">The direction the robot is turning (Left or Right).</param>
        /// <param name="marginLeft">The left margin from which to calculate the new coordinates</param>
        /// <param name="marginTop">The top margin from which to calculate the new coordinates</param>
        /// <returns></returns>
        private Coordinates GetNextCoordinates(int distance, Orientation orientation, Direction runDirection, Direction turnDirection, double marginLeft, double marginTop)
        {
            switch (orientation)
            {
                case Lib.Orientation.North:
                    if (runDirection == Direction.Forward && turnDirection == Direction.None) // forward and straight
                    {
                        return new Coordinates(marginLeft, marginTop - distance);
                    }
                    else if (runDirection == Direction.Forward && turnDirection == Direction.Left) // forward and left
                    {
                        return new Coordinates(marginLeft - distance, marginTop);
                    }
                    else if (runDirection == Direction.Forward && turnDirection == Direction.Right) // forward and right
                    {
                        return new Coordinates(marginLeft + _robotWidth + distance, marginTop);
                    }
                    if (runDirection == Direction.Reverse && turnDirection == Direction.None) // reverse and straight
                    {
                        return new Coordinates(marginLeft, marginTop + _robotLength + distance);
                    }
                    else if (runDirection == Direction.Reverse && turnDirection == Direction.Left) // reverse and left
                    {
                        return new Coordinates(marginLeft - distance, marginTop + _robotLength - _pathWidth);
                    }
                    else if (runDirection == Direction.Reverse && turnDirection == Direction.Right) // reverse and right
                    {
                        return new Coordinates(marginLeft + _robotWidth + distance, marginTop + _robotLength - _pathWidth);
                    }
                    else
                    {
                        throw new Exception(string.Format("Unsupported run and turn direction '{0} {1}' for 'GetNextCoordinates'", runDirection, turnDirection));
                    }

                case Lib.Orientation.East:
                    if (runDirection == Direction.Forward && turnDirection == Direction.None) // forward and straight
                    {
                        return new Coordinates(marginLeft + _robotLength + distance, marginTop);
                    }
                    else if (runDirection == Direction.Forward && turnDirection == Direction.Left) // forward and left
                    {
                        return new Coordinates(marginLeft + _robotLength - _pathWidth, marginTop - distance);
                    }
                    else if (runDirection == Direction.Forward && turnDirection == Direction.Right) // forward and right
                    {
                        return new Coordinates(marginLeft + _robotLength - _pathWidth, marginTop + _robotWidth + distance);
                    }
                    if (runDirection == Direction.Reverse && turnDirection == Direction.None) // reverse and straight
                    {
                        return new Coordinates(marginLeft - distance, marginTop);
                    }
                    else if (runDirection == Direction.Reverse && turnDirection == Direction.Left) // reverse and left
                    {
                        return new Coordinates(marginLeft, marginTop - distance);
                    }
                    else if (runDirection == Direction.Reverse && turnDirection == Direction.Right) // reverse and right
                    {
                        return new Coordinates(marginLeft, marginTop + _robotWidth + distance);
                    }
                    else
                    {
                        throw new Exception(string.Format("Unsupported run and turn direction '{0} {1}' for 'GetNextCoordinates'", runDirection, turnDirection));
                    }

                case Lib.Orientation.South:
                    if (runDirection == Direction.Forward && turnDirection == Direction.None) // forward and straight
                    {
                        return new Coordinates(marginLeft, marginTop + _robotLength + distance);
                    }
                    else if (runDirection == Direction.Forward && turnDirection == Direction.Left) // forward and left
                    {
                        return new Coordinates(marginLeft + _robotWidth + distance, marginTop + _robotLength - _pathWidth);
                    }
                    else if (runDirection == Direction.Forward && turnDirection == Direction.Right) // forward and right
                    {
                        return new Coordinates(marginLeft - distance, marginTop + _robotLength - _pathWidth);
                    }
                    if (runDirection == Direction.Reverse && turnDirection == Direction.None) // reverse and straight
                    {
                        return new Coordinates(marginLeft, marginTop - distance);
                    }
                    else if (runDirection == Direction.Reverse && turnDirection == Direction.Left) // reverse and left
                    {
                        return new Coordinates(marginLeft + _robotWidth + distance, marginTop);
                    }
                    else if (runDirection == Direction.Reverse && turnDirection == Direction.Right) // reverse and right
                    {
                        return new Coordinates(marginLeft - distance, marginTop);
                    }
                    else
                    {
                        throw new Exception(string.Format("Unsupported run and turn direction '{0} {1}' for 'GetNextCoordinates'", runDirection, turnDirection));
                    }

                case Lib.Orientation.West:
                    if (runDirection == Direction.Forward && turnDirection == Direction.None) // forward and straight
                    {
                        return new Coordinates(marginLeft - distance, marginTop);
                    }
                    else if (runDirection == Direction.Forward && turnDirection == Direction.Left) // forward and left
                    {
                        return new Coordinates(marginLeft, marginTop + _robotWidth + distance);
                    }
                    else if (runDirection == Direction.Forward && turnDirection == Direction.Right) // forward and right
                    {
                        return new Coordinates(marginLeft, marginTop - distance);
                    }
                    if (runDirection == Direction.Reverse && turnDirection == Direction.None) // reverse and straight
                    {
                        return new Coordinates(marginLeft + _robotLength + distance, marginTop);
                    }
                    else if (runDirection == Direction.Reverse && turnDirection == Direction.Left) // reverse and left
                    {
                        return new Coordinates(marginLeft + _robotLength - _pathWidth, marginTop + _robotWidth + distance);
                    }
                    else if (runDirection == Direction.Reverse && turnDirection == Direction.Right) // reverse and right
                    {
                        return new Coordinates(marginLeft + _robotLength - _pathWidth, marginTop - distance);
                    }
                    else
                    {
                        throw new Exception(string.Format("Unsupported run and turn direction '{0} {1}' for 'GetNextCoordinates'", runDirection, turnDirection));
                    }

                default:
                    throw new Exception("Unknown orientation to 'GetNextCoordinates'");
            }
        }

        private Direction GetNextDirection(Direction runDirection, int distance, bool forceTurn)
        {
            Coordinates leftPath = GetNextCoordinates(distance, this.Orientation, runDirection, Direction.Left, Coordinates.MarginLeft, Coordinates.MarginTop);
            Coordinates rightPath = GetNextCoordinates(distance, this.Orientation, runDirection, Direction.Right, Coordinates.MarginLeft, Coordinates.MarginTop);
            Coordinates straightPath = GetNextCoordinates(distance, this.Orientation, runDirection, Direction.None, Coordinates.MarginLeft, Coordinates.MarginTop);

            if (!forceTurn && GetPath(straightPath, this.Orientation) != null)
            {
                return runDirection;
            }
            else if (GetPath(leftPath, GetNextOrientation(runDirection, Direction.Left)) != null)
            {
                return Direction.Left;
            }
            else if (GetPath(rightPath, GetNextOrientation(runDirection, Direction.Right)) != null)
            {
                return Direction.Right;
            }
            else
            {
                return Direction.None;
            }
        }

        private Direction GetNextDirection(Direction runDirection, PathMarker currentPathMarker, PathMarker nextPathMarker)
        {
            Orientation nextLeftOrientation = GetNextOrientation(runDirection, Direction.Left);
            Orientation nextRightOrientation = GetNextOrientation(runDirection, Direction.Right);

            if ((this.Orientation == Lib.Orientation.North && currentPathMarker.NorthPathMarkers.Contains(nextPathMarker.Name))
                || this.Orientation == Lib.Orientation.East && currentPathMarker.EastPathMarkers.Contains(nextPathMarker.Name)
                || this.Orientation == Lib.Orientation.South && currentPathMarker.SouthPathMarkers.Contains(nextPathMarker.Name)
                || this.Orientation == Lib.Orientation.West && currentPathMarker.WestPathMarkers.Contains(nextPathMarker.Name))
            {
                return runDirection;
            }
            else if ((nextLeftOrientation == Lib.Orientation.North && currentPathMarker.NorthPathMarkers.Contains(nextPathMarker.Name))
                || (nextLeftOrientation == Lib.Orientation.East && currentPathMarker.EastPathMarkers.Contains(nextPathMarker.Name))
                || (nextLeftOrientation == Lib.Orientation.South && currentPathMarker.SouthPathMarkers.Contains(nextPathMarker.Name)
                || nextLeftOrientation == Lib.Orientation.West && currentPathMarker.WestPathMarkers.Contains(nextPathMarker.Name)))
            {
                return Direction.Left;
            }
            else if ((nextRightOrientation == Lib.Orientation.North && currentPathMarker.NorthPathMarkers.Contains(nextPathMarker.Name))
                || (nextRightOrientation == Lib.Orientation.East && currentPathMarker.EastPathMarkers.Contains(nextPathMarker.Name))
                || (nextRightOrientation == Lib.Orientation.South && currentPathMarker.SouthPathMarkers.Contains(nextPathMarker.Name)
                || nextRightOrientation == Lib.Orientation.West && currentPathMarker.WestPathMarkers.Contains(nextPathMarker.Name)))
            {
                return Direction.Right;
            }
            else
            {
                return Direction.None;
            }
        }

        private Orientation GetNextOrientation(Direction runDirection, Direction turnDirection)
        {
            if (turnDirection == Direction.None) // if not turning then continue on current orientation
                return this.Orientation;

            else if ((runDirection == Direction.Forward && this.Orientation == Lib.Orientation.North && turnDirection == Direction.Left)
                || (runDirection == Direction.Forward && this.Orientation == Lib.Orientation.South && turnDirection == Direction.Right)
                || (runDirection == Direction.Reverse && this.Orientation == Lib.Orientation.North && turnDirection == Direction.Right)
                || (runDirection == Direction.Reverse && this.Orientation == Lib.Orientation.South && turnDirection == Direction.Left))
                return Lib.Orientation.West;

            else if ((runDirection == Direction.Forward && this.Orientation == Lib.Orientation.North && turnDirection == Direction.Right)
                || (runDirection == Direction.Forward && this.Orientation == Lib.Orientation.South && turnDirection == Direction.Left)
                || (runDirection == Direction.Reverse && this.Orientation == Lib.Orientation.North && turnDirection == Direction.Left)
                || (runDirection == Direction.Reverse && this.Orientation == Lib.Orientation.South && turnDirection == Direction.Right))
                return Lib.Orientation.East;

            else if ((runDirection == Direction.Forward && this.Orientation == Lib.Orientation.East && turnDirection == Direction.Left)
                || (runDirection == Direction.Forward && this.Orientation == Lib.Orientation.West && turnDirection == Direction.Right)
                || (runDirection == Direction.Reverse && this.Orientation == Lib.Orientation.East && turnDirection == Direction.Right)
                || (runDirection == Direction.Reverse && this.Orientation == Lib.Orientation.West && turnDirection == Direction.Left))
                return Lib.Orientation.North;

            else if ((runDirection == Direction.Forward && this.Orientation == Lib.Orientation.East && turnDirection == Direction.Right)
                || (runDirection == Direction.Forward && this.Orientation == Lib.Orientation.West && turnDirection == Direction.Left)
                || (runDirection == Direction.Reverse && this.Orientation == Lib.Orientation.East && turnDirection == Direction.Left)
                || (runDirection == Direction.Reverse && this.Orientation == Lib.Orientation.West && turnDirection == Direction.Right))
                return Lib.Orientation.South;

            else
                throw new Exception("Unknown orientation and direction to 'GetNextOrientation'");
        }

        private Path GetPath(Coordinates coordinates, Orientation orientation)
        {
            switch (orientation)
            {
                case Orientation.North:
                case Orientation.South:
                    return _world.Paths.FirstOrDefault(x => 
                        x.PathDirection == orientation
                        && coordinates.MarginLeft == x.Coordinates.MarginLeft
                        && coordinates.MarginTop >= x.Coordinates.MarginTop
                        && coordinates.MarginTop <= (x.Coordinates.MarginTop + x.Length));

                case Orientation.East:
                case Orientation.West:
                    return _world.Paths.FirstOrDefault(x =>
                        x.PathDirection == orientation
                        && coordinates.MarginTop == x.Coordinates.MarginTop
                        && coordinates.MarginLeft >= x.Coordinates.MarginLeft
                        && coordinates.MarginLeft <= (x.Coordinates.MarginLeft + x.Length));

                default:
                    return null;
            }
        }

        private bool IsPathMarker(PathMarker pathMarker)
        {
            if (pathMarker == null) return false;

            switch (this.Orientation)
            {
                case Lib.Orientation.North:
                case Lib.Orientation.West:
                    return _world.PathMarkers.Any(x => x.Name == pathMarker.Name && x.Type == pathMarker.Type 
                        && x.Coordinates.Equals(this.Coordinates));

                case Lib.Orientation.South:
                    return _world.PathMarkers.Any(x => x.Name == pathMarker.Name && x.Type == pathMarker.Type
                        && x.Coordinates.MarginLeft == this.Coordinates.MarginLeft 
                        && x.Coordinates.MarginTop == this.Coordinates.MarginTop + _robotLength - _pathMarkerSize);

                case Lib.Orientation.East:
                    return _world.PathMarkers.Any(x => x.Name == pathMarker.Name && x.Type == pathMarker.Type
                        && x.Coordinates.MarginLeft == this.Coordinates.MarginLeft + _robotLength - _pathMarkerSize
                        && x.Coordinates.MarginTop == this.Coordinates.MarginTop);

                default:
                    return false;
            }
        }

        private bool IsObstacle(Direction runDirection, Direction nextDirection)
        {
            // Set the turn direction if turning, otherwise set to None
            Direction turnDirection = nextDirection == Direction.Left || nextDirection == Direction.Right ? nextDirection : Direction.None;

            int distance = 1;
            Orientation nextOrientation = GetNextOrientation(runDirection, turnDirection);
            Coordinates nextCoordinates = GetNextCoordinates(distance, nextOrientation, runDirection, turnDirection, Coordinates.MarginLeft, Coordinates.MarginTop);

            SensingCoordinates = nextCoordinates;
            SensingDistance = GetSensingDistance(runDirection, nextCoordinates, nextOrientation);

            // If there is no more path then return false
            if (SensingDistance == 0) return false;

            List<Path> nextLeftPaths = new List<Path>();
            if ((runDirection == Direction.Forward && nextOrientation == Lib.Orientation.North)
                || (runDirection == Direction.Reverse && nextOrientation == Lib.Orientation.South))
            {
                nextLeftPaths = _world.Paths
                    .Where(x => x.Coordinates.MarginTop > nextCoordinates.MarginTop - SensingDistance
                        && x.Coordinates.MarginTop + _pathWidth < nextCoordinates.MarginTop
                        && x.Coordinates.MarginLeft + x.Length > nextCoordinates.MarginLeft - PeripheralSensorRange)
                    .ToList();
            }
            else if ((runDirection == Direction.Forward && nextOrientation == Lib.Orientation.East)
                || (runDirection == Direction.Reverse && nextOrientation == Lib.Orientation.West))
            {
                nextLeftPaths = _world.Paths
                    .Where(x => x.Coordinates.MarginLeft > nextCoordinates.MarginLeft
                        && x.Coordinates.MarginLeft + _pathWidth < nextCoordinates.MarginLeft + SensingDistance
                        && x.Coordinates.MarginTop + x.Length > nextCoordinates.MarginTop - PeripheralSensorRange)
                    .ToList();
            }
            else if ((runDirection == Direction.Forward && nextOrientation == Lib.Orientation.South)
                || (runDirection == Direction.Reverse && nextOrientation == Lib.Orientation.North))
            {
                nextLeftPaths = _world.Paths
                    .Where(x => x.Coordinates.MarginTop > nextCoordinates.MarginTop
                        && x.Coordinates.MarginTop + _pathWidth < nextCoordinates.MarginTop + SensingDistance
                        && x.Coordinates.MarginLeft < nextCoordinates.MarginLeft + _pathWidth + PeripheralSensorRange)
                    .ToList();
            }
            else if ((runDirection == Direction.Forward && nextOrientation == Lib.Orientation.West)
                || (runDirection == Direction.Reverse && nextOrientation == Lib.Orientation.East))
            {
            }

            return IsObstacle(runDirection, nextCoordinates, nextOrientation, SensingDistance);

            //// Don't check for obstacles if going in reverse, leave it up to robots behind to stop
            //// ToDo: need to account for non robot obstacles (i.e. humans)
            //if (runDirection == Direction.Reverse) return false;

            //for (int distance = 1; distance <= SensorRange; distance++)
            //{
            //    Orientation nextOrientation = GetNextOrientation(runDirection, turnDirection);
            //    Coordinates nextCoordinates = GetNextCoordinates(distance, nextOrientation, runDirection, turnDirection, Coordinates.MarginLeft, Coordinates.MarginTop);

            //    if (distance <= PeripheralSensorRange)
            //    {
            //        Orientation leftOrientation = GetNextOrientation(runDirection, Direction.Left);
            //        Coordinates leftPath = GetNextCoordinates(distance, leftOrientation, runDirection, Direction.Left, nextCoordinates.MarginLeft, nextCoordinates.MarginTop);

            //        if (IsPath(leftPath, leftOrientation))
            //        {
            //            for (int peripheralDistance = 1; peripheralDistance <= PeripheralSensorRangeTurn; peripheralDistance++)
            //            {
            //                Coordinates nextPeripheralCoordinates = GetNextCoordinates(peripheralDistance, leftOrientation, runDirection, Direction.None, leftPath.MarginLeft, leftPath.MarginTop);

            //                OnRobotObstacleCheck(new RobotObstacleCheckEventArgs(nextPeripheralCoordinates, leftOrientation));
            //                if (_world.IsObstacle(nextPeripheralCoordinates)) { ObstacleDistance = distance; return true; }
            //            }
            //        }

            //        Orientation rightOrientation = GetNextOrientation(runDirection, Direction.Right);
            //        Coordinates rightPath = GetNextCoordinates(distance, rightOrientation, runDirection, Direction.Right, nextCoordinates.MarginLeft, nextCoordinates.MarginTop);

            //        if (IsPath(rightPath, rightOrientation))
            //        {
            //            for (int peripheralDistance = 1; peripheralDistance <= PeripheralSensorRangeTurn; peripheralDistance++)
            //            {
            //                Coordinates nextPeripheralCoordinates = GetNextCoordinates(peripheralDistance, rightOrientation, runDirection, Direction.None, rightPath.MarginLeft, rightPath.MarginTop);

            //                OnRobotObstacleCheck(new RobotObstacleCheckEventArgs(nextPeripheralCoordinates, rightOrientation));
            //                if (_world.IsObstacle(nextPeripheralCoordinates)) { ObstacleDistance = distance; return true; }
            //            }
            //        }
            //    }

            //    // If there is no more path then return
            //    if (!IsPath(nextCoordinates, nextOrientation)) { ObstacleDistance = distance; return false; }

            //    // Return if an obstacle has been found
            //    OnRobotObstacleCheck(new RobotObstacleCheckEventArgs(nextCoordinates, nextOrientation));
            //    if (_world.IsObstacle(nextCoordinates)) { ObstacleDistance = distance; return true; }
            //}

            //ObstacleDistance = SensorRange;
            //return false;
        }

        private bool IsObstacle(Direction runDirection, Coordinates coordinates, Orientation orientation, double sensorRange)
        {
            if ((runDirection == Direction.Forward && orientation == Lib.Orientation.North)
                || (runDirection == Direction.Reverse && orientation == Lib.Orientation.South))
            {
                return _world.Obstacles.Any(x =>
                    (x.Value.Coordinates.MarginTop + x.Value.Height) <= coordinates.MarginTop
                    && (x.Value.Coordinates.MarginTop + x.Value.Height) > coordinates.MarginTop - sensorRange
                    && x.Value.Coordinates.MarginLeft == coordinates.MarginLeft);
            }
            else if ((runDirection == Direction.Forward && orientation == Lib.Orientation.East)
                || (runDirection == Direction.Reverse && orientation == Lib.Orientation.West))
            {
                return _world.Obstacles.Any(x =>
                    x.Value.Coordinates.MarginLeft >= coordinates.MarginLeft
                    && x.Value.Coordinates.MarginLeft < coordinates.MarginLeft + sensorRange
                    && x.Value.Coordinates.MarginTop == coordinates.MarginTop);
            }
            else if ((runDirection == Direction.Forward && orientation == Lib.Orientation.South)
                || (runDirection == Direction.Reverse && orientation == Lib.Orientation.North))
            {
                return _world.Obstacles.Any(x =>
                    x.Value.Coordinates.MarginTop >= coordinates.MarginTop
                    && x.Value.Coordinates.MarginTop < coordinates.MarginTop + sensorRange
                    && x.Value.Coordinates.MarginLeft == coordinates.MarginLeft);
            }
            else if ((runDirection == Direction.Forward && orientation == Lib.Orientation.West)
                || (runDirection == Direction.Reverse && orientation == Lib.Orientation.East))
            {
                return _world.Obstacles.Any(x =>
                    (x.Value.Coordinates.MarginLeft + x.Value.Width) <= coordinates.MarginLeft
                    && (x.Value.Coordinates.MarginLeft + x.Value.Width) > coordinates.MarginLeft - sensorRange
                    && x.Value.Coordinates.MarginTop == coordinates.MarginTop);
            }
            else
            {
                return false;
            }
        }

        private double GetSensingDistance(Direction runDirection, Coordinates nextCoordinates, Orientation nextOrientation)
        {
            Path path = GetPath(nextCoordinates, nextOrientation);

            if (path == null)
            {
                return 0;
            }
            else if (((runDirection == Direction.Forward && nextOrientation == Lib.Orientation.North)
                || (runDirection == Direction.Reverse && nextOrientation == Lib.Orientation.South))
                && path.Coordinates.MarginTop > nextCoordinates.MarginTop - SensorRange)
            {
                return nextCoordinates.MarginTop - path.Coordinates.MarginTop;
            }
            else if (((runDirection == Direction.Forward && nextOrientation == Lib.Orientation.East)
                || (runDirection == Direction.Reverse && nextOrientation == Lib.Orientation.West))
                && path.Coordinates.MarginLeft + path.Length < nextCoordinates.MarginLeft + SensorRange)
            {
                return path.Coordinates.MarginLeft + path.Length - nextCoordinates.MarginLeft;
            }
            else if (((runDirection == Direction.Forward && nextOrientation == Lib.Orientation.South)
                || (runDirection == Direction.Reverse && nextOrientation == Lib.Orientation.North))
                && path.Coordinates.MarginTop + path.Length < nextCoordinates.MarginTop + SensorRange)
            {
                return path.Coordinates.MarginTop + path.Length - nextCoordinates.MarginTop;
            }
            else if (((runDirection == Direction.Forward && nextOrientation == Lib.Orientation.West)
                || (runDirection == Direction.Reverse && nextOrientation == Lib.Orientation.East))
                && path.Coordinates.MarginLeft > nextCoordinates.MarginLeft - SensorRange)
            {
                return nextCoordinates.MarginLeft - path.Coordinates.MarginLeft;
            }
            else
            {
                return SensorRange;
            }
        }

        private List<PathMarker> BuildPath(PathMarker start, PathMarker end)
        {
            List<PathMarker> path = BuildPath(start, end, end, new List<PathMarker>());

            return path;
        }

        private List<PathMarker> BuildPath(PathMarker start, PathMarker end, PathMarker current, List<PathMarker> path)
        {
            if (path.Contains(current)) return null; // we have a path looping on itself so abandon it

            path.Insert(0, current); // insert current path marker into path being built

            if (current.Equals(start)) return path; // we've built the whole path so return it

            foreach (PathMarker m in _world.PathMarkers)
            {
                bool isSibling = false;

                // find the destination marker amongst this markers siblings
                if (m.NorthPathMarkers.Contains(current.Name)) isSibling = true;
                else if (m.EastPathMarkers.Contains(current.Name)) isSibling = true;
                else if (m.SouthPathMarkers.Contains(current.Name)) isSibling = true;
                else if (m.WestPathMarkers.Contains(current.Name)) isSibling = true;

                if (isSibling)
                {
                    List<PathMarker> builtPath = BuildPath(start, end, m, path); // continue to build the path using the sibling

                    if (builtPath != null && builtPath.Contains(start)) // if the path is complete pass it up the stack
                    {
                        // ToDo: instead of returning the first built path compare all paths built for the shortest path 
                        // (would need to compare by distance)
                        return builtPath;
                    }
                    else // path is a dead end
                    {
                        int i = path.IndexOf(current);
                        path = path.Skip(i).ToList();
                    }
                }
            }

            return null; // a path could not be found so return null
        }

        private void Move(Direction runDirection)
        {
            Coordinates oldCoordinates = this.Coordinates.Clone();

            if ((this.Orientation == Lib.Orientation.North && runDirection == Direction.Forward)
                || (this.Orientation == Lib.Orientation.South && runDirection == Direction.Reverse))
            {
                this.Coordinates.MarginTop--;
            }
            else if ((this.Orientation == Lib.Orientation.East && runDirection == Direction.Forward)
                || (this.Orientation == Lib.Orientation.West && runDirection == Direction.Reverse))
            {
                this.Coordinates.MarginLeft++;
            }
            else if ((this.Orientation == Lib.Orientation.South && runDirection == Direction.Forward)
                || (this.Orientation == Lib.Orientation.North && runDirection == Direction.Reverse))
            {
                this.Coordinates.MarginTop++;
            }
            else if ((this.Orientation == Lib.Orientation.West && runDirection == Direction.Forward)
                || (this.Orientation == Lib.Orientation.East && runDirection == Direction.Reverse))
            {
                this.Coordinates.MarginLeft--;
            }
            else
            {
                throw new Exception("Unknown orientation and run direction to 'Move'");
            }

            // Set payload coordinates
            SetPayloadCoordinates();

            RobotPositionChangedEventArgs eventArg = new RobotPositionChangedEventArgs();
            eventArg.OldCoordinates = oldCoordinates;
            eventArg.Coordinates = this.Coordinates;
            eventArg.Orientation = this.Orientation;
            eventArg.Direction = runDirection;

            OnRobotPositionChanged(eventArg);

            if (runDirection == Direction.Forward)
                Thread.Sleep(_runForwardStep);
            else if (runDirection == Direction.Reverse)
                Thread.Sleep(_runReverseStep);
            else
                throw new Exception("Unsupported run direction in 'Move'");
        }

        private void Turn(Direction runDirection, Direction turnDirection)
        {
            // Turn pivot point on the robot is half way down the side on the edge
            // i.e. a robot facing north whose length is 20 and origin is at the top left corner has a pivot point at (0,10)

            Coordinates oldCoordinates = this.Coordinates.Clone();

            if (runDirection == Direction.Forward && this.Orientation == Lib.Orientation.North && turnDirection == Direction.Left)
            {
                this.Coordinates.MarginLeft -= _robotLength / 2;

                this.Orientation = Lib.Orientation.West;
            }
            else if (runDirection == Direction.Forward && this.Orientation == Lib.Orientation.North && turnDirection == Direction.Right)
            {
                this.Orientation = Lib.Orientation.East;
            }
            else if (runDirection == Direction.Forward && this.Orientation == Lib.Orientation.East && turnDirection == Direction.Left)
            {
                this.Coordinates.MarginTop -= _robotLength / 2;
                this.Coordinates.MarginLeft += _robotLength - _pathWidth;

                this.Orientation = Lib.Orientation.North;
            }
            else if (runDirection == Direction.Forward && this.Orientation == Lib.Orientation.East && turnDirection == Direction.Right)
            {
                this.Coordinates.MarginLeft += _robotLength - _pathWidth;

                this.Orientation = Lib.Orientation.South;
            }
            else if (runDirection == Direction.Forward && this.Orientation == Lib.Orientation.South && turnDirection == Direction.Left)
            {
                this.Coordinates.MarginTop += _robotLength - _pathWidth;

                this.Orientation = Lib.Orientation.East;
            }
            else if (runDirection == Direction.Forward && this.Orientation == Lib.Orientation.South && turnDirection == Direction.Right)
            {
                this.Coordinates.MarginTop += _robotLength - _pathWidth;
                this.Coordinates.MarginLeft -= _robotLength / 2;

                this.Orientation = Lib.Orientation.West;
            }
            else if (runDirection == Direction.Forward && this.Orientation == Lib.Orientation.West && turnDirection == Direction.Left)
            {
                this.Orientation = Lib.Orientation.South;
            }
            else if (runDirection == Direction.Forward && this.Orientation == Lib.Orientation.West && turnDirection == Direction.Right)
            {
                this.Coordinates.MarginTop -= _robotLength / 2;

                this.Orientation = Lib.Orientation.North;
            }
            else if (runDirection == Direction.Reverse && this.Orientation == Lib.Orientation.North && turnDirection == Direction.Left)
            {
                this.Coordinates.MarginTop += _robotLength - _pathWidth;
                this.Coordinates.MarginLeft -= _robotLength / 2;

                this.Orientation = Lib.Orientation.East;
            }
            else if (runDirection == Direction.Reverse && this.Orientation == Lib.Orientation.North && turnDirection == Direction.Right)
            {
                this.Coordinates.MarginTop += _robotLength - _pathWidth;

                this.Orientation = Lib.Orientation.West;
            }
            else if (runDirection == Direction.Reverse && this.Orientation == Lib.Orientation.East && turnDirection == Direction.Left)
            {
                this.Coordinates.MarginTop -= _robotLength / 2;

                this.Orientation = Lib.Orientation.South;
            }
            else if (runDirection == Direction.Reverse && this.Orientation == Lib.Orientation.East && turnDirection == Direction.Right)
            {
                this.Orientation = Lib.Orientation.North;
            }
            else if (runDirection == Direction.Reverse && this.Orientation == Lib.Orientation.South && turnDirection == Direction.Left)
            {
                this.Orientation = Lib.Orientation.West;
            }
            else if (runDirection == Direction.Reverse && this.Orientation == Lib.Orientation.South && turnDirection == Direction.Right)
            {
                this.Coordinates.MarginLeft -= _robotLength / 2;

                this.Orientation = Lib.Orientation.East;
            }
            else if (runDirection == Direction.Reverse && this.Orientation == Lib.Orientation.West && turnDirection == Direction.Left)
            {
                this.Coordinates.MarginLeft += _robotLength - _pathWidth;

                this.Orientation = Lib.Orientation.North;
            }
            else if (runDirection == Direction.Reverse && this.Orientation == Lib.Orientation.West && turnDirection == Direction.Right)
            {
                this.Coordinates.MarginTop -= _robotLength / 2;
                this.Coordinates.MarginLeft += _robotLength - _pathWidth;

                this.Orientation = Lib.Orientation.South;
            }
            else
            {
                throw new Exception(string.Format("Invalid turn parameters: {0},{1},{2}", runDirection, turnDirection, this.Orientation));
            }

            // Set payload coordinates
            SetPayloadCoordinates();

            RobotPositionChangedEventArgs eventArg = new RobotPositionChangedEventArgs();
            eventArg.OldCoordinates = oldCoordinates;
            eventArg.Coordinates = this.Coordinates;
            eventArg.Orientation = this.Orientation;
            eventArg.Direction = turnDirection;
            eventArg.TurnStep = _turnStep;

            OnRobotPositionChanged(eventArg);

            Thread.Sleep(_turnStep);
        }

        private void RetrievePayload(Guid payloadId)
        {
            _payloadSize = 10;

            PayloadId = payloadId;

            Thread.Sleep(1000);
        }

        private void ReleasePayload()
        {
            _payloadSize = 0;

            PayloadId = Guid.Empty;

            Thread.Sleep(1000);
        }

        private void SetPayloadCoordinates()
        {
            if (!this.HasPayload)
            {
                if (PayloadCoordinates != null) PayloadCoordinates = null;
            }
            else
            {
                if (PayloadCoordinates == null) PayloadCoordinates = new Coordinates();

                switch (this.Orientation)
                {
                    case Lib.Orientation.North:
                        PayloadCoordinates.MarginLeft = this.Coordinates.MarginLeft;
                        PayloadCoordinates.MarginTop = this.Coordinates.MarginTop - _payloadSize;
                        break;

                    case Lib.Orientation.East:
                        PayloadCoordinates.MarginLeft = this.Coordinates.MarginLeft + _robotLength;
                        PayloadCoordinates.MarginTop = this.Coordinates.MarginTop;
                        break;

                    case Lib.Orientation.South:
                        PayloadCoordinates.MarginLeft = this.Coordinates.MarginLeft;
                        PayloadCoordinates.MarginTop = this.Coordinates.MarginTop + _robotLength;
                        break;

                    case Lib.Orientation.West:
                        PayloadCoordinates.MarginLeft = this.Coordinates.MarginLeft - _payloadSize;
                        PayloadCoordinates.MarginTop = this.Coordinates.MarginTop;
                        break;
                }
            }
        }

        private void ChangeState(RobotState newState)
        {
            RobotState oldState = this.State;
            this.State = newState;

            OnRobotStateChanged(new RobotStateChangedEventArgs(oldState, newState));
        }

        #endregion
    }
}

