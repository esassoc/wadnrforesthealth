CREATE TABLE [dbo].[GisUploadAttemptGisMetadataAttribute](
    [GisUploadAttemptGisMetadataAttributeID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_GisUploadAttemptGisMetadataAttribute_GisUploadAttemptGisMetadataAttributeID] PRIMARY KEY,
    [GisUploadAttemptID] [int] NOT NULL CONSTRAINT [FK_GisUploadAttemptGisMetadataAttribute_GisUploadAttempt_GisUploadAttemptID] FOREIGN KEY REFERENCES [dbo].[GisUploadAttempt]([GisUploadAttemptID]),
    [GisMetadataAttributeID] [int] NOT NULL CONSTRAINT [FK_GisUploadAttemptGisMetadataAttribute_GisMetadataAttribute_GisMetadataAttributeID] FOREIGN KEY REFERENCES [dbo].[GisMetadataAttribute]([GisMetadataAttributeID]),
    [SortOrder] [int] NOT NULL
)
GO