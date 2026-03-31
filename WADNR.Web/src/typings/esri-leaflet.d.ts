declare module "esri-leaflet" {
    import * as L from "leaflet";

    interface TiledMapLayerOptions extends L.TileLayerOptions {
        url: string;
    }

    interface DynamicMapLayerOptions extends L.LayerOptions {
        url: string;
        layers?: number[];
        layerDefs?: Record<number, string>;
        format?: string;
        transparent?: boolean;
        opacity?: number;
        f?: string;
    }

    interface FeatureLayerOptions extends L.LayerOptions {
        url: string;
        where?: string;
        onEachFeature?: (feature: any, layer: L.Layer) => void;
    }

    function tiledMapLayer(options: TiledMapLayerOptions): L.TileLayer;
    function dynamicMapLayer(options: DynamicMapLayerOptions): L.Layer;
    function featureLayer(options: FeatureLayerOptions): L.Layer;
}

// Side-effect import: patches esri-leaflet featureLayer to render with server symbology
declare module "esri-leaflet-renderers" {}
