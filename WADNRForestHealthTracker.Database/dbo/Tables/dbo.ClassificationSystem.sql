CREATE TABLE dbo.ClassificationSystem
(
    ClassificationSystemID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ClassificationSystem_ClassificationSystemID PRIMARY KEY,
    ClassificationSystemName varchar(100) NOT NULL,
    CONSTRAINT AK_ClassificationSystem_ClassificationSystemName UNIQUE (ClassificationSystemName)
)
GO
