SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FocusAreaLocationStaging](
	[FocusAreaLocationStagingID] [int] IDENTITY(1,1) NOT NULL,
	[FocusAreaID] [int] NOT NULL,
	[FeatureClassName] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[GeoJson] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_FocusAreaLocationStaging_FocusAreaLocationStagingID] PRIMARY KEY CLUSTERED 
(
	[FocusAreaLocationStagingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[FocusAreaLocationStaging]  WITH CHECK ADD  CONSTRAINT [FK_FocusAreaLocationStaging_FocusArea_FocusAreaID] FOREIGN KEY([FocusAreaID])
REFERENCES [dbo].[FocusArea] ([FocusAreaID])
GO
ALTER TABLE [dbo].[FocusAreaLocationStaging] CHECK CONSTRAINT [FK_FocusAreaLocationStaging_FocusArea_FocusAreaID]