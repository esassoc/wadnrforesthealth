SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvoicePaymentRequest](
	[InvoicePaymentRequestID] [int] IDENTITY(1,1) NOT NULL,
	[ProjectID] [int] NOT NULL,
	[VendorID] [int] NULL,
	[PreparedByPersonID] [int] NULL,
	[PurchaseAuthority] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[PurchaseAuthorityIsLandownerCostShareAgreement] [bit] NOT NULL,
	[Duns] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[InvoicePaymentRequestDate] [datetime] NOT NULL,
	[Notes] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_InvoicePaymentRequest_InvoicePaymentRequestID] PRIMARY KEY CLUSTERED 
(
	[InvoicePaymentRequestID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[InvoicePaymentRequest]  WITH CHECK ADD  CONSTRAINT [FK_InvoicePaymentRequest_Person_PreparedByPersonID_PersonID] FOREIGN KEY([PreparedByPersonID])
REFERENCES [dbo].[Person] ([PersonID])
GO
ALTER TABLE [dbo].[InvoicePaymentRequest] CHECK CONSTRAINT [FK_InvoicePaymentRequest_Person_PreparedByPersonID_PersonID]
GO
ALTER TABLE [dbo].[InvoicePaymentRequest]  WITH CHECK ADD  CONSTRAINT [FK_InvoicePaymentRequest_Project_ProjectID] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[Project] ([ProjectID])
GO
ALTER TABLE [dbo].[InvoicePaymentRequest] CHECK CONSTRAINT [FK_InvoicePaymentRequest_Project_ProjectID]
GO
ALTER TABLE [dbo].[InvoicePaymentRequest]  WITH CHECK ADD  CONSTRAINT [FK_InvoicePaymentRequest_Vendor_VendorID] FOREIGN KEY([VendorID])
REFERENCES [dbo].[Vendor] ([VendorID])
GO
ALTER TABLE [dbo].[InvoicePaymentRequest] CHECK CONSTRAINT [FK_InvoicePaymentRequest_Vendor_VendorID]