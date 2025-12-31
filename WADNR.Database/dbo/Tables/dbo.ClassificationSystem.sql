CREATE TABLE dbo.ClassificationSystem
(
    ClassificationSystemID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ClassificationSystem_ClassificationSystemID PRIMARY KEY,
    [ClassificationSystemName] [varchar](200) NOT NULL,
    [ClassificationSystemDefinition] [dbo].[html] NULL,
    [ClassificationSystemListPageContent] [dbo].[html] NULL
)
GO
