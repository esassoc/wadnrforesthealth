SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GisUploadAttemptGisMetadataAttribute](
	[GisUploadAttemptGisMetadataAttributeID] [int] IDENTITY(1,1) NOT NULL,
	[GisUploadAttemptID] [int] NOT NULL,
	[GisMetadataAttributeID] [int] NOT NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_GisUploadAttemptGisMetadataAttribute_GisUploadAttemptGisMetadataAttributeID] PRIMARY KEY CLUSTERED 
(
	[GisUploadAttemptGisMetadataAttributeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[GisUploadAttemptGisMetadataAttribute]  WITH CHECK ADD  CONSTRAINT [FK_GisUploadAttemptGisMetadataAttribute_GisMetadataAttribute_GisMetadataAttributeID] FOREIGN KEY([GisMetadataAttributeID])
REFERENCES [dbo].[GisMetadataAttribute] ([GisMetadataAttributeID])
GO
ALTER TABLE [dbo].[GisUploadAttemptGisMetadataAttribute] CHECK CONSTRAINT [FK_GisUploadAttemptGisMetadataAttribute_GisMetadataAttribute_GisMetadataAttributeID]
GO
ALTER TABLE [dbo].[GisUploadAttemptGisMetadataAttribute]  WITH CHECK ADD  CONSTRAINT [FK_GisUploadAttemptGisMetadataAttribute_GisUploadAttempt_GisUploadAttemptID] FOREIGN KEY([GisUploadAttemptID])
REFERENCES [dbo].[GisUploadAttempt] ([GisUploadAttemptID])
GO
ALTER TABLE [dbo].[GisUploadAttemptGisMetadataAttribute] CHECK CONSTRAINT [FK_GisUploadAttemptGisMetadataAttribute_GisUploadAttempt_GisUploadAttemptID]