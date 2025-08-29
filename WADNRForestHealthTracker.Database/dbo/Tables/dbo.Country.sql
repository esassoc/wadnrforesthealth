CREATE TABLE dbo.Country
(
    CountryID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Country_CountryID PRIMARY KEY,
    CountryName varchar(100) NOT NULL,
    CountryCode varchar(10) NOT NULL,
    CONSTRAINT AK_Country_CountryName UNIQUE (CountryName)
)
GO
