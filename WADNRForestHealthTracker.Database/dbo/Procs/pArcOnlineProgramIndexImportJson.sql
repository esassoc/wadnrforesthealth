IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('dbo.pArcOnlineProgramIndexImportJson'))
    DROP PROCEDURE dbo.pArcOnlineProgramIndexImportJson
GO

CREATE PROCEDURE dbo.pArcOnlineProgramIndexImportJson
(
    @ArcOnlineFinanceApiRawJsonImportID int null
)
AS
begin

/*

fields in ArcOnline
ACTIVITY_CODE,SUB_ACTIVITY_CODE,BIENNIUM,PROGRAM_INDEX_CODE,TITLE,PROGRAM_CODE,SUB_PROGRAM_CODE

*/

    -- Create temp table from the bulk JSON field
    ---------------------------------------------

    SELECT programIndexTemp.*
    into #programIndexArcOnlineTemp
    --FROM OPENROWSET (BULK '{pathToVendorJsonFile}', SINGLE_CLOB) as j
    from (select rji.RawJsonString from dbo.ArcOnlineFinanceApiRawJsonImport as rji where rji.ArcOnlineFinanceApiRawJsonImportID = @ArcOnlineFinanceApiRawJsonImportID) as j 
    CROSS APPLY OPENJSON(RawJsonString)
    WITH
    (
        ACTIVITY_CODE nvarchar(256),
        BIENNIUM int,
        PROGRAM nvarchar(256),
        PROGRAM_INDEX_CODE nvarchar(256),
        SUB_ACTIVITY_CODE nvarchar(256),
        SUB_PROGRAM_CODE nvarchar(256),
        TITLE nvarchar(256)
    )
    AS programIndexTemp


-- Make sure we found something to import before going any further
------------------------------------------------------------------

