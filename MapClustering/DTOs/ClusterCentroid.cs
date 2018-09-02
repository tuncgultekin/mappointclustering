using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapClustering.DTOs
{
    /// <summary>
    /// ClusterCentroid point
    /// </summary>
    public class ClusterCentroid : Point
    {
        private double[] _coordinates;

        [JsonIgnore]
        public List<Point> InnerPoints { get; private set; }

        [JsonProperty(PropertyName = "geometry")]
        public override Geometry Geometry { get { return new Geometry() { Coordinates = new double[] { _coordinates[0], _coordinates[1] } }; } set { } }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="firstPoint">Raw centroid point</param>
        public ClusterCentroid(Point firstPoint)
        {
            InnerPoints = new List<Point>();
            _coordinates = new double[] { firstPoint.Geometry.Coordinates[0], firstPoint.Geometry.Coordinates[1] };
            Label = firstPoint.Label;
            Properties = new Dictionary<string, object>();
            Properties.Add("id", "C - " + Label);
            Properties.Add("pointcount", 0);
            Properties.Add("pointlist", "");
            AddPoint(firstPoint);
        }

        /// <summary>
        /// Adds a new point to cluster
        /// </summary>
        /// <param name="p">Point</param>
        public void AddPoint(Point p)
        {
            InnerPoints.Add(p);
            Properties["pointcount"] = (int)Properties["pointcount"] + 1;
            Properties["pointlist"] = ((string)Properties["pointlist"]) + "P-" + p.Properties["id"].ToString()+", ";            
        }

        /// <summary>
        /// Adds a new points to cluster
        /// </summary>
        /// <param name="points">Point list</param>
        public void AddPoints(IEnumerable<Point> points)
        {
            foreach (var p in points)
            {
                AddPoint(p);
            }
        }

        /// <summary>
        /// Sets the coordinates of center point
        /// </summary>
        /// <param name="coordinates">lng,lat array</param>
        public void SetCenterCoordinates(double[] coordinates)
        {
            _coordinates = coordinates;
        }
    }
}
