SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Invoice](
	[InvoiceID] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceIdentifyingName] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[InvoiceDate] [datetime] NOT NULL,
	[PaymentAmount] [money] NULL,
	[InvoiceApprovalStatusID] [int] NOT NULL,
	[InvoiceApprovalStatusComment] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[InvoiceMatchAmountTypeID] [int] NOT NULL,
	[MatchAmount] [money] NULL,
	[InvoiceStatusID] [int] NOT NULL,
	[InvoiceFileResourceID] [int] NULL,
	[InvoicePaymentRequestID] [int] NOT NULL,
	[FundSourceID] [int] NULL,
	[ProgramIndexID] [int] NULL,
	[ProjectCodeID] [int] NULL,
	[OrganizationCodeID] [int] NULL,
	[InvoiceNumber] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Fund] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Appn] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[SubObject] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_Invoice_InvoiceID] PRIMARY KEY CLUSTERED 
(
	[InvoiceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[Invoice]  WITH CHECK ADD  CONSTRAINT [FK_Invoice_FileResource_InvoiceFileResourceID_FileResourceID] FOREIGN KEY([InvoiceFileResourceID])
REFERENCES [dbo].[FileResource] ([FileResourceID])
GO
ALTER TABLE [dbo].[Invoice] CHECK CONSTRAINT [FK_Invoice_FileResource_InvoiceFileResourceID_FileResourceID]
GO
ALTER TABLE [dbo].[Invoice]  WITH CHECK ADD  CONSTRAINT [FK_Invoice_FundSource_FundSourceID] FOREIGN KEY([FundSourceID])
REFERENCES [dbo].[FundSource] ([FundSourceID])
GO
ALTER TABLE [dbo].[Invoice] CHECK CONSTRAINT [FK_Invoice_FundSource_FundSourceID]
GO
ALTER TABLE [dbo].[Invoice]  WITH CHECK ADD  CONSTRAINT [FK_Invoice_InvoiceApprovalStatus_InvoiceApprovalStatusID] FOREIGN KEY([InvoiceApprovalStatusID])
REFERENCES [dbo].[InvoiceApprovalStatus] ([InvoiceApprovalStatusID])
GO
ALTER TABLE [dbo].[Invoice] CHECK CONSTRAINT [FK_Invoice_InvoiceApprovalStatus_InvoiceApprovalStatusID]
GO
ALTER TABLE [dbo].[Invoice]  WITH CHECK ADD  CONSTRAINT [FK_Invoice_InvoiceMatchAmountType_InvoiceMatchAmountTypeID] FOREIGN KEY([InvoiceMatchAmountTypeID])
REFERENCES [dbo].[InvoiceMatchAmountType] ([InvoiceMatchAmountTypeID])
GO
ALTER TABLE [dbo].[Invoice] CHECK CONSTRAINT [FK_Invoice_InvoiceMatchAmountType_InvoiceMatchAmountTypeID]
GO
ALTER TABLE [dbo].[Invoice]  WITH CHECK ADD  CONSTRAINT [FK_Invoice_InvoicePaymentRequest_InvoicePaymentRequestID] FOREIGN KEY([InvoicePaymentRequestID])
REFERENCES [dbo].[InvoicePaymentRequest] ([InvoicePaymentRequestID])
GO
ALTER TABLE [dbo].[Invoice] CHECK CONSTRAINT [FK_Invoice_InvoicePaymentRequest_InvoicePaymentRequestID]
GO
ALTER TABLE [dbo].[Invoice]  WITH CHECK ADD  CONSTRAINT [FK_Invoice_InvoiceStatus_InvoiceStatusID] FOREIGN KEY([InvoiceStatusID])
REFERENCES [dbo].[InvoiceStatus] ([InvoiceStatusID])
GO
ALTER TABLE [dbo].[Invoice] CHECK CONSTRAINT [FK_Invoice_InvoiceStatus_InvoiceStatusID]
GO
ALTER TABLE [dbo].[Invoice]  WITH CHECK ADD  CONSTRAINT [FK_Invoice_OrganizationCode_OrganizationCodeID] FOREIGN KEY([OrganizationCodeID])
REFERENCES [dbo].[OrganizationCode] ([OrganizationCodeID])
GO
ALTER TABLE [dbo].[Invoice] CHECK CONSTRAINT [FK_Invoice_OrganizationCode_OrganizationCodeID]
GO
ALTER TABLE [dbo].[Invoice]  WITH CHECK ADD  CONSTRAINT [FK_Invoice_ProgramIndex_ProgramIndexID] FOREIGN KEY([ProgramIndexID])
REFERENCES [dbo].[ProgramIndex] ([ProgramIndexID])
GO
ALTER TABLE [dbo].[Invoice] CHECK CONSTRAINT [FK_Invoice_ProgramIndex_ProgramIndexID]
GO
ALTER TABLE [dbo].[Invoice]  WITH CHECK ADD  CONSTRAINT [FK_Invoice_ProjectCode_ProjectCodeID] FOREIGN KEY([ProjectCodeID])
REFERENCES [dbo].[ProjectCode] ([ProjectCodeID])
GO
ALTER TABLE [dbo].[Invoice] CHECK CONSTRAINT [FK_Invoice_ProjectCode_ProjectCodeID]