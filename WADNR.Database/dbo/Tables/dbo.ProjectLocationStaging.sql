CREATE TABLE [dbo].[ProjectLocationStaging](
    [ProjectLocationStagingID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectLocationStaging_ProjectLocationStagingID] PRIMARY KEY,
    [ProjectID] [int] NOT NULL CONSTRAINT [FK_ProjectLocationStaging_Project_ProjectID] FOREIGN KEY REFERENCES [dbo].[Project]([ProjectID]),
    [PersonID] [int] NOT NULL CONSTRAINT [FK_ProjectLocationStaging_Person_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
    [FeatureClassName] [varchar](255) NOT NULL,
    [GeoJson] [varchar](max) NOT NULL,
    [SelectedProperty] [varchar](255) NULL,
    [ShouldImport] [bit] NOT NULL,
    CONSTRAINT [AK_ProjectLocationStaging_ProjectID_PersonID_FeatureClassName] UNIQUE ([ProjectID], [PersonID], [FeatureClassName])
)
GO