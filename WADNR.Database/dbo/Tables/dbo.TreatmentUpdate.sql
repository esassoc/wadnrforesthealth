CREATE TABLE [dbo].[TreatmentUpdate](
    [TreatmentUpdateID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_TreatmentUpdate_TreatmentUpdateID] PRIMARY KEY,
    [ProjectUpdateBatchID] [int] NOT NULL CONSTRAINT [FK_TreatmentUpdate_ProjectUpdateBatch_ProjectUpdateBatchID] FOREIGN KEY REFERENCES [dbo].[ProjectUpdateBatch]([ProjectUpdateBatchID]),
    [TreatmentStartDate] [date] NULL,
    [TreatmentEndDate] [date] NULL,
    [TreatmentFootprintAcres] [decimal](38, 10) NOT NULL,
    [TreatmentNotes] [varchar](2000) NULL,
    [TreatmentTypeID] [int] NOT NULL CONSTRAINT [FK_TreatmentUpdate_TreatmentType_TreatmentTypeID] FOREIGN KEY REFERENCES [dbo].[TreatmentType]([TreatmentTypeID]),
    [TreatmentTreatedAcres] [decimal](38, 10) NULL,
    [TreatmentTypeImportedText] [varchar](200) NULL,
    [CreateGisUploadAttemptID] [int] NULL CONSTRAINT [FK_TreatmentUpdate_GisUploadAttempt_CreateGisUploadAttemptID] FOREIGN KEY REFERENCES [dbo].[GisUploadAttempt]([GisUploadAttemptID]),
    [UpdateGisUploadAttemptID] [int] NULL CONSTRAINT [FK_TreatmentUpdate_GisUploadAttempt_UpdateGisUploadAttemptID] FOREIGN KEY REFERENCES [dbo].[GisUploadAttempt]([GisUploadAttemptID]),
    [TreatmentDetailedActivityTypeID] [int] NOT NULL CONSTRAINT [FK_TreatmentUpdate_TreatmentDetailedActivityType_TreatmentDetailedActivityTypeID] FOREIGN KEY REFERENCES [dbo].[TreatmentDetailedActivityType]([TreatmentDetailedActivityTypeID]),
    [TreatmentDetailedActivityTypeImportedText] [varchar](200) NULL,
    [ProgramID] [int] NULL CONSTRAINT [FK_TreatmentUpdate_Program_ProgramID] FOREIGN KEY REFERENCES [dbo].[Program]([ProgramID]),
    [ImportedFromGis] [bit] NULL,
    [ProjectLocationUpdateID] [int] NULL CONSTRAINT [FK_TreatmentUpdate_ProjectLocationUpdate_ProjectLocationUpdateID] FOREIGN KEY REFERENCES [dbo].[ProjectLocationUpdate]([ProjectLocationUpdateID]),
    [TreatmentCodeID] [int] NULL CONSTRAINT [FK_TreatmentUpdate_TreatmentCode_TreatmentCodeID] FOREIGN KEY REFERENCES [dbo].[TreatmentCode]([TreatmentCodeID]),
    [CostPerAcre] [money] NULL
)
GO