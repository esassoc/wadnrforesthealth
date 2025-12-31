CREATE TABLE [dbo].[ProjectClassification](
    [ProjectClassificationID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectClassification_ProjectClassificationID] PRIMARY KEY,
    [ProjectID] [int] NOT NULL CONSTRAINT [FK_ProjectClassification_Project_ProjectID] FOREIGN KEY REFERENCES [dbo].[Project]([ProjectID]),
    [ClassificationID] [int] NOT NULL CONSTRAINT [FK_ProjectClassification_Classification_ClassificationID] FOREIGN KEY REFERENCES [dbo].[Classification]([ClassificationID]),
    [ProjectClassificationNotes] [varchar](600) NULL,
    CONSTRAINT [AK_ProjectClassification_ProjectID_ClassificationID] UNIQUE ([ProjectID], [ClassificationID])
)
GO