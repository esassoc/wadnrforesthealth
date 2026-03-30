/*-----------------------------------------------------------------------
<copyright file="ProjectFirmaMaps.js" company="Tahoe Regional Planning Agency and Environmental Science Associates">
Copyright (c) Tahoe Regional Planning Agency and Environmental Science Associates. All rights reserved.
<author>Environmental Science Associates</author>
</copyright>

<license>
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License <http://www.gnu.org/licenses/> for more details.

Source code is available upon request via <support@sitkatech.com>.
</license>
-----------------------------------------------------------------------*/
var ProjectFirmaMaps = {};
var highlightOverlay; //Used when highlighting all a Project's Detailed Locations; see formatGeospatialAreaResponse
var mapOutsideScope; //Used when highlighting all a Project's Detailed Locations; see formatGeospatialAreaResponse

/* ====== Main Map ====== */
ProjectFirmaMaps.Map = function (mapInitJson, initialBaseLayerShown, treatAllLayersAsBaseLayers, projectDetailedLocationSelectedLayerUrl)
{
    var self = this;
    this.MapDivId = mapInitJson.MapDivID;
    this.ProjectDetailedLocationSelectedLayerUrl = projectDetailedLocationSelectedLayerUrl;

    var esriAerialUrl = 'https://services.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}';
    var esriAerial = new L.TileLayer(esriAerialUrl, {});

    var esriStreetUrl = 'https://services.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer/tile/{z}/{y}/{x}';
    var esriStreet = new L.TileLayer(esriStreetUrl, {});

    var esriTerrainUrl = 'https://services.arcgisonline.com/ArcGIS/rest/services/World_Topo_Map/MapServer/tile/{z}/{y}/{x}';
    var esriTerrain = new L.TileLayer(esriTerrainUrl, {});

    var streetLabelsLayer = new L.TileLayer('https://services.arcgisonline.com/ArcGIS/rest/services/Reference/World_Transportation/MapServer/tile/{z}/{y}/{x}', {});

    var baseLayers = { 'Aerial': esriAerial, 'Street': esriStreet, 'Terrain': esriTerrain };
    var overlayLayers = {};
    if (!treatAllLayersAsBaseLayers) {
       overlayLayers = { 'Street Labels': streetLabelsLayer }
    }
    

    var streetLayerGroup;
    if (initialBaseLayerShown === "Hybrid")
    {
         streetLayerGroup = new L.LayerGroup();
        initialBaseLayerShown = "Aerial"; //Since switching to ArcGIS maps, there is no "Hybrid", so use "Aerial" instead.
        streetLabelsLayer.addTo(streetLayerGroup);
    }
    else if (Sitka.Methods.isUndefinedNullOrEmpty(initialBaseLayerShown) || baseLayers[initialBaseLayerShown] == null)
    {
        initialBaseLayerShown = "Terrain";
    }    

    var options = {
        scrollWheelZoom: false, // If this is on (default) scrolling down the page will intercept and starting zooming the map
        layers: [baseLayers[initialBaseLayerShown]],
        attributionControl: false,
        fullscreenControl: mapInitJson.AllowFullScreen ? { pseudoFullscreen: true } : false
    };
    this.map = L.map(this.MapDivId, options);

    mapOutsideScope = this.map;
    this.map.on("overlayremove", function(e) {
        if (highlightOverlay && e.name.includes("All Project Locations - Detail"))
            highlightOverlay.remove(); //If Detailed Locations is toggled off, also remove the highlight
    });

    // Initialize the map
    if (streetLayerGroup != null)
    {
        streetLayerGroup.addTo(this.map);
    }

    // add vector layers
    this.vectorLayers = [];


    if (!treatAllLayersAsBaseLayers) {//ArcGIS layers should not be added to the Manage Find Your Forester Page
        //Add external tile layers from ArcGIS Online
        for (var i = 0; i < mapInitJson.ExternalMapLayers.length; ++i) {
            var layerConfig = mapInitJson.ExternalMapLayers[i];
            if (layerConfig.IsTiledMapService) {
                this.addTiledLayerFromAGOL(layerConfig, overlayLayers);
            }
        }

        //Add external vector layers from ArcGIS Online
        for (var i = 0; i < mapInitJson.ExternalMapLayers.length; ++i) {
            var layerConfig = mapInitJson.ExternalMapLayers[i];
            if (!layerConfig.IsTiledMapService) {
                this.addVectorLayerFromAGOL(layerConfig, overlayLayers);
            }
        }
    }

    // Add main layers from geojson
    for (var i = 0; i < mapInitJson.Layers.length; ++i) {
        var currentLayer = mapInitJson.Layers[i];
        switch (currentLayer.LayerType) {
            case "Vector":
                this.addVectorLayer(currentLayer, overlayLayers);
                break;
            case "Wms":
                this.addWmsLayer(currentLayer, overlayLayers);
                break;
            default:
                console.error("Invalid LayerType " + currentLayer.LayerType + " not added to map layers.");
        }
    }

    this.addLayersToMapLayersControl(baseLayers, overlayLayers, treatAllLayersAsBaseLayers);

    var modalDialog = jQuery(".modal");
    if (!Sitka.Methods.isUndefinedNullOrEmpty(modalDialog))
    {
        modalDialog.on("shown.bs.modal", function()
        {
            self.map.invalidateSize();
            self.setMapBounds(mapInitJson);
        });
    }
    if (!mapInitJson.DisablePopups) {
        this.map.on("click", function (e) { self.getFeatureInfo(e); });
    }
   
    self.setMapBounds(mapInitJson);
};

