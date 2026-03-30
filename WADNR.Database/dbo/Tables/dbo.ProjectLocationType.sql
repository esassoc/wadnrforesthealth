CREATE TABLE [dbo].[ProjectLocationType](
	[ProjectLocationTypeID] [int] NOT NULL CONSTRAINT [PK_ProjectLocationType_ProjectLocationTypeID] PRIMARY KEY,
	[ProjectLocationTypeName] [varchar](50) NULL CONSTRAINT [AK_ProjectLocationType_ProjectLocationTypeName] UNIQUE,
	[ProjectLocationTypeDisplayName] [varchar](50) NULL CONSTRAINT [AK_ProjectLocationType_ProjectLocationTypeDisplayName] UNIQUE,
	[ProjectLocationTypeMapLayerColor] [varchar](20) NOT NULL
)
GO
