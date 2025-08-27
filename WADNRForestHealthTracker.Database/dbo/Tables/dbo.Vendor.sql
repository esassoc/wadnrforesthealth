SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Vendor](
	[VendorID] [int] IDENTITY(1,1) NOT NULL,
	[VendorName] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[StatewideVendorNumber] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[StatewideVendorNumberSuffix] [varchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[VendorType] [varchar](3) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[BillingAgency] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[BillingSubAgency] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[BillingFund] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[BillingFundBreakout] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[VendorAddressLine1] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[VendorAddressLine2] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[VendorAddressLine3] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[VendorCity] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[VendorState] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[VendorZip] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Remarks] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[VendorPhone] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[VendorStatus] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TaxpayerIdNumber] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Email] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_Vendor_VendorID] PRIMARY KEY CLUSTERED 
(
	[VendorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_Vendor_StatewideVendorNumber_StatewideVendorNumberSuffix] UNIQUE NONCLUSTERED 
(
	[StatewideVendorNumber] ASC,
	[StatewideVendorNumberSuffix] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
