-- UnixTimeToDateTime2
--
-- Parameter:  64-bit integer
-- Number of milliseconds 
-- since 1970-01-01T00:00:00.000
-- May be negative before 1970
--
-- Returns datetime2
-- Works with all values in  
-- range   0001-01-01T00:00:00.000
-- through 9999-12-31T23:59:59.999

-- Returns NULL if parameter is out of range
IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.fUnixTimeToDateTime2'))
    drop function dbo.fUnixTimeToDateTime2
go
create function dbo.fUnixTimeToDateTime2(@x bigint)
returns datetime2 
as 
begin 
  return
    case 
      -- If the parameter is out of range,
      -- return NULL
      when    ( @x < -62135596800000 ) 
           or ( @x > 253402300799999 ) then null
      else

      -- We would like to add this number of milliseconds
      -- directly to 1970-01-01T00:00:00.000, but this 
      -- can lead to an overflow.
      -- Instead we break the addition into a number of days
      -- and a number of milliseconds. 
      -- To get the number of days, we divide by the number 
      -- of milliseconds in a day. Then add the remainder.

        dateadd ( millisecond, 
                  @x % 86400000, 
                  dateadd ( day, 
                            @x / 86400000, 
                            cast( '1970-01-01T00:00:00.000' 
                                  as datetime2 )) )
    end
end
