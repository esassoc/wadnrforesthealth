CREATE TABLE [dbo].[TaxonomyTrunk](
    [TaxonomyTrunkID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_TaxonomyTrunk_TaxonomyTrunkID] PRIMARY KEY,
    [TaxonomyTrunkName] [varchar](100) NOT NULL,
    [TaxonomyTrunkDescription] [dbo].[html] NULL,
    [ThemeColor] [varchar](20) NULL,
    [TaxonomyTrunkCode] [varchar](10) NULL,
    [TaxonomyTrunkSortOrder] [int] NULL
)
GO
