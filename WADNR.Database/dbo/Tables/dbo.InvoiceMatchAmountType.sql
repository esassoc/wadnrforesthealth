CREATE TABLE [dbo].[InvoiceMatchAmountType](
    [InvoiceMatchAmountTypeID] [int] NOT NULL CONSTRAINT [PK_InvoiceMatchAmountType_InvoiceMatchAmountTypeID] PRIMARY KEY,
    [InvoiceMatchAmountTypeName] [varchar](50) NOT NULL,
    [InvoiceMatchAmountTypeDisplayName] [varchar](50) NOT NULL,
    CONSTRAINT [AK_InvoiceMatchAmountType_InvoiceMatchAmountTypeDisplayName] UNIQUE ([InvoiceMatchAmountTypeDisplayName]),
    CONSTRAINT [AK_InvoiceMatchAmountType_InvoiceMatchAmountTypeName] UNIQUE ([InvoiceMatchAmountTypeName])
)
GO
