using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapClustering.DTOs
{
    public class Point : IEquatable<Point>, ICloneable
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get { return "Feature"; } }

        [JsonProperty(PropertyName = "geometry")]
        public virtual Geometry Geometry { get; set; }

        [JsonProperty(PropertyName = "properties")]
        public virtual Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();

        [JsonIgnore]
        public string Label { get; set; }

        public object Clone()
        {
            return new Point() {
                Geometry = new Geometry() { Coordinates = new double[] { this.Geometry.Coordinates[0], this.Geometry.Coordinates[1] } },
                Label = Label,
                Properties = new Dictionary<string, object>()
            };
        }

        public bool Equals(Point other)
        {
            if ((Geometry.Coordinates[0] == other.Geometry.Coordinates[0])
                && (Geometry.Coordinates[1] == other.Geometry.Coordinates[1]))
                return true;

            return false;
        }

        /// <summary>
        /// Calculates the distance of 2D projection to specifed point
        /// </summary>
        /// <param name="q">Point</param>
        /// <param name="zoom">Zoom level</param>
        /// <returns>Distance</returns>
        internal double CalculateDistance(Point q, double zoom)
        {
            double x1, y1, x2, y2;
            Get2DCoordinates(zoom, out x1, out y1);
            q.Get2DCoordinates(zoom, out x2, out y2);

            return Math.Sqrt(Math.Pow((x1 - x2), 2) + Math.Pow((y1 - y2), 2));
        }

        /// <summary>
        /// Calculates x,y coordinates of 2D projection from geo-spatial coordinates.
        /// </summary>
        /// <param name="zoom">Zoom level</param>
        /// <param name="x">2D x coordinate</param>
        /// <param name="y">2D y coordinate</param>
        internal void Get2DCoordinates(double zoom, out double x, out double y)
        {            
            double alpha = Geometry.Coordinates[0] * Math.PI / 180;
            double tetha = Geometry.Coordinates[1] * Math.PI / 180;

            x = 256 * Math.Pow(2, zoom) * (Math.PI + alpha) / (2 * Math.PI);
            y = 256 * Math.Pow(2, zoom) * (Math.PI - Math.Log(Math.Tan((Math.PI / 4) + (tetha / 2)))) / (2 * Math.PI);
        }

        /// <summary>
        /// Calculates geographical coordinates of 2D projection.
        /// </summary>
        /// <param name="zoom">Zoom level</param>
        /// <param name="x">2D x coordinate</param>
        /// <param name="y">2D y coordinate</param>
        /// <param name="lng">Geo x coordinate</param>
        /// <param name="lat">Geo y coordinate</param>
        internal static void GetGeoCoordinates(double zoom, double x, double y, out double lng, out double lat)
        {
            double alpha = (x * (2 * Math.PI) / (256 * Math.Pow(2, zoom))) - Math.PI;
            double tmp = Math.PI -(y * (2 * Math.PI) / (256 * Math.Pow(2, zoom)));
            double tetha = (Math.Atan(Math.Pow(Math.E, tmp))- (Math.PI / 4))*2;

            lng = alpha * 180 / Math.PI;
            lat = tetha * 180 / Math.PI;
        }

        /// <summary>
        /// Degree to radian converter
        /// </summary>
        /// <param name="val">Degree</param>
        /// <returns>Radian</returns>
        private double ToRadians(double val)
        {
            return (Math.PI / 180) * val;
        }
    }

    /// <summary>
    /// Geometry class, which is used by point class
    /// </summary>
    public class Geometry
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get { return "Point"; } }

        [JsonProperty(PropertyName = "coordinates")]
        public double[] Coordinates { get; set; }
    }
}
