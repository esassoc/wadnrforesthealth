export const environment = {
    production: false,
    staging: false,
    dev: true,
    mainAppApiUrl: "/api",
    geoserverMapServiceUrl: "http://localhost:3280/geoserver/WADNRForestHealth",
    datadog: {
        // Leave clientToken empty locally so dev noise is not forwarded to Datadog.
        clientToken: "",
        site: "datadoghq.com",
        service: "wadnr-web",
        env: "local",
    },
    auth0: {
        domain: "wadnr-qa.us.auth0.com",
        clientId: "QioXm2t3RjItBMOqcFgWOqbRoYzK1gk7",
        audience: "WADNRAPI",
    },
};
