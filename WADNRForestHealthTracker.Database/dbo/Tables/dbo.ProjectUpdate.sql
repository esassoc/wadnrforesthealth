SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectUpdate](
	[ProjectUpdateID] [int] IDENTITY(1,1) NOT NULL,
	[ProjectUpdateBatchID] [int] NOT NULL,
	[ProjectStageID] [int] NOT NULL,
	[ProjectDescription] [varchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[CompletionDate] [datetime] NULL,
	[EstimatedTotalCost] [money] NULL,
	[ProjectLocationPoint] [geometry] NULL,
	[ProjectLocationNotes] [varchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[PlannedDate] [datetime] NULL,
	[ProjectLocationSimpleTypeID] [int] NOT NULL,
	[FocusAreaID] [int] NULL,
	[ExpirationDate] [datetime] NULL,
	[ProjectFundingSourceNotes] [varchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[PercentageMatch] [int] NULL,
 CONSTRAINT [PK_ProjectUpdate_ProjectUpdateID] PRIMARY KEY CLUSTERED 
(
	[ProjectUpdateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_ProjectUpdate_ProjectUpdateBatchID] UNIQUE NONCLUSTERED 
(
	[ProjectUpdateBatchID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[ProjectUpdate]  WITH CHECK ADD  CONSTRAINT [FK_ProjectUpdate_FocusArea_FocusAreaID] FOREIGN KEY([FocusAreaID])
REFERENCES [dbo].[FocusArea] ([FocusAreaID])
GO
ALTER TABLE [dbo].[ProjectUpdate] CHECK CONSTRAINT [FK_ProjectUpdate_FocusArea_FocusAreaID]
GO
ALTER TABLE [dbo].[ProjectUpdate]  WITH CHECK ADD  CONSTRAINT [FK_ProjectUpdate_ProjectLocationSimpleType_ProjectLocationSimpleTypeID] FOREIGN KEY([ProjectLocationSimpleTypeID])
REFERENCES [dbo].[ProjectLocationSimpleType] ([ProjectLocationSimpleTypeID])
GO
ALTER TABLE [dbo].[ProjectUpdate] CHECK CONSTRAINT [FK_ProjectUpdate_ProjectLocationSimpleType_ProjectLocationSimpleTypeID]
GO
ALTER TABLE [dbo].[ProjectUpdate]  WITH CHECK ADD  CONSTRAINT [FK_ProjectUpdate_ProjectStage_ProjectStageID] FOREIGN KEY([ProjectStageID])
REFERENCES [dbo].[ProjectStage] ([ProjectStageID])
GO
ALTER TABLE [dbo].[ProjectUpdate] CHECK CONSTRAINT [FK_ProjectUpdate_ProjectStage_ProjectStageID]
GO
ALTER TABLE [dbo].[ProjectUpdate]  WITH CHECK ADD  CONSTRAINT [FK_ProjectUpdate_ProjectUpdateBatch_ProjectUpdateBatchID] FOREIGN KEY([ProjectUpdateBatchID])
REFERENCES [dbo].[ProjectUpdateBatch] ([ProjectUpdateBatchID])
GO
ALTER TABLE [dbo].[ProjectUpdate] CHECK CONSTRAINT [FK_ProjectUpdate_ProjectUpdateBatch_ProjectUpdateBatchID]
GO
ALTER TABLE [dbo].[ProjectUpdate]  WITH CHECK ADD  CONSTRAINT [CK_ProjectUpdate_ProjectLocationPoint_IsPointData] CHECK  (([ProjectLocationPoint] IS NULL OR [ProjectLocationPoint] IS NOT NULL AND [ProjectLocationPoint].[STGeometryType]()='Point'))
GO
ALTER TABLE [dbo].[ProjectUpdate] CHECK CONSTRAINT [CK_ProjectUpdate_ProjectLocationPoint_IsPointData]
GO
SET ARITHABORT ON
SET CONCAT_NULL_YIELDS_NULL ON
SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
SET NUMERIC_ROUNDABORT OFF

GO
CREATE SPATIAL INDEX [SPATIAL_ProjectUpdate_ProjectLocationPoint] ON [dbo].[ProjectUpdate]
(
	[ProjectLocationPoint]
)USING  GEOMETRY_AUTO_GRID 
WITH (BOUNDING_BOX =(-124, 46, -117, 49), 
CELLS_PER_OBJECT = 8, PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]