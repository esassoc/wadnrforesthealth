CREATE TABLE dbo.Division
(
    DivisionID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Division_DivisionID PRIMARY KEY,
    [DivisionName] [varchar](200) NOT NULL CONSTRAINT AK_Division_DivisionName UNIQUE,
    [DivisionDisplayName] [varchar](200) NOT NULL CONSTRAINT AK_Division_DivisionDisplayName UNIQUE
)
GO
