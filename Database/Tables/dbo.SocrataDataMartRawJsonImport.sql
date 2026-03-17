SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SocrataDataMartRawJsonImport](
	[SocrataDataMartRawJsonImportID] [int] IDENTITY(1,1) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[SocrataDataMartRawJsonImportTableTypeID] [int] NOT NULL,
	[BienniumFiscalYear] [int] NULL,
	[FinanceApiLastLoadDate] [datetime] NULL,
	[RawJsonString] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[JsonImportDate] [datetime] NULL,
	[JsonImportStatusTypeID] [int] NOT NULL,
 CONSTRAINT [PK_SocrataDataMartRawJsonImport_SocrataDataMartRawJsonImportID] PRIMARY KEY CLUSTERED 
(
	[SocrataDataMartRawJsonImportID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[SocrataDataMartRawJsonImport]  WITH CHECK ADD  CONSTRAINT [FK_SocrataDataMartRawJsonImport_JsonImportStatusType_JsonImportStatusTypeID] FOREIGN KEY([JsonImportStatusTypeID])
REFERENCES [dbo].[JsonImportStatusType] ([JsonImportStatusTypeID])
GO
ALTER TABLE [dbo].[SocrataDataMartRawJsonImport] CHECK CONSTRAINT [FK_SocrataDataMartRawJsonImport_JsonImportStatusType_JsonImportStatusTypeID]
GO
ALTER TABLE [dbo].[SocrataDataMartRawJsonImport]  WITH CHECK ADD  CONSTRAINT [FK_SocrataDataMartRawJsonImport_SocrataDataMartRawJsonImportTableType_SocrataDataMartRawJsonImportTableTypeID] FOREIGN KEY([SocrataDataMartRawJsonImportTableTypeID])
REFERENCES [dbo].[SocrataDataMartRawJsonImportTableType] ([SocrataDataMartRawJsonImportTableTypeID])
GO
ALTER TABLE [dbo].[SocrataDataMartRawJsonImport] CHECK CONSTRAINT [FK_SocrataDataMartRawJsonImport_SocrataDataMartRawJsonImportTableType_SocrataDataMartRawJsonImportTableTypeID]