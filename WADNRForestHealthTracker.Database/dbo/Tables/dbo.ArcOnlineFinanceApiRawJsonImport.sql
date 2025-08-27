SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ArcOnlineFinanceApiRawJsonImport](
	[ArcOnlineFinanceApiRawJsonImportID] [int] IDENTITY(1,1) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[ArcOnlineFinanceApiRawJsonImportTableTypeID] [int] NOT NULL,
	[BienniumFiscalYear] [int] NULL,
	[FinanceApiLastLoadDate] [datetime] NULL,
	[RawJsonString] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[JsonImportDate] [datetime] NULL,
	[JsonImportStatusTypeID] [int] NOT NULL,
 CONSTRAINT [PK_ArcOnlineFinanceApiRawJsonImport_ArcOnlineFinanceApiRawJsonImportID] PRIMARY KEY CLUSTERED 
(
	[ArcOnlineFinanceApiRawJsonImportID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[ArcOnlineFinanceApiRawJsonImport]  WITH CHECK ADD  CONSTRAINT [FK_ArcOnlineFinanceApiRawJsonImport_ArcOnlineFinanceApiRawJsonImportTableType_ArcOnlineFinanceApiRawJsonImportTableTypeID] FOREIGN KEY([ArcOnlineFinanceApiRawJsonImportTableTypeID])
REFERENCES [dbo].[ArcOnlineFinanceApiRawJsonImportTableType] ([ArcOnlineFinanceApiRawJsonImportTableTypeID])
GO
ALTER TABLE [dbo].[ArcOnlineFinanceApiRawJsonImport] CHECK CONSTRAINT [FK_ArcOnlineFinanceApiRawJsonImport_ArcOnlineFinanceApiRawJsonImportTableType_ArcOnlineFinanceApiRawJsonImportTableTypeID]
GO
ALTER TABLE [dbo].[ArcOnlineFinanceApiRawJsonImport]  WITH CHECK ADD  CONSTRAINT [FK_ArcOnlineFinanceApiRawJsonImport_JsonImportStatusType_JsonImportStatusTypeID] FOREIGN KEY([JsonImportStatusTypeID])
REFERENCES [dbo].[JsonImportStatusType] ([JsonImportStatusTypeID])
GO
ALTER TABLE [dbo].[ArcOnlineFinanceApiRawJsonImport] CHECK CONSTRAINT [FK_ArcOnlineFinanceApiRawJsonImport_JsonImportStatusType_JsonImportStatusTypeID]