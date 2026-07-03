export const environment = {
    production: true,
    staging: false,
    dev: false,
    mainAppApiUrl: "https://internalapi-wadnr.esa-prod.sitkatech.com",
    geoserverMapServiceUrl: "https://geoserver-wadnr.esa-prod.sitkatech.com/geoserver/WADNRForestHealth",
    datadog: {
        clientToken: "pub6bc5bcb39be6b4c926271a35cb8cb46a",
        site: "datadoghq.com",
        service: "wadnr-web",
        env: "prod",
    },
    auth0: {
        domain: "wadnr.us.auth0.com",
        clientId: "q7AHuE3OqhPErLAQm8J1RWowIPJoKEay",
        audience: "WADNRAPI",
    },
};
