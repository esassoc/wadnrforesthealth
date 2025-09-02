CREATE TABLE [dbo].[TaxonomyBranch](
    [TaxonomyBranchID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_TaxonomyBranch_TaxonomyBranchID] PRIMARY KEY,
    [TaxonomyTrunkID] [int] NOT NULL CONSTRAINT [FK_TaxonomyBranch_TaxonomyTrunk_TaxonomyTrunkID] FOREIGN KEY REFERENCES [dbo].[TaxonomyTrunk]([TaxonomyTrunkID]),
    [TaxonomyBranchName] [varchar](100) NOT NULL,
    [TaxonomyBranchDescription] [dbo].[html] NULL,
    [ThemeColor] [varchar](20) NULL,
    [TaxonomyBranchCode] [varchar](10) NULL,
    [TaxonomyBranchSortOrder] [int] NULL
)
GO