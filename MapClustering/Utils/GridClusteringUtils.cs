using MapClustering.DTOs;
using MapClustering.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapClustering.Utils
{
    /// <summary>
    /// Grid based clustering algorithm
    /// </summary>
    public class GridClusteringUtils : IClusteringUtils
    {
        /// <summary>
        /// Algorihtm name
        /// </summary>
        public string Name => "grid";

        /// <summary>
        /// Clusters the point by using a grid based clustering algorithm
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

            return GridBasedCluster(provider, points, iconSize, zoomLevel, ne_lng, ne_lat, sw_lng, sw_lat);
        }

        /// <summary>
        /// Grid based clustering algorithm
        /// </summary>
        /// <param name="provider">Point data provider</param>
        /// <param name="points">Point list</param>
        /// <param name="iconSize">Icon size</param>
        /// <param name="zoomLevel">Zoom level</param>
        /// <param name="ne_lng">North east longitude of the box</param>
        /// <param name="ne_lat">North east latitude of the box</param>
        /// <param name="sw_lng">South west longitude of the box</param>
        /// <param name="sw_lat">South west latitude of the box</param>
        /// <returns>Clustered point list</returns>
        private List<Point> GridBasedCluster(IDataProvider provider, List<Point> points, int iconSize, double zoomLevel, double? ne_lng, double? ne_lat, double? sw_lng, double? sw_lat)
        {
            int gridWidth, gridHeight;
            ClusterCentroid[][] grid;
            List<Point> clusteredPoints;
            //
            
            // Create a grid and place the points to the appropriate grid cells
            CreateGridAndPutPoints(points, iconSize, zoomLevel, ne_lng, ne_lat, sw_lng, sw_lat, out gridWidth, out gridHeight, out grid, out clusteredPoints);            
            
            // Merge collided grid cell centroid with respect to their item counts
            MergeCentroidsIfTheyCollide(iconSize, zoomLevel, gridWidth, gridHeight, grid);

            // Return only filled grid cells as centroids
            return clusteredPoints.Where(t => ((ClusterCentroid)t).InnerPoints.Count > 0).ToList();
        }

        /// <summary>
        /// Merges the cluster centroids if they are collided on the clustering grid
        /// </summary>
        /// <param name="iconSize">Icon Size (eps)</param>
        /// <param name="zoomLevel">Zoom level</param>
        /// <param name="gridWidth">Grid witdh</param>
        /// <param name="gridHeight">Grid height</param>
        /// <param name="grid">Clustering grid</param>
        private static void MergeCentroidsIfTheyCollide(int iconSize, double zoomLevel, int gridWidth, int gridHeight, ClusterCentroid[][] grid)
        {
            // Neighbor cell access helpers
            int[] neighborsY = new int[] { -1, -1, -1, 0, 1, 1, 1, 0 };
            int[] neighborsX = new int[] { -1, 0, 1, 1, 1, 0, -1, -1 };

            // Enumerate all grid cells and check centroid points for any collision            
            for (int yIndex = 0; yIndex < gridHeight; yIndex++)
            {
                for (int xIndex = 0; xIndex < gridWidth; xIndex++)
                {
                    var currentCell = grid[yIndex][xIndex];
                    if (currentCell == null)
                        continue;

                    // Check the all 8 neighbor cells of the current cell
                    for (int i = 0; i < 8; i++)
                    {

                        // Check grid bounds
                        var nY = yIndex + neighborsY[i];
                        var nX = xIndex + neighborsX[i];
                        if (nY < 0 || nY >= gridHeight || nX < 0 || nX >= gridWidth)
                            continue;

                        // Skip empty cells
                        var neighborCell = grid[nY][nX];
                        if (neighborCell == null)
                            continue;

                        // Calculate the distance of another cluster centroid which is placed on neigbor cell
                        var dist = currentCell.CalculateDistance(neighborCell, zoomLevel);
                        if (dist < iconSize)
                        {
                            // If the distance is smaller than the icon size, merge these points to avoid collision                            
                            if (currentCell.InnerPoints.Count > neighborCell.InnerPoints.Count)
                            {
                                currentCell.AddPoints(neighborCell.InnerPoints);
                                neighborCell.InnerPoints.Clear();
                            }
                            else
                            {
                                neighborCell.AddPoints(currentCell.InnerPoints);
                                currentCell.InnerPoints.Clear();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  Create a grid and place the points to the appropriate grid cells
        /// </summary>        
        /// <param name="points">Point list</param>
        /// <param name="iconSize">Icon size</param>
        /// <param name="zoomLevel">Zoom level</param>
        /// <param name="ne_lng">North east longitude of the box</param>
        /// <param name="ne_lat">North east latitude of the box</param>
        /// <param name="sw_lng">South west longitude of the box</param>
        /// <param name="sw_lat">South west latitude of the box</param>
        /// <param name="gridWidth">Grid witdh</param>
        /// <param name="gridHeight">Grid height</param>
        /// <param name="grid">Clustering grid</param>
        /// <param name="clusteredPoints">Clustered points</param>        
        /// <returns>Clustered point list</returns>                
        private void CreateGridAndPutPoints(List<Point> points, int iconSize, double zoomLevel, double? ne_lng, double? ne_lat, double? sw_lng, double? sw_lat, out int gridWidth, out int gridHeight, out ClusterCentroid[][] grid, out List<Point> clusteredPoints)
        {
            // In order to get grid x and y coordinates,  
            Point upperRightPoint = new Point()
            {
                Geometry = new Geometry() { Coordinates = new double[] { ne_lng.Value, ne_lat.Value } },
            };

            Point lowerLeftPoint = new Point()
            {
                Geometry = new Geometry() { Coordinates = new double[] { sw_lng.Value, sw_lat.Value } },
            };

            double upperRightX, upperRightY;
            upperRightPoint.Get2DCoordinates(zoomLevel, out upperRightX, out upperRightY);

            double lowerLeftX, lowerLeftY;
            lowerLeftPoint.Get2DCoordinates(zoomLevel, out lowerLeftX, out lowerLeftY);

            gridWidth = (int)(400 / iconSize);
            gridHeight = (int)(300 / iconSize);

            grid = new ClusterCentroid[gridHeight][];
            for (int i = 0; i < gridHeight; i++)
            {
                grid[i] = new ClusterCentroid[gridWidth];
            }

            clusteredPoints = new List<Point>();
            foreach (var p in points)
            {
                int x, y;
                FindGridCell(grid, p, zoomLevel, upperRightX, upperRightY, lowerLeftX, lowerLeftY, iconSize, out x, out y);
                if (grid[y][x] == null)
                {
                    p.Label = y + "-" + x;
                    grid[y][x] = new ClusterCentroid(p);                    
                    clusteredPoints.Add(grid[y][x]);
                }
                else
                    grid[y][x].AddPoint(p);
            }
        }

        private void FindGridCell(ClusterCentroid[][] grid, Point p, double zoomLevel, double upperRightX, double upperRightY, double lowerLeftX, double lowerLeftY, int iconSize, out int x, out int y)
        {
            double pointX, pointY;
            p.Get2DCoordinates(zoomLevel, out pointX, out pointY);

            x = (int)Math.Floor((pointX - lowerLeftX) / iconSize);
            if (grid[0].GetLength(0) <= x)
                x = grid[0].GetLength(0) - 1;

            y = (int)Math.Floor((pointY - upperRightY) / iconSize);
            if (grid.GetLength(0) <= y)
                y = grid.GetLength(0) - 1;

        }
    }
}
