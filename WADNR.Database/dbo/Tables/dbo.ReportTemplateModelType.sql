CREATE TABLE [dbo].[ReportTemplateModelType](
    [ReportTemplateModelTypeID] [int] NOT NULL CONSTRAINT [PK_ReportTemplateModelType_ReportTemplateModelTypeID] PRIMARY KEY,
    [ReportTemplateModelTypeName] [varchar](100) NOT NULL CONSTRAINT [AK_ReportTemplateModelType_ReportTemplateModelTypeName] UNIQUE,
    [ReportTemplateModelTypeDisplayName] [varchar](100) NOT NULL CONSTRAINT [AK_ReportTemplateModelType_ReportTemplateModelTypeDisplayName] UNIQUE,
    [ReportTemplateModelTypeDescription] [varchar](250) NOT NULL
)
GO
