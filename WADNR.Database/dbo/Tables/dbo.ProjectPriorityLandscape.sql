CREATE TABLE [dbo].[ProjectPriorityLandscape](
    [ProjectPriorityLandscapeID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectPriorityLandscape_ProjectPriorityLandscapeID] PRIMARY KEY,
    [ProjectID] [int] NOT NULL CONSTRAINT [FK_ProjectPriorityLandscape_Project_ProjectID] FOREIGN KEY REFERENCES [dbo].[Project]([ProjectID]),
    [PriorityLandscapeID] [int] NOT NULL CONSTRAINT [FK_ProjectPriorityLandscape_PriorityLandscape_PriorityLandscapeID] FOREIGN KEY REFERENCES [dbo].[PriorityLandscape]([PriorityLandscapeID]),
    CONSTRAINT [AK_ProjectPriorityLandscape_ProjectID_PriorityLandscapeID] UNIQUE ([ProjectID], [PriorityLandscapeID])
)
GO