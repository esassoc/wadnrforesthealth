merge into dbo.ProjectPersonRelationshipType as Target
using (values
(1, 'PrimaryContact', 'Primary Contact', 0, 275, 0, 10),
(2, 'PrivateLandowner', 'Private Landowner', 0, 273, 1, 30),
(3, 'Contractor', 'Contractor', 0, 272, 0, 20),
(4, 'ServiceForestryRegionalCoordinator', 'Service Forestry Regional Coordinator', 0, 507, 0, 40)
) as Source (ProjectPersonRelationshipTypeID, ProjectPersonRelationshipTypeName, ProjectPersonRelationshipTypeDisplayName, IsRequired, FieldDefinitionID, IsRestrictedToAdminAndProjectStewardAndCanViewLandownerInfo, SortOrder)
on Target.ProjectPersonRelationshipTypeID = Source.ProjectPersonRelationshipTypeID
when matched then
    update set
        ProjectPersonRelationshipTypeName = Source.ProjectPersonRelationshipTypeName,
        ProjectPersonRelationshipTypeDisplayName = Source.ProjectPersonRelationshipTypeDisplayName,
        IsRequired = Source.IsRequired,
        FieldDefinitionID = Source.FieldDefinitionID,
        IsRestrictedToAdminAndProjectStewardAndCanViewLandownerInfo = Source.IsRestrictedToAdminAndProjectStewardAndCanViewLandownerInfo,
        SortOrder = Source.SortOrder
when not matched by target then
    insert (ProjectPersonRelationshipTypeID, ProjectPersonRelationshipTypeName, ProjectPersonRelationshipTypeDisplayName, IsRequired, FieldDefinitionID, IsRestrictedToAdminAndProjectStewardAndCanViewLandownerInfo, SortOrder)
    values (ProjectPersonRelationshipTypeID, ProjectPersonRelationshipTypeName, ProjectPersonRelationshipTypeDisplayName, IsRequired, FieldDefinitionID, IsRestrictedToAdminAndProjectStewardAndCanViewLandownerInfo, SortOrder)
when not matched by source then
    delete;