ProjectFirmaMaps.Map.prototype.addTiledLayerFromAGOL = function (layerConfig, overlayLayers) {
    var tileLayer = L.esri.tiledMapLayer({ url: layerConfig.LayerUrl });
    overlayLayers[layerConfig.DisplayName] = tileLayer;
    // Add to map if layer is on by default
    if (layerConfig.LayerIsOnByDefault) {
        tileLayer.addTo(this.map);
    }
}

ProjectFirmaMaps.Map.prototype.addVectorLayerFromAGOL = function (layerConfig, overlayLayers) {
    var featureLayer = L.esri.featureLayer({ url: layerConfig.LayerUrl });
    if (layerConfig.FeatureNameField) {
        featureLayer.bindPopup(function (evt) {
            var latlng = this.getLatLng();
            if (evt.feature.properties[layerConfig.FeatureNameField]) {
                return L.Util.template('<strong>' + layerConfig.DisplayName + ': </strong> {' + layerConfig.FeatureNameField + '}<br \><strong>Location: </strong>' + latlng.lat.toFixed(4) + ', ' + latlng.lng.toFixed(4), evt.feature.properties);
            }
            return L.Util.template('<div class="alert alert-danger">The configured Feature Name was not found.', evt.feature.properties);
        });
    }
    overlayLayers[layerConfig.DisplayName] = featureLayer;
    // Add to map if layer is on by default
    if (layerConfig.LayerIsOnByDefault) {
        featureLayer.addTo(this.map);
    }
}

ProjectFirmaMaps.Map.prototype.addVectorLayer = function (currentLayer, overlayLayers) {
    var self = this;

    var layerGroup = new L.LayerGroup();
    var layerGeoJson = L.geoJson(currentLayer.GeoJsonFeatureCollection, {
        pointToLayer: function (feature, latlng) {
            var featureColor = feature.properties.FeatureColor == null ? currentLayer.LayerColor : feature.properties.FeatureColor;
            var marker = L.marker(latlng, { icon: L.MakiMarkers.icon({ icon: "marker", color: featureColor, size: "s" }) });
            return marker;
        },
        style: function (feature) {
            var fillPolygonByDefault = _.includes(["Polygon", "MultiPolygon"], feature.geometry.type);
            return {
                color: feature.properties.FeatureColor == null ? currentLayer.LayerColor : feature.properties.FeatureColor,
                weight: feature.properties.FeatureWeight == null ? 2 : feature.properties.FeatureWeight,
                fill: feature.properties.FillPolygon == null ? fillPolygonByDefault : feature.properties.FillPolygon,
                fillOpacity: feature.properties.FillOpacity == null ? 0.2 : feature.properties.FillOpacity
            };
        },
        onEachFeature: function(feature, layer) {
            self.bindPopupToFeature(layer, feature);
            self.bindHoverToFeature(layer, feature);
        }
    }).addTo(layerGroup);

    if (currentLayer.LayerInitialVisibility === 1) {
        layerGroup.addTo(this.map);
    }
   
    if (currentLayer.LayerIconImageLocation != undefined && currentLayer.LayerIconImageLocation != null && currentLayer.LayerIconImageLocation != "") {
        overlayLayers["<img src=\"" + currentLayer.LayerIconImageLocation + "\" />" + currentLayer.LayerName] =
            layerGroup;
    } else {
        overlayLayers[currentLayer.LayerName] = layerGroup;
    }
    this.vectorLayers.push(layerGeoJson);
};

