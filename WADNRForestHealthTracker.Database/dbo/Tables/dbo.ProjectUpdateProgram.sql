CREATE TABLE [dbo].[ProjectUpdateProgram](
    [ProjectUpdateProgramID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectUpdateProgram_ProjectUpdateProgramID] PRIMARY KEY,
    [ProjectUpdateBatchID] [int] NOT NULL CONSTRAINT [FK_ProjectUpdateProgram_ProjectUpdateBatch_ProjectUpdateBatchID] FOREIGN KEY REFERENCES [dbo].[ProjectUpdateBatch]([ProjectUpdateBatchID]),
    [ProgramID] [int] NOT NULL CONSTRAINT [FK_ProjectUpdateProgram_Program_ProgramID] FOREIGN KEY REFERENCES [dbo].[Program]([ProgramID])
)
GO