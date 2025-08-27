IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('dbo.pArcOnlineVendorImportJson'))
	DROP PROCEDURE dbo.pArcOnlineVendorImportJson
GO

CREATE PROCEDURE dbo.pArcOnlineVendorImportJson
(
    @ArcOnlineFinanceApiRawJsonImportID int null
)
AS
begin

    SELECT vendorTemp.*
    into #vendorArcOnlineTemp
    --FROM OPENROWSET (BULK '{pathToVendorJsonFile}', SINGLE_CLOB) as j
    from (select rji.RawJsonString from dbo.ArcOnlineFinanceApiRawJsonImport as rji where rji.ArcOnlineFinanceApiRawJsonImportID = @ArcOnlineFinanceApiRawJsonImportID) as j 
    CROSS APPLY OPENJSON(RawJsonString)
    WITH
    (
	/*
	REMARKS: null
LAST_PROCESS_DATE: null
VENDOR_NUMBER:
VENDOR_NUMBER_SUFFIX:
VENDOR_NAME: null
ADDRESS_LINE1: null
ADDRESS_LINE2: null
ADDRESS_LINE3: null
CITY: null
STATE: null
ZIP_CODE: null
ZIP_PLUS_4: null
PHONE_NUMBER: null
VENDOR_STATUS: null
VENDOR_TYPE: null
BILLING_AGENCY: null
BILLING_SUBAGENCY: null
BILLING_FUND: null
BILLING_FUND_BREAKOUT: null
CCD_CTX_FLAG: null
EMAIL: null
	*/

        VENDOR_NUMBER nvarchar(256),
        VENDOR_NUMBER_SUFFIX nvarchar(256),
        VENDOR_NAME nvarchar(256),
        ADDRESS_LINE1 nvarchar(256),
        ADDRESS_LINE2 nvarchar(256),
        ADDRESS_LINE3 nvarchar(256),
        CITY nvarchar(256),
        STATE nvarchar(256),
        ZIP_CODE nvarchar(256),
        ZIP_PLUS_4 nvarchar(256),
        PHONE_NUMBER varchar(200),
        VENDOR_STATUS nvarchar(256),
        VENDOR_TYPE nvarchar(256),
        BILLING_AGENCY nvarchar(256),
        BILLING_SUBAGENCY nvarchar(256),
        BILLING_FUND nvarchar(256),
        BILLING_FUND_BREAKOUT nvarchar(256),
        CCD_CTX_FLAG nvarchar(256),
        --taxpayer_id_num varchar(200),
        EMAIL nvarchar(256),
        REMARKS nvarchar(256),
        LAST_PROCESS_DATE DateTime
    )
    AS vendorTemp

	select * from #vendorArcOnlineTemp

--create temp table to track which vendor records have a fk from another table
select distinct o.VendorID into #VendorFKs
from dbo.Organization o

insert into #VendorFKs
select distinct p.VendorId from Person p

insert into #VendorFKs
select distinct ipr.VendorID from InvoicePaymentRequest ipr

