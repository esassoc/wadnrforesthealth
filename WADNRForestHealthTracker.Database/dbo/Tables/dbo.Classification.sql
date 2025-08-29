CREATE TABLE dbo.Classification
(
    ClassificationID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Classification_ClassificationID PRIMARY KEY,
    ClassificationSystemID int NOT NULL CONSTRAINT FK_Classification_ClassificationSystem_ClassificationSystemID FOREIGN KEY REFERENCES dbo.ClassificationSystem(ClassificationSystemID),
    ClassificationName varchar(100) NOT NULL,
    CONSTRAINT AK_Classification_ClassificationSystemID_ClassificationName UNIQUE (ClassificationSystemID, ClassificationName)
)
GO