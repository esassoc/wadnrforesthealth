CREATE TABLE [dbo].[GisFeatureMetadataAttribute](
	[GisFeatureMetadataAttributeID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_GisFeatureMetadataAttribute_GisFeatureMetadataAttributeID] PRIMARY KEY,
	[GisFeatureID] [int] NOT NULL CONSTRAINT [FK_GisFeatureMetadataAttribute_GisFeature_GisFeatureID] FOREIGN KEY REFERENCES [dbo].[GisFeature]([GisFeatureID]),
	[GisMetadataAttributeID] [int] NOT NULL CONSTRAINT [FK_GisFeatureMetadataAttribute_GisMetadataAttribute_GisMetadataAttributeID] FOREIGN KEY REFERENCES [dbo].[GisMetadataAttribute]([GisMetadataAttributeID]),
	[GisFeatureMetadataAttributeValue] [varchar](max) NULL
)
GO
CREATE NONCLUSTERED INDEX [IDX_GisFeatureMetadataAttribute_GisFeatureID] ON [dbo].[GisFeatureMetadataAttribute]([GisFeatureID])
GO
CREATE NONCLUSTERED INDEX [IDX_GisFeatureMetadataAttribute_GisMetadataAttributeID] ON [dbo].[GisFeatureMetadataAttribute]([GisMetadataAttributeID])
GO