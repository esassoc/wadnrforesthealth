merge into dbo.JsonImportStatusType as Target
using (values
(1, 'NotYetProcessed'),
(2, 'ProcessingFailed'),
(3, 'ProcessingSuceeded'),
(4, 'ProcessingIndeterminate')
) as Source (JsonImportStatusTypeID, JsonImportStatusTypeName)
on Target.JsonImportStatusTypeID = Source.JsonImportStatusTypeID
when matched then
    update set
        JsonImportStatusTypeName = Source.JsonImportStatusTypeName
when not matched by target then
    insert (JsonImportStatusTypeID, JsonImportStatusTypeName)
    values (JsonImportStatusTypeID, JsonImportStatusTypeName)
when not matched by source then
    delete;