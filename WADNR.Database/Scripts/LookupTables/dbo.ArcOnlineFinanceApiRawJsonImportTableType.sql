merge into dbo.ArcOnlineFinanceApiRawJsonImportTableType as Target
using (values
(1, 'Vendor'),
(2, 'ProgramIndex'),
(3, 'ProjectCode'),
(4, 'FundSourceExpenditure')
) as Source (ArcOnlineFinanceApiRawJsonImportTableTypeID, ArcOnlineFinanceApiRawJsonImportTableTypeName)
on Target.ArcOnlineFinanceApiRawJsonImportTableTypeID = Source.ArcOnlineFinanceApiRawJsonImportTableTypeID
when matched then
    update set
        ArcOnlineFinanceApiRawJsonImportTableTypeName = Source.ArcOnlineFinanceApiRawJsonImportTableTypeName
when not matched by target then
    insert (ArcOnlineFinanceApiRawJsonImportTableTypeID, ArcOnlineFinanceApiRawJsonImportTableTypeName)
    values (ArcOnlineFinanceApiRawJsonImportTableTypeID, ArcOnlineFinanceApiRawJsonImportTableTypeName)
when not matched by source then
    delete;


