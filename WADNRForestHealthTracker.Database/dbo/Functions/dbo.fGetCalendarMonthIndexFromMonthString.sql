

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