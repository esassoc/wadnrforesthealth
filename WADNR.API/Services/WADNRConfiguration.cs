namespace WADNR.API.Services
{
    public class WADNRConfiguration
    {
        public string DatabaseConnectionString { get; set; }
        public string SendGridApiKey { get; set; }
        public string SitkaEmailRedirect { get; set; }
        public string SitkaSupportEmail { get; set; }
        public string DoNotReplyEmail { get; set; }
        public string LakeTahoeInfoBaseUrl { get; set; }
        public string ParcelTrackerBaseUrl { get; set; }
        public string RECAPTCHA_SECRET_KEY { get; set; }
        public string RECAPTCHA_VERIFY_URL { get; set; }
        public double RECAPTCHA_SCORE_THRESHOLD { get; set; }
        public string WadnrApiKey { get; set; }
        public string AzureBlobStorageConnectionString { get; set; }
        public string SitkaCaptureServiceUrl { get; set; }
        public string WebUrl { get; set; } = "https://wadnrforesthealth.wa.gov";
        public string GDALAPIBaseUrl { get; set; }
        public bool EnableE2ETestAuth { get; set; }
        public Auth0Configuration Auth0 { get; set; } = new();

        // Finance API (ArcGIS Online)
        public string ProjectCodeJsonApiBaseUrl { get; set; }
        public string ProgramIndexJsonApiBaseUrl { get; set; }
        public string VendorJsonApiBaseUrl { get; set; }
        public string FundSourceExpendituresJsonApiBaseUrl { get; set; }
        public string DataImportAuthUrl { get; set; }
        public string DataImportAuthUsername { get; set; }
        public string DataImportAuthPassword { get; set; }
        public string LastLoadDateUrl { get; set; }

        // GIS Data Import (ArcGIS Online)
        public string ArcGisAuthUrl { get; set; }
        public string ArcGisClientId { get; set; }
        public string ArcGisClientSecret { get; set; }
        public string ArcGisLoaDataEasternUrl { get; set; }
        public string ArcGisLoaDataWesternUrl { get; set; }
        public string ArcGisUsfsDataUrl { get; set; }
        public string ArcGisUsfsNepaBoundaryDataUrl { get; set; }

        // Notifications
        public string WebsiteDisplayName { get; set; } = "WA DNR Forest Health Tracker";

        public int Pre2007ProjectCount => 266; // todo: keeping this as a constant here since I doubt it changes now
    }

    public class Auth0Configuration
    {
        public string Authority { get; set; } = "https://wadnr.us.auth0.com/";
        public string Audience { get; set; } = "WADNRAPI";
    }
}