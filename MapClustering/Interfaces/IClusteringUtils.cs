using MapClustering.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapClustering.Interfaces
{
    /// <summary>
    /// Clustering algorithm interface
    /// </summary>
    public interface IClusteringUtils
    {
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
        List<Point> Cluster(IDataProvider provider, int iconSize, double zoomLevel, double? ne_lng, double? ne_lat, double? sw_lng, double? sw_lat);

        /// <summary>
        /// Algorihtm name
        /// </summary>
        string Name { get; }
    }
}
