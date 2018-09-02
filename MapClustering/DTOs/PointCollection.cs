using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapClustering.DTOs
{
    /// <summary>
    /// PointCollection
    /// </summary>
    public class PointCollection
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get { return "FeatureCollection"; } }

        [JsonProperty(PropertyName = "features")]
        public List<Point> Features { get; set; }
    }
}
