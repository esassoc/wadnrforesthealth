IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('dbo.pArcOnlineProjectCodeImportJson'))
    DROP PROCEDURE dbo.pArcOnlineProjectCodeImportJson
GO

CREATE PROCEDURE dbo.pArcOnlineProjectCodeImportJson
(
    @ArcOnlineFinanceApiRawJsonImportID int null
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
    into #ProjectCodeArcOnlineTemp
    --FROM OPENROWSET (BULK '{pathToVendorJsonFile}', SINGLE_CLOB) as j
    from (select rji.RawJsonString from dbo.ArcOnlineFinanceApiRawJsonImport as rji where rji.ArcOnlineFinanceApiRawJsonImportID = @ArcOnlineFinanceApiRawJsonImportID) as j 
    CROSS APPLY OPENJSON(RawJsonString)
    WITH
    (
        CREATE_DATE datetime,
        PROJECT_CODE varchar(200),
        PROJECT_START_DATE datetime,
        PROJECT_END_DATE datetime,
        TITLE nvarchar(256)
    )
    AS ProjectCodeTemp


-- Remove leading zeros from incoming ProjectCodes before we start comparing, since we store them in the WADNR tables without leading zeroes.
update #ProjectCodeArcOnlineTemp
set PROJECT_CODE = dbo.fRemoveLeadingZeroes(PROJECT_CODE)

-- DELETE
-- Delete ProjectCodes in our table not found in incoming temp table
delete from dbo.ProjectCode 
where ProjectCodeID in 
(
    select dbpc.ProjectCodeID
    from dbo.ProjectCode as dbpc
    full outer join #ProjectCodeArcOnlineTemp as tpc on tpc.PROJECT_CODE = dbpc.ProjectCodeName
    where (tpc.PROJECT_CODE is null)
)



-- UPDATE
-- Update values for keys found in both sides
update dbpc
set 
    ProjectCodeTitle = tpc.TITLE,
    CreateDate = tpc.CREATE_DATE,
    ProjectStartDate = tpc.PROJECT_START_DATE,
    ProjectEndDate = tpc.PROJECT_END_DATE
from 
    dbo.ProjectCode as dbpc
    join #ProjectCodeArcOnlineTemp as tpc on tpc.PROJECT_CODE = dbpc.ProjectCodeName
    where 
    (
        isnull(dbpc.ProjectCodeTitle, '') != isnull(tpc.TITLE, '') or
        isnull(dbpc.CreateDate, '') != isnull(tpc.CREATE_DATE, '') or
        isnull(dbpc.ProjectStartDate, '') != isnull(tpc.PROJECT_START_DATE, '') or
        isnull(dbpc.ProjectEndDate, '') != isnull(tpc.PROJECT_END_DATE, '')
    )


-- INSERT 
-- Insert values not already found in our table
insert into dbo.ProjectCode (ProjectCodeName,
                             ProjectCodeTitle,
                             CreateDate,
                             ProjectStartDate,
                             ProjectEndDate)
select tpc.PROJECT_CODE,
       tpc.TITLE,
       tpc.CREATE_DATE,
       tpc.PROJECT_START_DATE,
       tpc.PROJECT_END_DATE
from dbo.ProjectCode as dbpc
full outer join #ProjectCodeArcOnlineTemp as tpc on tpc.PROJECT_CODE = dbpc.ProjectCodeName
where  (dbpc.ProjectCodeName is null)


end
go



/*
select * from dbo.ArcOnlineFinanceApiRawJsonImport

select * from dbo.JsonImportStatusType

set statistics time on

exec pProjectCodeImportJson @ArcOnlineFinanceApiRawJsonImportID = 1

set statistics time off

*/