using System.Collections.Generic;

namespace WADNR.API.ReportTemplates.Models
{
    public class ReportTemplateProjectBaseViewModel
    {
        public string ReportTitle { get; set; }
        public List<ReportTemplateProjectModel> ReportModel { get; set; }
    }

    public class ReportTemplateInvoicePaymentRequestBaseViewModel
    {
        public string ReportTitle { get; set; }
        public List<ReportTemplateInvoicePaymentRequestModel> ReportModel { get; set; }
    }
}
