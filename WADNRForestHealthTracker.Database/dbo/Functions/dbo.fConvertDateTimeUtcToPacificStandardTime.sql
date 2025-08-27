-- fConvertDateTimeUtcToPacificStandardTime

IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.fConvertDateTimeUtcToPacificStandardTime'))
    drop function dbo.fConvertDateTimeUtcToPacificStandardTime
go
create function dbo.fConvertDateTimeUtcToPacificStandardTime(@x datetime)
returns datetime 
as 
begin 
  return
    cast(cast(@x as datetime)  AT TIME ZONE 'UTC' AT TIME ZONE 'Pacific Standard Time' as datetime)
    
end
