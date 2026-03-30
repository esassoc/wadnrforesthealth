CREATE TABLE [dbo].[ProjectLocationFilterType](
    [ProjectLocationFilterTypeID] [int] NOT NULL CONSTRAINT [PK_ProjectLocationFilterType_ProjectLocationFilterTypeID] PRIMARY KEY,
    [ProjectLocationFilterTypeName] [varchar](100) NOT NULL CONSTRAINT [AK_ProjectLocationFilterType_ProjectLocationFilterTypeName] UNIQUE,
    [ProjectLocationFilterTypeNameWithIdentifier] [varchar](50) NOT NULL CONSTRAINT [AK_ProjectLocationFilterType_ProjectLocationFilterTypeNameWithIdentifier] UNIQUE,
    [ProjectLocationFilterTypeDisplayName] [varchar](100) NOT NULL CONSTRAINT [AK_ProjectLocationFilterType_ProjectLocationFilterTypeDisplayName] UNIQUE,
    [SortOrder] [int] NOT NULL,
    [DisplayGroup] [int] NOT NULL
)
GO
