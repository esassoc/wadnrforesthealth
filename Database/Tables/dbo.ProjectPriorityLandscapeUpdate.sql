SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectPriorityLandscapeUpdate](
	[ProjectPriorityLandscapeUpdateID] [int] IDENTITY(1,1) NOT NULL,
	[ProjectUpdateBatchID] [int] NOT NULL,
	[PriorityLandscapeID] [int] NOT NULL,
 CONSTRAINT [PK_ProjectPriorityLandscapeUpdate_ProjectPriorityLandscapeUpdateID] PRIMARY KEY CLUSTERED 
(
	[ProjectPriorityLandscapeUpdateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_ProjectPriorityLandscapeUpdate_ProjectUpdateBatchID_PriorityLandscapeID] UNIQUE NONCLUSTERED 
(
	[ProjectUpdateBatchID] ASC,
	[PriorityLandscapeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[ProjectPriorityLandscapeUpdate]  WITH CHECK ADD  CONSTRAINT [FK_ProjectPriorityLandscapeUpdate_PriorityLandscape_PriorityLandscapeID] FOREIGN KEY([PriorityLandscapeID])
REFERENCES [dbo].[PriorityLandscape] ([PriorityLandscapeID])
GO
ALTER TABLE [dbo].[ProjectPriorityLandscapeUpdate] CHECK CONSTRAINT [FK_ProjectPriorityLandscapeUpdate_PriorityLandscape_PriorityLandscapeID]
GO
ALTER TABLE [dbo].[ProjectPriorityLandscapeUpdate]  WITH CHECK ADD  CONSTRAINT [FK_ProjectPriorityLandscapeUpdate_ProjectUpdateBatch_ProjectUpdateBatchID] FOREIGN KEY([ProjectUpdateBatchID])
REFERENCES [dbo].[ProjectUpdateBatch] ([ProjectUpdateBatchID])
GO
ALTER TABLE [dbo].[ProjectPriorityLandscapeUpdate] CHECK CONSTRAINT [FK_ProjectPriorityLandscapeUpdate_ProjectUpdateBatch_ProjectUpdateBatchID]