declare @socrataTempRowCount as bigint
set @socrataTempRowCount= (select count(*) from #programIndexArcOnlineTemp)
if @socrataTempRowCount = 0
begin
    RAISERROR ('No rows in incoming Program Index temp table #programIndexArcOnlineTemp', 16, 1)
    rollback tran
    return
end


-- Current and Previous Bienniums
---------------------------------
DROP TABLE IF EXISTS #CurrentAndNextBiennium
-- Current BIENNIUM
select dbo.fGetCurrentFiscalYearBiennium() as BienniumYear into #CurrentAndNextBiennium
-- Next BIENNIUM
insert into #CurrentAndNextBiennium (BienniumYear) values(dbo.fGetCurrentFiscalYearBiennium() + 2)

-- Bienniums found in Incoming data
-----------------------------------
DROP TABLE IF EXISTS #BienniumsFoundInIncomingData;
select distinct pist.BIENNIUM as BienniumYear
into #BienniumsFoundInIncomingData
from #programIndexArcOnlineTemp as pist

--select * from #CurrentAndNextBiennium
--select * from #BienniumsFoundInIncomingData

-- Make sure expected and incoming Bienniums overlap exactly.
-- If they do not, we likely have a problem and should stop.
--------------------------------------------------------------

-- 7/7/21 TK & JV - Commenting out this check for now because the feed is sending 2019 and 2021 data right now and the code is looking for 2021 and 2023. 
--		We are adding a new check to make sure the feed is not ahead of our code, which would cause useful data to be deleted.

--DECLARE @BIENNIUMsOverlapExactly bit
--set @BIENNIUMsOverlapExactly =
--(
--  CASE 
--    WHEN EXISTS (
--      SELECT * FROM #CurrentAndNextBiennium as cpb 
--      FULL JOIN #BienniumsFoundInIncomingData as bfid ON cpb.BienniumYear = bfid.BienniumYear
--      WHERE cpb.BienniumYear IS NULL OR bfid.BienniumYear IS NULL
--      )
--    -- Elements in tables are not equal'
--    THEN 0
--    -- Elements in tables are equal
--    ELSE 1
--  END
--)

--if @BIENNIUMsOverlapExactly = 0
--begin
--    RAISERROR ('Incoming Bienniums do not match expected Bienniums!', 16, 1)
--    rollback tran
--    return
--end


-- Check that the feed BIENNIUMs are not ahead of our codes expected BIENNIUMs
------------------------------------------------------------------------------
DECLARE @BIENNIUMsAreAheadOfExpectedBienniums bit
set @BIENNIUMsAreAheadOfExpectedBienniums =
(
  CASE 
    WHEN EXISTS (
      SELECT * FROM #CurrentAndNextBiennium as cpb
	  join dbo.ProgramIndex as pix on cpb.BienniumYear = pix.Biennium
	  left join #BienniumsFoundInIncomingData as bfid on cpb.BienniumYear = bfid.BienniumYear
      WHERE bfid.BienniumYear IS NULL
      )
    -- We have existing data for BIENNIUMs that is not in current or next and could be removed in the upcoming delete
    THEN 1
    -- BIENNIUMs are good
    ELSE 0
  END
)

if @BIENNIUMsAreAheadOfExpectedBienniums = 1
begin
    RAISERROR ('Feed Bienniums are ahead of expected Bienniums!', 16, 1)
    rollback tran
    return
end



-- Get started doing import
----------------------------

-- Remove leading zeros from incoming ProgramIndexCodes before we start comparing, since we store them in the WADNR tables without leading zeroes.
update #programIndexArcOnlineTemp
set PROGRAM_INDEX_CODE = dbo.fRemoveLeadingZeroes(PROGRAM_INDEX_CODE)

-- DELETE
-- Delete ProgramIndexes in our table not found in incoming temp table
delete from dbo.ProgramIndex 
where ProgramIndexID in 
(
    select dbpi.ProgramIndexID
    from dbo.ProgramIndex as dbpi
    full outer join #programIndexArcOnlineTemp as tpi on 
            tpi.PROGRAM_INDEX_CODE = dbpi.ProgramIndexCode 
            and 
            tpi.BIENNIUM = dbpi.Biennium
    where 
    (tpi.PROGRAM_INDEX_CODE is null and tpi.BIENNIUM is null)
    and 
    -- Ignore fake code on the Sitka side. 
    -- (Eventually this needs to go away, but not immediately.)
    (dbpi.ProgramIndexCode != '000' and dbpi.ProgramIndexTitle not like '%FAKE%')
)
and
-- Only delete if we are dealing with current or next BIENNIUM
Biennium in (select BienniumYear from #CurrentAndNextBiennium)



-- UPDATE
-- Update values for keys found in both sides
update dbpi
set 
    Activity = tpi.ACTIVITY_CODE,
    --Biennium = tpi.BIENNIUM,
    Program = tpi.PROGRAM,
    Subactivity = tpi.SUB_ACTIVITY_CODE,
    Subprogram = tpi.SUB_PROGRAM_CODE,
    ProgramIndexTitle = tpi.TITLE
from 
    dbo.ProgramIndex as dbpi
    join #programIndexArcOnlineTemp as tpi on tpi.PROGRAM_INDEX_CODE = dbpi.ProgramIndexCode and tpi.BIENNIUM = dbpi.Biennium
    where 
    (
        isnull(dbpi.Activity, '') != isnull(tpi.ACTIVITY_CODE, '') or
        --isnull(dbpi.Biennium, '') != isnull(tpi.BIENNIUM, '') or
        isnull(dbpi.Program, '') != isnull(tpi.PROGRAM, '') or
        isnull(dbpi.Subactivity, '') != isnull(tpi.SUB_ACTIVITY_CODE, '') or
        isnull(dbpi.Subprogram, '') != isnull(tpi.SUB_PROGRAM_CODE, '') or
        isnull(dbpi.ProgramIndexTitle, '') != isnull(tpi.TITLE, '')
    )

-- INSERT 
-- Insert values not already found in our table
insert into dbo.ProgramIndex (ProgramIndexCode,
                              ProgramIndexTitle,
                              Activity,
                              Biennium,
                              Program,
                              Subprogram,
                              Subactivity)
select tpi.PROGRAM_INDEX_CODE,
       tpi.TITLE,
       tpi.ACTIVITY_CODE,
       tpi.BIENNIUM,
       tpi.PROGRAM,
       tpi.SUB_PROGRAM_CODE,
       tpi.SUB_ACTIVITY_CODE
from dbo.ProgramIndex as dbpi
full outer join #programIndexArcOnlineTemp as tpi on tpi.PROGRAM_INDEX_CODE = dbpi.ProgramIndexCode and tpi.BIENNIUM = dbpi.Biennium
where  (dbpi.ProgramIndexCode is null and dbpi.Biennium is null)
       and
       (tpi.PROGRAM_INDEX_CODE is not null and tpi.BIENNIUM is not null)

end
go



/*
select * from dbo.ArcOnlineFinanceApiRawJsonImport
select * from dbo.ProgramIndex

set statistics time on

exec pProgramIndexImportJson @ArcOnlineFinanceApiRawJsonImportID = 49

exec pProgramIndexImportJson @ArcOnlineFinanceApiRawJsonImportID = 10564

set statistics time off

*/