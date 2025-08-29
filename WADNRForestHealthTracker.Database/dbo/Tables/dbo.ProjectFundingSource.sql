CREATE TABLE [dbo].[ProjectFundingSource](
    [ProjectFundingSourceID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectFundingSource_ProjectFundingSourceID] PRIMARY KEY,
    [ProjectID] [int] NOT NULL CONSTRAINT [FK_ProjectFundingSource_Project_ProjectID] FOREIGN KEY REFERENCES [dbo].[Project]([ProjectID]),
    [FundingSourceID] [int] NOT NULL CONSTRAINT [FK_ProjectFundingSource_FundingSource_FundingSourceID] FOREIGN KEY REFERENCES [dbo].[FundingSource]([FundingSourceID])
)
GO