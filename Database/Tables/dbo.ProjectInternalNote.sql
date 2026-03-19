SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectInternalNote](
	[ProjectInternalNoteID] [int] IDENTITY(1,1) NOT NULL,
	[ProjectID] [int] NOT NULL,
	[Note] [varchar](8000) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[CreatePersonID] [int] NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdatePersonID] [int] NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_ProjectInternalNote_ProjectInternalNoteID] PRIMARY KEY CLUSTERED 
(
	[ProjectInternalNoteID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[ProjectInternalNote]  WITH CHECK ADD  CONSTRAINT [FK_ProjectInternalNote_Person_CreatePersonID_PersonID] FOREIGN KEY([CreatePersonID])
REFERENCES [dbo].[Person] ([PersonID])
GO
ALTER TABLE [dbo].[ProjectInternalNote] CHECK CONSTRAINT [FK_ProjectInternalNote_Person_CreatePersonID_PersonID]
GO
ALTER TABLE [dbo].[ProjectInternalNote]  WITH CHECK ADD  CONSTRAINT [FK_ProjectInternalNote_Person_UpdatePersonID_PersonID] FOREIGN KEY([UpdatePersonID])
REFERENCES [dbo].[Person] ([PersonID])
GO
ALTER TABLE [dbo].[ProjectInternalNote] CHECK CONSTRAINT [FK_ProjectInternalNote_Person_UpdatePersonID_PersonID]
GO
ALTER TABLE [dbo].[ProjectInternalNote]  WITH CHECK ADD  CONSTRAINT [FK_ProjectInternalNote_Project_ProjectID] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[Project] ([ProjectID])
GO
ALTER TABLE [dbo].[ProjectInternalNote] CHECK CONSTRAINT [FK_ProjectInternalNote_Project_ProjectID]