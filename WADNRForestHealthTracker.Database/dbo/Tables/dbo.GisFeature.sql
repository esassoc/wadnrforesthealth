SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GisFeature](
	[GisFeatureID] [int] IDENTITY(1,1) NOT NULL,
	[GisUploadAttemptID] [int] NOT NULL,
	[GisFeatureGeometry] [geometry] NOT NULL,
	[GisImportFeatureKey] [int] NOT NULL,
	[IsValid]  AS ([GisFeatureGeometry].[STIsValid]()),
	[CalculatedArea] [decimal](38, 20) NULL,
 CONSTRAINT [PK_GisFeature_GisFeatureID] PRIMARY KEY CLUSTERED 
(
	[GisFeatureID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[GisFeature]  WITH CHECK ADD  CONSTRAINT [FK_GisFeature_GisUploadAttempt_GisUploadAttemptID] FOREIGN KEY([GisUploadAttemptID])
REFERENCES [dbo].[GisUploadAttempt] ([GisUploadAttemptID])
GO
ALTER TABLE [dbo].[GisFeature] CHECK CONSTRAINT [FK_GisFeature_GisUploadAttempt_GisUploadAttemptID]