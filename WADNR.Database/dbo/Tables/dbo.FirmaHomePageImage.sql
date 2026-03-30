CREATE TABLE [dbo].[FirmaHomePageImage](
    [FirmaHomePageImageID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FirmaHomePageImage_FirmaHomePageImageID] PRIMARY KEY,
    [FileResourceID] [int] NOT NULL CONSTRAINT [FK_FirmaHomePageImage_FileResource_FileResourceID] FOREIGN KEY REFERENCES [dbo].[FileResource]([FileResourceID]),
    [Caption] [varchar](300) NOT NULL,
    [SortOrder] [int] NOT NULL
)
GO