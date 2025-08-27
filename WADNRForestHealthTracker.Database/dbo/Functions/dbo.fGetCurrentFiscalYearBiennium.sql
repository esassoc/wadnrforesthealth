
IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.fGetCurrentFiscalYearBiennium'))
    drop function dbo.fGetCurrentFiscalYearBiennium
go

/*
Returns the Current Fiscal Year Biennium
*/

create function dbo.fGetCurrentFiscalYearBiennium()
returns int
begin
    return dbo.fGetFiscalYearBienniumForDate(GETDATE())
end
go

/*

select dbo.fGetCurrentFiscalYearBiennium()

*/