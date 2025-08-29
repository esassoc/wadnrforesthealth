CREATE TABLE dbo.DNRUplandRegion
(
    DNRUplandRegionID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_DNRUplandRegion_DNRUplandRegionID PRIMARY KEY,
    DNRUplandRegionName varchar(100) NOT NULL,
    DNRUplandRegionCode varchar(10) NOT NULL,
    CONSTRAINT AK_DNRUplandRegion_DNRUplandRegionName UNIQUE (DNRUplandRegionName)
)
GO