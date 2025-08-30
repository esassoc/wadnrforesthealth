CREATE TABLE [dbo].[ProjectUpdateProgram](
    [ProjectUpdateProgramID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectUpdateProgram_ProjectUpdateProgramID] PRIMARY KEY,
    [ProjectUpdateID] [int] NOT NULL CONSTRAINT [FK_ProjectUpdateProgram_ProjectUpdate_ProjectUpdateID] FOREIGN KEY REFERENCES [dbo].[ProjectUpdate]([ProjectUpdateID]),
    [ProgramID] [int] NOT NULL CONSTRAINT [FK_ProjectUpdateProgram_Program_ProgramID] FOREIGN KEY REFERENCES [dbo].[Program]([ProgramID])
)
GO