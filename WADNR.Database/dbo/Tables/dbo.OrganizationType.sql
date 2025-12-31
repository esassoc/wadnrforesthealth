CREATE TABLE [dbo].[OrganizationType](
	[OrganizationTypeID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_OrganizationType_OrganizationTypeID] PRIMARY KEY,
	[OrganizationTypeName] [varchar](200) NOT NULL,
	[OrganizationTypeAbbreviation] [varchar](100) NOT NULL,
	[LegendColor] [varchar](10) NOT NULL,
	[ShowOnProjectMaps] [bit] NOT NULL,
	[IsDefaultOrganizationType] [bit] NOT NULL,
	[IsFundingType] [bit] NOT NULL
)
GO
