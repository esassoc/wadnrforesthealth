CREATE TABLE [dbo].[ProjectExternalLink](
	[ProjectExternalLinkID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectExternalLink_ProjectExternalLinkID] PRIMARY KEY,
	[ProjectID] [int] NOT NULL CONSTRAINT [FK_ProjectExternalLink_Project_ProjectID] FOREIGN KEY REFERENCES [dbo].[Project]([ProjectID]),
	[ExternalLinkLabel] [varchar](300) NOT NULL,
	[ExternalLinkUrl] [varchar](300) NOT NULL
)
GO