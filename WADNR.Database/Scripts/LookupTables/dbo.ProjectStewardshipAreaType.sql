merge into dbo.ProjectStewardshipAreaType as Target
using (values
(1,'ProjectStewardingOrganizations', 'Project Stewarding Organizations'),
(2,'TaxonomyBranches', 'Taxonomy Branches'),
(3,'Regions', 'Regions')
) as Source (ProjectStewardshipAreaTypeID, ProjectStewardshipAreaTypeName, ProjectStewardshipAreaTypeDisplayName)
on Target.ProjectStewardshipAreaTypeID = Source.ProjectStewardshipAreaTypeID
when matched then
    update set
        ProjectStewardshipAreaTypeName = Source.ProjectStewardshipAreaTypeName,
        ProjectStewardshipAreaTypeDisplayName = Source.ProjectStewardshipAreaTypeDisplayName
when not matched by target then
    insert (ProjectStewardshipAreaTypeID, ProjectStewardshipAreaTypeName, ProjectStewardshipAreaTypeDisplayName)
    values (ProjectStewardshipAreaTypeID, ProjectStewardshipAreaTypeName, ProjectStewardshipAreaTypeDisplayName)
when not matched by source then
    delete;  