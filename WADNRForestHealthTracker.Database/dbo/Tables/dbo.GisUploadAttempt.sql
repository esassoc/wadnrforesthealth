SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GisUploadAttempt](
	[GisUploadAttemptID] [int] IDENTITY(1,1) NOT NULL,
	[GisUploadSourceOrganizationID] [int] NOT NULL,
	[GisUploadAttemptCreatePersonID] [int] NOT NULL,
	[GisUploadAttemptCreateDate] [datetime] NOT NULL,
	[ImportTableName] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[FileUploadSuccessful] [bit] NULL,
	[FeaturesSaved] [bit] NULL,
	[AttributesSaved] [bit] NULL,
	[AreaCalculationComplete] [bit] NULL,
	[ImportedToGeoJson] [bit] NULL,
 CONSTRAINT [PK_GisUploadAttempt_GisUploadAttemptID] PRIMARY KEY CLUSTERED 
(
	[GisUploadAttemptID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX [IX_GisUploadAttempt_GisUploadAttemptCreatePersonID] ON [dbo].[GisUploadAttempt]
(
	[GisUploadAttemptCreatePersonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[GisUploadAttempt]  WITH CHECK ADD  CONSTRAINT [FK_GisUploadAttempt_GisUploadSourceOrganization_GisUploadSourceOrganizationID] FOREIGN KEY([GisUploadSourceOrganizationID])
REFERENCES [dbo].[GisUploadSourceOrganization] ([GisUploadSourceOrganizationID])
GO
ALTER TABLE [dbo].[GisUploadAttempt] CHECK CONSTRAINT [FK_GisUploadAttempt_GisUploadSourceOrganization_GisUploadSourceOrganizationID]
GO
ALTER TABLE [dbo].[GisUploadAttempt]  WITH CHECK ADD  CONSTRAINT [FK_GisUploadAttempt_Person_GisUploadAttemptCreatePersonID_PersonID] FOREIGN KEY([GisUploadAttemptCreatePersonID])
REFERENCES [dbo].[Person] ([PersonID])
GO
ALTER TABLE [dbo].[GisUploadAttempt] CHECK CONSTRAINT [FK_GisUploadAttempt_Person_GisUploadAttemptCreatePersonID_PersonID]