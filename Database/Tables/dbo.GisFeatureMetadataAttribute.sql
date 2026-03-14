SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GisFeatureMetadataAttribute](
	[GisFeatureMetadataAttributeID] [int] IDENTITY(1,1) NOT NULL,
	[GisFeatureID] [int] NOT NULL,
	[GisMetadataAttributeID] [int] NOT NULL,
	[GisFeatureMetadataAttributeValue] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_GisFeatureMetadataAttribute_GisFeatureMetadataAttributeID] PRIMARY KEY CLUSTERED 
(
	[GisFeatureMetadataAttributeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX [IDX_GisFeatureMetadataAttribute_GisFeatureID] ON [dbo].[GisFeatureMetadataAttribute]
(
	[GisFeatureID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IDX_GisFeatureMetadataAttribute_GisMetadataAttributeID] ON [dbo].[GisFeatureMetadataAttribute]
(
	[GisMetadataAttributeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[GisFeatureMetadataAttribute]  WITH CHECK ADD  CONSTRAINT [FK_GisFeatureMetadataAttribute_GisFeature_GisFeatureID] FOREIGN KEY([GisFeatureID])
REFERENCES [dbo].[GisFeature] ([GisFeatureID])
GO
ALTER TABLE [dbo].[GisFeatureMetadataAttribute] CHECK CONSTRAINT [FK_GisFeatureMetadataAttribute_GisFeature_GisFeatureID]
GO
ALTER TABLE [dbo].[GisFeatureMetadataAttribute]  WITH CHECK ADD  CONSTRAINT [FK_GisFeatureMetadataAttribute_GisMetadataAttribute_GisMetadataAttributeID] FOREIGN KEY([GisMetadataAttributeID])
REFERENCES [dbo].[GisMetadataAttribute] ([GisMetadataAttributeID])
GO
ALTER TABLE [dbo].[GisFeatureMetadataAttribute] CHECK CONSTRAINT [FK_GisFeatureMetadataAttribute_GisMetadataAttribute_GisMetadataAttributeID]