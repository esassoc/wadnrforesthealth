IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('dbo.pProjectCodeImportJson'))
    DROP PROCEDURE dbo.pProjectCodeImportJson
GO

CREATE PROCEDURE dbo.pProjectCodeImportJson
(
    @SocrataDataMartRawJsonImportID int null
)
AS
begin

/*

JSON format:

,{"create_date":"2010-05-10T00:00:00.000","project_code":"08AY","project_end_date":"2025-09-15T00:00:00.000","project_start_date":"2010-07-01T00:00:00.000","title":"86A IT MAINTENANCE"}

*/

    -- Create temp table from the bulk JSON field
    ---------------------------------------------

    SELECT ProjectCodeTemp.*
    into #ProjectCodeSocrataTemp
    --FROM OPENROWSET (BULK '{pathToVendorJsonFile}', SINGLE_CLOB) as j
    from (select rji.RawJsonString from dbo.SocrataDataMartRawJsonImport as rji where rji.SocrataDataMartRawJsonImportID = @SocrataDataMartRawJsonImportID) as j 
    CROSS APPLY OPENJSON(RawJsonString)
    WITH
    (
        create_date datetime,
        project_code varchar(200),
        project_start_date datetime,
        project_end_date datetime,
        title nvarchar(256)
    )
    AS ProjectCodeTemp


-- Remove leading zeros from incoming ProjectCodes before we start comparing, since we store them in the WADNR tables without leading zeroes.
update #ProjectCodeSocrataTemp
set project_code = dbo.fRemoveLeadingZeroes(project_code)

-- DELETE
-- Delete ProjectCodes in our table not found in incoming temp table
delete from dbo.ProjectCode 
where ProjectCodeID in 
(
    select dbpc.ProjectCodeID
    from dbo.ProjectCode as dbpc
    full outer join #ProjectCodeSocrataTemp as tpc on tpc.project_code = dbpc.ProjectCodeName
    where (tpc.project_code is null)
)



-- UPDATE
-- Update values for keys found in both sides
update dbpc
set 
    ProjectCodeTitle = tpc.title,
    CreateDate = tpc.create_date,
    ProjectStartDate = tpc.project_start_date,
    ProjectEndDate = tpc.project_end_date
from 
    dbo.ProjectCode as dbpc
    join #ProjectCodeSocrataTemp as tpc on tpc.project_code = dbpc.ProjectCodeName
    where 
    (
        isnull(dbpc.ProjectCodeTitle, '') != isnull(tpc.title, '') or
        isnull(dbpc.CreateDate, '') != isnull(tpc.create_date, '') or
        isnull(dbpc.ProjectStartDate, '') != isnull(tpc.project_start_date, '') or
        isnull(dbpc.ProjectEndDate, '') != isnull(tpc.project_end_date, '')
    )


-- INSERT 
-- Insert values not already found in our table
insert into dbo.ProjectCode (ProjectCodeName,
                             ProjectCodeTitle,
                             CreateDate,
                             ProjectStartDate,
                             ProjectEndDate)
select tpc.project_code,
       tpc.title,
       tpc.create_date,
       tpc.project_start_date,
       tpc.project_end_date
from dbo.ProjectCode as dbpc
full outer join #ProjectCodeSocrataTemp as tpc on tpc.project_code = dbpc.ProjectCodeName
where  (dbpc.ProjectCodeName is null)


end
go



/*
select * from dbo.SocrataDataMartRawJsonImport

set statistics time on

exec pProjectCodeImportJson @SocrataDataMartRawJsonImportID = 1

set statistics time off

*/