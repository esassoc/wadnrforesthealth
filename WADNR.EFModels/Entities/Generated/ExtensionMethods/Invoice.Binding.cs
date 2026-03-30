//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Invoice]
namespace WADNR.EFModels.Entities
{
    public partial class Invoice
    {
        public int PrimaryKey => InvoiceID;
        public InvoiceMatchAmountType InvoiceMatchAmountType => InvoiceMatchAmountType.AllLookupDictionary[InvoiceMatchAmountTypeID];
        public InvoiceStatus InvoiceStatus => InvoiceStatus.AllLookupDictionary[InvoiceStatusID];
        public OrganizationCode? OrganizationCode => OrganizationCodeID.HasValue ? OrganizationCode.AllLookupDictionary[OrganizationCodeID.Value] : null;

        public static class FieldLengths
        {
            public const int InvoiceIdentifyingName = 255;
            public const int InvoiceNumber = 50;
            public const int Fund = 255;
            public const int Appn = 255;
            public const int SubObject = 255;
        }
    }
}