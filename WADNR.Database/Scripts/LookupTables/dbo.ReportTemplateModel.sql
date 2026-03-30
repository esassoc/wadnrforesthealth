

merge into dbo.ReportTemplateModel as Target
using (values

           (1, 'Project', 'Project', 'Templates will be with the "Project" model.'),
           (2, 'InvoicePaymentRequest', 'Invoice Payment Request', 'Templates will be with the "Invoice Payment Request" model.')
)
    as Source (ReportTemplateModelID, ReportTemplateModelName, ReportTemplateModelDisplayName, ReportTemplateModelDescription)
on Target.ReportTemplateModelID = Source.ReportTemplateModelID
when matched then
    update set
               ReportTemplateModelName = Source.ReportTemplateModelName,
               ReportTemplateModelDisplayName = Source.ReportTemplateModelDisplayName,
               ReportTemplateModelDescription = Source.ReportTemplateModelDescription
when not matched by target then
    insert (ReportTemplateModelID, ReportTemplateModelName, ReportTemplateModelDisplayName, ReportTemplateModelDescription)
    values (ReportTemplateModelID, ReportTemplateModelName, ReportTemplateModelDisplayName, ReportTemplateModelDescription)
when not matched by source then
    delete;