ProjectFirmaMaps.Map.prototype.addWmsLayer = function (currentLayer, overlayLayers) {
    var layerGroup = new L.LayerGroup();
    var tmpWmsParams;
    if (currentLayer.HasCqlFilter) {
        tmpWmsParams  = L.Util.extend(this.wmsParams, { layers: currentLayer.MapServerLayerName, cql_filter: currentLayer.CqlFilter});
    } else {
        tmpWmsParams = L.Util.extend(this.wmsParams, { layers: currentLayer.MapServerLayerName, cql_filter: '1=1' });
    }

    var wmsLayer = L.tileLayer.wms(currentLayer.MapServerUrl, tmpWmsParams).addTo(layerGroup);

    if (currentLayer.ContextObjectId) {
        wmsLayer.ContextObjectId = currentLayer.ContextObjectId;
    }

   

    if (currentLayer.LayerInitialVisibility === 1) {
        layerGroup.addTo(this.map);
    }

    wmsLayer.on("click", function (e) { self.getFeatureInfo(e); });

    if (currentLayer.LayerIconImageLocation != undefined && currentLayer.LayerIconImageLocation != null && currentLayer.LayerIconImageLocation != "") {
        overlayLayers["<img src=\"" + currentLayer.LayerIconImageLocation+"\" />" + currentLayer.LayerName] =
            layerGroup;
    } else {
        overlayLayers[currentLayer.LayerName] = layerGroup;
    }
    this.vectorLayers.push(wmsLayer);
};

ProjectFirmaMaps.Map.prototype.wmsParams = {
    service: "WMS",
    transparent: true,
    version: "1.1.1",
    format: "image/png",
    info_format: "application/json",
    tiled: true
};

ProjectFirmaMaps.Map.prototype.wfsParams = {
    service: "WFS",
    version: "2.0",
    request: "GetFeature",
    outputFormat: "application/json",
    SrsName: "EPSG:4326"
};

ProjectFirmaMaps.Map.prototype.addLayersToMapLayersControl = function (baseLayers, overlayLayers, treatAllLayersAsBaseLayers) {

    if (treatAllLayersAsBaseLayers) {

        var groupName = "Find Your Forester";
        var options = {
            // Make the "Find Your Forester" group exclusive (use radio inputs)
            exclusiveGroups: [groupName],
            // Show a checkbox next to non-exclusive group labels for toggling all
            groupCheckboxes: true
        };

        var groupedOverlays = {
            "Find Your Forester" : overlayLayers

        };

        // Use the custom grouped layer control, not "L.control.layers"
        this.layerControl = L.control.groupedLayers(baseLayers, groupedOverlays, options);
        this.layerControl.addTo(this.map);

    } else {
        this.layerControl = L.control.layers(baseLayers, overlayLayers);
        this.layerControl.addTo(this.map);
    }
};

ProjectFirmaMaps.Map.prototype.setMapBounds = function(mapInitJson) {
    this.map.fitBounds([
        [mapInitJson.BoundingBox.Northeast.Latitude, mapInitJson.BoundingBox.Northeast.Longitude],
        [mapInitJson.BoundingBox.Southwest.Latitude, mapInitJson.BoundingBox.Southwest.Longitude]
    ]);
};

ProjectFirmaMaps.Map.prototype.bindPopupToFeature = function (layer, feature) {
    var self = this;
    if (!Sitka.Methods.isUndefinedNullOrEmpty(feature.properties.PopupUrl)) {
        layer.bindPopup("Loading...");
        layer.on("click",
            function(e) {
                //var popup = e.target.getPopup();
                var isMarker = e.target instanceof L.Marker;
                var latlng = isMarker ? e.target.getLatLng() : e.latlng;
                self.map.setView(latlng);
                jQuery.get(feature.properties.PopupUrl).done(function(data) {
                    layer.bindPopup(data).openPopup();
                });

            });
    }
};

