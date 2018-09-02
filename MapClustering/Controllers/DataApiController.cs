using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MapClustering.DTOs;
using MapClustering.Interfaces;
using MapClustering.Utils;
using Microsoft.AspNetCore.Mvc;

namespace MapClustering.Controllers
{
    /// <summary>
    /// Api controller for clustered points
    /// </summary>
    [Route("api/data")]
    [ApiController]
    public class DataApiController : Controller
    {        
        public const int ICON_SIZE = 20;
        private IDataProvider _dataProvider;
        private List<IClusteringUtils> _algorithms;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="dataProvider">Point data provider which is automatically injected by IoC</param>
        public DataApiController(IDataProvider dataProvider, List<IClusteringUtils> algorithms)
        {            
            _dataProvider = dataProvider;
            _algorithms = algorithms;
        }

        /// <summary>
        /// Retrives the clustered points of the specified bounding box
        /// </summary>
        /// <param name="algo">Clustering algorithm</param>
        /// <param name="zoom">Zoom level</param>
        /// <param name="ne_lng">North east longitude of the box</param>
        /// <param name="ne_lat">North east latitude of the box</param>
        /// <param name="sw_lng">South west longitude of the box</param>
        /// <param name="sw_lat">South west latitude of the box</param>
        /// <returns>Clustered point list</returns>
        [HttpGet]
        public JsonResult Get([FromQuery]string algo, [FromQuery]double zoom, [FromQuery]double? ne_lng, [FromQuery]double? ne_lat, [FromQuery]double? sw_lng, [FromQuery]double? sw_lat)
        {
            PointCollection result = new PointCollection();
            IClusteringUtils clusteringAlgorithm = GetClusteringAlgorithm(algo);
            result.Features = clusteringAlgorithm.Cluster(_dataProvider, ICON_SIZE, zoom, ne_lng, ne_lat, sw_lng, sw_lat);

            return new JsonResult(result);
        }

        /// <summary>
        /// Creates a clustering algorithm utility instance
        /// </summary>
        /// <param name="algo">Algorithm Code</param>
        /// <returns>IClusteringUtils</returns>
        private IClusteringUtils GetClusteringAlgorithm(string algo)
        {
            return _algorithms.Where(t => t.Name == algo).First();
        }
    }
}
