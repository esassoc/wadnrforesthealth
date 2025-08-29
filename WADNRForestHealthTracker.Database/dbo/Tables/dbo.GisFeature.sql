CREATE TABLE [dbo].[GisFeature](
    [GisFeatureID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_GisFeature_GisFeatureID] PRIMARY KEY,
    [GisUploadAttemptID] [int] NOT NULL CONSTRAINT [FK_GisFeature_GisUploadAttempt_GisUploadAttemptID] FOREIGN KEY REFERENCES [dbo].[GisUploadAttempt]([GisUploadAttemptID]),
    [GisFeatureGeometry] [geometry] NOT NULL,
    [GisImportFeatureKey] [int] NOT NULL,
    [IsValid]  AS ([GisFeatureGeometry].[STIsValid]()),
    [CalculatedArea] [decimal](38, 20) NULL
)
GO