CREATE TABLE [dbo].[TreatmentArea](
	[TreatmentAreaID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_TreatmentArea_TreatmentAreaID] PRIMARY KEY,
	[TreatmentAreaFeature] [geometry] NOT NULL,
	[TemporaryTreatmentCacheID] [int] NULL
)
GO
CREATE SPATIAL INDEX [SPATIAL_TreatmentArea_TreatmentAreaFeature] ON [dbo].[TreatmentArea]([TreatmentAreaFeature]) USING GEOMETRY_AUTO_GRID WITH (BOUNDING_BOX =(-125, 45, -117, 50), CELLS_PER_OBJECT = 8)
GO