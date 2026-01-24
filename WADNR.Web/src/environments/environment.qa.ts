export const environment = {
    production: false,
    staging: true,
    dev: false,
    mainAppApiUrl: "https://internalapi-wadnr.esa-qa.sitkatech.com",
    geoserverMapServiceUrl: "https://geoserver-wadnr.esa-qa.sitkatech.com/geoserver/WADNRForestHealth",
    // datadogClientToken: "pub6bc5bcb39be6b4c926271a35cb8cb46a",
    auth0: {
        domain: "wadnr.us.auth0.com",
        clientId: "q7AHuE3OqhPErLAQm8J1RWowIPJoKEay",
        redirectUri: "https://wadnr.esa-qa.sitkatech.com",
        audience: "WADNRAPI",
    },
};
