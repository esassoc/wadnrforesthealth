SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectProgram](
	[ProjectProgramID] [int] IDENTITY(1,1) NOT NULL,
	[ProjectID] [int] NOT NULL,
	[ProgramID] [int] NOT NULL,
 CONSTRAINT [PK_ProjectProgram_ProjectProgramID] PRIMARY KEY CLUSTERED 
(
	[ProjectProgramID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_ProjectProgram_ProjectID_ProgramID] UNIQUE NONCLUSTERED 
(
	[ProjectID] ASC,
	[ProgramID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[ProjectProgram]  WITH CHECK ADD  CONSTRAINT [FK_ProjectProgram_Program_ProgramID] FOREIGN KEY([ProgramID])
REFERENCES [dbo].[Program] ([ProgramID])
GO
ALTER TABLE [dbo].[ProjectProgram] CHECK CONSTRAINT [FK_ProjectProgram_Program_ProgramID]
GO
ALTER TABLE [dbo].[ProjectProgram]  WITH CHECK ADD  CONSTRAINT [FK_ProjectProgram_Project_ProjectID] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[Project] ([ProjectID])
GO
ALTER TABLE [dbo].[ProjectProgram] CHECK CONSTRAINT [FK_ProjectProgram_Project_ProjectID]