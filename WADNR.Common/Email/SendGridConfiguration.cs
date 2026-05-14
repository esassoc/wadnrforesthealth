namespace WADNR.Common.EMail;

public class SendGridConfiguration
{
    public string SendGridApiKey { get; set; } = string.Empty;
    public string SitkaEmailRedirect { get; set; } = string.Empty;
    public string MailLogBcc { get; set; } = string.Empty;
    public string SitkaSupportEmail { get; set; } = string.Empty;
    public string DoNotReplyEmail { get; set; } = string.Empty;
}