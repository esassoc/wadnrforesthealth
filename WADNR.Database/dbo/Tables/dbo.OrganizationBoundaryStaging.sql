CREATE TABLE [dbo].[OrganizationBoundaryStaging](
	[OrganizationBoundaryStagingID] [int] IDENTITY(1,1) NOT NULL,
	[OrganizationID] [int] NOT NULL,
	[FeatureClassName] [varchar](255) NOT NULL,
	[GeoJson] [varchar](max) NOT NULL,
 CONSTRAINT [PK_OrganizationBoundaryStaging_OrganizationBoundaryStagingID] PRIMARY KEY CLUSTERED 
(
	[OrganizationBoundaryStagingID] ASC
)
) 
GO

ALTER TABLE [dbo].[OrganizationBoundaryStaging] ADD  CONSTRAINT [FK_OrganizationBoundaryStaging_Organization_OrganizationID] FOREIGN KEY([OrganizationID])
REFERENCES [dbo].[Organization] ([OrganizationID])
GO