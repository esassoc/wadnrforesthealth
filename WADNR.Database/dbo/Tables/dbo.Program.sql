CREATE TABLE [dbo].[Program](
    [ProgramID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Program_ProgramID] PRIMARY KEY,
    [OrganizationID] [int] NOT NULL CONSTRAINT [FK_Program_Organization_OrganizationID] FOREIGN KEY REFERENCES [dbo].[Organization]([OrganizationID]),
    [ProgramName] [varchar](200) NULL,
    [ProgramShortName] [varchar](200) NULL,
    [ProgramPrimaryContactPersonID] [int] NULL CONSTRAINT [FK_Program_Person_ProgramPrimaryContactPersonID_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
    [ProgramIsActive] [bit] NOT NULL,
    [ProgramCreateDate] [datetime] NOT NULL,
    [ProgramCreatePersonID] [int] NOT NULL CONSTRAINT [FK_Program_Person_ProgramCreatePersonID_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
    [ProgramLastUpdatedDate] [datetime] NULL,
    [ProgramLastUpdatedByPersonID] [int] NULL CONSTRAINT [FK_Program_Person_ProgramLastUpdatedByPersonID_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
    [IsDefaultProgramForImportOnly] [bit] NOT NULL,
    [ProgramFileResourceID] [int] NULL CONSTRAINT [FK_Program_FileResource_ProgramFileResourceID_FileResourceID] FOREIGN KEY REFERENCES [dbo].[FileResource]([FileResourceID]),
    [ProgramNotes] [varchar](max) NULL,
    [ProgramExampleGeospatialUploadFileResourceID] [int] NULL CONSTRAINT [FK_Program_FileResource_ProgramExampleGeospatialUploadFileResourceID_FileResourceID] FOREIGN KEY REFERENCES [dbo].[FileResource]([FileResourceID]),
    CONSTRAINT [AK_Program_ProgramName_OrganizationID] UNIQUE ([ProgramName], [OrganizationID]),
    CONSTRAINT [AK_Program_ProgramShortName_OrganizationID] UNIQUE ([ProgramShortName], [OrganizationID])
)
GO