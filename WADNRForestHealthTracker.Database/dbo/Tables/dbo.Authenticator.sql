CREATE TABLE dbo.Authenticator
(
    AuthenticatorID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Authenticator_AuthenticatorID PRIMARY KEY,
    PersonID int NOT NULL CONSTRAINT FK_Authenticator_Person_PersonID FOREIGN KEY REFERENCES dbo.Person(PersonID),
    AuthenticatorType varchar(50) NOT NULL,
    AuthenticatorValue varchar(256) NOT NULL,
    ExpirationDate datetime NULL
)
GO
