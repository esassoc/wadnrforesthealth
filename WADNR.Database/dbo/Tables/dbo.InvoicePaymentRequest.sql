CREATE TABLE [dbo].[InvoicePaymentRequest](
    [InvoicePaymentRequestID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_InvoicePaymentRequest_InvoicePaymentRequestID] PRIMARY KEY,
    [ProjectID] [int] NOT NULL CONSTRAINT [FK_InvoicePaymentRequest_Project_ProjectID] FOREIGN KEY REFERENCES [dbo].[Project]([ProjectID]),
    [VendorID] [int] NULL CONSTRAINT [FK_InvoicePaymentRequest_Vendor_VendorID] FOREIGN KEY REFERENCES [dbo].[Vendor]([VendorID]),
    [PreparedByPersonID] [int] NULL CONSTRAINT [FK_InvoicePaymentRequest_Person_PreparedByPersonID_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
    [PurchaseAuthority] [varchar](255) NULL,
    [PurchaseAuthorityIsLandownerCostShareAgreement] [bit] NOT NULL,
    [Duns] [varchar](20) NULL,
    [InvoicePaymentRequestDate] [date] NOT NULL,
    [Notes] [varchar](max) NULL
)
GO