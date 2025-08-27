SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GisMetadataAttribute](
	[GisMetadataAttributeID] [int] IDENTITY(1,1) NOT NULL,
	[GisMetadataAttributeName] [varchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[GisMetadataAttributeType] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_GisMetadataAttribute_GisMetadataAttributeID] PRIMARY KEY CLUSTERED 
(
	[GisMetadataAttributeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
