merge into dbo.ProjectLocationFilterType as Target
using (values
(1, 'TaxonomyTrunk', 'Taxonomy Trunk', 'TaxonomyTrunkID', 10, 1),
(2, 'TaxonomyBranch', 'Taxonomy Branch', 'TaxonomyBranchID', 20, 1),
(3, 'ProjectType', 'Project Type', 'ProjectTypeID', 30, 1),
(4, 'Classification', 'Classification', 'ClassificationID', 40, 3),
(5, 'ProjectStage', 'Project Stage', 'ProjectStageID', 50, 3),
(6, 'LeadImplementer', 'Lead Implementer', 'LeadImplementerID', 60, 4),
(7, 'Program', 'Program', 'ProgramID', 70, 4)
) as Source (ProjectLocationFilterTypeID, ProjectLocationFilterTypeName, ProjectLocationFilterTypeDisplayName, ProjectLocationFilterTypeNameWithIdentifier, SortOrder, DisplayGroup)
on Target.ProjectLocationFilterTypeID = Source.ProjectLocationFilterTypeID
when matched then
    update set
        ProjectLocationFilterTypeName = Source.ProjectLocationFilterTypeName,
        ProjectLocationFilterTypeDisplayName = Source.ProjectLocationFilterTypeDisplayName,
        ProjectLocationFilterTypeNameWithIdentifier = Source.ProjectLocationFilterTypeNameWithIdentifier,
        SortOrder = Source.SortOrder,
        DisplayGroup = Source.DisplayGroup
when not matched by target then
    insert (ProjectLocationFilterTypeID, ProjectLocationFilterTypeName, ProjectLocationFilterTypeDisplayName, ProjectLocationFilterTypeNameWithIdentifier, SortOrder, DisplayGroup)
    values (ProjectLocationFilterTypeID, ProjectLocationFilterTypeName, ProjectLocationFilterTypeDisplayName, ProjectLocationFilterTypeNameWithIdentifier, SortOrder, DisplayGroup)
when not matched by source then
    delete;
