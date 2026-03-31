export interface AreaConfig {
    subdomain: string;
    displayName: string;
    colorScheme: string;
}

export const AREA_CONFIGS: AreaConfig[] = [
    {
        subdomain: "main",
        displayName: "Lake Tahoe Info",
        colorScheme: "default",
    },
    {
        subdomain: "climate",
        displayName: "Climate Resilience Dashboard",
        colorScheme: "climate",
    },
    {
        subdomain: "stormwater",
        displayName: "Stormwater Tools",
        colorScheme: "stormwater",
    },
    {
        subdomain: "eip",
        displayName: "EIP Project Tracker",
        colorScheme: "eip",
    },
    {
        subdomain: "clarity",
        displayName: "Lake Clarity Tracker",
        colorScheme: "clarity",
    },
    {
        subdomain: "monitoring",
        displayName: "Monitoring Dashboard",
        colorScheme: "monitoring",
    },
    {
        subdomain: "parcels",
        displayName: "Parcel Tracker",
        colorScheme: "parcels",
    },
    {
        subdomain: "marketplace",
        displayName: "TDR Marketplace",
        colorScheme: "marketplace",
    },
    {
        subdomain: "threshold",
        displayName: "Threshold Dashboard",
        colorScheme: "threshold",
    },
    {
        subdomain: "transportation",
        displayName: "Transportation Tracker",
        colorScheme: "transportation",
    },
];
