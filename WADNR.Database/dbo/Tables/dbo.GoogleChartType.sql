CREATE TABLE [dbo].[GoogleChartType](
	[GoogleChartTypeID] [int] NOT NULL CONSTRAINT [PK_GoogleChartType_GoogleChartTypeID] PRIMARY KEY,
	[GoogleChartTypeName] [varchar](50) NOT NULL,
	[GoogleChartTypeDisplayName] [varchar](50) NOT NULL,
	[SeriesDataDisplayType] [varchar](50) NULL,
	CONSTRAINT [AK_GoogleChartType_GoogleChartTypeDisplayName] UNIQUE ([GoogleChartTypeDisplayName]),
	CONSTRAINT [AK_GoogleChartType_GoogleChartTypeName] UNIQUE ([GoogleChartTypeName])
)
GO
