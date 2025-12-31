CREATE TABLE [dbo].[SystemAttribute](
    [SystemAttributeID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_SystemAttribute_SystemAttributeID] PRIMARY KEY,
    [DefaultBoundingBox] [geometry] NOT NULL,
    [MinimumYear] [int] NOT NULL,
    [PrimaryContactPersonID] [int] NULL CONSTRAINT [FK_SystemAttribute_Person_PrimaryContactPersonID_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
    [SquareLogoFileResourceID] [int] NULL CONSTRAINT [FK_SystemAttribute_FileResource_SquareLogoFileResourceID_FileResourceID] FOREIGN KEY REFERENCES [dbo].[FileResource]([FileResourceID]),
    [BannerLogoFileResourceID] [int] NULL CONSTRAINT [FK_SystemAttribute_FileResource_BannerLogoFileResourceID_FileResourceID] FOREIGN KEY REFERENCES [dbo].[FileResource]([FileResourceID]),
    [RecaptchaPublicKey] [varchar](100) NULL,
    [RecaptchaPrivateKey] [varchar](100) NULL,
    [ShowApplicationsToThePublic] [bit] NOT NULL,
    [TaxonomyLevelID] [int] NOT NULL CONSTRAINT [FK_SystemAttribute_TaxonomyLevel_TaxonomyLevelID] FOREIGN KEY REFERENCES [dbo].[TaxonomyLevel]([TaxonomyLevelID]),
    [AssociatePerfomanceMeasureTaxonomyLevelID] [int] NOT NULL CONSTRAINT [FK_SystemAttribute_TaxonomyLevel_AssociatePerfomanceMeasureTaxonomyLevelID_TaxonomyLevelID] FOREIGN KEY REFERENCES [dbo].[TaxonomyLevel]([TaxonomyLevelID]),
    [IsActive] [bit] NOT NULL,
    [ShowLeadImplementerLogoOnFactSheet] [bit] NOT NULL,
    [EnableAccomplishmentsDashboard] [bit] NOT NULL,
    [ProjectStewardshipAreaTypeID] [int] NULL CONSTRAINT [FK_SystemAttribute_ProjectStewardshipAreaType_ProjectStewardshipAreaTypeID] FOREIGN KEY REFERENCES [dbo].[ProjectStewardshipAreaType]([ProjectStewardshipAreaTypeID]),
    [SocrataAppToken] [varchar](200) NOT NULL,
    CONSTRAINT [CK_TenantAttribute_AssociatedPerfomanceMeasureTaxonomyLevelLessThanEqualToTaxonomyLevelID] CHECK (([AssociatePerfomanceMeasureTaxonomyLevelID] <= [TaxonomyLevelID]))
)
GO
CREATE SPATIAL INDEX [SPATIAL_SystemAttribute_DefaultBoundingBox] ON [dbo].[SystemAttribute]([DefaultBoundingBox]) USING GEOMETRY_AUTO_GRID WITH (BOUNDING_BOX =(-125, 45, -117, 50), CELLS_PER_OBJECT = 8)
GO