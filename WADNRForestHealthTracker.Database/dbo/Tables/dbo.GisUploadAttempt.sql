CREATE TABLE [dbo].[GisUploadAttempt](
	[GisUploadAttemptID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_GisUploadAttempt_GisUploadAttemptID] PRIMARY KEY,
	[GisUploadSourceOrganizationID] [int] NOT NULL CONSTRAINT [FK_GisUploadAttempt_GisUploadSourceOrganization_GisUploadSourceOrganizationID] FOREIGN KEY REFERENCES [dbo].[GisUploadSourceOrganization]([GisUploadSourceOrganizationID]),
	[GisUploadAttemptCreatePersonID] [int] NOT NULL CONSTRAINT [FK_GisUploadAttempt_Person_GisUploadAttemptCreatePersonID_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
	[GisUploadAttemptCreateDate] [datetime] NOT NULL,
	[ImportTableName] [varchar](100) NULL,
	[FileUploadSuccessful] [bit] NULL,
	[FeaturesSaved] [bit] NULL,
	[AttributesSaved] [bit] NULL,
	[AreaCalculationComplete] [bit] NULL,
	[ImportedToGeoJson] [bit] NULL
)
GO
CREATE NONCLUSTERED INDEX [IX_GisUploadAttempt_GisUploadAttemptCreatePersonID] ON [dbo].[GisUploadAttempt]([GisUploadAttemptCreatePersonID])
GO