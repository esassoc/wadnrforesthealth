CREATE TABLE [dbo].[ProjectProgram](
    [ProjectProgramID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectProgram_ProjectProgramID] PRIMARY KEY,
    [ProjectID] [int] NOT NULL CONSTRAINT [FK_ProjectProgram_Project_ProjectID] FOREIGN KEY REFERENCES [dbo].[Project]([ProjectID]),
    [ProgramID] [int] NOT NULL CONSTRAINT [FK_ProjectProgram_Program_ProgramID] FOREIGN KEY REFERENCES [dbo].[Program]([ProgramID]),
    CONSTRAINT [AK_ProjectProgram_ProjectID_ProgramID] UNIQUE ([ProjectID], [ProgramID])
)
GO
CREATE NONCLUSTERED INDEX IX_ProjectProgram_ProjectID ON dbo.ProjectProgram(ProjectID) INCLUDE (ProgramID);
GO