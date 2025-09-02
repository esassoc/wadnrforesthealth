CREATE TABLE [dbo].[TaxonomyLevel](
    [TaxonomyLevelID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_TaxonomyLevel_TaxonomyLevelID] PRIMARY KEY,
    [TaxonomyBranchID] [int] NOT NULL CONSTRAINT [FK_TaxonomyLevel_TaxonomyBranch_TaxonomyBranchID] FOREIGN KEY REFERENCES [dbo].[TaxonomyBranch]([TaxonomyBranchID]),
    [TaxonomyLevelName] [varchar](100) NOT NULL,
    [TaxonomyLevelDescription] [dbo].[html] NULL,
    [ThemeColor] [varchar](20) NULL,
    [TaxonomyLevelCode] [varchar](10) NULL,
    [TaxonomyLevelSortOrder] [int] NULL
)
GO
