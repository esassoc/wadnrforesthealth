using System.Collections.Generic;

namespace SitkaCaptureService
{
    public class CapturePostData
    {
        public List<string> cssStrings { get; set; }
        public List<string> cssUrls { get; set; }
        public string url { get; set; }
        public string cssSelector { get; set; }
        public string html { get; set; }

        public string waitForSelector { get; set; }
        public int timeoutInMilliseconds { get; set; }
        public bool debug { get; set; }
        public bool debugNetwork { get; set; }
    }
}