CREATE TABLE [dbo].[ProjectColorByType](
    [ProjectColorByTypeID] [int] NOT NULL CONSTRAINT [PK_ProjectColorByType_ProjectColorByTypeID] PRIMARY KEY,
    [ProjectColorByTypeName] [varchar](100) NOT NULL CONSTRAINT [AK_ProjectColorByType_ProjectColorByTypeName] UNIQUE,
    [ProjectColorByTypeNameWithIdentifier] [varchar](50) NOT NULL CONSTRAINT [AK_ProjectColorByType_ProjectColorByTypeNameWithIdentifier] UNIQUE,
    [ProjectColorByTypeDisplayName] [varchar](100) NOT NULL CONSTRAINT [AK_ProjectColorByType_ProjectColorByTypeDisplayName] UNIQUE,
    [SortOrder] [int] NOT NULL
)
GO
