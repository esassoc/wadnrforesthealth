CREATE TABLE [dbo].[ProjectType](
    [ProjectTypeID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectType_ProjectTypeID] PRIMARY KEY,
    [TaxonomyBranchID] [int] NOT NULL CONSTRAINT [FK_ProjectType_TaxonomyBranch_TaxonomyBranchID] FOREIGN KEY REFERENCES [dbo].[TaxonomyBranch] ([TaxonomyBranchID]),
    [ProjectTypeName] [varchar](100) NOT NULL,
    [ProjectTypeDescription] [dbo].[html] NULL,
    [ProjectTypeCode] [varchar](10) NULL,
    [ThemeColor] [varchar](7) NULL,
    [ProjectTypeSortOrder] [int] NULL,
    [LimitVisibilityToAdmin] [bit] NOT NULL
)
GO