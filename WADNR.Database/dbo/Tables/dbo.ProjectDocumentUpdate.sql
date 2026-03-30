CREATE TABLE [dbo].[ProjectDocumentUpdate](
	[ProjectDocumentUpdateID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectDocumentUpdate_ProjectDocumentUpdateID] PRIMARY KEY,
	[ProjectUpdateBatchID] [int] NOT NULL CONSTRAINT [FK_ProjectDocumentUpdate_ProjectUpdateBatch_ProjectUpdateBatchID] FOREIGN KEY REFERENCES [dbo].[ProjectUpdateBatch]([ProjectUpdateBatchID]),
	[FileResourceID] [int] NOT NULL CONSTRAINT [FK_ProjectDocumentUpdate_FileResource_FileResourceID] FOREIGN KEY REFERENCES [dbo].[FileResource]([FileResourceID]),
	[DisplayName] [varchar](200) NOT NULL,
	[Description] [varchar](1000) NULL,
	[ProjectDocumentTypeID] [int] NULL CONSTRAINT [FK_ProjectDocumentUpdate_ProjectDocumentType_ProjectDocumentTypeID] FOREIGN KEY REFERENCES [dbo].[ProjectDocumentType]([ProjectDocumentTypeID]),
 CONSTRAINT [AK_ProjectDocumentUpdate_DisplayName_ProjectUpdateBatchID] UNIQUE ([DisplayName], [ProjectUpdateBatchID]),
 CONSTRAINT [AK_ProjectDocumentUpdate_ProjectUpdateBatchID_FileResourceID] UNIQUE ([ProjectUpdateBatchID], [FileResourceID])
)
GO