ProjectFirmaMaps.Map.prototype.bindHoverToFeature = function (layer, feature) {
    if (!Sitka.Methods.isUndefinedNullOrEmpty(feature.properties["Hover Name"])) {
        layer.bindTooltip(feature.properties["Hover Name"], { direction: "auto", sticky: "true" });

        var originalFillOpacity = layer.options.fillOpacity;
        layer.on("mouseover",
            function () {
                layer.setStyle({ fillOpacity: Math.min(originalFillOpacity + 0.4, 1) });
            });
        layer.on("mouseout",
            function () {
                layer.setStyle({ fillOpacity: originalFillOpacity });
            });
    };
    return layer;
};

ProjectFirmaMaps.Map.prototype.assignClickEventHandler = function(clickEventFunction) {
    var self = this;
    for (var i = 0; i < this.vectorLayers.length; ++i) {
        var currentLayer = this.vectorLayers[i];
        currentLayer.on("click", function(e) { clickEventFunction(self, e); });
    }
    this.map.on("click", function(e) { clickEventFunction(self, e); });
};

ProjectFirmaMaps.Map.prototype.removeClickEventHandler = function() {
    for (var i = 0; i < this.vectorLayers.length; ++i) {
        var currentLayer = this.vectorLayers[i];
        currentLayer.off("click");
    }
    this.map.off("click");
};

    ProjectFirmaMaps.Map.prototype.formatLayerProperty = function (propertyName, propertyValue)
    {
        if (Sitka.Methods.isUndefinedNullOrEmpty(propertyValue))
        {
            propertyValue = "&nbsp";
        }
        return "<span><strong>" + propertyName + ":</strong> " + propertyValue + "</span></br>";
    };
    
    ProjectFirmaMaps.Map.prototype.removeLayerFromMap = function (layerToRemove) {
        if (!Sitka.Methods.isUndefinedNullOrEmpty(layerToRemove)) {
            this.map.removeLayer(layerToRemove);
        }
    };

    ProjectFirmaMaps.Map.prototype.addLayerToLayerControl = function(layer, layerName) {
        this.layerControl.addOverlay(layer, layerName);
    };

    ProjectFirmaMaps.Map.prototype.disableUserInteraction = function() {
        this.map.dragging.disable();
        this.map.touchZoom.disable();
        this.map.doubleClickZoom.disable();
        this.map.scrollWheelZoom.disable();
        this.map.boxZoom.disable();
        this.map.keyboard.disable();
        this.map.attributionControl = false;
        if (this.map.tap) this.map.tap.disable();
        document.getElementById(this.MapDivId).style.cursor = 'default';
        jQuery(".leaflet-control-zoom").css("visibility", "hidden");
        jQuery(".leaflet-control-layers").css("visibility", "hidden");
        this.removeClickEventHandler();
};

    ProjectFirmaMaps.Map.prototype.blockMapImpl = function() {
        this.map.dragging.disable();
        this.map.scrollWheelZoom.disable();
        jQuery("#" + this.MapDivId).block(
            {
                message:
                    "<span style='font-weight: bold; font-size: 14px; margin: 0 2px'>Location Not Specified</span>",
                css: {
                    border: "none",
                    cursor: "default"
                },
                overlayCSS: {
                    backgroundColor: "#D3D3D3",
                    cursor: "default"
                },
                baseZ: 999
            });
    };

    ProjectFirmaMaps.Map.prototype.blockMap = function() {
        var self = this;
        this.blockMapImpl();
        var modalDialog = jQuery(".modal");
        modalDialog.on("shown.bs.modal", function() { self.blockMapImpl(); });
    };

    ProjectFirmaMaps.Map.prototype.unblockMap = function() {
        this.map.dragging.enable();
        jQuery("#" + this.MapDivId).unblock();
    };

    ProjectFirmaMaps.Map.prototype.getWmsLayers = function (filterToActive) {
        var self = this;
        var wmsLayers = this.vectorLayers.filter(function(layer) {
            return layer.hasOwnProperty('wmsParams') && (!filterToActive || self.map.hasLayer(layer));
        });
        return wmsLayers;
    };

    ProjectFirmaMaps.Map.prototype.getVectorLayers = function (filterToActive) {
        var self = this;
        var vectorLayers = this.vectorLayers.filter(function (layer) {
            return !layer.hasOwnProperty('wmsParams') && (!filterToActive || self.map.hasLayer(layer));
        });
        return vectorLayers;
    };

    ProjectFirmaMaps.Map.prototype.getFeatureInfo = function(e) {
        var latlng = e.latlng;
        var self = this;

        var wmsLayers = this.getWmsLayers(true);

        var vecLayers = this.getVectorLayers(true);

        if (highlightOverlay)
            highlightOverlay.remove(); //For any click, remove the existing highlight

        if (wmsLayers.length > 0) {
            this.popupForWMSAndVectorLayers(wmsLayers, vecLayers, latlng);
        } else {
            this.popupForVectorLayers(vecLayers, latlng);
        }
    };

    ProjectFirmaMaps.Map.prototype.popupForVectorLayers = function (vecLayers, latlng) {
        var allLayers = [];

        for (var j = 0; j < vecLayers.length; ++j) {
            var currentLayer = vecLayers[j];
            var vectorLayerInfoHtmlForPopup = this.getVectorLayerInfoHtmlForPopup(currentLayer, latlng);
            allLayers.push(vectorLayerInfoHtmlForPopup);            
        }

        var lat = L.Util.formatNum(latlng.lat, 4);
        var lon = L.Util.formatNum(latlng.lng, 4);
        allLayers.push({
            label: "Location",
            link: lat + ", " + lon
        });
    
        this.map.setView(latlng);
        if (!jQuery('#findYourForesterContainer')) {
            this.map.openPopup(L.popup({ maxWidth: 200 }).setLatLng(latlng).setContent(this.htmlPopupContents(allLayers)).openOn(this.map));
        }
    };

