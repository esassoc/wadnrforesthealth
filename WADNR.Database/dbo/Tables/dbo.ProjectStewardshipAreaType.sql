CREATE TABLE [dbo].[ProjectStewardshipAreaType](
    [ProjectStewardshipAreaTypeID] [int] NOT NULL CONSTRAINT [PK_ProjectStewardshipAreaType_ProjectStewardshipAreaTypeID] PRIMARY KEY,
    [ProjectStewardshipAreaTypeName] [varchar](50) NOT NULL CONSTRAINT [AK_ProjectStewardshipAreaType_ProjectStewardshipAreaTypeName] UNIQUE,
    [ProjectStewardshipAreaTypeDisplayName] [varchar](50) NOT NULL CONSTRAINT [AK_ProjectStewardshipAreaType_ProjectStewardshipAreaTypeDisplayName] UNIQUE
)
GO
