CREATE TABLE [dbo].[TaxonomyLevel](
    [TaxonomyLevelID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_TaxonomyLevel_TaxonomyLevelID] PRIMARY KEY,
    [TaxonomyLevelName] [varchar](100) NOT NULL CONSTRAINT [AK_TaxonomyLevel_TaxonomyLevelName] UNIQUE,
    [TaxonomyLevelDisplayName] [varchar](100) NOT NULL CONSTRAINT [AK_TaxonomyLevel_TaxonomyLevelDisplayName] UNIQUE
)
GO
