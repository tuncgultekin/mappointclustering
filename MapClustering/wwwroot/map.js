
mapboxgl.accessToken = 'pk.eyJ1Ijoic21pY2tpZSIsImEiOiJjaWtiM2JkdW0wMDJudnRseTY0NWdrbjFnIn0.WxGYL18BJjWUiNIu-r3MSA';
const map = new mapboxgl.Map({
    container: document.getElementById("container"),
    style: {
        version: 8,
        zoom: 8, // default zoom.
        center: [0, 51.5], // default center coordinate in [longitude, latitude] format.
        sources: {
            // Using an open-source map tile layer.
            "simple-tiles": {
                type: "raster",
                tiles: [
                    "https://a.tile.openstreetmap.org/{z}/{x}/{y}.png",
                    "https://b.tile.openstreetmap.org/{z}/{x}/{y}.png"
                ],
                tileSize: 256
            }
        },
        glyphs: "mapbox://fonts/mapbox/{fontstack}/{range}.pbf",
        layers: [
            {
                id: "simple-tiles",
                type: "raster",
                source: "simple-tiles",
                minzoom: 0,
                maxzoom: 22
            }
        ]
    }
});

// Load data points from GeoJSON file.
fetch("api/data?algo=" + document.getElementById("algorithm").value +"&zoom=8.197958123926325&ne_lng=0.9324147015564108&ne_lat=51.941070776573724&sw_lng=-0.9831253116909409&sw_lat=51.046649302618334")
    .then(response => response.json())
    .then(data => {

        // Add points to map as a GeoJSON source.
        map.addSource("points", {
            type: "geojson",
            data
        });

        // Add a layer to the map to render the GeoJSON points.
        map.addLayer({
            id: "points",
            type: "circle",
            source: "points",
            paint: {
                "circle-radius": 20,
                "circle-color": "#ff5500",
                "circle-stroke-width": 1,
                "circle-stroke-color": "#000",
            }
        });

        map.addLayer({
            id: "points1",
            type: "symbol",
            source: "points",
            layout: {
                "text-field": "{pointcount}",
                "text-font": ["DIN Offc Pro Medium", "Arial Unicode MS Bold"],
                "text-size": 12
            }
        });

        // Show a popup when clicking on a point.
        map.on("click", "points", event => {

            new mapboxgl.Popup()
                .setLngLat(event.lngLat)
                .setHTML(event.features[0].properties["pointlist"] + "")
                .addTo(map);
        });

        // Change the cursor to a pointer when the mouse is over the points layer.
        map.on("mouseenter", "points", function () {
            map.getCanvas().style.cursor = "pointer";
        });

        // Change it back to a pointer when it leaves.
        map.on("mouseleave", "points", function () {
            map.getCanvas().style.cursor = "";
        });

        // Reload points in case of zoom event
        map.on("zoom", "points", function (e) {
            reloadPoints(e.target.scrollZoom._targetZoom);
        });

        // Reload points in case of drag event
        map.on("drag", "points", function (e) {
            reloadPoints(map.getZoom());
        });
    });


function reloadPoints(zoom) {

    if (zoom == null || zoom == undefined)
        zoom = map.getZoom();

    // In order to avoid multiple calls in a limited of time (200 ms), timeout function is used
    if (dataLoader != null) {
        clearTimeout(dataLoader);
    }

    dataLoader = setTimeout(function () {

        var bounds = map.getBounds();
        fetch("/api/data?algo=" + document.getElementById("algorithm").value +"&zoom=" + zoom + "&ne_lng=" + bounds._ne.lng + "&ne_lat=" + bounds._ne.lat + "&sw_lng=" + bounds._sw.lng + "&sw_lat=" + bounds._sw.lat)
            .then(response => response.json())
            .then(data => {
                map.getSource("points").setData(data);
            });

    }, 200);
}
var dataLoader = null;