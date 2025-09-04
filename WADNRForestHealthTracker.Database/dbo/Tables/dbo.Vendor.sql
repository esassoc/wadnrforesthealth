CREATE TABLE [dbo].[Vendor](
    [VendorID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Vendor_VendorID] PRIMARY KEY,
    [VendorName] [varchar](100) NOT NULL,
    [StatewideVendorNumber] [varchar](20) NOT NULL,
    [StatewideVendorNumberSuffix] [varchar](10) NOT NULL,
    [VendorType] [varchar](3) NULL,
    [BillingAgency] [varchar](200) NULL,
    [BillingSubAgency] [varchar](200) NULL,
    [BillingFund] [varchar](200) NULL,
    [BillingFundBreakout] [varchar](200) NULL,
    [VendorAddressLine1] [varchar](200) NULL,
    [VendorAddressLine2] [varchar](200) NULL,
    [VendorAddressLine3] [varchar](200) NULL,
    [VendorCity] [varchar](200) NULL,
    [VendorState] [varchar](200) NULL,
    [VendorZip] [varchar](200) NULL,
    [Remarks] [varchar](200) NULL,
    [VendorPhone] [varchar](200) NULL,
    [VendorStatus] [varchar](200) NULL,
    [TaxpayerIdNumber] [varchar](200) NULL,
    [Email] [varchar](200) NULL,
    CONSTRAINT [AK_Vendor_StatewideVendorNumber_StatewideVendorNumberSuffix] UNIQUE ([StatewideVendorNumber], [StatewideVendorNumberSuffix])
)
GO
