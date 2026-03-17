SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PersonAllowedAuthenticator](
	[PersonAllowedAuthenticatorID] [int] IDENTITY(1,1) NOT NULL,
	[PersonID] [int] NOT NULL,
	[AuthenticatorID] [int] NOT NULL,
 CONSTRAINT [PK_PersonAllowedAuthenticator_PersonAllowedAuthenticatorID] PRIMARY KEY NONCLUSTERED 
(
	[PersonAllowedAuthenticatorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_PersonAllowedAuthenticator_PersonID_AuthenticatorID] UNIQUE CLUSTERED 
(
	[PersonID] ASC,
	[AuthenticatorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[PersonAllowedAuthenticator]  WITH CHECK ADD  CONSTRAINT [FK_PersonAllowedAuthenticator_Authenticator_AuthenticatorID] FOREIGN KEY([AuthenticatorID])
REFERENCES [dbo].[Authenticator] ([AuthenticatorID])
GO
ALTER TABLE [dbo].[PersonAllowedAuthenticator] CHECK CONSTRAINT [FK_PersonAllowedAuthenticator_Authenticator_AuthenticatorID]
GO
ALTER TABLE [dbo].[PersonAllowedAuthenticator]  WITH CHECK ADD  CONSTRAINT [FK_PersonAllowedAuthenticator_Person_PersonID] FOREIGN KEY([PersonID])
REFERENCES [dbo].[Person] ([PersonID])
GO
ALTER TABLE [dbo].[PersonAllowedAuthenticator] CHECK CONSTRAINT [FK_PersonAllowedAuthenticator_Person_PersonID]