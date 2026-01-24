export const environment = {
    production: false,
    staging: false,
    dev: true,
    mainAppApiUrl: "/api",
    geoserverMapServiceUrl: "http://localhost:3280/geoserver/WADNRForestHealth",
    // datadogClientToken: "pub6bc5bcb39be6b4c926271a35cb8cb46a",
    auth0: {
        domain: "wadnr.us.auth0.com",
        clientId: "q7AHuE3OqhPErLAQm8J1RWowIPJoKEay",
        redirectUri: "https://wadnr.localhost.esassoc.com:3215",
        audience: "WADNRAPI",
    },
};
