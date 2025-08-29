CREATE TABLE [dbo].[FocusArea](
	[FocusAreaID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FocusArea_FocusAreaID] PRIMARY KEY,
	[FocusAreaName] [varchar](200) NOT NULL CONSTRAINT [AK_FocusArea_FocusAreaName] UNIQUE,
	[FocusAreaStatusID] [int] NOT NULL CONSTRAINT [FK_FocusArea_FocusAreaStatus_FocusAreaStatusID] FOREIGN KEY REFERENCES [dbo].[FocusAreaStatus]([FocusAreaStatusID]),
	[FocusAreaLocation] [geometry] NULL,
	[DNRUplandRegionID] [int] NOT NULL CONSTRAINT [FK_FocusArea_DNRUplandRegion_DNRUplandRegionID] FOREIGN KEY REFERENCES [dbo].[DNRUplandRegion]([DNRUplandRegionID]),
	[PlannedFootprintAcres] [decimal](18, 0) NULL
)
GO