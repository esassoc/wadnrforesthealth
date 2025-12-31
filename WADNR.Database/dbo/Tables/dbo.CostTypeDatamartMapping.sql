CREATE TABLE dbo.CostTypeDatamartMapping
(
    CostTypeDatamartMappingID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_CostTypeDatamartMapping_CostTypeDatamartMappingID PRIMARY KEY,
    CostTypeID int NOT NULL CONSTRAINT FK_CostTypeDatamartMapping_CostType_CostTypeID FOREIGN KEY REFERENCES dbo.CostType(CostTypeID),
    DatamartObjectCode varchar(10) NOT NULL,
    DatamartObjectName varchar(100) NOT NULL,
    DatamartSubObjectCode varchar(10) NOT NULL,
    DatamartSubObjectName varchar(250) NOT NULL
)
GO