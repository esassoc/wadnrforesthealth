CREATE TABLE [dbo].[PriorityLandscapeFileResource](
	[PriorityLandscapeFileResourceID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_PriorityLandscapeFileResource_PriorityLandscapeFileResourceID] PRIMARY KEY,
	[PriorityLandscapeID] [int] NOT NULL CONSTRAINT [FK_PriorityLandscapeFileResource_PriorityLandscape_PriorityLandscapeID] FOREIGN KEY REFERENCES [dbo].[PriorityLandscape]([PriorityLandscapeID]),
	[FileResourceID] [int] NOT NULL CONSTRAINT [FK_PriorityLandscapeFileResource_FileResource_FileResourceID] FOREIGN KEY REFERENCES [dbo].[FileResource]([FileResourceID]) ON DELETE CASCADE,
	[DisplayName] [varchar](200) NOT NULL,
	[Description] [varchar](1000) NULL,
 CONSTRAINT [AK_PriorityLandscapeFileResource_FileResourceID] UNIQUE ([FileResourceID])
)
GO