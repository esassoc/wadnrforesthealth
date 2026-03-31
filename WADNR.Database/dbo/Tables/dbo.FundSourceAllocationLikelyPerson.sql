CREATE TABLE [dbo].[FundSourceAllocationLikelyPerson](
    [FundSourceAllocationLikelyPersonID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FundSourceAllocationLikelyPerson_FundSourceAllocationLikelyPersonID] PRIMARY KEY,
    [FundSourceAllocationID] [int] NOT NULL CONSTRAINT [FK_FundSourceAllocationLikelyPerson_FundSourceAllocation_FundSourceAllocationID] FOREIGN KEY REFERENCES [dbo].[FundSourceAllocation]([FundSourceAllocationID]),
    [PersonID] [int] NOT NULL CONSTRAINT [FK_FundSourceAllocationLikelyPerson_Person_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
    CONSTRAINT [AK_FundSourceAllocationLikelyPerson_FundSourceAllocationID_PersonID] UNIQUE ([FundSourceAllocationID], [PersonID])
)
GO