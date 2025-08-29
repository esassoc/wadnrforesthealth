CREATE TABLE dbo.DNRUplandRegionContentImage
(
    DNRUplandRegionContentImageID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_DNRUplandRegionContentImage_DNRUplandRegionContentImageID PRIMARY KEY,
    DNRUplandRegionID int NOT NULL CONSTRAINT FK_DNRUplandRegionContentImage_DNRUplandRegion_DNRUplandRegionID FOREIGN KEY REFERENCES dbo.DNRUplandRegion(DNRUplandRegionID),
    ImageUrl varchar(256) NOT NULL,
    AltText varchar(256) NULL
)
GO