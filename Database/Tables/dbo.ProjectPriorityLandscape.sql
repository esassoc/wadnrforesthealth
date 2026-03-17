SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectPriorityLandscape](
	[ProjectPriorityLandscapeID] [int] IDENTITY(1,1) NOT NULL,
	[ProjectID] [int] NOT NULL,
	[PriorityLandscapeID] [int] NOT NULL,
 CONSTRAINT [PK_ProjectPriorityLandscape_ProjectPriorityLandscapeID] PRIMARY KEY CLUSTERED 
(
	[ProjectPriorityLandscapeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_ProjectPriorityLandscape_ProjectID_PriorityLandscapeID] UNIQUE NONCLUSTERED 
(
	[ProjectID] ASC,
	[PriorityLandscapeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[ProjectPriorityLandscape]  WITH CHECK ADD  CONSTRAINT [FK_ProjectPriorityLandscape_PriorityLandscape_PriorityLandscapeID] FOREIGN KEY([PriorityLandscapeID])
REFERENCES [dbo].[PriorityLandscape] ([PriorityLandscapeID])
GO
ALTER TABLE [dbo].[ProjectPriorityLandscape] CHECK CONSTRAINT [FK_ProjectPriorityLandscape_PriorityLandscape_PriorityLandscapeID]
GO
ALTER TABLE [dbo].[ProjectPriorityLandscape]  WITH CHECK ADD  CONSTRAINT [FK_ProjectPriorityLandscape_Project_ProjectID] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[Project] ([ProjectID])
GO
ALTER TABLE [dbo].[ProjectPriorityLandscape] CHECK CONSTRAINT [FK_ProjectPriorityLandscape_Project_ProjectID]