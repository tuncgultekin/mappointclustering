using MapClustering.DTOs;
using MapClustering.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MapClustering.DIServices
{
    /// <summary>
    /// Static file provider implementation of the IDataProvider interface
    /// </summary>
    public class FileDataProvider : IDataProvider
    {
        /// <summary>
        /// Retrieved the list of points which are located in the specified box
        /// </summary>
        /// <param name="ne_lng">North east longitude of the box</param>
        /// <param name="ne_lat">North east latitude of the box</param>
        /// <param name="sw_lng">South west longitude of the box</param>
        /// <param name="sw_lat">South west latitude of the box</param>
        /// <returns>Point list</returns>
        public List<Point> GetData(double? ne_lng, double? ne_lat, double? sw_lng, double? sw_lat)
        {
            if (ne_lng != null && ne_lat != null && sw_lat != null && sw_lng != null)
            {
                return GetDataByCoordinates(ne_lng, ne_lat, sw_lng, sw_lat);
            }
            else
            {
                return GetAllData();
            }
        }

        /// <summary>
        /// Filters the points by the coordinates
        /// </summary>
        /// <param name="ne_lng">North east longitude of the box</param>
        /// <param name="ne_lat">North east latitude of the box</param>
        /// <param name="sw_lng">South west longitude of the box</param>
        /// <param name="sw_lat">South west latitude of the box</param>
        /// <returns>Point list</returns>
        private List<Point> GetDataByCoordinates(double? ne_lng, double? ne_lat, double? sw_lng, double? sw_lat)
        {
            var allData = GetAllData();
            var filteredData = allData.Where(t => (ne_lat.Value > t.Geometry.Coordinates[1])
                                       && (sw_lat.Value < t.Geometry.Coordinates[1])
                                       && (ne_lng.Value > t.Geometry.Coordinates[0])
                                       && (sw_lng.Value < t.Geometry.Coordinates[0])).ToList();

            return filteredData;
        }

        /// <summary>
        /// Read all point data from file
        /// </summary>
        /// <returns>Point list</returns>
        private List<Point> GetAllData()
        {
            var data = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "wwwroot/exampledata-set.json"));
            var pointCollection = Newtonsoft.Json.JsonConvert.DeserializeObject<PointCollection>(data);

            return pointCollection.Features;
        }
    }
}
