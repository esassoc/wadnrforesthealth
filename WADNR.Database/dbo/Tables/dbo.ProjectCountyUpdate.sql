CREATE TABLE [dbo].[ProjectCountyUpdate](
	[ProjectCountyUpdateID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectCountyUpdate_ProjectCountyUpdateID] PRIMARY KEY,
	[ProjectUpdateBatchID] [int] NOT NULL CONSTRAINT [FK_ProjectCountyUpdate_ProjectUpdateBatch_ProjectUpdateBatchID] FOREIGN KEY REFERENCES [dbo].[ProjectUpdateBatch]([ProjectUpdateBatchID]),
	[CountyID] [int] NOT NULL CONSTRAINT [FK_ProjectCountyUpdate_County_CountyID] FOREIGN KEY REFERENCES [dbo].[County]([CountyID]),
 CONSTRAINT [AK_ProjectCountyUpdate_ProjectUpdateBatchID_CountyID] UNIQUE ([ProjectUpdateBatchID], [CountyID])
)
GO