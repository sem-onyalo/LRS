using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRS.Lib
{
    public interface IWorld
    {
        List<Path> Paths { get; set; }

        List<PathMarker> PathMarkers { get; set; }

        ConcurrentDictionary<Guid, Obstacle> Obstacles { get; set; }

        void RobotPositionChanged(object sender, RobotPositionChangedEventArgs args);
    }
}
