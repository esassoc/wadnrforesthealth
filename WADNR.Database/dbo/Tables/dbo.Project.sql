CREATE TABLE [dbo].[Project](
    [ProjectID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Project_ProjectID] PRIMARY KEY,
    [ProjectTypeID] [int] NOT NULL CONSTRAINT [FK_Project_ProjectType_ProjectTypeID] FOREIGN KEY REFERENCES [dbo].[ProjectType]([ProjectTypeID]),
    [ProjectStageID] [int] NOT NULL CONSTRAINT [FK_Project_ProjectStage_ProjectStageID] FOREIGN KEY REFERENCES [dbo].[ProjectStage]([ProjectStageID]),
    [ProjectName] [varchar](140) NOT NULL,
    [ProjectDescription] [varchar](4000) NULL,
    [CompletionDate] [date] NULL,
    [EstimatedTotalCost] [money] NULL,
    [ProjectLocationPoint] [geometry] NULL,
    [IsFeatured] [bit] NOT NULL,
    [ProjectLocationNotes] [varchar](4000) NULL,
    [PlannedDate] [date] NULL,
    [ProjectLocationSimpleTypeID] [int] NOT NULL CONSTRAINT [FK_Project_ProjectLocationSimpleType_ProjectLocationSimpleTypeID] FOREIGN KEY REFERENCES [dbo].[ProjectLocationSimpleType]([ProjectLocationSimpleTypeID]),
    [ProjectApprovalStatusID] [int] NOT NULL CONSTRAINT [FK_Project_ProjectApprovalStatus_ProjectApprovalStatusID] FOREIGN KEY REFERENCES [dbo].[ProjectApprovalStatus]([ProjectApprovalStatusID]),
    [ProposingPersonID] [int] NULL CONSTRAINT [FK_Project_Person_ProposingPersonID_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
    [ProposingDate] [datetime] NULL,
    [SubmissionDate] [datetime] NULL,
    [ApprovalDate] [datetime] NULL,
    [ReviewedByPersonID] [int] NULL CONSTRAINT [FK_Project_Person_ReviewedByPersonID_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
    [DefaultBoundingBox] [geometry] NULL,
    [NoExpendituresToReportExplanation] [varchar](max) NULL,
    [FocusAreaID] [int] NULL CONSTRAINT [FK_Project_FocusArea_FocusAreaID] FOREIGN KEY REFERENCES [dbo].[FocusArea]([FocusAreaID]),
    [NoRegionsExplanation] [varchar](4000) NULL,
    [ExpirationDate] [date] NULL,
    [FhtProjectNumber] [varchar](20) NOT NULL CONSTRAINT [AK_Project_FhtProjectNumber] UNIQUE,
    [NoPriorityLandscapesExplanation] [varchar](4000) NULL,
    [CreateGisUploadAttemptID] [int] NULL CONSTRAINT [FK_Project_GisUploadAttempt_CreateGisUploadAttemptID_GisUploadAttemptID] FOREIGN KEY REFERENCES [dbo].[GisUploadAttempt]([GisUploadAttemptID]),
    [LastUpdateGisUploadAttemptID] [int] NULL CONSTRAINT [FK_Project_GisUploadAttempt_LastUpdateGisUploadAttemptID_GisUploadAttemptID] FOREIGN KEY REFERENCES [dbo].[GisUploadAttempt]([GisUploadAttemptID]),
    [ProjectGisIdentifier] [varchar](140) NULL,
    [ProjectFundingSourceNotes] [varchar](4000) NULL,
    [NoCountiesExplanation] [varchar](4000) NULL,
    [PercentageMatch] [int] NULL,
    CONSTRAINT [CK_Project_ProjectCannotHaveProjectStageProposalAndApprovalStatusApproved] CHECK ([ProjectStageID]<>(1) OR [ProjectApprovalStatusID]<>(3)),
    CONSTRAINT [CK_Project_ProjectGisIdentifierDoesNotEndWithSpace] CHECK ((right([ProjectGisIdentifier],(1))<>' ')),
    CONSTRAINT [CK_Project_ProjectLocationPoint_IsPointData] CHECK (([ProjectLocationPoint] IS NULL OR [ProjectLocationPoint] IS NOT NULL AND [ProjectLocationPoint].[STGeometryType]()='Point'))
)
GO;

--CREATE SPATIAL INDEX [SPATIAL_Project_ProjectLocationPoint] ON [dbo].[Project]([ProjectLocationPoint]) 
--USING GEOMETRY_AUTO_GRID WITH 
--    (BOUNDING_BOX = (-125, 45, -117, 50), CELLS_PER_OBJECT = 8)
--GO