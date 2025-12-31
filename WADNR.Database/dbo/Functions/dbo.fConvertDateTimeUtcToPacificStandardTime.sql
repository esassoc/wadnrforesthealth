
create function dbo.fConvertDateTimeUtcToPacificStandardTime(@x datetime)
returns datetime 
as 
begin 
  return
    cast(cast(@x as datetime)  AT TIME ZONE 'UTC' AT TIME ZONE 'Pacific Standard Time' as datetime)
    
end
