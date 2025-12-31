merge into dbo.ProjectCostType as Target
using (values
(1, 'PreliminaryEngineering', 'Preliminary Engineering', 10),
(2, 'RightOfWay', 'Right of Way (aka Land Acquisition)', 20),
(3, 'Construction', 'Construction', 30)
) as Source (ProjectCostTypeID, ProjectCostTypeName, ProjectCostTypeDisplayName, SortOrder)
on Target.ProjectCostTypeID = Source.ProjectCostTypeID
when matched then
    update set
        ProjectCostTypeName = Source.ProjectCostTypeName,
        ProjectCostTypeDisplayName = Source.ProjectCostTypeDisplayName,
        SortOrder = Source.SortOrder
when not matched by target then
    insert (ProjectCostTypeID, ProjectCostTypeName, ProjectCostTypeDisplayName, SortOrder)
    values (ProjectCostTypeID, ProjectCostTypeName, ProjectCostTypeDisplayName, SortOrder)
when not matched by source then
    delete;