ProjectFirmaMaps.Map.prototype.htmlPopupContents = function (allLayers) {
    var self = this;
        var uniqueArray = this.removeDuplicatesFromArray(allLayers, "label");

        var html = "<div>";

        _.forEach(uniqueArray,
            function (layer) {
                if (layer.label != null) {
                    html += self.formatLayerProperty(layer.label, layer.link);
                }
                
            });

        html += "</div>";

        return html;
    };



    ProjectFirmaMaps.Map.prototype.getVectorLayerInfoHtmlForPopup = function(currentLayer, latlng) {
        var key = null,
            value = null;
        var match = leafletPip.pointInLayer(
            // the clicked point
            latlng,
            // this layer
            currentLayer,
            // whether to stop at first match
            true);

        var propertiesGroupedByKey = _(match)
        .map(function (x) {
            return _.keys(x.feature.properties);
        })
        .flatten()
        .uniq()
        .map(function (x) {
            return {
                key: x,
                values: _(match)
                    .map(function (y) {
                        return y.feature.properties[x];
                    })
                    .filter()
                    .value()
            };
        })
        .value();

        // if there's overlap, add some content to the popup: the layer name
        // and a table of attributes
        for (var i = 0; i < propertiesGroupedByKey.length; i++) {
            var group = propertiesGroupedByKey[i];
            if (group.key !== "Hover Name") {
                key = group.values.length > 1
                        ? group.key + "s" // pluralized
                        : group.key,
                value = group.values.join(", ");
            }
        }
        
        return {
            label: key,
            link: value
        };
    };

    ProjectFirmaMaps.Map.prototype.popupForWMSAndVectorLayers = function(wmsLayers, vecLayers, latlng) {
        var self = this;

        var mapBounds = this.map.getBounds();
        var crs = this.map.options.crs; // or L.CRS.EPSG3857, or your custom CRS

        // these are the SouthWest and NorthEast points 
        // projected from LatLng into used crs
        var sw = crs.project(this.map.getBounds().getSouthWest());
        var ne = crs.project(this.map.getBounds().getNorthEast());
        var bbox_in_crs = sw.x + ',' + sw.y + ',' + ne.x + ',' + ne.y;

        
        var point = this.map.latLngToContainerPoint(latlng, this.map.getZoom()),
            size = this.map.getSize(),
            projection = this.map.options.crs.code,

            geospatialAreaWMSParams = {
                service: 'WMS',
                version: "1.1.1",
                request: 'GetFeatureInfo',
                layers: null,
                styles: "",
                srs: projection,
                bbox: bbox_in_crs, 
                height: size.y,
                width: size.x,
                query_layers: null,
                info_format: 'application/json'
            };

        geospatialAreaWMSParams['x'] = point.x; 
        geospatialAreaWMSParams['y'] = point.y;
        
        var ajaxCalls = [];

        for (var j = 0; j < wmsLayers.length; ++j) {
            var layer = wmsLayers[j];
            geospatialAreaWMSParams.layers = wmsLayers[j].wmsParams.layers;
            geospatialAreaWMSParams.query_layers = wmsLayers[j].wmsParams.layers;

            if (wmsLayers[j].wmsParams.cql_filter) {
                geospatialAreaWMSParams.cql_filter = wmsLayers[j].wmsParams.cql_filter;
            }
            var query = layer._url;// + L.Util.getParamString(geospatialAreaWMSParams, null, true);     
            ajaxCalls.push(jQuery.when(jQuery.ajax({ url: query, type: "POST", data: L.Util.getParamString(geospatialAreaWMSParams, null, true) }))
                .then(function (response) {
                    return self.formatGeospatialAreaResponse(response).then(function(status) {
                            return status;
                        });
                }));
     
        }

        this.carryOutPromises(ajaxCalls).then(
            function (responses) {
                var allLayers = [];
                //vector layers
                for (var j = 0; j < vecLayers.length; ++j) {
                    var currentLayer = vecLayers[j];

                    var vectorLayerInfoHtmlForPopup = self.getVectorLayerInfoHtmlForPopup(currentLayer, latlng);
                    allLayers.push(vectorLayerInfoHtmlForPopup);
                }
                //wms layers
                _.forEach(responses,
                    function (resp) {
                        if (resp == null) {
                            return;
                        }
                        allLayers.push(resp);
                    });
                //lat lon
                var lat = L.Util.formatNum(latlng.lat, 4);
                var lon = L.Util.formatNum(latlng.lng, 4);
                allLayers.push({
                    label: "Location",
                    link: lat + ", " + lon
                });

                self.map.setView(latlng);
                if (jQuery('#findYourForesterContainer').length > 0) {
                    //jQuery('#findYourForesterContainer').html(self.htmlPopupContents(allLayers));
                } else {
                    self.map.openPopup(L.popup({ maxWidth: 200, maxHeight: 250 }).setLatLng(latlng).setContent(self.htmlPopupContents(allLayers)).openOn(self.map));
                }
            },
            function(responses) {
                console.log("error getting wms feature info");
            }
        );
        return ajaxCalls;
    };


