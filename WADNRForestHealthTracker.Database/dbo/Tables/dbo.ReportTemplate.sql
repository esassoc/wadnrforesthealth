CREATE TABLE [dbo].[ReportTemplate](
    [ReportTemplateID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ReportTemplate_ReportTemplateID] PRIMARY KEY,
    [ReportTemplateName] [varchar](100) NOT NULL CONSTRAINT [AK_ReportTemplate_ReportTemplateName] UNIQUE,
    [ReportTemplateDisplayName] [varchar](100) NOT NULL CONSTRAINT [AK_ReportTemplate_ReportTemplateDisplayName] UNIQUE,
    [ReportTemplateDescription] [varchar](250) NOT NULL,
    [ReportTemplateModelID] [int] NOT NULL CONSTRAINT [FK_ReportTemplate_ReportTemplateModel_ReportTemplateModelID] FOREIGN KEY REFERENCES [dbo].[ReportTemplateModel]([ReportTemplateModelID])
)
GO