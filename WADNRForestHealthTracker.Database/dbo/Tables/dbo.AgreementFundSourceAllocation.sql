SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AgreementFundSourceAllocation](
	[AgreementFundSourceAllocationID] [int] IDENTITY(1,1) NOT NULL,
	[AgreementID] [int] NOT NULL,
	[FundSourceAllocationID] [int] NOT NULL,
 CONSTRAINT [PK_AgreementFundSourceAllocation_AgreementFundSourceAllocationID] PRIMARY KEY CLUSTERED 
(
	[AgreementFundSourceAllocationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_AgreementFundSourceAllocation_AgreementID_FundSourceAllocationID] UNIQUE NONCLUSTERED 
(
	[AgreementID] ASC,
	[FundSourceAllocationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[AgreementFundSourceAllocation]  WITH CHECK ADD  CONSTRAINT [FK_AgreementFundSourceAllocation_Agreement_AgreementID] FOREIGN KEY([AgreementID])
REFERENCES [dbo].[Agreement] ([AgreementID])
GO
ALTER TABLE [dbo].[AgreementFundSourceAllocation] CHECK CONSTRAINT [FK_AgreementFundSourceAllocation_Agreement_AgreementID]
GO
ALTER TABLE [dbo].[AgreementFundSourceAllocation]  WITH CHECK ADD  CONSTRAINT [FK_AgreementFundSourceAllocation_FundSourceAllocation_FundSourceAllocationID] FOREIGN KEY([FundSourceAllocationID])
REFERENCES [dbo].[FundSourceAllocation] ([FundSourceAllocationID])
GO
ALTER TABLE [dbo].[AgreementFundSourceAllocation] CHECK CONSTRAINT [FK_AgreementFundSourceAllocation_FundSourceAllocation_FundSourceAllocationID]