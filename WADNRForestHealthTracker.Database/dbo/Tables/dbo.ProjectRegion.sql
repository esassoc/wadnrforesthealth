CREATE TABLE [dbo].[ProjectRegion](
    [ProjectRegionID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectRegion_ProjectRegionID] PRIMARY KEY,
    [ProjectID] [int] NOT NULL CONSTRAINT [FK_ProjectRegion_Project_ProjectID] FOREIGN KEY REFERENCES [dbo].[Project]([ProjectID]),
    [DNRUplandRegionID] [int] NOT NULL CONSTRAINT [FK_ProjectRegion_DNRUplandRegion_DNRUplandRegionID] FOREIGN KEY REFERENCES [dbo].[DNRUplandRegion]([DNRUplandRegionID]),
    CONSTRAINT [AK_ProjectRegion_ProjectID_DNRUplandRegionID] UNIQUE ([ProjectID], [DNRUplandRegionID])
)
GO