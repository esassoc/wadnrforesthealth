CREATE TABLE [dbo].[FocusAreaLocationStaging](
	[FocusAreaLocationStagingID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FocusAreaLocationStaging_FocusAreaLocationStagingID] PRIMARY KEY,
	[FocusAreaID] [int] NOT NULL CONSTRAINT [FK_FocusAreaLocationStaging_FocusArea_FocusAreaID] FOREIGN KEY REFERENCES [dbo].[FocusArea]([FocusAreaID]),
	[FeatureClassName] [varchar](255) NOT NULL,
	[GeoJson] [varchar](max) NOT NULL
)
GO