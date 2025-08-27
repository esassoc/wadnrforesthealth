SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tmpCostTypeMapping](
	[OBJECT_CODE] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[OBJECT_NAME] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[SUB_OBJECT_CODE] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[SUB_OBJECT_NAME] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[CostTypeDescription] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
