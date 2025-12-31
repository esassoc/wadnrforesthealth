CREATE TABLE [dbo].[ProjectFundingSourceUpdate](
    [ProjectFundingSourceUpdateID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectFundingSourceUpdate_ProjectFundingSourceUpdateID] PRIMARY KEY,
    [ProjectUpdateBatchID] [int] NOT NULL CONSTRAINT [FK_ProjectFundingSourceUpdate_ProjectUpdateBatch_ProjectUpdateBatchID] FOREIGN KEY REFERENCES [dbo].[ProjectUpdateBatch]([ProjectUpdateBatchID]),
    [FundingSourceID] [int] NOT NULL CONSTRAINT [FK_ProjectFundingSourceUpdate_FundingSource_FundingSourceID] FOREIGN KEY REFERENCES [dbo].[FundingSource]([FundingSourceID])
)
GO