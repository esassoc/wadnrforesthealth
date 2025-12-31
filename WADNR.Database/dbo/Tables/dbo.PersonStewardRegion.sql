CREATE TABLE [dbo].[PersonStewardRegion](
    [PersonStewardRegionID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_PersonStewardRegion_PersonStewardRegionID] PRIMARY KEY,
    [PersonID] [int] NOT NULL CONSTRAINT [FK_PersonStewardRegion_Person_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
    [DNRUplandRegionID] [int] NOT NULL CONSTRAINT [FK_PersonStewardRegion_DNRUplandRegion_DNRUplandRegionID] FOREIGN KEY REFERENCES [dbo].[DNRUplandRegion]([DNRUplandRegionID])
)
GO