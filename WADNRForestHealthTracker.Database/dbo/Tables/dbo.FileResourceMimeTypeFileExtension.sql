CREATE TABLE [dbo].[FileResourceMimeTypeFileExtension](
	[FileResourceMimeTypeFileExtensionID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FileResourceMimeTypeFileExtension_FileResourceMimeTypeFileExtensionID] PRIMARY KEY,
	[FileResourceMimeTypeID] [int] NOT NULL CONSTRAINT [FK_FileResourceMimeTypeFileExtension_FileResourceMimeType_FileResourceMimeTypeID] FOREIGN KEY REFERENCES [dbo].[FileResourceMimeType]([FileResourceMimeTypeID]),
	[FileResourceMimeTypeFileExtensionText] [varchar](100) NOT NULL,
 CONSTRAINT [AK_FileResourceMimeTypeFileExtension_FileResourceMimeTypeID_FileResourceMimeTypeFileExtensionText] UNIQUE ([FileResourceMimeTypeID], [FileResourceMimeTypeFileExtensionText])
)
GO