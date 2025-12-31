CREATE TABLE [dbo].[ReportTemplateModel](
    [ReportTemplateModelID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ReportTemplateModel_ReportTemplateModelID] PRIMARY KEY,
    [ReportTemplateModelName] [varchar](100) NOT NULL CONSTRAINT [AK_ReportTemplateModel_ReportTemplateModelName] UNIQUE,
    [ReportTemplateModelDisplayName] [varchar](100) NOT NULL CONSTRAINT [AK_ReportTemplateModel_ReportTemplateModelDisplayName] UNIQUE,
    [ReportTemplateModelDescription] [varchar](250) NOT NULL
)
GO
