
merge into dbo.TabularDataImportTableType as Target
using (values

           (1, 'LoaNortheast'),
           (2, 'LoaSoutheast')
)
    as Source (TabularDataImportTableTypeID, TabularDataImportTableTypeName)
on Target.TabularDataImportTableTypeID = Source.TabularDataImportTableTypeID
when matched then
    update set
        TabularDataImportTableTypeName = Source.TabularDataImportTableTypeName
when not matched by target then
    insert (TabularDataImportTableTypeID, TabularDataImportTableTypeName)
    values (TabularDataImportTableTypeID, TabularDataImportTableTypeName)
when not matched by source then
    delete;