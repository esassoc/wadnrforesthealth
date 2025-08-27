SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CostTypeDatamartMapping](
	[CostTypeDatamartMappingID] [int] IDENTITY(1,1) NOT NULL,
	[CostTypeID] [int] NOT NULL,
	[DatamartObjectCode] [varchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[DatamartObjectName] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[DatamartSubObjectCode] [varchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[DatamartSubObjectName] [varchar](250) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_CostTypeDatamartMapping_CostTypeDatamartMappingID] PRIMARY KEY CLUSTERED 
(
	[CostTypeDatamartMappingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[CostTypeDatamartMapping]  WITH CHECK ADD  CONSTRAINT [FK_CostTypeDatamartMapping_CostType_CostTypeID] FOREIGN KEY([CostTypeID])
REFERENCES [dbo].[CostType] ([CostTypeID])
GO
ALTER TABLE [dbo].[CostTypeDatamartMapping] CHECK CONSTRAINT [FK_CostTypeDatamartMapping_CostType_CostTypeID]