CREATE TABLE [dbo].[FirmaPageImage](
	[FirmaPageImageID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FirmaPageImage_FirmaPageImageID] PRIMARY KEY,
	[FirmaPageID] [int] NOT NULL CONSTRAINT [FK_FirmaPageImage_FirmaPage_FirmaPageID] FOREIGN KEY REFERENCES [dbo].[FirmaPage]([FirmaPageID]),
	[FileResourceID] [int] NOT NULL CONSTRAINT [FK_FirmaPageImage_FileResource_FileResourceID] FOREIGN KEY REFERENCES [dbo].[FileResource]([FileResourceID]),
 CONSTRAINT [AK_FirmaPageImage_FirmaPageImageID_FileResourceID] UNIQUE ([FirmaPageImageID], [FileResourceID])
)
GO