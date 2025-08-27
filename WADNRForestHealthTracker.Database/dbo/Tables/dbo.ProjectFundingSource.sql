SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectFundingSource](
	[ProjectFundingSourceID] [int] IDENTITY(1,1) NOT NULL,
	[ProjectID] [int] NOT NULL,
	[FundingSourceID] [int] NOT NULL,
 CONSTRAINT [PK_ProjectFundingSource_ProjectFundingSourceID] PRIMARY KEY CLUSTERED 
(
	[ProjectFundingSourceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[ProjectFundingSource]  WITH CHECK ADD  CONSTRAINT [FK_ProjectFundingSource_FundingSource_FundingSourceID] FOREIGN KEY([FundingSourceID])
REFERENCES [dbo].[FundingSource] ([FundingSourceID])
GO
ALTER TABLE [dbo].[ProjectFundingSource] CHECK CONSTRAINT [FK_ProjectFundingSource_FundingSource_FundingSourceID]
GO
ALTER TABLE [dbo].[ProjectFundingSource]  WITH CHECK ADD  CONSTRAINT [FK_ProjectFundingSource_Project_ProjectID] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[Project] ([ProjectID])
GO
ALTER TABLE [dbo].[ProjectFundingSource] CHECK CONSTRAINT [FK_ProjectFundingSource_Project_ProjectID]