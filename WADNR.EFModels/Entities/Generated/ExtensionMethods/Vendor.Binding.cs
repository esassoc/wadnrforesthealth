//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Vendor]
namespace WADNR.EFModels.Entities
{
    public partial class Vendor
    {
        public int PrimaryKey => VendorID;


        public static class FieldLengths
        {
            public const int VendorName = 100;
            public const int StatewideVendorNumber = 20;
            public const int StatewideVendorNumberSuffix = 10;
            public const int VendorType = 3;
            public const int BillingAgency = 200;
            public const int BillingSubAgency = 200;
            public const int BillingFund = 200;
            public const int BillingFundBreakout = 200;
            public const int VendorAddressLine1 = 200;
            public const int VendorAddressLine2 = 200;
            public const int VendorAddressLine3 = 200;
            public const int VendorCity = 200;
            public const int VendorState = 200;
            public const int VendorZip = 200;
            public const int Remarks = 200;
            public const int VendorPhone = 200;
            public const int VendorStatus = 200;
            public const int TaxpayerIdNumber = 200;
            public const int Email = 200;
        }
    }
}