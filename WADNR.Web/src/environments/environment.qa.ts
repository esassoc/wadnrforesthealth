export const environment = {
    production: false,
    staging: true,
    dev: false,
    mainAppApiUrl: "https://internalapi-wadnr.esa-qa.sitkatech.com",
    geoserverMapServiceUrl: "https://geoserver-wadnr.esa-qa.sitkatech.com/geoserver/WADNRForestHealth",
    // datadogClientToken: "pub6bc5bcb39be6b4c926271a35cb8cb46a",
    auth0: {
        domain: "wadnr-qa.us.auth0.com",
        clientId: "QioXm2t3RjItBMOqcFgWOqbRoYzK1gk7",
        redirectUri: "https://wadnr.esa-qa.sitkatech.com",
        audience: "WADNRAPI",
    },
};
