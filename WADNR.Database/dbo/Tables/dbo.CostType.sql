CREATE TABLE dbo.CostType
(
    CostTypeID int NOT NULL CONSTRAINT PK_CostType_CostTypeID PRIMARY KEY,
    [CostTypeDisplayName] [varchar](255) NOT NULL,
    [CostTypeName] [varchar](31) NOT NULL,
    [IsValidInvoiceLineItemCostType] [bit] NOT NULL,
    [SortOrder] [int] NOT NULL
)
GO
