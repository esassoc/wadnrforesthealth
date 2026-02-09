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

        public int Pre2007ProjectCount => 266; // todo: keeping this as a constant here since I doubt it changes now
    }
}