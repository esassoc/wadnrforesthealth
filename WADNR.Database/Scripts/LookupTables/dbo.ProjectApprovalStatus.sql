merge into dbo.ProjectApprovalStatus as Target
using (values
(1, 'Draft', 'Draft'),
(2, 'PendingApproval', 'Pending Approval'),
(3, 'Approved', 'Approved and Archived'),
(4, 'Rejected', 'Rejected'),
(5, 'Returned', 'Returned')
) as Source (ProjectApprovalStatusID, ProjectApprovalStatusName, ProjectApprovalStatusDisplayName)
on Target.ProjectApprovalStatusID = Source.ProjectApprovalStatusID
when matched then
    update set
        ProjectApprovalStatusName = Source.ProjectApprovalStatusName,
        ProjectApprovalStatusDisplayName = Source.ProjectApprovalStatusDisplayName
when not matched by target then
    insert (ProjectApprovalStatusID, ProjectApprovalStatusName, ProjectApprovalStatusDisplayName)
    values (ProjectApprovalStatusID, ProjectApprovalStatusName, ProjectApprovalStatusDisplayName)
when not matched by source then
    delete;
