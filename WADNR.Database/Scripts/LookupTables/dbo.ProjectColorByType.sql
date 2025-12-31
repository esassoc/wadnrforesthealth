merge into dbo.ProjectColorByType as Target
using (values
(1, 'TaxonomyTrunk', 'Taxonomy Trunk', 'TaxonomyTrunkID', 10),
(2, 'ProjectStage', 'Stage', 'ProjectStageID', 20),
(3, 'TaxonomyBranch', 'Taxonomy Branch', 'TaxonomyBranchID', 11)
) as Source (ProjectColorByTypeID, ProjectColorByTypeName, ProjectColorByTypeDisplayName, ProjectColorByTypeNameWithIdentifier, SortOrder)
on Target.ProjectColorByTypeID = Source.ProjectColorByTypeID
when matched then
    update set
        ProjectColorByTypeName = Source.ProjectColorByTypeName,
        ProjectColorByTypeDisplayName = Source.ProjectColorByTypeDisplayName,
        ProjectColorByTypeNameWithIdentifier = Source.ProjectColorByTypeNameWithIdentifier,
        SortOrder = Source.SortOrder
when not matched by target then
    insert (ProjectColorByTypeID, ProjectColorByTypeName, ProjectColorByTypeDisplayName, ProjectColorByTypeNameWithIdentifier, SortOrder)
    values (ProjectColorByTypeID, ProjectColorByTypeName, ProjectColorByTypeDisplayName, ProjectColorByTypeNameWithIdentifier, SortOrder)
when not matched by source then
    delete;
