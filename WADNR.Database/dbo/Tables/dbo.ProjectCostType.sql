CREATE TABLE [dbo].[ProjectCostType](
	[ProjectCostTypeID] [int] NOT NULL CONSTRAINT [PK_ProjectCostType_ProjectCostTypeID] PRIMARY KEY,
	[ProjectCostTypeName] [varchar](100) NOT NULL CONSTRAINT [AK_ProjectCostType_ProjectCostTypeName] UNIQUE,
	[ProjectCostTypeDisplayName] [varchar](100) NOT NULL CONSTRAINT [AK_ProjectCostType_ProjectCostTypeDisplayName] UNIQUE,
	[SortOrder] [int] NOT NULL
)
GO
