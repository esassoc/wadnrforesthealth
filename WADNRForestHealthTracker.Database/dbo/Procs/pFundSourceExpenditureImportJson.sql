IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('dbo.pFundSourceExpenditureImportJson'))
    DROP PROCEDURE dbo.pFundSourceExpenditureImportJson
GO

CREATE PROCEDURE dbo.pFundSourceExpenditureImportJson
(
    @SocrataDataMartRawJsonImportID int null,
    -- Calendar year of the data we are importing
    -- (Currently the JSON url we pull from is limited to year-by-year input)
    @BienniumToImport int null
)
AS
begin

/*

JSON format:
{
\"Biennium\":\"2019\",
\"FiscalMo\":\"22\",
\"FiscalAdjMo\":\"00\",
\"CalYr\":2019,\
"MoString\":\"April\",
\"SourceSystem\":\"FES\",
\"DocNo\":\"11490119\",
\"DocSuffix\":\"FM\",
\"DocDate\":\"2019-04-16T00:00:00\",
\"InvoiceDesc\":\"- WATECH INVOICE 03/2019\",
\"InvoiceDate\":\"2019-04-01T00:00:00\",
\"InvoiceNo\":\"90112019030311\",
\"GlAcctNo\":\"6505\",
\"ObjCd\":\"E\",
\"ObjName\":\"GOODS AND SERVICES\",
\"SubObjCd\":\"EB\",
\"SubObjName\":\"COMMUNICATIONS/TELECOMMUNICATIONS\",
\"SubSubObjCd\":\"B030\",
\"SubSubObjName\":\"STATE PROVIDED TELECOMMUNICATION SERVICE\",
\"ApprnCd\":\"965\",
\"ApprnName\":\"SALARIES AND EXPENSES\",
\"FundCd\":\"190\",
\"FundName\":\"FOREST FIRE PROTECTION ASSESSMENT\",
\"OrgCd\":\"2300\",
\"OrgName\":\"NORTHEAST REGION\",
\"ProgIdxCd\":\"00214\",
\"ProgIdxName\":\"214-PREPAREDNESS\",
\"ProgCd\":\"020\",
\"ProgName\":\"RESOURCE PROTECTION AND REGULATION\",
\"SubProgCd\":\"01\",
\"SubProgName\":\"WILDFIRE DIVISION\",
\"ActivityCd\":\"01\",
\"ActivityName\":\"FIRE ADMINISTRATION AND PREPAREDNESS\",
\"SubActivityCd\":\"02\",
\"SubActivityName\":\"214-PREPAREDNESS\",
\"ProjectCd\":\"0KPV\",
\"ProjectName\":\"NEWICC\",
\"VendorNo\":\"SWV0098113\",
\"VendorName\":\"CTS SERVICES\",
\"ExpendAccrued\":446.98
},

*/

    -- Create temp table from the bulk JSON field
    ---------------------------------------------

    insert into dbo.FundSourceAllocationExpenditureJsonStage
    ( Biennium, FiscalMo, FiscalAdjMo, CalYr, MoString, SourceSystem, DocNo, DocSuffix, DocDate, InvoiceDesc, InvoiceDate, GlAcctNo, ObjCd, ObjName, SubObjCd, SubObjName, SubSubObjCd, SubSubObjName, ApprnCd, ApprnName, FundCd, FundName, OrgCd, OrgName, ProgIdxCd, ProgIdxName, ProgCd, ProgName, SubProgCd, SubProgName, ActivityCd, ActivityName, SubActivityCd, SubActivityName, ProjectCd, ProjectName, VendorNo, VendorName, ExpendAccrued)
    SELECT  Biennium, FiscalMo, FiscalAdjMo, CalYr, MoString, SourceSystem, DocNo, DocSuffix, DocDate, InvoiceDesc, InvoiceDate, GlAcctNo, ObjCd, ObjName, SubObjCd, SubObjName, SubSubObjCd, SubSubObjName, ApprnCd, ApprnName, FundCd, FundName, OrgCd, OrgName, ProgIdxCd, ProgIdxName, ProgCd, ProgName, SubProgCd, SubProgName, ActivityCd, ActivityName, SubActivityCd, SubActivityName, ProjectCd, ProjectName, VendorNo, VendorName, ExpendAccrued
    from (select rji.RawJsonString from dbo.SocrataDataMartRawJsonImport as rji where rji.SocrataDataMartRawJsonImportID = @SocrataDataMartRawJsonImportID) as j 
    CROSS APPLY OPENJSON(RawJsonString)
    WITH
    (
        Biennium int,
        FiscalMo int,
        FiscalAdjMo int,
        CalYr int,
        MoString varchar(20),
        SourceSystem varchar(50),
        DocNo varchar(200),
        DocSuffix varchar(10),
        DocDate datetime,
        InvoiceDesc varchar(max),
        InvoiceDate datetime,
        GlAcctNo int,
        -- Object Code
        ObjCd varchar(20),
        ObjName varchar(max),
        SubObjCd varchar(20),
        SubObjName varchar(max),
        SubSubObjCd varchar(20),
        SubSubObjName varchar(max),
        ApprnCd varchar(20),
        ApprnName varchar(max),
        FundCd varchar(20),
        FundName varchar(max),
        OrgCd varchar(20),
        OrgName varchar(max),
        -- Program Index
        ProgIdxCd varchar(20),
        ProgIdxName varchar(max),
        ProgCd varchar(20),
        ProgName varchar(max),
        SubProgCd varchar(20),
        SubProgName varchar(max),
        ActivityCd varchar(20),
        ActivityName varchar(max),
        SubActivityCd varchar(20),
        SubActivityName varchar(max),
        -- Project Code
        ProjectCd varchar(20),
        ProjectName varchar(max),
        VendorNo varchar(100),
        VendorName varchar(max),
        ExpendAccrued money
    )
    AS FundSourceExpenditureTemp
    -- Enforce the year limit from here on in the sproc
    where FundSourceExpenditureTemp.Biennium = @BienniumToImport or @BienniumToImport is null


