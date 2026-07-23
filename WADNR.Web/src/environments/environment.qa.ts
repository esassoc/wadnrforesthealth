export const environment = {
    production: false,
    staging: true,
    dev: false,
    mainAppApiUrl: "https://internalapi-wadnr.esa-qa.sitkatech.com",
    geoserverMapServiceUrl: "https://geoserver-wadnr.esa-qa.sitkatech.com/geoserver/WADNRForestHealth",
    datadog: {
        clientToken: "pub6bc5bcb39be6b4c926271a35cb8cb46a",
        site: "datadoghq.com",
        service: "wadnr-web",
        env: "qa",
    },
    auth0: {
        domain: "wadnr-qa.us.auth0.com",
        clientId: "QioXm2t3RjItBMOqcFgWOqbRoYzK1gk7",
        audience: "WADNRAPI",
    },
};
