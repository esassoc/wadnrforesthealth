
merge into dbo.ProjectWorkflowSectionGrouping as Target
using (values

           (1, 'Overview', 'Overview', 10),
           (2, 'Location', 'Location', 20),
           (4, 'Expenditures', 'Expenditures', 40),
           (5, 'AdditionalData', 'Additional Data', 50),
           (6, 'ProjectSetup', 'Project Setup', 15)
)
    as Source (ProjectWorkflowSectionGroupingID, ProjectWorkflowSectionGroupingName, ProjectWorkflowSectionGroupingDisplayName, SortOrder)
on Target.ProjectWorkflowSectionGroupingID = Source.ProjectWorkflowSectionGroupingID
when matched then
    update set
               ProjectWorkflowSectionGroupingName = Source.ProjectWorkflowSectionGroupingName,
               ProjectWorkflowSectionGroupingDisplayName = Source.ProjectWorkflowSectionGroupingDisplayName,
               SortOrder = Source.SortOrder
when not matched by target then
    insert (ProjectWorkflowSectionGroupingID, ProjectWorkflowSectionGroupingName, ProjectWorkflowSectionGroupingDisplayName, SortOrder)
    values (ProjectWorkflowSectionGroupingID, ProjectWorkflowSectionGroupingName, ProjectWorkflowSectionGroupingDisplayName, SortOrder)
when not matched by source then
    delete;