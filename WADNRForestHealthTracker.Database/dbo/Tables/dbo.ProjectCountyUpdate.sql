SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectCountyUpdate](
	[ProjectCountyUpdateID] [int] IDENTITY(1,1) NOT NULL,
	[ProjectUpdateBatchID] [int] NOT NULL,
	[CountyID] [int] NOT NULL,
 CONSTRAINT [PK_ProjectCountyUpdate_ProjectCountyUpdateID] PRIMARY KEY CLUSTERED 
(
	[ProjectCountyUpdateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_ProjectCountyUpdate_ProjectUpdateBatchID_CountyID] UNIQUE NONCLUSTERED 
(
	[ProjectUpdateBatchID] ASC,
	[CountyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[ProjectCountyUpdate]  WITH CHECK ADD  CONSTRAINT [FK_ProjectCountyUpdate_County_CountyID] FOREIGN KEY([CountyID])
REFERENCES [dbo].[County] ([CountyID])
GO
ALTER TABLE [dbo].[ProjectCountyUpdate] CHECK CONSTRAINT [FK_ProjectCountyUpdate_County_CountyID]
GO
ALTER TABLE [dbo].[ProjectCountyUpdate]  WITH CHECK ADD  CONSTRAINT [FK_ProjectCountyUpdate_ProjectUpdateBatch_ProjectUpdateBatchID] FOREIGN KEY([ProjectUpdateBatchID])
REFERENCES [dbo].[ProjectUpdateBatch] ([ProjectUpdateBatchID])
GO
ALTER TABLE [dbo].[ProjectCountyUpdate] CHECK CONSTRAINT [FK_ProjectCountyUpdate_ProjectUpdateBatch_ProjectUpdateBatchID]