-- Insert incoming FundSourceExpenditures into FundSourceAllocationExpenditure
-- ==================================================================
insert into dbo.FundSourceAllocationExpenditure
    (
        FundSourceAllocationID,
        CostTypeID,
        Biennium,
        FiscalMonth,
        CalendarYear,
        CalendarMonth,
        ExpenditureAmount
    )
select
    gapc.FundSourceAllocationID,
    ctdm.CostTypeID as CostTypeID,
    tgp.Biennium,
    tgp.FiscalMo,
    tgp.CalYr as CalendarYear,
    (select dbo.fGetCalendarMonthIndexFromMonthString(tgp.MoString)) as CalendarMonth,
    tgp.ExpendAccrued
from dbo.FundSourceAllocationProgramIndexProjectCode as gapc
inner join ProgramIndex as pin on gapc.ProgramIndexID = pin.ProgramIndexID
inner join ProjectCode as pc on gapc.ProjectCodeID = pc.ProjectCodeID
inner join dbo.FundSourceAllocationExpenditureJsonStage as tgp
        on 
        dbo.fRemoveLeadingZeroes(tgp.ProjectCd) = pc.ProjectCodeName 
        and 
        dbo.fRemoveLeadingZeroes(tgp.ProgIdxCd) = pin.ProgramIndexCode
inner join dbo.CostTypeDatamartMapping as ctdm on ctdm.DatamartObjectCode = tgp.ObjCd
                                                  and
                                                  ctdm.DatamartSubObjectCode = tgp.SubObjCd
                                                  and
                                                  ctdm.DatamartObjectName = tgp.ObjName
                                                  and 
                                                  ctdm.DatamartSubObjectName = tgp.SubObjName
where tgp.Biennium = @BienniumToImport or @BienniumToImport is null
order by CalendarYear, CalendarMonth, CostTypeID, Biennium, FiscalMo, FiscalAdjMo, MoString, SourceSystem, DocNo, DocSuffix, DocDate, InvoiceDesc, InvoiceDate, GlAcctNo, ObjCd, ObjName, SubObjCd, SubObjName, SubSubObjCd, SubSubObjName, ApprnCd, ApprnName, FundCd, FundName, OrgCd, OrgName, ProgIdxCd, ProgIdxName, ProgCd, ProgName, SubProgCd, SubProgName, ActivityCd, ActivityName, SubActivityCd, SubActivityName, ProjectCd, ProjectName, VendorNo, VendorName, ExpendAccrued

end
go



/*
select * from dbo.SocrataDataMartRawJsonImport

set statistics time on
exec pFundSourceExpenditureImportJson @SocrataDataMartRawJsonImportID = 9, @BienniumToImport = 2009

exec pFundSourceExpenditureImportJson @SocrataDataMartRawJsonImportID = 1308, @BienniumToImport = 2007

set statistics time off

*/