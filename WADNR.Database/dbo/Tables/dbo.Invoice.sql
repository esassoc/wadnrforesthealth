
CREATE TABLE [dbo].[Invoice](
	[InvoiceID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Invoice_InvoiceID] PRIMARY KEY,
	[InvoiceIdentifyingName] [varchar](255) NULL,
	[InvoiceDate] [datetime] NOT NULL,
	[PaymentAmount] [money] NULL,
	[InvoiceApprovalStatusID] [int] NOT NULL CONSTRAINT [FK_Invoice_InvoiceApprovalStatus_InvoiceApprovalStatusID] FOREIGN KEY REFERENCES [dbo].[InvoiceApprovalStatus]([InvoiceApprovalStatusID]),
	[InvoiceApprovalStatusComment] [varchar](max) NULL,
	[InvoiceMatchAmountTypeID] [int] NOT NULL CONSTRAINT [FK_Invoice_InvoiceMatchAmountType_InvoiceMatchAmountTypeID] FOREIGN KEY REFERENCES [dbo].[InvoiceMatchAmountType]([InvoiceMatchAmountTypeID]),
	[MatchAmount] [money] NULL,
	[InvoiceStatusID] [int] NOT NULL CONSTRAINT [FK_Invoice_InvoiceStatus_InvoiceStatusID] FOREIGN KEY REFERENCES [dbo].[InvoiceStatus]([InvoiceStatusID]),
	[InvoiceFileResourceID] [int] NULL CONSTRAINT [FK_Invoice_FileResource_InvoiceFileResourceID_FileResourceID] FOREIGN KEY REFERENCES [dbo].[FileResource]([FileResourceID]),
	[InvoicePaymentRequestID] [int] NOT NULL CONSTRAINT [FK_Invoice_InvoicePaymentRequest_InvoicePaymentRequestID] FOREIGN KEY REFERENCES [dbo].[InvoicePaymentRequest]([InvoicePaymentRequestID]),
	[FundSourceID] [int] NULL CONSTRAINT [FK_Invoice_FundSource_FundSourceID] FOREIGN KEY REFERENCES [dbo].[FundSource]([FundSourceID]),
	[ProgramIndexID] [int] NULL CONSTRAINT [FK_Invoice_ProgramIndex_ProgramIndexID] FOREIGN KEY REFERENCES [dbo].[ProgramIndex]([ProgramIndexID]),
	[ProjectCodeID] [int] NULL CONSTRAINT [FK_Invoice_ProjectCode_ProjectCodeID] FOREIGN KEY REFERENCES [dbo].[ProjectCode]([ProjectCodeID]),
	[OrganizationCodeID] [int] NULL CONSTRAINT [FK_Invoice_OrganizationCode_OrganizationCodeID] FOREIGN KEY REFERENCES [dbo].[OrganizationCode]([OrganizationCodeID]),
	[InvoiceNumber] [varchar](50) NOT NULL,
	[Fund] [varchar](255) NULL,
	[Appn] [varchar](255) NULL,
	[SubObject] [varchar](255) NULL
)
GO