CREATE TABLE [dbo].[GisUploadSourceOrganization](
    [GisUploadSourceOrganizationID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_GisUploadSourceOrganization_GisUploadSourceOrganizationID] PRIMARY KEY,
    [GisUploadSourceOrganizationName] [varchar](100) NOT NULL,
    [ProjectTypeDefaultName] [varchar](100) NULL,
    [TreatmentTypeDefaultName] [varchar](100) NULL,
    [ImportIsFlattened] [bit] NULL,
    [AdjustProjectTypeBasedOnTreatmentTypes] [bit] NOT NULL,
    [ProjectStageDefaultID] [int] NOT NULL CONSTRAINT [FK_GisUploadSourceOrganization_ProjectStage_ProjectStageDefaultID_ProjectStageID] FOREIGN KEY REFERENCES [dbo].[ProjectStage]([ProjectStageID]),
    [DataDeriveProjectStage] [bit] NOT NULL,
    [DefaultLeadImplementerOrganizationID] [int] NOT NULL CONSTRAINT [FK_GisUploadSourceOrganization_Organization_DefaultLeadImplementerOrganizationID_OrganizationID] FOREIGN KEY REFERENCES [dbo].[Organization]([OrganizationID]),
    [RelationshipTypeForDefaultOrganizationID] [int] NOT NULL CONSTRAINT [FK_GisUploadSourceOrganization_RelationshipType_RelationshipTypeForDefaultOrganizationID_RelationshipTypeID] FOREIGN KEY REFERENCES [dbo].[RelationshipType]([RelationshipTypeID]),
    [ImportAsDetailedLocationInsteadOfTreatments] [bit] NOT NULL,
    [ProjectDescriptionDefaultText] [varchar](4000) NULL,
    [ApplyCompletedDateToProject] [bit] NOT NULL,
    [ApplyStartDateToProject] [bit] NOT NULL,
    [ProgramID] [int] NOT NULL CONSTRAINT [FK_GisUploadSourceOrganization_Program_ProgramID] FOREIGN KEY REFERENCES [dbo].[Program]([ProgramID]),
    [ImportAsDetailedLocationInAdditionToTreatments] [bit] NOT NULL,
    [ApplyStartDateToTreatments] [bit] NOT NULL,
    [ApplyEndDateToTreatments] [bit] NOT NULL,
    [GisUploadProgramMergeGroupingID] [int] NULL CONSTRAINT [FK_GisUploadSourceOrganization_GisUploadProgramMergeGrouping_GisUploadProgramMergeGroupingID] FOREIGN KEY REFERENCES [dbo].[GisUploadProgramMergeGrouping]([GisUploadProgramMergeGroupingID]),
    CONSTRAINT [AK_GisUploadSourceOrganization_ProgramID] UNIQUE ([ProgramID])
)
GO