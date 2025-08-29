CREATE TABLE [dbo].[GisMetadataAttribute](
    [GisMetadataAttributeID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_GisMetadataAttribute_GisMetadataAttributeID] PRIMARY KEY,
    [GisMetadataAttributeName] [varchar](500) NOT NULL,
    [GisMetadataAttributeType] [varchar](100) NULL
)
GO
