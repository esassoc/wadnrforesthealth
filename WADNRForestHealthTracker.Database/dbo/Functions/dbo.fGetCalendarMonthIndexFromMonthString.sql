
IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.fGetCalendarMonthIndexFromMonthString'))
    drop function dbo.fGetCalendarMonthIndexFromMonthString
go

/*
Returns the current Fiscal Year Biennium
*/

create function dbo.fGetCalendarMonthIndexFromMonthString
(
   @monthString NVARCHAR(MAX)
)
returns int
begin
    return (SELECT MONTH(@monthString + ' 1 1901'))
end
go

/*
select dbo.fGetCalendarMonthIndexFromMonthString('March')
select dbo.fGetCalendarMonthIndexFromMonthString('december')
select dbo.fGetCalendarMonthIndexFromMonthString('January')


*/