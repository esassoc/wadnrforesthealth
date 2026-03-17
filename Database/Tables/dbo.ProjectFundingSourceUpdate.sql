SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectFundingSourceUpdate](
	[ProjectFundingSourceUpdateID] [int] IDENTITY(1,1) NOT NULL,
	[ProjectUpdateBatchID] [int] NOT NULL,
	[FundingSourceID] [int] NOT NULL,
 CONSTRAINT [PK_ProjectFundingSourceUpdate_ProjectFundingSourceUpdateID] PRIMARY KEY CLUSTERED 
(
	[ProjectFundingSourceUpdateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[ProjectFundingSourceUpdate]  WITH CHECK ADD  CONSTRAINT [FK_ProjectFundingSourceUpdate_FundingSource_FundingSourceID] FOREIGN KEY([FundingSourceID])
REFERENCES [dbo].[FundingSource] ([FundingSourceID])
GO
ALTER TABLE [dbo].[ProjectFundingSourceUpdate] CHECK CONSTRAINT [FK_ProjectFundingSourceUpdate_FundingSource_FundingSourceID]
GO
ALTER TABLE [dbo].[ProjectFundingSourceUpdate]  WITH CHECK ADD  CONSTRAINT [FK_ProjectFundingSourceUpdate_ProjectUpdateBatch_ProjectUpdateBatchID] FOREIGN KEY([ProjectUpdateBatchID])
REFERENCES [dbo].[ProjectUpdateBatch] ([ProjectUpdateBatchID])
GO
ALTER TABLE [dbo].[ProjectFundingSourceUpdate] CHECK CONSTRAINT [FK_ProjectFundingSourceUpdate_ProjectUpdateBatch_ProjectUpdateBatchID]