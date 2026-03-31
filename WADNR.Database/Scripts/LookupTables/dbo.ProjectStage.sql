merge into dbo.ProjectStage as Target
using (values
(2, '#80B2FF', 'Planned', 'Planned', 20),
(3, '#1975FF', 'Implementation', 'Implementation', 30),
(4, '#000066', 'Completed', 'Completed', 50),
(5, '#D6D6D6', 'Cancelled', 'Cancelled', 25)
) as Source (ProjectStageID, ProjectStageColor, ProjectStageName, ProjectStageDisplayName, SortOrder)
on Target.ProjectStageID = Source.ProjectStageID
when matched then
    update set
        ProjectStageColor = Source.ProjectStageColor,
        ProjectStageName = Source.ProjectStageName,
        ProjectStageDisplayName = Source.ProjectStageDisplayName,
        SortOrder = Source.SortOrder
when not matched by target then
    insert (ProjectStageID, ProjectStageColor, ProjectStageName, ProjectStageDisplayName, SortOrder)
    values (ProjectStageID, ProjectStageColor, ProjectStageName, ProjectStageDisplayName, SortOrder)
when not matched by source then
    delete;
