CREATE TABLE [dbo].[PersonStewardTaxonomyBranch](
	[PersonStewardTaxonomyBranchID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_PersonStewardTaxonomyBranch_PersonStewardTaxonomyBranchID] PRIMARY KEY,
	[PersonID] [int] NOT NULL CONSTRAINT [FK_PersonStewardTaxonomyBranch_Person_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
	[TaxonomyBranchID] [int] NOT NULL CONSTRAINT [FK_PersonStewardTaxonomyBranch_TaxonomyBranch_TaxonomyBranchID] FOREIGN KEY REFERENCES [dbo].[TaxonomyBranch]([TaxonomyBranchID])
)
GO