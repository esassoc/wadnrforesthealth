CREATE TABLE [dbo].[ProjectTag](
	[ProjectTagID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectTag_ProjectTagID] PRIMARY KEY,
	[ProjectTagName] [varchar](100) NOT NULL CONSTRAINT [AK_ProjectTag_ProjectTagName] UNIQUE,
	[ProjectTagDisplayName] [varchar](100) NOT NULL CONSTRAINT [AK_ProjectTag_ProjectTagDisplayName] UNIQUE
)
GO