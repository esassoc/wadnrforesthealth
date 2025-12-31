CREATE TABLE dbo.CustomPageDisplayType
(
    CustomPageDisplayTypeID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_CustomPageDisplayType_CustomPageDisplayTypeID PRIMARY KEY,
    CustomPageDisplayTypeName varchar(100) NOT NULL,
    CustomPageDisplayTypeDisplayName varchar(100) NOT NULL,
    CustomPageDisplayTypeDescription varchar(256) NOT NULL,
    CONSTRAINT AK_CustomPageDisplayType_CustomPageDisplayTypeName UNIQUE (CustomPageDisplayTypeName)
)
GO
