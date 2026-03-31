CREATE TABLE dbo.Country
(
    CountryID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Country_CountryID PRIMARY KEY,
    [CountryName] [varchar](255) NOT NULL,
    [CountryAbbrev] [varchar](2) NOT NULL
)
GO
