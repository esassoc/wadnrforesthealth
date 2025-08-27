merge into dbo.ProjectDocumentType as Target
using (values
(14, 'CostShareApplication', 'Cost Share Application'),
(15, 'CostShareSheet', 'Cost Share Sheet'),
(16, 'TreatmentSpecs', 'Treatment Specs'),
(17, 'Map', 'Map'),
(18, 'ApprovalLetter', 'Approval Letter'),
(19, 'ClaimForm', 'Claim Form'),
(20, 'Other', 'Other'),
(21, 'ManagementPlan', 'Management Plan'),
(22, 'MonitoringReport', 'Monitoring Report'),
(23, 'ProjectScoringMatrix', 'Project Scoring Matrix'),
(24, 'SiteVisitNotes', 'Site Visit Notes'),
(25, 'ApprovalChecklist', 'Approval Checklist'),
(26, 'Self-CostStatement', 'Self-Cost Statement')
) as Source (ProjectDocumentTypeID, ProjectDocumentTypeName, ProjectDocumentTypeDisplayName)
on Target.ProjectDocumentTypeID = Source.ProjectDocumentTypeID
when matched then
    update set
        ProjectDocumentTypeName = Source.ProjectDocumentTypeName,
        ProjectDocumentTypeDisplayName = Source.ProjectDocumentTypeDisplayName
when not matched by target then
    insert (ProjectDocumentTypeID, ProjectDocumentTypeName, ProjectDocumentTypeDisplayName)
    values (ProjectDocumentTypeID, ProjectDocumentTypeName, ProjectDocumentTypeDisplayName)
when not matched by source then
    delete;