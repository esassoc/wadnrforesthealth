import * as L from "leaflet";
declare var require: any;

export class MarkerHelper {
    // Dynamic SVG marker icon generator
    public static svgMarkerIcon(color: string): L.Icon {
        const svg = `<svg width="59" height="76" viewBox="0 0 59 76" fill="none" xmlns="http://www.w3.org/2000/svg">
<g filter="url(#filter0_d_292_393)">
<circle cx="29" cy="27" r="12" fill="white"/>
<path d="M29.5 1C35.98 1.00803 42.2029 3.70755 46.7998 8.52344C51.2549 13.1907 53.829 19.4761 53.9922 26.0762L54 26.7158C53.9996 37.8114 48.3662 47.6207 42.5107 54.8066C36.6682 61.9766 30.7015 66.4156 30.2402 66.7529L30.2383 66.7539C30.0171 66.9162 29.7595 67 29.5 67C29.2405 67 28.9829 66.9162 28.7617 66.7539L28.7598 66.7529C28.2985 66.4156 22.3318 61.9766 16.4893 54.8066C10.6338 47.6207 5.00043 37.8114 5 26.7158C5.0074 19.8834 7.60147 13.3412 12.2002 8.52344C16.7971 3.70755 23.02 1.00803 29.5 1ZM29.5 16C26.7601 16 24.1423 17.1412 22.2197 19.1553C20.2988 21.1678 19.2275 23.8879 19.2275 26.7148C19.2276 28.8238 19.8245 30.889 20.9473 32.6494C22.0704 34.4103 23.6716 35.7897 25.5537 36.6064C27.4364 37.4234 29.5108 37.6388 31.5127 37.2217C33.5144 36.8046 35.3469 35.776 36.7803 34.2744C38.2132 32.7732 39.1842 30.8655 39.5771 28.7959C39.97 26.7266 39.7688 24.5808 38.9971 22.6289C38.2252 20.6769 36.915 19.0011 35.2256 17.8184C33.5353 16.6352 31.5424 16 29.5 16Z" fill="${color}" stroke="#528E33" stroke-width="2"/>
</g>
<defs>
<filter id="filter0_d_292_393" x="0" y="0" width="59" height="76" filterUnits="userSpaceOnUse" color-interpolation-filters="sRGB">
<feFlood flood-opacity="0" result="BackgroundImageFix"/>
<feColorMatrix in="SourceAlpha" type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0" result="hardAlpha"/>
<feOffset dy="4"/>
<feGaussianBlur stdDeviation="2"/>
<feComposite in2="hardAlpha" operator="out"/>
<feColorMatrix type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0.2 0"/>
<feBlend mode="normal" in2="BackgroundImageFix" result="effect1_dropShadow_292_393"/>
<feBlend mode="normal" in="SourceGraphic" in2="effect1_dropShadow_292_393" result="shape"/>
</filter>
</defs>
</svg>`;
        const url = "data:image/svg+xml;base64," + btoa(svg);
        return L.icon({
            iconUrl: url,
            iconSize: [20, 29],
            iconAnchor: [10, 29],
            popupAnchor: [0, -36],
        });
    }
    //Known bug in leaflet that during bundling the default image locations can get messed up
    //https://stackoverflow.com/questions/41144319/leaflet-marker-not-found-production-env
    static iconDefault = MarkerHelper.buildDefaultLeafletMarkerFromMarkerPath("/assets/main/map-icons/marker-icon-black.png");
    static selectedMarker = MarkerHelper.buildDefaultLeafletMarkerFromMarkerPath("/assets/main/map-icons/marker-icon-selected.png");
    static treatmentBMPMarker = MarkerHelper.buildDefaultLeafletMarkerFromMarkerPath("/assets/main/map-icons/marker-icon-violet.png");
    static inventoriedTreatmentBMPMarker = MarkerHelper.buildDefaultLeafletMarkerFromMarkerPath("/assets/main/map-icons/marker-icon-orange.png");
    static pinkMarker = MarkerHelper.buildDefaultLeafletMarkerFromMarkerPath("/assets/main/map-icons/marker-icon-F2BBE0.png");

    public static fixMarkerPath() {
        //delete L.Icon.Default.prototype._getIconUrl;

        L.Icon.Default.mergeOptions({
            iconRetinaUrl: "assets/main/map-icons/marker-icon-2x-black.png",
            iconUrl: "assets/main/map-icons/marker-icon-black.png",
            shadowUrl: "assets/main/map-icons/marker-shadow.png",
        });
    }

    //Function assumes there is a retina version of your image under the same name just with "-2x" appended
    public static buildDefaultLeafletMarkerFromMarkerPath(iconUrl: string): any {
        var retinaUrl = iconUrl.replace("marker-icon", "marker-icon-2x");
        return MarkerHelper.buildDefaultLeafletMarker(iconUrl, retinaUrl);
    }

    private static buildDefaultLeafletMarker(iconUrl: string, iconRetinaUrl: string): any {
        let shadowUrl = "assets/main/map-icons/marker-shadow.png";
        return L.icon({
            iconRetinaUrl,
            iconUrl,
            shadowUrl,
            iconSize: [25, 41],
            iconAnchor: [12, 41],
            popupAnchor: [1, -34],
            tooltipAnchor: [16, -28],
            shadowSize: [41, 41],
        });
    }
}
