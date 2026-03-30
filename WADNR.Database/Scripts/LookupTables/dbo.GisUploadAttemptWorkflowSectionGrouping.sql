merge into dbo.GisUploadAttemptWorkflowSectionGrouping as Target
using (values
(1, 'GeospatialValidation', 'Geospatial Validation', 10),
(2, 'MetadataMapping', 'Metadata Mapping', 20)
) as Source (GisUploadAttemptWorkflowSectionGroupingID, GisUploadAttemptWorkflowSectionGroupingName, GisUploadAttemptWorkflowSectionGroupingDisplayName, SortOrder)
on Target.GisUploadAttemptWorkflowSectionGroupingID = Source.GisUploadAttemptWorkflowSectionGroupingID
when matched then
    update set
        GisUploadAttemptWorkflowSectionGroupingName = Source.GisUploadAttemptWorkflowSectionGroupingName,
        GisUploadAttemptWorkflowSectionGroupingDisplayName = Source.GisUploadAttemptWorkflowSectionGroupingDisplayName,
        SortOrder = Source.SortOrder
when not matched by target then
    insert (GisUploadAttemptWorkflowSectionGroupingID, GisUploadAttemptWorkflowSectionGroupingName, GisUploadAttemptWorkflowSectionGroupingDisplayName, SortOrder)
    values (GisUploadAttemptWorkflowSectionGroupingID, GisUploadAttemptWorkflowSectionGroupingName, GisUploadAttemptWorkflowSectionGroupingDisplayName, SortOrder)
when not matched by source then
    delete;
