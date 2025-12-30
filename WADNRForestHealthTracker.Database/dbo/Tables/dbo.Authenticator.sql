CREATE TABLE dbo.Authenticator
(
    AuthenticatorID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Authenticator_AuthenticatorID PRIMARY KEY,
    AuthenticatorName varchar(10) NOT NULL,
    AuthenticatorFullName varchar(100) NOT NULL
)
GO
