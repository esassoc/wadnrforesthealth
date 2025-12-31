CREATE TABLE [dbo].[ProgramPerson](
    [ProgramPersonID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProgramPerson_ProgramPersonID] PRIMARY KEY,
    [ProgramID] [int] NOT NULL CONSTRAINT [FK_ProgramPerson_Program_ProgramID] FOREIGN KEY REFERENCES [dbo].[Program]([ProgramID]),
    [PersonID] [int] NOT NULL CONSTRAINT [FK_ProgramPerson_Person_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
    CONSTRAINT [AK_ProgramPerson_ProgramID_PersonID] UNIQUE ([ProgramID], [PersonID])
)
GO