CREATE TABLE dbo.County
(
    CountyID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_County_CountyID PRIMARY KEY,
    CountyName varchar(100) NOT NULL,
    StateProvinceID int NOT NULL CONSTRAINT FK_County_StateProvince_StateProvinceID FOREIGN KEY REFERENCES dbo.StateProvince(StateProvinceID),
    CountyCode varchar(10) NOT NULL,
    CONSTRAINT AK_County_CountyName UNIQUE (CountyName)
)
GO