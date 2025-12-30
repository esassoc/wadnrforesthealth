CREATE TABLE dbo.DNRUplandRegionContentImage
(
    DNRUplandRegionContentImageID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_DNRUplandRegionContentImage_DNRUplandRegionContentImageID PRIMARY KEY,
    DNRUplandRegionID int NOT NULL CONSTRAINT FK_DNRUplandRegionContentImage_DNRUplandRegion_DNRUplandRegionID FOREIGN KEY REFERENCES dbo.DNRUplandRegion(DNRUplandRegionID),
    [FileResourceID] [int] NOT NULL CONSTRAINT [FK_DNRUplandRegionContentImage_FileResource_FileResourceID] FOREIGN KEY REFERENCES [dbo].[FileResource] ([FileResourceID]),
    CONSTRAINT [AK_DNRUplandRegionContentImage_DNRUplandRegionContentImageID_FileResourceID] UNIQUE
    (
	    [DNRUplandRegionContentImageID] ASC,
	    [FileResourceID] ASC
    )
)
GO