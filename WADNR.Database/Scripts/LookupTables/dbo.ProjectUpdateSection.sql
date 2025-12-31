merge into dbo.ProjectUpdateSection as Target
using (values
(2, 'Basics', 'Basics', 10, 1, 6),
(3, 'LocationSimple', 'Location - Simple', 20, 1, 6),
(4, 'LocationDetailed', 'Location - Detailed', 30, 0, 2),
(7, 'ExpectedFunding', 'Expected Funding', 100, 0, 5),
(9, 'Photos', 'Photos', 110, 0, 5),
(10, 'ExternalLinks', 'External Links', 130, 0, 5),
(11, 'NotesAndDocuments', 'Documents and Notes', 120, 0, 5),
(12, 'Organizations', 'Organizations', 90, 1, 5),
(13, 'Contacts', 'Contacts', 80, 1, 5),
(14, 'DNRUplandRegions', 'DNR Upland Regions', 50, 1, 2),
(15, 'PriorityLandscapes', 'Priority Landscapes', 40, 1, 2),
(17, 'Treatments', 'Treatments', 70, 0, 2),
(18, 'Counties', 'Counties', 60, 1, 2)
) as Source (ProjectUpdateSectionID, ProjectUpdateSectionName, ProjectUpdateSectionDisplayName, SortOrder, HasCompletionStatus, ProjectWorkflowSectionGroupingID)
on Target.ProjectUpdateSectionID = Source.ProjectUpdateSectionID
when matched then
    update set
        ProjectUpdateSectionName = Source.ProjectUpdateSectionName,
        ProjectUpdateSectionDisplayName = Source.ProjectUpdateSectionDisplayName,
        SortOrder = Source.SortOrder,
        HasCompletionStatus = Source.HasCompletionStatus,
        ProjectWorkflowSectionGroupingID = Source.ProjectWorkflowSectionGroupingID
when not matched by target then
    insert (ProjectUpdateSectionID, ProjectUpdateSectionName, ProjectUpdateSectionDisplayName, SortOrder, HasCompletionStatus, ProjectWorkflowSectionGroupingID)
    values (ProjectUpdateSectionID, ProjectUpdateSectionName, ProjectUpdateSectionDisplayName, SortOrder, HasCompletionStatus, ProjectWorkflowSectionGroupingID)
when not matched by source then
    delete;
