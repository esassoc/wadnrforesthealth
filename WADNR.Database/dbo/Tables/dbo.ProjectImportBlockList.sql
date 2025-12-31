CREATE TABLE [dbo].[ProjectImportBlockList](
    [ProjectImportBlockListID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectImportBlockList_ProjectImportBlockListID] PRIMARY KEY,
    [ProgramID] [int] NOT NULL CONSTRAINT [FK_ProjectImportBlockList_Program_ProgramID] FOREIGN KEY REFERENCES [dbo].[Program]([ProgramID]),
    [ProjectGisIdentifier] [varchar](140) NULL,
    [ProjectName] [varchar](140) NULL,
    [ProjectID] [int] NULL CONSTRAINT [FK_ProjectImportBlockList_Project_ProjectID] FOREIGN KEY REFERENCES [dbo].[Project]([ProjectID]),
    [Notes] [varchar](500) NULL,
    CONSTRAINT [CK_ProjectImportBlockList_ProjectGisIdentifierOrProjectName_IsRequired] CHECK (([ProjectGisIdentifier] IS NOT NULL OR [ProjectName] IS NOT NULL))
)
GO