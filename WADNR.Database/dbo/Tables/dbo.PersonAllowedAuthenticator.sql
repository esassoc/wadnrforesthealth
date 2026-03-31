CREATE TABLE [dbo].[PersonAllowedAuthenticator](
    [PersonAllowedAuthenticatorID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_PersonAllowedAuthenticator_PersonAllowedAuthenticatorID] PRIMARY KEY NONCLUSTERED,
    [PersonID] [int] NOT NULL CONSTRAINT [FK_PersonAllowedAuthenticator_Person_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
    [AuthenticatorID] [int] NOT NULL CONSTRAINT [FK_PersonAllowedAuthenticator_Authenticator_AuthenticatorID] FOREIGN KEY REFERENCES [dbo].[Authenticator]([AuthenticatorID]),
    CONSTRAINT [AK_PersonAllowedAuthenticator_PersonID_AuthenticatorID] UNIQUE CLUSTERED ([PersonID], [AuthenticatorID])
)
GO