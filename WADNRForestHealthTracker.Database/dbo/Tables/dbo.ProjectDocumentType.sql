CREATE TABLE [dbo].[ProjectDocumentType](
	[ProjectDocumentTypeID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectDocumentType_ProjectDocumentTypeID] PRIMARY KEY,
	[ProjectDocumentTypeName] [varchar](100) NOT NULL,
	[ProjectDocumentTypeDisplayName] [varchar](200) NULL
)
GO
