CREATE TABLE [dbo].[TabularDataImport](
	[TabularDataImportID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT PK_TabularDataImport_TabularDataImportID PRIMARY KEY,
	[TabularDataImportTableTypeID] [int] NOT NULL CONSTRAINT FK_TabularDataImport_TabularDataImportTableType_TabularDataImportTableTypeID FOREIGN KEY REFERENCES [dbo].[TabularDataImportTableType]([TabularDataImportTableTypeID]),
	[UploadDate] [datetime] NULL,
	[UploadPersonID] [int] NULL CONSTRAINT FK_TabularDataImport_Person_UploadPersonID_PersonID FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
	[LastProcessedDate] [datetime] NULL,
	[LastProcessedPersonID] [int] NULL CONSTRAINT FK_TabularDataImport_Person_LastProcessedPersonID_PersonID FOREIGN KEY REFERENCES [dbo].[Person]([PersonID])
)
GO