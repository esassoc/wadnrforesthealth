
CREATE FUNCTION dbo.fRemoveLeadingZeroes
(
   @inputString varchar(MAX)
)
RETURNS varchar(MAX)
BEGIN
    RETURN SUBSTRING(@inputString, PATINDEX('%[^0]%', @inputString+'.'), LEN(@inputString))
END
go


/*

select dbo.fRemoveLeadingZeroes('00032')
select dbo.fRemoveLeadingZeroes('10032')
select dbo.fRemoveLeadingZeroes('AABADAD00000')
select dbo.fRemoveLeadingZeroes('00AABADAD00000')


*/

