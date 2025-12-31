CREATE TABLE dbo.FederalFundCode
(
    FederalFundCodeID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_FederalFundCode_FederalFundCodeID PRIMARY KEY,
    [FederalFundCodeAbbrev] [varchar](10) NULL,
    [FederalFundCodeProgram] [varchar](255) NULL
)
GO
