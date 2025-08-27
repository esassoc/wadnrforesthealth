SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FundSourceAllocationLikelyPerson](
	[FundSourceAllocationLikelyPersonID] [int] IDENTITY(1,1) NOT NULL,
	[FundSourceAllocationID] [int] NOT NULL,
	[PersonID] [int] NOT NULL,
 CONSTRAINT [PK_FundSourceAllocationLikelyPerson_FundSourceAllocationLikelyPersonID] PRIMARY KEY CLUSTERED 
(
	[FundSourceAllocationLikelyPersonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_FundSourceAllocationLikelyPerson_FundSourceAllocationID_PersonID] UNIQUE NONCLUSTERED 
(
	[FundSourceAllocationID] ASC,
	[PersonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[FundSourceAllocationLikelyPerson]  WITH CHECK ADD  CONSTRAINT [FK_FundSourceAllocationLikelyPerson_FundSourceAllocation_FundSourceAllocationID] FOREIGN KEY([FundSourceAllocationID])
REFERENCES [dbo].[FundSourceAllocation] ([FundSourceAllocationID])
GO
ALTER TABLE [dbo].[FundSourceAllocationLikelyPerson] CHECK CONSTRAINT [FK_FundSourceAllocationLikelyPerson_FundSourceAllocation_FundSourceAllocationID]
GO
ALTER TABLE [dbo].[FundSourceAllocationLikelyPerson]  WITH CHECK ADD  CONSTRAINT [FK_FundSourceAllocationLikelyPerson_Person_PersonID] FOREIGN KEY([PersonID])
REFERENCES [dbo].[Person] ([PersonID])
GO
ALTER TABLE [dbo].[FundSourceAllocationLikelyPerson] CHECK CONSTRAINT [FK_FundSourceAllocationLikelyPerson_Person_PersonID]