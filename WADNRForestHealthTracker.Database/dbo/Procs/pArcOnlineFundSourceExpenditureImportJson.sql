IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('dbo.pArcOnlineFundSourceExpenditureImportJson'))
    DROP PROCEDURE dbo.pArcOnlineFundSourceExpenditureImportJson
GO

CREATE PROCEDURE dbo.pArcOnlineFundSourceExpenditureImportJson
(
    @ArcOnlineFinanceApiRawJsonImportID int null,
    -- Calendar year of the data we are importing
    -- (Currently the JSON url we pull from is limited to year-by-year input)
    @BienniumToImport int null
)
AS
begin

/*

fields from api:
FTE_AMOUNT,TAR_HR_AMOUNT,BIENNIUM,FISCAL_MONTH,FISCAL_ADJUSTMENT_MONTH,CALENDAR_YEAR,MONTH_NAME,SOURCE_SYSTEM,DOCUMENT_NUMBER,DOCUMENT_SUFFIX,DOCUMENT_DATE,DOCUMENT_INVOICE_NUMBER,INVOICE_DESCRIPTION,INVOICE_DATE,INVOICE_NUMBER,GL_ACCOUNT_NUMBER,OBJECT_CODE,OBJECT_NAME,SUB_OBJECT_CODE,SUB_OBJECT_NAME,SUB_SUB_OBJECT_CODE,SUB_SUB_OBJECT_NAME,APPROPRIATION_CODE,APPROPRIATION_NAME,FUND_CODE,FUND_NAME,ORG_CODE,ORG_NAME,PROGRAM_INDEX_CODE,PROGRAM_INDEX_NAME,PROGRAM_CODE,PROGRAM_NAME,SUB_PROGRAM_CODE,SUB_PROGRAM_NAME,ACTIVITY_CODE,ACTIVITY_NAME,SUB_ACTIVITY_CODE,SUB_ACTIVITY_NAME,PROJECT_CODE,PROJECT_NAME,VENDOR_NUMBER,VENDOR_NAME,EXPENDITURE_ACCURED,ENCUMBRANCE

*/

    -- Create temp table from the bulk JSON field
    ---------------------------------------------

    insert into dbo.FundSourceAllocationExpenditureJsonStage
    ( Biennium, FiscalMo, FiscalAdjMo, CalYr, MoString, SourceSystem, DocNo, DocSuffix, DocDate, InvoiceDesc, InvoiceDate, GlAcctNo, ObjCd, ObjName, SubObjCd, SubObjName, SubSubObjCd, SubSubObjName, ApprnCd, ApprnName, FundCd, FundName, OrgCd, OrgName, ProgIdxCd, ProgIdxName, ProgCd, ProgName, SubProgCd, SubProgName, ActivityCd, ActivityName, SubActivityCd, SubActivityName, ProjectCd, ProjectName, VendorNo, VendorName, ExpendAccrued)
    SELECT  BIENNIUM, FISCAL_MONTH, FISCAL_ADJUSTMENT_MONTH, CALENDAR_YEAR, MONTH_NAME, SOURCE_SYSTEM, DOCUMENT_NUMBER, DOCUMENT_SUFFIX, DOCUMENT_DATE, INVOICE_DESCRIPTION, INVOICE_DATE, GL_ACCOUNT_NUMBER, OBJECT_CODE, [OBJECT_NAME], SUB_OBJECT_CODE, SUB_OBJECT_NAME, SUB_SUB_OBJECT_CODE, SUB_SUB_OBJECT_NAME, APPROPRIATION_CODE, APPROPRIATION_NAME, FUND_CODE, FUND_NAME, ORG_CODE, ORG_NAME, PROGRAM_INDEX_CODE, PROGRAM_INDEX_NAME, PROGRAM_CODE, [PROGRAM_NAME], SUB_PROGRAM_CODE, SUB_PROGRAM_NAME, ACTIVITY_CODE, ACTIVITY_NAME, SUB_ACTIVITY_CODE, SUB_ACTIVITY_NAME, PROJECT_CODE, PROJECT_NAME, VENDOR_NUMBER, VENDOR_NAME, EXPENDITURE_ACCURED
    from (select rji.RawJsonString from dbo.ArcOnlineFinanceApiRawJsonImport as rji where rji.ArcOnlineFinanceApiRawJsonImportID = @ArcOnlineFinanceApiRawJsonImportID) as j 
    CROSS APPLY OPENJSON(RawJsonString)
    WITH
    (
        BIENNIUM int,
        FISCAL_MONTH int,
        FISCAL_ADJUSTMENT_MONTH int,
        CALENDAR_YEAR int,
        MONTH_NAME varchar(20),
        SOURCE_SYSTEM varchar(50),
        DOCUMENT_NUMBER varchar(200),
        DOCUMENT_SUFFIX varchar(10),
        DOCUMENT_DATE datetime,
        INVOICE_DESCRIPTION varchar(max),
        INVOICE_DATE datetime,
        GL_ACCOUNT_NUMBER int,
        -- Object Code
        OBJECT_CODE varchar(20),
        [OBJECT_NAME] varchar(max),
        SUB_OBJECT_CODE varchar(20),
        SUB_OBJECT_NAME varchar(max),
        SUB_SUB_OBJECT_CODE varchar(20),
        SUB_SUB_OBJECT_NAME varchar(max),
        APPROPRIATION_CODE varchar(20),
        APPROPRIATION_NAME varchar(max),
        FUND_CODE varchar(20),
        FUND_NAME varchar(max),
        ORG_CODE varchar(20),
        ORG_NAME varchar(max),
        -- Program Index
        PROGRAM_INDEX_CODE varchar(20),
        PROGRAM_INDEX_NAME varchar(max),
        PROGRAM_CODE varchar(20),
        [PROGRAM_NAME] varchar(max),
        SUB_PROGRAM_CODE varchar(20),
        SUB_PROGRAM_NAME varchar(max),
        ACTIVITY_CODE varchar(20),
        ACTIVITY_NAME varchar(max),
        SUB_ACTIVITY_CODE varchar(20),
        SUB_ACTIVITY_NAME varchar(max),
        -- Project Code
        PROJECT_CODE varchar(20),
        PROJECT_NAME varchar(max),
        VENDOR_NUMBER varchar(100),
        VENDOR_NAME varchar(max),
        EXPENDITURE_ACCURED money
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
    tgp.BIENNIUM,
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
select * from dbo.ArcOnlineFinanceApiRawJsonImport

set statistics time on
exec pArcOnlineFundSourceExpenditureImportJson @ArcOnlineFinanceApiRawJsonImportID = 9, @BienniumToImport = 2009

exec pArcOnlineFundSourceExpenditureImportJson @ArcOnlineFinanceApiRawJsonImportID = 1308, @BienniumToImport = 2007

set statistics time off

*/