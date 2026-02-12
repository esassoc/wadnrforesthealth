using System;
using WADNR.EFModels.Entities;

namespace WADNR.API.ReportTemplates.Models
{
    public class ReportTemplateInvoiceModel : ReportTemplateBaseModel
    {
        public DateTime InvoiceDate { get; set; }
        public string InvoiceDateDisplay => InvoiceDate.ToShortDateString();
        public decimal? PaymentAmount { get; set; }
        public string PaymentAmountDisplay(int decimalPlaces = 2) => PaymentAmount.HasValue ? PaymentAmount.Value.ToString($"C{decimalPlaces}", UsCulture) : string.Empty;
        public decimal? MatchAmount { get; set; }
        private string MatchAmountDisplayFromModel { get; set; }
        public string MatchAmountDisplay(int decimalPlaces = 2) => MatchAmount.HasValue ? MatchAmount.Value.ToString($"C{decimalPlaces}", UsCulture) : MatchAmountDisplayFromModel;
        public string FundSourceNumber { get; set; }
        public string ProgramIndexCode { get; set; }
        public string ProjectCodeName { get; set; }
        public string OrganizationCodeValue { get; set; }
        public string OrganizationCodeName { get; set; }
        public string OrganizationCodeDisplay =>
            !string.IsNullOrEmpty(OrganizationCodeValue) && !string.IsNullOrEmpty(OrganizationCodeName)
                ? $"{OrganizationCodeValue} - {OrganizationCodeName}"
                : string.Empty;

        public string InvoiceNumber { get; set; }
        public string Fund { get; set; }
        public string Appn { get; set; }
        public string SubObject { get; set; }

        public ReportTemplateInvoiceModel(Invoice invoice)
        {
            if (invoice != null)
            {
                InvoiceDate = invoice.InvoiceDate;
                PaymentAmount = invoice.PaymentAmount;
                MatchAmount = invoice.MatchAmount;
                // Resolve MatchAmountForDisplay: if type is DollarAmount, show the amount; otherwise show the type name
                if (invoice.InvoiceMatchAmountTypeID == (int)InvoiceMatchAmountTypeEnum.DollarAmount)
                {
                    MatchAmountDisplayFromModel = MatchAmount.HasValue ? MatchAmount.Value.ToString("C2", UsCulture) : string.Empty;
                }
                else if (InvoiceMatchAmountType.AllLookupDictionary.TryGetValue(invoice.InvoiceMatchAmountTypeID, out var matchAmountType))
                {
                    MatchAmountDisplayFromModel = matchAmountType.InvoiceMatchAmountTypeDisplayName;
                }
                else
                {
                    MatchAmountDisplayFromModel = string.Empty;
                }

                FundSourceNumber = invoice.FundSource?.FundSourceNumber;
                ProgramIndexCode = invoice.ProgramIndex?.ProgramIndexCode;
                ProjectCodeName = invoice.ProjectCode?.ProjectCodeName;
                // OrganizationCode FK exists on Invoice but nav property may not be scaffolded
                OrganizationCodeValue = null;
                OrganizationCodeName = null;
                InvoiceNumber = invoice.InvoiceNumber;
                Fund = invoice.Fund;
                Appn = invoice.Appn;
                SubObject = invoice.SubObject;
            }
        }
    }
}