-- DELETE (2nd attempt)
-- Delete Vendors in our table not found in incoming temp table
delete from dbo.Vendor 
where VendorID in 
(
	select dbv.VendorID 
	from dbo.Vendor as dbv
	full outer join #vendorArcOnlineTemp as tv on tv.VENDOR_NUMBER = dbv.StatewideVendorNumber and tv.VENDOR_NUMBER_SUFFIX = dbv.StatewideVendorNumberSuffix
	where (tv.VENDOR_NUMBER is null or tv.VENDOR_NUMBER_SUFFIX is null) 
	and not exists (select 1 from #VendorFKs where dbv.VendorID = #VendorFKs.VendorID)
)

-- UPDATE (2nd attempt)
-- Update values for keys found in both sides
update dbv
set 
    VendorName = tv.VENDOR_NAME,
    VendorType = tv.VENDOR_TYPE,
    BillingAgency = tv.BILLING_AGENCY,
    BillingSubAgency = tv.BILLING_SUBAGENCY,
    BillingFund = tv.BILLING_FUND,
    VendorAddressLine1 = tv.ADDRESS_LINE1,
    VendorAddressLine2 = tv.ADDRESS_LINE2,
    VendorAddressLine3 = tv.ADDRESS_LINE3,
    VendorCity = tv.CITY,
    VendorState = tv.[STATE],
    VendorZip = tv.ZIP_CODE,
    Remarks = tv.REMARKS,
    VendorPhone = tv.PHONE_NUMBER,
    VendorStatus = tv.VENDOR_STATUS,
    --TaxpayerIdNumber = tv.taxpayer_id_num,
    Email = tv.EMAIL
from 
    dbo.Vendor as dbv
    join #vendorArcOnlineTemp as tv on tv.VENDOR_NUMBER = dbv.StatewideVendorNumber and tv.VENDOR_NUMBER_SUFFIX = dbv.StatewideVendorNumberSuffix
--    full outer join #vendorArcOnlineTemp as tv on tv.VENDOR_NUMBER = dbv.StatewideVendorNumber and tv.VENDOR_NUMBER_SUFFIX = dbv.StatewideVendorNumberSuffix
--where  dbv.StatewideVendorNumber is not null and dbv.StatewideVendorNumberSuffix is not null
--       and
--       tv.VENDOR_NUMBER is not null and tv.VENDOR_NUMBER_SUFFIX is not null
    where 
    (
        isnull(dbv.VendorName, '') != isnull(tv.VENDOR_NAME, '') or
        isnull(dbv.VendorType, '') != isnull(tv.VENDOR_TYPE, '') or
        isnull(dbv.BillingAgency, '') != isnull(tv.BILLING_AGENCY, '') or
        isnull(dbv.BillingSubAgency, '') != isnull(tv.BILLING_SUBAGENCY, '') or
        isnull(dbv.BillingFund, '') != isnull(tv.BILLING_FUND, '') or
        isnull(dbv.VendorAddressLine1, '') != isnull(tv.ADDRESS_LINE1, '') or
        isnull(dbv.VendorAddressLine2, '') != isnull(tv.ADDRESS_LINE2, '') or
        isnull(dbv.VendorAddressLine3, '') != isnull(tv.ADDRESS_LINE3, '') or
        isnull(dbv.VendorCity, '') != isnull(tv.CITY, '') or
        isnull(dbv.VendorState, '') != isnull(tv.[STATE], '') or
        isnull(dbv.VendorZip, '') != isnull(tv.ZIP_CODE, '') or
        isnull(dbv.Remarks, '') != isnull(tv.REMARKS, '') or
        isnull(dbv.VendorPhone, '') != isnull(tv.PHONE_NUMBER, '') or
        isnull(dbv.VendorStatus, '') != isnull(tv.VENDOR_STATUS, '') or
        --isnull(dbv.TaxpayerIdNumber, '') != isnull(tv.taxpayer_id_num, '') or
        isnull(dbv.Email, '') != isnull(tv.EMAIL, '')
    )


-- INSERT (2nd attempt)
-- Insert values not already found in our table
insert into dbo.Vendor (VendorName,
                        StatewideVendorNumber,
                        StatewideVendorNumberSuffix,
                        VendorType,
                        BillingAgency,
                        BillingSubAgency,
                        BillingFund,
                        BillingFundBreakout,
                        VendorAddressLine1,
                        VendorAddressLine2,
                        VendorAddressLine3,
                        VendorCity,
                        VendorState,
                        VendorZip,
                        Remarks,
                        VendorPhone,
                        VendorStatus,
                        --TaxpayerIdNumber,
                        Email)
select tv.VENDOR_NAME,
       tv.VENDOR_NUMBER,
       tv.VENDOR_NUMBER_SUFFIX,
       tv.VENDOR_TYPE,
       tv.BILLING_AGENCY,
       tv.BILLING_SUBAGENCY,
       tv.BILLING_FUND,
       tv.BILLING_FUND_BREAKOUT,
       tv.ADDRESS_LINE1,
       tv.ADDRESS_LINE2,
       tv.ADDRESS_LINE3,
       tv.CITY,
       tv.[STATE],
       tv.ZIP_CODE,
       tv.REMARKS,
       tv.PHONE_NUMBER,
       tv.VENDOR_STATUS,
       --tv.taxpayer_id_num,
       tv.EMAIL
from dbo.Vendor as dbv
full outer join #vendorArcOnlineTemp as tv on tv.VENDOR_NUMBER = dbv.StatewideVendorNumber and tv.VENDOR_NUMBER_SUFFIX = dbv.StatewideVendorNumberSuffix
where  (dbv.StatewideVendorNumber is null and dbv.StatewideVendorNumberSuffix is null)
       and
       (tv.VENDOR_NUMBER is not null and tv.VENDOR_NUMBER_SUFFIX is not null)
	   


end
go



/*


select * from  dbo.ArcOnlineFinanceApiRawJsonImportTableType where ArcOnlineFinanceApiRawJsonImportTableTypeID = 1

delete from dbo.Vendor

set statistics time on
exec pArcOnlineVendorImportJson @ArcOnlineFinanceApiRawJsonImportID = 1
set statistics time off


select * from dbo.ArcOnlineFinanceApiRawJsonImport

@ArcOnlineFinanceApiRawJsonImportID


*/
