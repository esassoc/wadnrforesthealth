SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FirmaHomePageImage](
	[FirmaHomePageImageID] [int] IDENTITY(1,1) NOT NULL,
	[FileResourceID] [int] NOT NULL,
	[Caption] [varchar](300) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_FirmaHomePageImage_FirmaHomePageImageID] PRIMARY KEY CLUSTERED 
(
	[FirmaHomePageImageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[FirmaHomePageImage]  WITH CHECK ADD  CONSTRAINT [FK_FirmaHomePageImage_FileResource_FileResourceID] FOREIGN KEY([FileResourceID])
REFERENCES [dbo].[FileResource] ([FileResourceID])
GO
ALTER TABLE [dbo].[FirmaHomePageImage] CHECK CONSTRAINT [FK_FirmaHomePageImage_FileResource_FileResourceID]