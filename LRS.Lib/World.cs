using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRS.Lib
{
    /// <summary>
    /// Represents the world in which all objects exist.
    /// </summary>
    public class World : IWorld
    {
        #region Fields

        private Object _obstacleLock = new object();

        #endregion

        #region Properties

        /// <summary>
        /// Represents a collection of paths.
        /// </summary>
        public List<Path> Paths { get; set; }

        /// <summary>
        /// Represents a collection of objects the determine path changes.
        /// </summary>
        public List<PathMarker> PathMarkers { get; set; }

        /// <summary>
        /// Represents a collection of obstacles.
        /// </summary>
        public ConcurrentDictionary<Guid, Obstacle> Obstacles { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Instantiates a new <see cref="World"/> object.
        /// </summary>
        public World()
        {
            Paths = new List<Path>();

            PathMarkers = new List<PathMarker>();

            Obstacles = new ConcurrentDictionary<Guid, Obstacle>();

            AddPaths();
        }

        #endregion

        #region Event Handlers

        public void RobotPositionChanged(object sender, RobotPositionChangedEventArgs args)
        {
            Robot robot = sender as Robot;

            Obstacle obstacle = new Obstacle(args.Coordinates, robot.RelativeWidth, robot.RelativeHeight);

            Obstacles.AddOrUpdate(robot.Id, obstacle, (k, v) => { return obstacle; });
        }

        #endregion

        #region Methods

        private void AddPaths()
        {
            // to Rx Paths
            Paths.Add(new Path(new Coordinates(420, 80), 470, Orientation.West, 1));
            Paths.Add(new Path(new Coordinates(420, 130), 470, Orientation.East, 1));
            Paths.Add(new Path(new Coordinates(880, 80), 60, Orientation.North, 1));
            Paths.Add(new Path(new Coordinates(660, 130), 465, Orientation.North, 2));

            // to Ax/Bx Paths
            Paths.Add(new Path(new Coordinates(420, 80), 465, Orientation.South, 1));
            Paths.Add(new Path(new Coordinates(25, 535), 405, Orientation.West, 1));
            Paths.Add(new Path(new Coordinates(25, 535), 60, Orientation.South, 1));

            // to Fx Paths
            Paths.Add(new Path(new Coordinates(480, 185), 410, Orientation.North, 2));
            Paths.Add(new Path(new Coordinates(420, 185), 70, Orientation.West, 2));
            Paths.Add(new Path(new Coordinates(25, 585), 685, Orientation.East, 1));

            // to Ox Paths
            Paths.Add(new Path(new Coordinates(545, 130), 65, Orientation.South, 2));
            Paths.Add(new Path(new Coordinates(545, 185), 75, Orientation.East, 2));
            Paths.Add(new Path(new Coordinates(610, 185), 410, Orientation.South, 2));

            // to Lx Paths
            Paths.Add(new Path(new Coordinates(710, 130), 465, Orientation.North, 2));

            // Rx Paths
            Paths.Add(new Path(new Coordinates(850, 20), 70, Orientation.South, 2));
            Paths.Add(new Path(new Coordinates(800, 20), 70, Orientation.South, 2));
            Paths.Add(new Path(new Coordinates(750, 20), 70, Orientation.South, 2));
            Paths.Add(new Path(new Coordinates(700, 20), 70, Orientation.South, 2));
            Paths.Add(new Path(new Coordinates(650, 20), 70, Orientation.South, 2));
            Paths.Add(new Path(new Coordinates(600, 20), 70, Orientation.South, 2));
            Paths.Add(new Path(new Coordinates(550, 20), 70, Orientation.South, 2));
            Paths.Add(new Path(new Coordinates(500, 20), 70, Orientation.South, 2));
            Paths.Add(new Path(new Coordinates(450, 20), 70, Orientation.South, 2));

            // Ax Paths
            Paths.Add(new Path(new Coordinates(270, 20), 525, Orientation.North, 2));
            Paths.Add(new Path(new Coordinates(225, 20), 525, Orientation.South, 2));

            Paths.Add(new Path(new Coordinates(135, 20), 525, Orientation.North, 2));
            Paths.Add(new Path(new Coordinates(180, 20), 525, Orientation.South, 2));

            // Bx Paths
            Paths.Add(new Path(new Coordinates(270, 435), 120, Orientation.East, 3));
            Paths.Add(new Path(new Coordinates(380, 435), 60, Orientation.South, 3));
            Paths.Add(new Path(new Coordinates(270, 485), 120, Orientation.West, 3));

            Paths.Add(new Path(new Coordinates(270, 325), 120, Orientation.East, 3));
            Paths.Add(new Path(new Coordinates(380, 275), 60, Orientation.North, 3));
            Paths.Add(new Path(new Coordinates(270, 275), 120, Orientation.West, 3));

            Paths.Add(new Path(new Coordinates(270, 180), 120, Orientation.East, 3));
            Paths.Add(new Path(new Coordinates(380, 180), 60, Orientation.South, 3));
            Paths.Add(new Path(new Coordinates(270, 230), 120, Orientation.West, 3));

            Paths.Add(new Path(new Coordinates(270, 70), 120, Orientation.East, 3));
            Paths.Add(new Path(new Coordinates(380, 20), 60, Orientation.North, 3));
            Paths.Add(new Path(new Coordinates(225, 20), 165, Orientation.West, 3));

            Paths.Add(new Path(new Coordinates(25, 435), 120, Orientation.West, 3));
            Paths.Add(new Path(new Coordinates(25, 435), 60, Orientation.South, 3));
            Paths.Add(new Path(new Coordinates(25, 485), 120, Orientation.East, 3));

            Paths.Add(new Path(new Coordinates(25, 325), 120, Orientation.West, 3));
            Paths.Add(new Path(new Coordinates(25, 275), 60, Orientation.North, 3));
            Paths.Add(new Path(new Coordinates(25, 275), 120, Orientation.East, 3));

            Paths.Add(new Path(new Coordinates(25, 180), 120, Orientation.West, 3));
            Paths.Add(new Path(new Coordinates(25, 180), 60, Orientation.South, 3));
            Paths.Add(new Path(new Coordinates(25, 230), 120, Orientation.East, 3));

            Paths.Add(new Path(new Coordinates(25, 70), 120, Orientation.West, 3));
            Paths.Add(new Path(new Coordinates(25, 20), 60, Orientation.North, 3));
            Paths.Add(new Path(new Coordinates(25, 20), 165, Orientation.East, 3));

            // Cx Paths
            Paths.Add(new Path(new Coordinates(305, 395), 50, Orientation.North, 4));
            Paths.Add(new Path(new Coordinates(320, 395), 50, Orientation.North, 4));
            Paths.Add(new Path(new Coordinates(335, 395), 50, Orientation.North, 4));
            Paths.Add(new Path(new Coordinates(350, 395), 50, Orientation.North, 4));
            Paths.Add(new Path(new Coordinates(365, 395), 50, Orientation.North, 4));

            Paths.Add(new Path(new Coordinates(305, 325), 50, Orientation.South, 4));
            Paths.Add(new Path(new Coordinates(320, 325), 50, Orientation.South, 4));
            Paths.Add(new Path(new Coordinates(335, 325), 50, Orientation.South, 4));
            Paths.Add(new Path(new Coordinates(350, 325), 50, Orientation.South, 4));
            Paths.Add(new Path(new Coordinates(365, 325), 50, Orientation.South, 4));

            Paths.Add(new Path(new Coordinates(305, 140), 50, Orientation.North, 4));
            Paths.Add(new Path(new Coordinates(320, 140), 50, Orientation.North, 4));
            Paths.Add(new Path(new Coordinates(335, 140), 50, Orientation.North, 4));
            Paths.Add(new Path(new Coordinates(350, 140), 50, Orientation.North, 4));
            Paths.Add(new Path(new Coordinates(365, 140), 50, Orientation.North, 4));

            Paths.Add(new Path(new Coordinates(305, 70), 50, Orientation.South, 4));
            Paths.Add(new Path(new Coordinates(320, 70), 50, Orientation.South, 4));
            Paths.Add(new Path(new Coordinates(335, 70), 50, Orientation.South, 4));
            Paths.Add(new Path(new Coordinates(350, 70), 50, Orientation.South, 4));
            Paths.Add(new Path(new Coordinates(365, 70), 50, Orientation.South, 4));

            Paths.Add(new Path(new Coordinates(40, 395), 50, Orientation.North, 4));
            Paths.Add(new Path(new Coordinates(55, 395), 50, Orientation.North, 4));
            Paths.Add(new Path(new Coordinates(70, 395), 50, Orientation.North, 4));
            Paths.Add(new Path(new Coordinates(85, 395), 50, Orientation.North, 4));
            Paths.Add(new Path(new Coordinates(100, 395), 50, Orientation.North, 4));

            Paths.Add(new Path(new Coordinates(40, 325), 50, Orientation.South, 4));
            Paths.Add(new Path(new Coordinates(55, 325), 50, Orientation.South, 4));
            Paths.Add(new Path(new Coordinates(70, 325), 50, Orientation.South, 4));
            Paths.Add(new Path(new Coordinates(85, 325), 50, Orientation.South, 4));
            Paths.Add(new Path(new Coordinates(100, 325), 50, Orientation.South, 4));

            Paths.Add(new Path(new Coordinates(40, 140), 50, Orientation.North, 4));
            Paths.Add(new Path(new Coordinates(55, 140), 50, Orientation.North, 4));
            Paths.Add(new Path(new Coordinates(70, 140), 50, Orientation.North, 4));
            Paths.Add(new Path(new Coordinates(85, 140), 50, Orientation.North, 4));
            Paths.Add(new Path(new Coordinates(100, 140), 50, Orientation.North, 4));

            Paths.Add(new Path(new Coordinates(40, 70), 50, Orientation.South, 4));
            Paths.Add(new Path(new Coordinates(55, 70), 50, Orientation.South, 4));
            Paths.Add(new Path(new Coordinates(70, 70), 50, Orientation.South, 4));
            Paths.Add(new Path(new Coordinates(85, 70), 50, Orientation.South, 4));
            Paths.Add(new Path(new Coordinates(100, 70), 50, Orientation.South, 4));

            // Fx Paths
            Paths.Add(new Path(new Coordinates(480, 235), 50, Orientation.East, 3));
            Paths.Add(new Path(new Coordinates(480, 285), 50, Orientation.East, 3));
            Paths.Add(new Path(new Coordinates(480, 335), 50, Orientation.East, 3));
            Paths.Add(new Path(new Coordinates(480, 385), 50, Orientation.East, 3));
            Paths.Add(new Path(new Coordinates(480, 435), 50, Orientation.East, 3));
            Paths.Add(new Path(new Coordinates(480, 485), 50, Orientation.East, 3));

            // Ox Paths
            Paths.Add(new Path(new Coordinates(570, 235), 50, Orientation.West, 3));
            Paths.Add(new Path(new Coordinates(570, 285), 50, Orientation.West, 3));
            Paths.Add(new Path(new Coordinates(570, 335), 50, Orientation.West, 3));
            Paths.Add(new Path(new Coordinates(570, 385), 50, Orientation.West, 3));
            Paths.Add(new Path(new Coordinates(570, 435), 50, Orientation.West, 3));
            Paths.Add(new Path(new Coordinates(570, 485), 50, Orientation.West, 3));

            // Lx Paths
            Paths.Add(new Path(new Coordinates(710, 235), 200, Orientation.West, 3));
            Paths.Add(new Path(new Coordinates(710, 285), 200, Orientation.East, 3));
            Paths.Add(new Path(new Coordinates(710, 335), 200, Orientation.West, 3));
            Paths.Add(new Path(new Coordinates(710, 385), 200, Orientation.East, 3));
            Paths.Add(new Path(new Coordinates(710, 435), 200, Orientation.West, 3));
            Paths.Add(new Path(new Coordinates(710, 485), 200, Orientation.East, 3));
            Paths.Add(new Path(new Coordinates(710, 535), 200, Orientation.West, 3));
            Paths.Add(new Path(new Coordinates(710, 585), 200, Orientation.East, 3));
            Paths.Add(new Path(new Coordinates(900, 235), 60, Orientation.North, 3));
            Paths.Add(new Path(new Coordinates(900, 335), 60, Orientation.North, 3));
            Paths.Add(new Path(new Coordinates(900, 435), 60, Orientation.North, 3));
            Paths.Add(new Path(new Coordinates(900, 535), 60, Orientation.North, 3));
        }

        #endregion
    }
}
