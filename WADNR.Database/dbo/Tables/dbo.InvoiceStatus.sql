CREATE TABLE [dbo].[InvoiceStatus](
    [InvoiceStatusID] [int] NOT NULL CONSTRAINT [PK_InvoiceStatus_InvoiceStatusID] PRIMARY KEY,
    [InvoiceStatusName] [varchar](50) NOT NULL,
    [InvoiceStatusDisplayName] [varchar](50) NOT NULL,
    CONSTRAINT [AK_InvoiceStatus_InvoiceStatusDisplayName] UNIQUE ([InvoiceStatusDisplayName]),
    CONSTRAINT [AK_InvoiceStatus_InvoiceStatusName] UNIQUE ([InvoiceStatusName])
)
GO
