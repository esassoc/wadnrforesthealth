IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('dbo.pVendorImportJson'))
	DROP PROCEDURE dbo.pVendorImportJson
GO

CREATE PROCEDURE dbo.pVendorImportJson
(
    @SocrataDataMartRawJsonImportID int null
)
AS
begin

    SELECT vendorTemp.*
    into #vendorSocrataTemp
    --FROM OPENROWSET (BULK '{pathToVendorJsonFile}', SINGLE_CLOB) as j
    from (select rji.RawJsonString from dbo.SocrataDataMartRawJsonImport as rji where rji.SocrataDataMartRawJsonImportID = @SocrataDataMartRawJsonImportID) as j 
    CROSS APPLY OPENJSON(RawJsonString)
    WITH
    (
        vendor_num nvarchar(256),
        vendor_num_suffix nvarchar(256),
        vendor_name nvarchar(256),
        addr1 nvarchar(256),
        addr2 nvarchar(256),
        addr3 nvarchar(256),
        city nvarchar(256),
        state nvarchar(256),
        zip_code nvarchar(256),
        zip_plus_4 nvarchar(256),
        phone varchar(200),
        vendor_status nvarchar(256),
        vendor_type nvarchar(256),
        billing_agency nvarchar(256),
        billing_subagency nvarchar(256),
        billing_fund nvarchar(256),
        billing_fund_breakout nvarchar(256),
        ccd_ctx_flag nvarchar(256),
        taxpayer_id_num varchar(200),
        email nvarchar(256),
        remarks nvarchar(256),
        last_process_date DateTime
    )
    AS vendorTemp
	where vendorTemp.vendor_status = 'A'

	--select * from #vendorSocrataTemp

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
	full outer join #vendorSocrataTemp as tv on tv.vendor_num = dbv.StatewideVendorNumber and tv.vendor_num_suffix = dbv.StatewideVendorNumberSuffix
	where (tv.vendor_num is null or tv.vendor_num_suffix is null) 
	and not exists (select 1 from #VendorFKs where dbv.VendorID = #VendorFKs.VendorID)
)

-- UPDATE (2nd attempt)
-- Update values for keys found in both sides
update dbv
set 
    VendorName = tv.vendor_name,
    VendorType = tv.vendor_type,
    BillingAgency = tv.billing_agency,
    BillingSubAgency = tv.billing_subagency,
    BillingFund = tv.billing_fund,
    VendorAddressLine1 = tv.addr1,
    VendorAddressLine2 = tv.addr2,
    VendorAddressLine3 = tv.addr3,
    VendorCity = tv.city,
    VendorState = tv.[state],
    VendorZip = tv.zip_code,
    Remarks = tv.remarks,
    VendorPhone = tv.phone,
    VendorStatus = tv.vendor_status,
    TaxpayerIdNumber = tv.taxpayer_id_num,
    Email = tv.email
from 
    dbo.Vendor as dbv
    join #vendorSocrataTemp as tv on tv.vendor_num = dbv.StatewideVendorNumber and tv.vendor_num_suffix = dbv.StatewideVendorNumberSuffix
--    full outer join #vendorSocrataTemp as tv on tv.vendor_num = dbv.StatewideVendorNumber and tv.vendor_num_suffix = dbv.StatewideVendorNumberSuffix
--where  dbv.StatewideVendorNumber is not null and dbv.StatewideVendorNumberSuffix is not null
--       and
--       tv.vendor_num is not null and tv.vendor_num_suffix is not null
    where 
    (
        isnull(dbv.VendorName, '') != isnull(tv.vendor_name, '') or
        isnull(dbv.VendorType, '') != isnull(tv.vendor_type, '') or
        isnull(dbv.BillingAgency, '') != isnull(tv.billing_agency, '') or
        isnull(dbv.BillingSubAgency, '') != isnull(tv.billing_subagency, '') or
        isnull(dbv.BillingFund, '') != isnull(tv.billing_fund, '') or
        isnull(dbv.VendorAddressLine1, '') != isnull(tv.addr1, '') or
        isnull(dbv.VendorAddressLine2, '') != isnull(tv.addr2, '') or
        isnull(dbv.VendorAddressLine3, '') != isnull(tv.addr3, '') or
        isnull(dbv.VendorCity, '') != isnull(tv.city, '') or
        isnull(dbv.VendorState, '') != isnull(tv.[state], '') or
        isnull(dbv.VendorZip, '') != isnull(tv.zip_code, '') or
        isnull(dbv.Remarks, '') != isnull(tv.remarks, '') or
        isnull(dbv.VendorPhone, '') != isnull(tv.phone, '') or
        isnull(dbv.VendorStatus, '') != isnull(tv.vendor_status, '') or
        isnull(dbv.TaxpayerIdNumber, '') != isnull(tv.taxpayer_id_num, '') or
        isnull(dbv.Email, '') != isnull(tv.email, '')
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
                        TaxpayerIdNumber,
                        Email)
select tv.vendor_name,
       tv.vendor_num,
       tv.vendor_num_suffix,
       tv.vendor_type,
       tv.billing_agency,
       tv.billing_subagency,
       tv.billing_fund,
       tv.billing_fund_breakout,
       tv.addr1,
       tv.addr2,
       tv.addr3,
       tv.city,
       tv.[state],
       tv.zip_code,
       tv.remarks,
       tv.phone,
       tv.vendor_status,
       tv.taxpayer_id_num,
       tv.email
from dbo.Vendor as dbv
full outer join #vendorSocrataTemp as tv on tv.vendor_num = dbv.StatewideVendorNumber and tv.vendor_num_suffix = dbv.StatewideVendorNumberSuffix
where  (dbv.StatewideVendorNumber is null and dbv.StatewideVendorNumberSuffix is null)
       and
       (tv.vendor_num is not null and tv.vendor_num_suffix is not null)
	   


end
go



/*
select * from  dbo.SocrataDataMartRawJsonImport where SocrataDataMartRawJsonImportTableTypeID = 1

delete from dbo.Vendor

set statistics time on
exec pVendorImportJson @SocrataDataMartRawJsonImportID = 16414
set statistics time off


*/