ProjectFirmaMaps.Map.prototype.carryOutPromises = function (deferreds) {
    var deferred = new jQuery.Deferred();
    $.when.apply(jQuery, deferreds).then(
        function () {
            deferred.resolve(Array.prototype.slice.call(arguments));
        },
        function () {
            deferred.fail(Array.prototype.slice.call(arguments));
        });

    return deferred;
}

ProjectFirmaMaps.Map.prototype.removeDuplicatesFromArray = function (originalArray, prop) {
    var newArray = [];
    var lookupObject = {};

    for (var i in originalArray) {
        lookupObject[originalArray[i][prop]] = originalArray[i];
    }

    for (i in lookupObject) {
        newArray.push(lookupObject[i]);
    }
    return newArray;
}

ProjectFirmaMaps.Map.prototype.formatGeospatialAreaResponse = function (json) {
    var deferred = new jQuery.Deferred();
    if (typeof json.features !== "undefined" && json.features.length > 0) {

        var firstFeature = json.features[0];

        var url = "";
        var linkText = "";
        var labelText = "";
        var linkHtml = "";
    switch (firstFeature.geometry_name) {
        case "DNRUplandRegionLocation":
            url = "/DNRUplandRegion/Detail/" + firstFeature.properties.DNRUplandRegionID;
            linkText = firstFeature.properties.DNRUplandRegionName;
            linkHtml = "<a title='' href='" + url + "'>" + linkText + "</a>";
            labelText = "DNR Upland Region";
            deferred.resolve({
                label: labelText,
                link: linkHtml
            });
            break;
        case "PriorityLandscapeLocation":
            url = "/PriorityLandscape/Detail/" + firstFeature.properties.PriorityLandscapeID;
            linkText = firstFeature.properties.PriorityLandscapeName;
            linkHtml = "<a title='' href='" + url + "'>" + linkText + "</a>";
            labelText = "Priority Landscape";
            deferred.resolve({
                label: labelText,
                link: linkHtml
            });
            break;
        case "TreatmentFeature":
            url = "/Project/Detail/" + firstFeature.properties.ProjectID;
            linkText = firstFeature.properties.ProjectName;
            linkHtml = "<a title='' href='" + url + "'>" + linkText + "</a>";
            labelText = "Project";
            deferred.resolve({
                label: labelText,
                link: linkHtml
            });
            break;
        case "ProjectLocationGeometry":
            if (this.ProjectDetailedLocationSelectedLayerUrl) {
                var projectLocationDetailedWfsParams = {
                    service: "WFS",
                    version: "2.0",
                    request: "GetFeature",
                    outputFormat: "application/json",
                    SrsName: "EPSG:4326",
                    maxFeatures: 10000,
                    typeNames: 'WADNRForestHealth:ProjectLocationDetailed',
                    cql_filter: 'ProjectID=' + firstFeature.properties.ProjectID
                };

                jQuery.ajax({
                    url: this.ProjectDetailedLocationSelectedLayerUrl +
                        L.Util.getParamString(projectLocationDetailedWfsParams),
                    dataType: 'json'
                }).done(function(data) {
                    if (data.features.length > 0) {
                        //Highlight feature(s) with new layer
                        var highlightStyle = {
                            "color": "#00FFFF",
                            "weight": 2,
                            "opacity": 0.5
                        };
                        highlightOverlay = L.geoJSON(data, { style: highlightStyle });
                        highlightOverlay.addTo(mapOutsideScope);
                    }
                }).fail(function(data) {
                    console.log(data);
                });
            }
            //break; //This is intentionally commented-out to fall through to ProjectLocationPoint to get the same popup
        case "ProjectLocationPoint":
            // TK 4/9/25 - At some point we moved the simple location layer from geoserver back to geojson. and the large popup is created there. commenting this out for now and replacing with a simple popup for usage on other maps that now use geoserver
            //queryUrl = "/Project/ProjectMapPopup/" + firstFeature.properties.ProjectID;
            //labelText = "Project";

            //var mapPopupAjaxCall = [];
            //mapPopupAjaxCall.push(jQuery.when(jQuery.ajax({ url: queryUrl }))
            //    .then(function(response) { return response; }));

            //this.carryOutPromises(mapPopupAjaxCall).then(
            //    function(responses) {
            //        linkHtml = deferred.resolve({
            //            label: labelText,
            //            link: responses[0]
            //        });
            //    },
            //    function(responses) {
            //        console.log("error getting project popup info: " + queryUrl);
            //        deferred.resolve(null);
            //    }
            //);
            url = "/Project/Detail/" + firstFeature.properties.ProjectID;
            linkText = firstFeature.properties.ProjectName;
            linkHtml = "<a title='' href='" + url + "'>" + linkText + "</a>";
            labelText = "Project";
            deferred.resolve({
                label: labelText,
                link: linkHtml
            });
            break;
        case "WashingtonCountyLocation":
            linkText = firstFeature.properties.WashingtonCountyFullName;
            labelText = "Washington County";
            deferred.resolve({
                label: labelText,
                link: linkText
            });
            break;
        case "WashingtonLegislativeDistrictLocation":
            linkText = firstFeature.properties.WashingtonLegislativeDistrictName;
            labelText = "Washington Legislative District";
            deferred.resolve({
                label: labelText,
                link: linkText
            });
                break;
        case "ForesterWorkUnitLocation":
            labelText = firstFeature.properties.ForesterRoleDisplayName;
            if (firstFeature.properties.ForesterRoleDefinition) {
                labelText += " <span tabindex=\"0\" class=\"helpicon glyphicon glyphicon-question-sign\" onmouseover=\"jQuery(this).popover('show')\" data-content=\"" + firstFeature.properties.ForesterRoleDefinition + "\" data-html=\"true\" data-toggle=\"popover\" data-placement=\"top\" data-trigger=\"hover focus\"></span>";
            }
            if (firstFeature.properties.FirstName) {
                linkText = "<br/>" + firstFeature.properties.FirstName + " " + firstFeature.properties.LastName + "<br/>";

                if (firstFeature.properties.Phone) {
                    linkText += "<a href=\"tel:" + firstFeature.properties.Phone + "\">" + firstFeature.properties.Phone + "</a> <br/>";
                }

                if (firstFeature.properties.Email) {
                    linkText += "<a href=\"mailto:" + firstFeature.properties.Email + "\">" + firstFeature.properties.Email + "</a> <br/>";
                }
            } else {
                linkText = "<br/>This role is unassigned for this region.<br/>";
            }
            
            deferred.resolve({
                label: labelText,
                link: linkText,
                properties: firstFeature.properties
            });
            break;
        default:
                console.log("layer not configured for feature info: " + firstFeature.geometry_name);
            deferred.resolve(null);
        }

    } else {
        console.log("json object contains no features. unexpected wms response.");
        deferred.resolve(null);
    }
    return deferred.promise();
};
