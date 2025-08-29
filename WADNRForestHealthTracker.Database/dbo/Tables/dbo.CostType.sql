CREATE TABLE dbo.CostType
(
    CostTypeID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_CostType_CostTypeID PRIMARY KEY,
    CostTypeName varchar(100) NOT NULL,
    CONSTRAINT AK_CostType_CostTypeName UNIQUE (CostTypeName)
)
GO
