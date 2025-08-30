CREATE TABLE [dbo].[ProjectLocation](
    [ProjectLocationID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectLocation_ProjectLocationID] PRIMARY KEY,
    [ProjectID] [int] NOT NULL CONSTRAINT [FK_ProjectLocation_Project_ProjectID] FOREIGN KEY REFERENCES [dbo].[Project]([ProjectID]),
    [ProjectLocationGeometry] [geometry] NOT NULL,
    [ProjectLocationNotes] [varchar](255) NULL,
    [ProjectLocationTypeID] [int] NOT NULL CONSTRAINT [FK_ProjectLocation_ProjectLocationType_ProjectLocationTypeID] FOREIGN KEY REFERENCES [dbo].[ProjectLocationType]([ProjectLocationTypeID]),
    [ProjectLocationName] [varchar](100) NOT NULL,
    [ArcGisObjectID] [int] NULL,
    [ArcGisGlobalID] [varchar](50) NULL,
    [ProgramID] [int] NULL CONSTRAINT [FK_ProjectLocation_Program_ProgramID] FOREIGN KEY REFERENCES [dbo].[Program]([ProgramID]),
    [ImportedFromGisUpload] [bit] NULL,
    [TemporaryTreatmentCacheID] [int] NULL,
    CONSTRAINT [AK_ProjectLocation_ProjectID_ProjectLocationName] UNIQUE ([ProjectID], [ProjectLocationName])
)
GO