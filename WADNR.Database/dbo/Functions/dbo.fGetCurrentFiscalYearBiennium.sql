

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