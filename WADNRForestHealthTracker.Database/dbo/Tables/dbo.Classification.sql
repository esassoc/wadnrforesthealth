CREATE TABLE dbo.Classification
(
    ClassificationID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Classification_ClassificationID PRIMARY KEY,
    [ClassificationDescription] [varchar](300) NOT NULL,
    [ThemeColor] [varchar](7) NOT NULL,
    [DisplayName] [varchar](50) NOT NULL,
    [GoalStatement] [varchar](200) NULL,
    [KeyImageFileResourceID] [int] NULL CONSTRAINT [FK_Classification_FileResource_KeyImageFileResourceID_FileResourceID] FOREIGN KEY REFERENCES dbo.FileResource(FileResourceID),
    ClassificationSystemID int NOT NULL CONSTRAINT FK_Classification_ClassificationSystem_ClassificationSystemID FOREIGN KEY REFERENCES dbo.ClassificationSystem(ClassificationSystemID),
    [ClassificationSortOrder] [int] NULL,
)
GO