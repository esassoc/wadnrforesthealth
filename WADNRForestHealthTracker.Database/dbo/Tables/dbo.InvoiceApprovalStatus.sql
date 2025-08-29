CREATE TABLE [dbo].[InvoiceApprovalStatus](
    [InvoiceApprovalStatusID] [int] NOT NULL CONSTRAINT [PK_InvoiceApprovalStatus_InvoiceApprovalStatusID] PRIMARY KEY,
    [InvoiceApprovalStatusName] [varchar](50) NOT NULL,
    CONSTRAINT [AK_InvoiceApprovalStatus_InvoiceApprovalStatusName] UNIQUE ([InvoiceApprovalStatusName])
)
GO
