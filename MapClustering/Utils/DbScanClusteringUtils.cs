using MapClustering.DTOs;
using MapClustering.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapClustering.Utils
{
    /// <summary>
    /// Modified DbScan clustering algorithm
    /// </summary>
    public class DbScanClusteringUtils : IClusteringUtils
    {
        /// <summary>
        /// Algorihtm name
        /// </summary>
        public string Name => "dbscan";

        /// <summary>
        /// Clusters the point by using a modified version of DbScan
        /// </summary>
        /// <param name="provider">Point data provider</param>
        /// <param name="iconSize">Icon size</param>
        /// <param name="zoomLevel">Zoom level</param>
        /// <param name="ne_lng">North east longitude of the box</param>
        /// <param name="ne_lat">North east latitude of the box</param>
        /// <param name="sw_lng">South west longitude of the box</param>
        /// <param name="sw_lat">South west latitude of the box</param>
        /// <returns>Clustered point list</returns>
        public List<Point> Cluster(IDataProvider provider, int iconSize, double zoomLevel, double? ne_lng, double? ne_lat, double? sw_lng, double? sw_lat)
        {
            var points = provider.GetData(ne_lng, ne_lat, sw_lng, sw_lat);

            return DbScan(provider, points, iconSize, zoomLevel, ne_lng, ne_lat, sw_lng, sw_lat);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider">Point data provider</param>
        /// <param name="pointList">List of points</param>
        /// <param name="eps">Clustering distance(iconSize)</param>
        /// <param name="zoomLevel">Zoom level</param>
        /// <param name="ne_lng">North east longitude of the box</param>
        /// <param name="ne_lat">North east latitude of the box</param>
        /// <param name="sw_lng">South west longitude of the box</param>
        /// <param name="sw_lat">South west latitude of the box</param>
        /// <returns></returns>
        public List<Point> DbScan(IDataProvider provider, List<Point> pointList, double eps, double zoomLevel, double? ne_lng, double? ne_lat, double? sw_lng, double? sw_lat)
        {
            // Use dictionary to lookup centroids quickly
            Dictionary<string, Point> centroids = new Dictionary<string, Point>();

            int clusterId = 0;
            foreach (var p in pointList)
            {
                if (p.Label != null)
                    continue;

                // Get the neighbors of the current point p within eps (iconsize) distance
                var neighborsOfCenter = RangeQuery(pointList, p, zoomLevel, eps, ne_lng, ne_lat, sw_lng, sw_lat);

                // Assign it to a cluster, if it has not been assigned yet.
                clusterId++;
                p.Label = clusterId.ToString();
                if (!centroids.ContainsKey(p.Label))
                    centroids.Add(p.Label, new ClusterCentroid(p));

                // Add the neighbors, which have not been assigned to a cluster yet, to the p's cluster
                int i = -1;
                while (i < neighborsOfCenter.Count - 1)
                {
                    i++;
                    var q = neighborsOfCenter.ElementAt(i).Value;
                    if (p.Equals(q))
                        continue;

                    if (q.Label != null)
                        continue;

                    q.Label = clusterId.ToString();
                    ((ClusterCentroid)centroids[q.Label]).AddPoint(q);
                }
            }

            // Return only the cluster centroids
            return centroids.ToList().Select(t => t.Value).ToList();
        }

        /// <summary>
        /// Retrieves the neighbor points of the center point p within 'eps' distance
        /// </summary>
        /// <param name="p">Center point</param>
        /// <param name="points">List of points</param>
        /// <param name="eps">Clustering distance(iconSize)</param>
        /// <param name="zoomLevel">Zoom level</param>
        /// <param name="ne_lng">North east longitude of the box</param>
        /// <param name="ne_lat">North east latitude of the box</param>
        /// <param name="sw_lng">South west longitude of the box</param>
        /// <param name="sw_lat">South west latitude of the box</param>
        /// <returns>Neighbor points</returns>
        public Dictionary<string, Point> RangeQuery(List<Point> points, Point p, double zoom, double eps, double? ne_lng, double? ne_lat, double? sw_lng, double? sw_lat)
        {
            Dictionary<string, Point> neighbors = new Dictionary<string, Point>();

            foreach (var q in points)
            {
                if (q.Label != null)
                    continue;

                var dist = p.CalculateDistance(q, zoom);                
                if (dist <= eps)
                    neighbors.Add(q.Properties["id"].ToString(), q);
            }

            return neighbors;
        }
    }
}
