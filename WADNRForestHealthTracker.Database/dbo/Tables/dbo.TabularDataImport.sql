CREATE TABLE [dbo].[TabularDataImport](
	[TabularDataImportID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_TabularDataImport_TabularDataImportID] PRIMARY KEY,
	[TabularDataImportTableTypeID] [int] NOT NULL CONSTRAINT [FK_TabularDataImport_TabularDataImportTableType_TabularDataImportTableTypeID] FOREIGN KEY REFERENCES [dbo].[TabularDataImportTableType]([TabularDataImportTableTypeID]),
	[TabularDataImportName] [varchar](200) NOT NULL,
	[TabularDataImportDisplayName] [varchar](200) NOT NULL,
	[TabularDataImportDescription] [varchar](max) NULL,
	[TabularDataImportDate] [datetime] NOT NULL,
	[TabularDataImportStatusTypeID] [int] NOT NULL CONSTRAINT [FK_TabularDataImport_TabularDataImportStatusType_TabularDataImportStatusTypeID] FOREIGN KEY REFERENCES [dbo].[TabularDataImportStatusType]([TabularDataImportStatusTypeID])
)
GO