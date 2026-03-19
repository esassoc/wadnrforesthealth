SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GisExcludeIncludeColumn](
	[GisExcludeIncludeColumnID] [int] IDENTITY(1,1) NOT NULL,
	[GisUploadSourceOrganizationID] [int] NOT NULL,
	[GisDefaultMappingColumnName] [varchar](300) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[IsWhitelist] [bit] NOT NULL,
	[IsBlacklist]  AS (CONVERT([bit],case when [IsWhitelist]=(1) then (0) else (1) end)),
 CONSTRAINT [PK_GisExcludeIncludeColumn_GisExcludeIncludeColumnID] PRIMARY KEY CLUSTERED 
(
	[GisExcludeIncludeColumnID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[GisExcludeIncludeColumn]  WITH CHECK ADD  CONSTRAINT [FK_GisExcludeIncludeColumn_GisUploadSourceOrganization_GisUploadSourceOrganizationID] FOREIGN KEY([GisUploadSourceOrganizationID])
REFERENCES [dbo].[GisUploadSourceOrganization] ([GisUploadSourceOrganizationID])
GO
ALTER TABLE [dbo].[GisExcludeIncludeColumn] CHECK CONSTRAINT [FK_GisExcludeIncludeColumn_GisUploadSourceOrganization_GisUploadSourceOrganizationID]