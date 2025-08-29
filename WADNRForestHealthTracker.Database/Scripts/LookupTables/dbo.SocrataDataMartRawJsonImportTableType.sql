
merge into dbo.SocrataDataMartRawJsonImportTableType as Target
using (values

           (1, 'Vendor'),
           (2, 'ProgramIndex'),
           (3, 'ProjectCode'),
           (4, 'FundSourceExpenditure')
)
    as Source (SocrataDataMartRawJsonImportTableTypeID, SocrataDataMartRawJsonImportTableTypeName)
on Target.SocrataDataMartRawJsonImportTableTypeID = Source.SocrataDataMartRawJsonImportTableTypeID
when matched then
    update set
               SocrataDataMartRawJsonImportTableTypeName = Source.SocrataDataMartRawJsonImportTableTypeName
when not matched by target then
    insert (SocrataDataMartRawJsonImportTableTypeID, SocrataDataMartRawJsonImportTableTypeName)
    values (SocrataDataMartRawJsonImportTableTypeID, SocrataDataMartRawJsonImportTableTypeName)
when not matched by source then
    delete;