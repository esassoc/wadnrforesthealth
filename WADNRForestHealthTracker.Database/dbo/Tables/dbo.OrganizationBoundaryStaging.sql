CREATE TABLE [dbo].[OrganizationBoundaryStaging](
	[OrganizationBoundaryStagingID] [int] IDENTITY(1,1) NOT NULL,
	[OrganizationID] [int] NOT NULL,
	[FeatureClassName] [varchar](255) NOT NULL,
	[GeoJson] [varchar](max) NOT NULL,
 CONSTRAINT [PK_OrganizationBoundaryStaging_OrganizationBoundaryStagingID] PRIMARY KEY CLUSTERED 
(
	[OrganizationBoundaryStagingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
) 
GO

ALTER TABLE [dbo].[OrganizationBoundaryStaging]  WITH CHECK ADD  CONSTRAINT [FK_OrganizationBoundaryStaging_Organization_OrganizationID] FOREIGN KEY([OrganizationID])
REFERENCES [dbo].[Organization] ([OrganizationID])
GO