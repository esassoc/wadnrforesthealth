CREATE TABLE [dbo].[ProjectLocationStagingUpdate](
    [ProjectLocationStagingUpdateID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectLocationStagingUpdate_ProjectLocationStagingUpdateID] PRIMARY KEY,
    [ProjectUpdateBatchID] [int] NOT NULL CONSTRAINT [FK_ProjectLocationStagingUpdate_ProjectUpdateBatch_ProjectUpdateBatchID] FOREIGN KEY REFERENCES [dbo].[ProjectUpdateBatch]([ProjectUpdateBatchID]),
    [PersonID] [int] NOT NULL CONSTRAINT [FK_ProjectLocationStagingUpdate_Person_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
    [FeatureClassName] [varchar](255) NOT NULL,
    [GeoJson] [varchar](max) NOT NULL,
    [SelectedProperty] [varchar](255) NULL,
    [ShouldImport] [bit] NOT NULL,
    CONSTRAINT [AK_ProjectLocationStagingUpdate_ProjectUpdateBatchID_PersonID_FeatureClassName] UNIQUE ([ProjectUpdateBatchID], [PersonID], [FeatureClassName])
)
GO