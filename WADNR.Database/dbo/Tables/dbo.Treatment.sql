CREATE TABLE [dbo].[Treatment](
    [TreatmentID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Treatment_TreatmentID] PRIMARY KEY,
    [ProjectID] [int] NOT NULL CONSTRAINT [FK_Treatment_Project_ProjectID] FOREIGN KEY REFERENCES [dbo].[Project]([ProjectID]),
    [TreatmentTypeID] [int] NOT NULL CONSTRAINT [FK_Treatment_TreatmentType_TreatmentTypeID] FOREIGN KEY REFERENCES [dbo].[TreatmentType]([TreatmentTypeID]),
    [TreatmentStartDate] [datetime] NULL,
    [TreatmentEndDate] [datetime] NULL,
    [TreatmentFootprintAcres] [decimal](38, 10) NOT NULL,
    [TreatmentNotes] [varchar](2000) NULL,
    [TreatmentTreatedAcres] [decimal](38, 10) NULL,
    [TreatmentTypeImportedText] [varchar](200) NULL,
    [CreateGisUploadAttemptID] [int] NULL CONSTRAINT [FK_Treatment_GisUploadAttempt_CreateGisUploadAttemptID] FOREIGN KEY REFERENCES [dbo].[GisUploadAttempt]([GisUploadAttemptID]),
    [UpdateGisUploadAttemptID] [int] NULL CONSTRAINT [FK_Treatment_GisUploadAttempt_UpdateGisUploadAttemptID] FOREIGN KEY REFERENCES [dbo].[GisUploadAttempt]([GisUploadAttemptID]),
    [TreatmentDetailedActivityTypeID] [int] NOT NULL CONSTRAINT [FK_Treatment_TreatmentDetailedActivityType_TreatmentDetailedActivityTypeID] FOREIGN KEY REFERENCES [dbo].[TreatmentDetailedActivityType]([TreatmentDetailedActivityTypeID]),
    [TreatmentDetailedActivityTypeImportedText] [varchar](200) NULL,
    [ProgramID] [int] NULL CONSTRAINT [FK_Treatment_Program_ProgramID] FOREIGN KEY REFERENCES [dbo].[Program]([ProgramID]),
    [ImportedFromGis] [bit] NULL,
    [ProjectLocationID] [int] NULL CONSTRAINT [FK_Treatment_ProjectLocation_ProjectLocationID] FOREIGN KEY REFERENCES [dbo].[ProjectLocation]([ProjectLocationID]),
    [TreatmentCodeID] [int] NULL CONSTRAINT [FK_Treatment_TreatmentCode_TreatmentCodeID] FOREIGN KEY REFERENCES [dbo].[TreatmentCode]([TreatmentCodeID]),
    [CostPerAcre] [money] NULL
)
GO