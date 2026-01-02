CREATE TABLE [dbo].[ProjectDocumentType](
	[ProjectDocumentTypeID] [int] NOT NULL CONSTRAINT [PK_ProjectDocumentType_ProjectDocumentTypeID] PRIMARY KEY,
	[ProjectDocumentTypeName] [varchar](100) NOT NULL,
	[ProjectDocumentTypeDisplayName] [varchar](200) NULL
)
GO
