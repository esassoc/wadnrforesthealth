merge into dbo.GisUploadAttemptWorkflowSection as Target
using (values
(2, 'UploadGisFile', 'Upload GIS File', 20, 1, 1),
(3, 'ValidateFeatures', 'Validate Features', 30, 1, 1),
(4, 'ValidateMetadata', 'Validate Metadata', 40, 1, 2),
(6, 'ReviewMapping', 'Review Mapping', 60, 1, 2),
(7, 'RviewStagedImport', 'Review Staged Import', 70, 1, 2)
) as Source (GisUploadAttemptWorkflowSectionID, GisUploadAttemptWorkflowSectionName, GisUploadAttemptWorkflowSectionDisplayName, SortOrder, HasCompletionStatus, GisUploadAttemptWorkflowSectionGroupingID)
on Target.GisUploadAttemptWorkflowSectionID = Source.GisUploadAttemptWorkflowSectionID
when matched then
    update set
        GisUploadAttemptWorkflowSectionName = Source.GisUploadAttemptWorkflowSectionName,
        GisUploadAttemptWorkflowSectionDisplayName = Source.GisUploadAttemptWorkflowSectionDisplayName,
        SortOrder = Source.SortOrder,
        HasCompletionStatus = Source.HasCompletionStatus,
        GisUploadAttemptWorkflowSectionGroupingID = Source.GisUploadAttemptWorkflowSectionGroupingID
when not matched by target then
    insert (GisUploadAttemptWorkflowSectionID, GisUploadAttemptWorkflowSectionName, GisUploadAttemptWorkflowSectionDisplayName, SortOrder, HasCompletionStatus, GisUploadAttemptWorkflowSectionGroupingID)
    values (GisUploadAttemptWorkflowSectionID, GisUploadAttemptWorkflowSectionName, GisUploadAttemptWorkflowSectionDisplayName, SortOrder, HasCompletionStatus, GisUploadAttemptWorkflowSectionGroupingID)
when not matched by source then
    delete;