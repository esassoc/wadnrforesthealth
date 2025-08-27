SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TreatmentUpdate](
	[TreatmentUpdateID] [int] IDENTITY(1,1) NOT NULL,
	[ProjectUpdateBatchID] [int] NOT NULL,
	[TreatmentStartDate] [datetime] NULL,
	[TreatmentEndDate] [datetime] NULL,
	[TreatmentFootprintAcres] [decimal](38, 10) NOT NULL,
	[TreatmentNotes] [varchar](2000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TreatmentTypeID] [int] NOT NULL,
	[TreatmentTreatedAcres] [decimal](38, 10) NULL,
	[TreatmentTypeImportedText] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[CreateGisUploadAttemptID] [int] NULL,
	[UpdateGisUploadAttemptID] [int] NULL,
	[TreatmentDetailedActivityTypeID] [int] NOT NULL,
	[TreatmentDetailedActivityTypeImportedText] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ProgramID] [int] NULL,
	[ImportedFromGis] [bit] NULL,
	[ProjectLocationUpdateID] [int] NULL,
	[TreatmentCodeID] [int] NULL,
	[CostPerAcre] [money] NULL,
 CONSTRAINT [PK_TreatmentUpdate_TreatmentUpdateID] PRIMARY KEY CLUSTERED 
(
	[TreatmentUpdateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[TreatmentUpdate]  WITH CHECK ADD  CONSTRAINT [FK_TreatmentUpdate_GisUploadAttempt_CreateGisUploadAttemptID_GisUploadAttemptID] FOREIGN KEY([CreateGisUploadAttemptID])
REFERENCES [dbo].[GisUploadAttempt] ([GisUploadAttemptID])
GO
ALTER TABLE [dbo].[TreatmentUpdate] CHECK CONSTRAINT [FK_TreatmentUpdate_GisUploadAttempt_CreateGisUploadAttemptID_GisUploadAttemptID]
GO
ALTER TABLE [dbo].[TreatmentUpdate]  WITH CHECK ADD  CONSTRAINT [FK_TreatmentUpdate_GisUploadAttempt_UpdateGisUploadAttemptID_GisUploadAttemptID] FOREIGN KEY([UpdateGisUploadAttemptID])
REFERENCES [dbo].[GisUploadAttempt] ([GisUploadAttemptID])
GO
ALTER TABLE [dbo].[TreatmentUpdate] CHECK CONSTRAINT [FK_TreatmentUpdate_GisUploadAttempt_UpdateGisUploadAttemptID_GisUploadAttemptID]
GO
ALTER TABLE [dbo].[TreatmentUpdate]  WITH CHECK ADD  CONSTRAINT [FK_TreatmentUpdate_Program_ProgramID] FOREIGN KEY([ProgramID])
REFERENCES [dbo].[Program] ([ProgramID])
GO
ALTER TABLE [dbo].[TreatmentUpdate] CHECK CONSTRAINT [FK_TreatmentUpdate_Program_ProgramID]
GO
ALTER TABLE [dbo].[TreatmentUpdate]  WITH CHECK ADD  CONSTRAINT [FK_TreatmentUpdate_ProjectLocationUpdate_ProjectLocationUpdateID] FOREIGN KEY([ProjectLocationUpdateID])
REFERENCES [dbo].[ProjectLocationUpdate] ([ProjectLocationUpdateID])
GO
ALTER TABLE [dbo].[TreatmentUpdate] CHECK CONSTRAINT [FK_TreatmentUpdate_ProjectLocationUpdate_ProjectLocationUpdateID]
GO
ALTER TABLE [dbo].[TreatmentUpdate]  WITH CHECK ADD  CONSTRAINT [FK_TreatmentUpdate_ProjectUpdateBatch_ProjectUpdateBatchID] FOREIGN KEY([ProjectUpdateBatchID])
REFERENCES [dbo].[ProjectUpdateBatch] ([ProjectUpdateBatchID])
GO
ALTER TABLE [dbo].[TreatmentUpdate] CHECK CONSTRAINT [FK_TreatmentUpdate_ProjectUpdateBatch_ProjectUpdateBatchID]
GO
ALTER TABLE [dbo].[TreatmentUpdate]  WITH CHECK ADD  CONSTRAINT [FK_TreatmentUpdate_TreatmentCode_TreatmentCodeID] FOREIGN KEY([TreatmentCodeID])
REFERENCES [dbo].[TreatmentCode] ([TreatmentCodeID])
GO
ALTER TABLE [dbo].[TreatmentUpdate] CHECK CONSTRAINT [FK_TreatmentUpdate_TreatmentCode_TreatmentCodeID]
GO
ALTER TABLE [dbo].[TreatmentUpdate]  WITH CHECK ADD  CONSTRAINT [FK_TreatmentUpdate_TreatmentDetailedActivityType_TreatmentDetailedActivityTypeID] FOREIGN KEY([TreatmentDetailedActivityTypeID])
REFERENCES [dbo].[TreatmentDetailedActivityType] ([TreatmentDetailedActivityTypeID])
GO
ALTER TABLE [dbo].[TreatmentUpdate] CHECK CONSTRAINT [FK_TreatmentUpdate_TreatmentDetailedActivityType_TreatmentDetailedActivityTypeID]
GO
ALTER TABLE [dbo].[TreatmentUpdate]  WITH CHECK ADD  CONSTRAINT [FK_TreatmentUpdate_TreatmentType_TreatmentTypeID] FOREIGN KEY([TreatmentTypeID])
REFERENCES [dbo].[TreatmentType] ([TreatmentTypeID])
GO
ALTER TABLE [dbo].[TreatmentUpdate] CHECK CONSTRAINT [FK_TreatmentUpdate_TreatmentType_TreatmentTypeID]