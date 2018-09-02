using MapClustering.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapClustering.Interfaces
{
    /// <summary>
    /// Point data provider interface
    /// </summary>
    public interface IDataProvider
    {
        /// <summary>
        /// Retrieved the list of points which are located in the specified box
        /// </summary>
        /// <param name="ne_lng">North east longitude of the box</param>
        /// <param name="ne_lat">North east latitude of the box</param>
        /// <param name="sw_lng">South west longitude of the box</param>
        /// <param name="sw_lat">South west latitude of the box</param>
        /// <returns>Point list</returns>
        List<Point> GetData(double? ne_lng, double? ne_lat, double? sw_lng, double? sw_lat);
    }
}
