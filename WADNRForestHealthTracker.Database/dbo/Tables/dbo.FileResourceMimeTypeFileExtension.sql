SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FileResourceMimeTypeFileExtension](
	[FileResourceMimeTypeFileExtensionID] [int] IDENTITY(1,1) NOT NULL,
	[FileResourceMimeTypeID] [int] NOT NULL,
	[FileResourceMimeTypeFileExtensionText] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_FileResourceMimeTypeFileExtension_FileResourceMimeTypeFileExtensionID] PRIMARY KEY CLUSTERED 
(
	[FileResourceMimeTypeFileExtensionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_FileResourceMimeTypeFileExtension_FileResourceMimeTypeID_FileResourceMimeTypeFileExtensionText] UNIQUE NONCLUSTERED 
(
	[FileResourceMimeTypeID] ASC,
	[FileResourceMimeTypeFileExtensionText] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[FileResourceMimeTypeFileExtension]  WITH CHECK ADD  CONSTRAINT [FK_FileResourceMimeTypeFileExtension_FileResourceMimeType_FileResourceMimeTypeID] FOREIGN KEY([FileResourceMimeTypeID])
REFERENCES [dbo].[FileResourceMimeType] ([FileResourceMimeTypeID])
GO
ALTER TABLE [dbo].[FileResourceMimeTypeFileExtension] CHECK CONSTRAINT [FK_FileResourceMimeTypeFileExtension_FileResourceMimeType_FileResourceMimeTypeID]