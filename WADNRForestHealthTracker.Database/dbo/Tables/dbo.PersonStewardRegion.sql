SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PersonStewardRegion](
	[PersonStewardRegionID] [int] IDENTITY(1,1) NOT NULL,
	[PersonID] [int] NOT NULL,
	[DNRUplandRegionID] [int] NOT NULL,
 CONSTRAINT [PK_PersonStewardRegion_PersonStewardRegionID] PRIMARY KEY CLUSTERED 
(
	[PersonStewardRegionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[PersonStewardRegion]  WITH CHECK ADD  CONSTRAINT [FK_PersonStewardRegion_DNRUplandRegion_DNRUplandRegionID] FOREIGN KEY([DNRUplandRegionID])
REFERENCES [dbo].[DNRUplandRegion] ([DNRUplandRegionID])
GO
ALTER TABLE [dbo].[PersonStewardRegion] CHECK CONSTRAINT [FK_PersonStewardRegion_DNRUplandRegion_DNRUplandRegionID]
GO
ALTER TABLE [dbo].[PersonStewardRegion]  WITH CHECK ADD  CONSTRAINT [FK_PersonStewardRegion_Person_PersonID] FOREIGN KEY([PersonID])
REFERENCES [dbo].[Person] ([PersonID])
GO
ALTER TABLE [dbo].[PersonStewardRegion] CHECK CONSTRAINT [FK_PersonStewardRegion_Person_PersonID]