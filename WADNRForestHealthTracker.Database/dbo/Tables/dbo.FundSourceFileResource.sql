CREATE TABLE [dbo].[FundSourceFileResource](
	[FundSourceFileResourceID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FundSourceFileResource_FundSourceFileResourceID] PRIMARY KEY,
	[FundSourceID] [int] NOT NULL CONSTRAINT [FK_FundSourceFileResource_FundSource_FundSourceID] FOREIGN KEY REFERENCES [dbo].[FundSource]([FundSourceID]),
	[FileResourceID] [int] NOT NULL CONSTRAINT [FK_FundSourceFileResource_FileResource_FileResourceID] FOREIGN KEY REFERENCES [dbo].[FileResource]([FileResourceID]),
	[DisplayName] [varchar](200) NOT NULL,
	[Description] [varchar](1000) NULL,
 CONSTRAINT [AK_FundSourceFileResource_FundSourceID_FileResourceID] UNIQUE ([FundSourceID], [FileResourceID])
) 
GO