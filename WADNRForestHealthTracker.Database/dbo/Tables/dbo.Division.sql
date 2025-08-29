CREATE TABLE dbo.Division
(
    DivisionID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Division_DivisionID PRIMARY KEY,
    DivisionName varchar(100) NOT NULL,
    DivisionCode varchar(10) NOT NULL,
    CONSTRAINT AK_Division_DivisionName UNIQUE (DivisionName)
)
GO
