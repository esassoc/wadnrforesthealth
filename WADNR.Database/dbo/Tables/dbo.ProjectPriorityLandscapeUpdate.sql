CREATE TABLE [dbo].[ProjectPriorityLandscapeUpdate](
	[ProjectPriorityLandscapeUpdateID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectPriorityLandscapeUpdate_ProjectPriorityLandscapeUpdateID] PRIMARY KEY,
	[ProjectUpdateBatchID] [int] NOT NULL CONSTRAINT [FK_ProjectPriorityLandscapeUpdate_ProjectUpdateBatch_ProjectUpdateBatchID] FOREIGN KEY REFERENCES [dbo].[ProjectUpdateBatch]([ProjectUpdateBatchID]),
	[PriorityLandscapeID] [int] NOT NULL CONSTRAINT [FK_ProjectPriorityLandscapeUpdate_PriorityLandscape_PriorityLandscapeID] FOREIGN KEY REFERENCES [dbo].[PriorityLandscape]([PriorityLandscapeID]),
 CONSTRAINT [AK_ProjectPriorityLandscapeUpdate_ProjectUpdateBatchID_PriorityLandscapeID] UNIQUE ([ProjectUpdateBatchID], [PriorityLandscapeID])
)
GO