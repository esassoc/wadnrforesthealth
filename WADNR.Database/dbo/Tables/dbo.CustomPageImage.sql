CREATE TABLE dbo.CustomPageImage
(
    CustomPageImageID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_CustomPageImage_CustomPageImageID PRIMARY KEY,
    [CustomPageID] [int] NOT NULL CONSTRAINT [FK_CustomPageImage_CustomPage_CustomPageID] FOREIGN KEY REFERENCES [dbo].[CustomPage] ([CustomPageID]),
    [FileResourceID] [int] NOT NULL CONSTRAINT [FK_CustomPageImage_FileResource_FileResourceID] FOREIGN KEY REFERENCES [dbo].[FileResource] ([FileResourceID])
)
GO