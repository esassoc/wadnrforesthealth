CREATE TABLE [dbo].[ProjectRegionUpdate](
    [ProjectRegionUpdateID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectRegionUpdate_ProjectRegionUpdateID] PRIMARY KEY,
    [ProjectUpdateBatchID] [int] NOT NULL CONSTRAINT [FK_ProjectRegionUpdate_ProjectUpdateBatch_ProjectUpdateBatchID] FOREIGN KEY REFERENCES [dbo].[ProjectUpdateBatch]([ProjectUpdateBatchID]),
    [DNRUplandRegionID] [int] NOT NULL CONSTRAINT [FK_ProjectRegionUpdate_DNRUplandRegion_DNRUplandRegionID] FOREIGN KEY REFERENCES [dbo].[DNRUplandRegion]([DNRUplandRegionID]),
    CONSTRAINT [AK_ProjectRegionUpdate_ProjectUpdateBatchID_DNRUplandRegionID] UNIQUE ([ProjectUpdateBatchID], [DNRUplandRegionID])
)
GO