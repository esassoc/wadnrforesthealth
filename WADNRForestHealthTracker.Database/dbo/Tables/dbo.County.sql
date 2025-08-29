CREATE TABLE dbo.County
(
    CountyID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_County_CountyID PRIMARY KEY,
    CountyName varchar(100) NOT NULL,
    StateID int NOT NULL CONSTRAINT FK_County_State_StateID FOREIGN KEY REFERENCES dbo.State(StateID),
    CountyCode varchar(10) NOT NULL,
    CONSTRAINT AK_County_CountyName UNIQUE (CountyName)
)
GO