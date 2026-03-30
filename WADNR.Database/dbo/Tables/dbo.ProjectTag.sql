CREATE TABLE [dbo].[ProjectTag](
    [ProjectTagID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT PK_ProjectTag_ProjectTagID PRIMARY KEY,
    [ProjectID] [int] NOT NULL CONSTRAINT FK_ProjectTag_Project_ProjectID FOREIGN KEY REFERENCES [dbo].[Project]([ProjectID]),
    [TagID] [int] NOT NULL CONSTRAINT FK_ProjectTag_Tag_TagID FOREIGN KEY REFERENCES [dbo].[Tag]([TagID]),
    CONSTRAINT AK_ProjectTag_ProjectID_TagID UNIQUE ([ProjectID], [TagID])
)
GO