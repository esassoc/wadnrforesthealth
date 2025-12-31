merge into dbo.ProjectCreateSection as Target
using (values
(2, 'Basics', 'Basics', 20, 1, 6, 1),
(3, 'LocationSimple', 'Location - Simple', 30, 1, 6, 1),
(4, 'LocationDetailed', 'Location - Detailed', 40, 0, 2, 0),
(8, 'ExpectedFunding', 'Expected Funding', 80, 0, 5, 0),
(11, 'Classifications', 'Classifications', 90, 0, 5, 0),
(13, 'Photos', 'Photos', 100, 0, 5, 0),
(14, 'NotesAndDocuments', 'Documents and Notes', 110, 0, 5, 0),
(15, 'Organizations', 'Organizations', 70, 0, 5, 0),
(16, 'Contacts', 'Contacts', 65, 0, 5, 0),
(17, 'DNRUplandRegions', 'DNR Upland Regions', 50, 1, 2, 0),
(18, 'PriorityLandscapes', 'Priority Landscapes', 45, 1, 2, 0),
(20, 'Treatments', 'Treatments', 60, 0, 2, 0),
(21, 'Counties', 'Counties', 55, 1, 2, 0)
) as Source (ProjectCreateSectionID, ProjectCreateSectionName, ProjectCreateSectionDisplayName, SortOrder, HasCompletionStatus, ProjectWorkflowSectionGroupingID, IsSectionRequired)
on Target.ProjectCreateSectionID = Source.ProjectCreateSectionID
when matched then
    update set
        ProjectCreateSectionName = Source.ProjectCreateSectionName,
        ProjectCreateSectionDisplayName = Source.ProjectCreateSectionDisplayName,
        SortOrder = Source.SortOrder,
        HasCompletionStatus = Source.HasCompletionStatus,
        ProjectWorkflowSectionGroupingID = Source.ProjectWorkflowSectionGroupingID,
        IsSectionRequired = Source.IsSectionRequired
when not matched by target then
    insert (ProjectCreateSectionID, ProjectCreateSectionName, ProjectCreateSectionDisplayName, SortOrder, HasCompletionStatus, ProjectWorkflowSectionGroupingID, IsSectionRequired)
    values (ProjectCreateSectionID, ProjectCreateSectionName, ProjectCreateSectionDisplayName, SortOrder, HasCompletionStatus, ProjectWorkflowSectionGroupingID, IsSectionRequired)
when not matched by source then
    delete;
