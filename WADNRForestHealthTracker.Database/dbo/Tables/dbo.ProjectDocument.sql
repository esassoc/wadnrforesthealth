CREATE TABLE [dbo].[ProjectDocument](
	[ProjectDocumentID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectDocument_ProjectDocumentID] PRIMARY KEY,
	[ProjectID] [int] NOT NULL CONSTRAINT [FK_ProjectDocument_Project_ProjectID] FOREIGN KEY REFERENCES [dbo].[Project]([ProjectID]),
	[FileResourceID] [int] NOT NULL CONSTRAINT [FK_ProjectDocument_FileResource_FileResourceID] FOREIGN KEY REFERENCES [dbo].[FileResource]([FileResourceID]),
	[DisplayName] [varchar](200) NOT NULL,
	[Description] [varchar](1000) NULL,
	[ProjectDocumentTypeID] [int] NULL CONSTRAINT [FK_ProjectDocument_ProjectDocumentType_ProjectDocumentTypeID] FOREIGN KEY REFERENCES [dbo].[ProjectDocumentType]([ProjectDocumentTypeID]),
	CONSTRAINT [AK_ProjectDocument_DisplayName_ProjectID] UNIQUE ([DisplayName], [ProjectID]),
	CONSTRAINT [AK_ProjectDocument_ProjectID_FileResourceID] UNIQUE ([ProjectID], [FileResourceID])
) 
GO