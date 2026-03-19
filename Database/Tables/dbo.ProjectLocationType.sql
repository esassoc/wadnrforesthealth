SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectLocationType](
	[ProjectLocationTypeID] [int] NOT NULL,
	[ProjectLocationTypeName] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ProjectLocationTypeDisplayName] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ProjectLocationTypeMapLayerColor] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_ProjectLocationType_ProjectLocationTypeID] PRIMARY KEY CLUSTERED 
(
	[ProjectLocationTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_ProjectLocationType_ProjectLocationTypeDisplayName] UNIQUE NONCLUSTERED 
(
	[ProjectLocationTypeDisplayName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_ProjectLocationType_ProjectLocationTypeName] UNIQUE NONCLUSTERED 
(
	[ProjectLocationTypeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
