CREATE TABLE [dbo].[ReportTemplate](
    [ReportTemplateID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ReportTemplate_ReportTemplateID] PRIMARY KEY,
    [FileResourceID] [int] NOT NULL CONSTRAINT [FK_ReportTemplate_FileResource_FileResourceID] FOREIGN KEY REFERENCES [dbo].[FileResource] ([FileResourceID]),
    [DisplayName] [varchar](50) NOT NULL,
    [Description] [varchar](250) NULL,
    [ReportTemplateModelTypeID] [int] NOT NULL CONSTRAINT [FK_ReportTemplate_ReportTemplateModelType_ReportTemplateModelTypeID] FOREIGN KEY REFERENCES [dbo].[ReportTemplateModelType] ([ReportTemplateModelTypeID]),
    [ReportTemplateModelID] [int] NOT NULL CONSTRAINT [FK_ReportTemplate_ReportTemplateModel_ReportTemplateModelID] FOREIGN KEY REFERENCES [dbo].[ReportTemplateModel]([ReportTemplateModelID]),
    [IsSystemTemplate] [bit] NOT NULL DEFAULT ((0))
)
GO