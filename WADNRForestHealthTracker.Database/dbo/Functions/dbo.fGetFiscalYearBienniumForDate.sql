
IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.fGetFiscalYearBienniumForDate'))
    drop function dbo.fGetFiscalYearBienniumForDate
go

/*
Returns the Fiscal Year Biennium for a given date
*/

create function dbo.fGetFiscalYearBienniumForDate(@dateToCheck DATETIME)
returns int
begin
    declare @yearToCheck int
    set @yearToCheck = (SELECT YEAR(@dateToCheck))

    declare @monthToCheck int
    set @monthToCheck = (SELECT MONTH(@dateToCheck))

    declare @dayNumberToCheck int
    set @dayNumberToCheck = (SELECT DAY(@dateToCheck))

    declare @isOddYear bit
    set @isOddYear = (@yearToCheck % 2)

    declare @afterJulyFirst bit
    set @afterJulyFirst = (case when @monthToCheck >= 7 then 1 else 0 end) --asdfadfsad

    -- Odd year, before July 1st
    if ((@isOddYear = 1) and (@afterJulyFirst = 0))
    begin
        return @yearToCheck - 2;
    end

    -- Odd Year, after July 1st
    if ((@isOddYear = 1) and (@afterJulyFirst = 1))
    begin
        return @yearToCheck;
    end

    -- Even year, before July 1st
    if ((@isOddYear = 0) and (@afterJulyFirst = 0))
    begin
        return @yearToCheck - 1;
    end

    -- Even year, after July 1st
    if ((@isOddYear = 0) and (@afterJulyFirst = 1))
    begin
        return @yearToCheck -1;
    end

    -- Not expected to ever happen
    return -1;

end
go

/*

select dbo.fGetFiscalYearBienniumForDate(GETDATE())
SELECT 1 FROM dbo.Project

SELECT dbo.fGetFiscalYearBienniumForDate(GETDATE())

SELECT dbo.fGetFiscalYearBienniumForDate('1/1/2017')
SELECT dbo.fGetFiscalYearBienniumForDate('1/1/2021')

SELECT dbo.fGetFiscalYearBienniumForDate('1/1/2018')

("1/1/2018"))  == 2017, 

SELECT dbo.fGetFiscalYearBienniumForDate('7/2/2021')


*/