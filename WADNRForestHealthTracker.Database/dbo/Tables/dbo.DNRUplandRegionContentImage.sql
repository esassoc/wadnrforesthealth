SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DNRUplandRegionContentImage](
	[DNRUplandRegionContentImageID] [int] IDENTITY(1,1) NOT NULL,
	[DNRUplandRegionID] [int] NOT NULL,
	[FileResourceID] [int] NOT NULL,
 CONSTRAINT [PK_DNRUplandRegionContentImage_DNRUplandRegionContentImageID] PRIMARY KEY CLUSTERED 
(
	[DNRUplandRegionContentImageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_DNRUplandRegionContentImage_DNRUplandRegionContentImageID_FileResourceID] UNIQUE NONCLUSTERED 
(
	[DNRUplandRegionContentImageID] ASC,
	[FileResourceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[DNRUplandRegionContentImage]  WITH CHECK ADD  CONSTRAINT [FK_DNRUplandRegionContentImage_DNRUplandRegion_DNRUplandRegionID] FOREIGN KEY([DNRUplandRegionID])
REFERENCES [dbo].[DNRUplandRegion] ([DNRUplandRegionID])
GO
ALTER TABLE [dbo].[DNRUplandRegionContentImage] CHECK CONSTRAINT [FK_DNRUplandRegionContentImage_DNRUplandRegion_DNRUplandRegionID]
GO
ALTER TABLE [dbo].[DNRUplandRegionContentImage]  WITH CHECK ADD  CONSTRAINT [FK_DNRUplandRegionContentImage_FileResource_FileResourceID] FOREIGN KEY([FileResourceID])
REFERENCES [dbo].[FileResource] ([FileResourceID])
GO
ALTER TABLE [dbo].[DNRUplandRegionContentImage] CHECK CONSTRAINT [FK_DNRUplandRegionContentImage_FileResource_FileResourceID]