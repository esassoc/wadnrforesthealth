CREATE TABLE [dbo].[ProjectUpdate](
    [ProjectUpdateID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectUpdate_ProjectUpdateID] PRIMARY KEY,
    [ProjectUpdateBatchID] [int] NOT NULL CONSTRAINT [FK_ProjectUpdate_ProjectUpdateBatch_ProjectUpdateBatchID] FOREIGN KEY REFERENCES [dbo].[ProjectUpdateBatch]([ProjectUpdateBatchID]),
    [ProjectStageID] [int] NOT NULL CONSTRAINT [FK_ProjectUpdate_ProjectStage_ProjectStageID] FOREIGN KEY REFERENCES [dbo].[ProjectStage] ([ProjectStageID]),
    [ProjectDescription] [varchar](4000) NULL,
    [CompletionDate] [date] NULL,
    [EstimatedTotalCost] [money] NULL,
    [ProjectLocationPoint] [geometry] NULL,
    [ProjectLocationNotes] [varchar](4000) NULL,
    [PlannedDate] [date] NULL,
    [ProjectLocationSimpleTypeID] [int] NOT NULL CONSTRAINT [FK_ProjectUpdate_ProjectLocationSimpleType_ProjectLocationSimpleTypeID] FOREIGN KEY REFERENCES [dbo].[ProjectLocationSimpleType] ([ProjectLocationSimpleTypeID]),
    [FocusAreaID] [int] NULL CONSTRAINT [FK_ProjectUpdate_FocusArea_FocusAreaID] FOREIGN KEY REFERENCES [dbo].[FocusArea] ([FocusAreaID]),
    [ExpirationDate] [date] NULL,
    [ProjectFundingSourceNotes] [varchar](4000) NULL,
    [PercentageMatch] [int] NULL
)
GO
CREATE SPATIAL INDEX [SPATIAL_ProjectUpdate_ProjectLocationPoint] ON [dbo].[ProjectUpdate]
(
	[ProjectLocationPoint]
)USING  GEOMETRY_AUTO_GRID 
WITH (BOUNDING_BOX =(-124, 46, -117, 49), 
CELLS_PER_OBJECT = 8, PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]