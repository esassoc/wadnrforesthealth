CREATE TABLE dbo.FederalFundCode
(
    FederalFundCodeID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_FederalFundCode_FederalFundCodeID PRIMARY KEY,
    FederalFundCodeName varchar(100) NOT NULL,
    FederalFundCodeValue varchar(50) NOT NULL,
    FederalFundCodeProgram varchar(255) NULL,
    CONSTRAINT AK_FederalFundCode_FederalFundCodeName UNIQUE (FederalFundCodeName)
)
GO
