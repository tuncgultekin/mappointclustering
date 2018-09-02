# Map Point Clustering

When rendering many data points on the map, some points may overlap and negatively affect the performance. The obvious solution is clustering the points according to zoom level and coordinates.

This project demonstrates the efficiency of two different map point clustering methods and it was developed as a .Net Core based web application. Front-end side of the project; consist only a html file and javascript file.  On the other hand, back-end side is a simple rest service, which reads point data from file (example data-set.json) and serves over an endpoint. 

2 different clustering methods, modified DBScan and grid-based clustering, were designed and implemented at back-end and then compared with each other. These algorithms were added to back-end to support further caching requirements.

Demo video of the web application can be viewed from https://youtu.be/7rugOermHgc

![Fig - 1 Map Point Clustering Application](https://github.com/tuncgultekin/mappointclustering/blob/master/Graphs/fig1.png)
<p><em>(Fig - 1)</em></p>

The map shows only cluster centroids and numbers on centroids denotes the point count in cluster. Ids of the points in any cluster can be seen in a dialog (Fig - 2) by clicking the cluster centroid.

![Fig - 2 Ids of the points in a cluster.](https://github.com/tuncgultekin/mappointclustering/blob/master/Graphs/fig2.png)
<p><em>(Fig - 2)</em></p>

# Clustering Algorithms
## a) Modified DbScan Algorithm

Initially DbScan (Density-Based Spatial Clustering of Applications with Noise) algorithm (https://en.wikipedia.org/wiki/DBSCAN), which is a density-based clustering method, was considered as a good choice for map point clustering. Since it does not require a cluster number parameter, allows arbitrary shaped clusters and appropriate for range queries that would be accelerated by interval trees.
However, after several trials, it was seen that original DbScan algorithm creates large clusters which are not necessary for point overlap use case. In other words, DbScan algorithm combines also non-overlapping points (as shown in Fig-2) due to the “neighbor of neighbor” labeling process.


<table style="border:0px; border-width:0px;">
  <tr style="border:0px; border-width:0px;">
    <td style="border:0px; border-width:0px;"><img src="https://github.com/tuncgultekin/mappointclustering/blob/master/Graphs/fig3.png" alt></td>   
    <td style="border:0px; border-width:0px;">--> Zoom Out --></td>
    <td style="border:0px; border-width:0px;"><img src="https://github.com/tuncgultekin/mappointclustering/blob/master/Graphs/fig4.png" alt></td>
  </tr> 
</table>
<p><em>Fig - 3 Problem of the original DbScan Algorithm</em></p>

To solve this problem, original algorithm’s “neighbor of neighbor” labeling process (as shown as yellow line in Fig-4) is removed.

![Fig - 4 Original DbScan Algorithm](https://github.com/tuncgultekin/mappointclustering/blob/master/Graphs/fig5.png)
<p><em>(Fig - 4)</em></p>

The final implementation can be seen on /MapClustering/Utils/DbScanClusteringUtils.cs

## b) Grid Based Clustering Algorithm
After doing some research on map point clustering techniques, it was learnt that grid-based point clustering methods is used by major map providers such as google maps (https://developers.google.com/maps/documentation/javascript/marker-clustering). To do so, a grid-based clustering algorithm is designed and implemented. 
Grid based algorithm firstly, divides the map into the equal sized cells. Widths and heights of the cells are equal to the map icon size. Then the algorithm puts points into the appropriate cells with respect to point coordinates. Point overlap possibility in any cell is greater than 0 so the algorithm considers the first points in cells as centroids and creates clusters from non-empty cells. Actual implementation of the algorithm can be seen on /MapClustering/Utils/GridClusteringUtils.cs

# Results and Comparison
Modified DbScan algorithm provides better results than grid-based algorithm. Its point clusters are robust and stable, and they are not affected by map-drag operations. On the other hand, its computational complexity is O(n^2).

This implementation of grid-based algorithm has better computational complexity O(n) than Modified DbScan O(n^2). However, resulting clusters are affected by map-drag operations. This problem can be fixed by adding some extra control logic, but this may degrade its performance.

To improve the overall performance of the system; clustering results of the regions for different zoom levels would be cached and range queries would be improved (O(n) to O(log(n))) by keeping points on interval trees or location-aware database technologies.
