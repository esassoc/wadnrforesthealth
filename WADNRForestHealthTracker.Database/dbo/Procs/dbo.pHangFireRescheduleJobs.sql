IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.pHangFireRescheduleJobs'))
  drop procedure dbo.pHangFireRescheduleJobs
go
create procedure dbo.pHangFireRescheduleJobs
as
    set transaction isolation level serializable
    set xact_abort on

    begin tran

    declare @now datetimeoffset
    set @now = SYSDATETIMEOFFSET()

    if object_id('tempdb.dbo.#tmpRecurringJob') is not null drop table #tmpRecurringJob
    select a1.RecurringJobName, a2.TimeZoneId, ltrim(rtrim(a3.Cron)) as Cron, a4.NextExecution, a5.LastExecution
    into #tmpRecurringJob
    from
    (
        select distinct [Key] as RecurringJobName from HangFire.Hash WITH (TABLOCKX)
    ) a1
    left join
    (
        select [key] as RecurringJobName, [value] as TimeZoneId
        from HangFire.Hash where field='TimeZoneId'
    ) a2 on a1.RecurringJobName = a2.RecurringJobName
    left join 
    (
        select [key] as RecurringJobName, [value] as Cron
        from HangFire.Hash where field='Cron'
    ) a3 on a1.RecurringJobName = a3.RecurringJobName
    left join
    (
        select [key] as RecurringJobName, [value] as NextExecution
        from HangFire.Hash where field='NextExecution'
    ) a4 on a1.RecurringJobName = a4.RecurringJobName
    left join
    (   
        select [key] as RecurringJobName, [value] as LastExecution
        from HangFire.Hash where field='LastExecution'
    ) a5 on a1.RecurringJobName = a5.RecurringJobName

    if object_id('tempdb.dbo.#tmpRecurringJobDaily') is not null drop table #tmpRecurringJobDaily
    select x.RecurringJobName, x.TimeZoneId, x.Cron, replace(x.Cron, ' * * *','') as CronMinuteHour, x.NextExecution, x.LastExecution
    into #tmpRecurringJobDaily
    from #tmpRecurringJob x
    where
            Cron like '[0-9] [0-9] * * *'
            or Cron like '[0-9][0-9] [0-9] * * *'
            or Cron like '[0-9] [0-9][0-9] * * *'
            or Cron like '[0-9][0-9] [0-9][0-9] * * *'

    if object_id('tempdb.dbo.#tmpRecurringJobDailyWithHourMinute') is not null drop table #tmpRecurringJobDailyWithHourMinute
    select x.RecurringJobName, x.TimeZoneId, x.Cron, 
        cast(substring(x.CronMinuteHour, 1, charindex(' ',x.CronMinuteHour) - 1) as int) as CronMinute,
        cast(substring(x.CronMinuteHour, charindex(' ',x.CronMinuteHour) + 1, len(x.CronMinuteHour)) as int) as CronHour,
        cast((@now at time zone TimeZoneId) as datetime2) As NowAsDateTime,
        x.NextExecution,
        cast((convert(datetimeoffset, x.NextExecution) at time zone TimeZoneId) as datetime2) As NextExecutionAsDateTime,
        x.LastExecution,
        cast((convert(datetimeoffset, x.LastExecution) at time zone TimeZoneId) as datetime2) As LastExecutionAsDateTime
    into #tmpRecurringJobDailyWithHourMinute
    from #tmpRecurringJobDaily x

    if object_id('tempdb.dbo.#tmpRecurringJobDailyWithHourMinuteTodayRunTime') is not null drop table #tmpRecurringJobDailyWithHourMinuteTodayRunTime
    select x.RecurringJobName, x.TimeZoneId, x.Cron, x.CronMinute, x.CronHour,
        x.NowAsDateTime,
        dateadd(minute, x.CronMinute, dateadd(hour, x.CronHour, cast(cast(x.NowAsDateTime as date) as datetime2))) as TodayRunTime,
        x.NextExecution,
        x.NextExecutionAsDateTime,
        x.LastExecution,
        x.LastExecutionAsDateTime
    into #tmpRecurringJobDailyWithHourMinuteTodayRunTime
    from #tmpRecurringJobDailyWithHourMinute x

    if object_id('tempdb.dbo.#tmpRecurringJobDailyWithHourMinuteTodayRunTimeAndNewExecutionTime') is not null drop table #tmpRecurringJobDailyWithHourMinuteTodayRunTimeAndNewExecutionTime
    select x.RecurringJobName, x.TimeZoneId, x.Cron, x.CronMinute, x.CronHour,
        x.NowAsDateTime,
        x.TodayRunTime,
        x.NextExecution,
        x.NextExecutionAsDateTime,
        case
            when x.NextExecutionAsDateTime > x.NowAsDateTime then x.NextExecutionAsDateTime
            when x.TodayRunTime > x.NowAsDateTime then x.TodayRunTime
            else dateadd(day, 1, x.TodayRunTime)
        end as NewNextExecutionAsDateTime,
        x.LastExecution,
        x.LastExecutionAsDateTime
    into #tmpRecurringJobDailyWithHourMinuteTodayRunTimeAndNewExecutionTime
    from #tmpRecurringJobDailyWithHourMinuteTodayRunTime x

    if object_id('tempdb.dbo.#tmpRecurringJobDailyWithHourMinuteTodayRunTimeAndNewTimes') is not null drop table #tmpRecurringJobDailyWithHourMinuteTodayRunTimeAndNewTimes
    select x.RecurringJobName, x.TimeZoneId, x.Cron, x.CronMinute, x.CronHour,
        x.NowAsDateTime,
        x.TodayRunTime,
        x.NextExecution,
        x.NextExecutionAsDateTime,
        x.NewNextExecutionAsDateTime,
        x.LastExecution,
        x.LastExecutionAsDateTime,
        dateadd(day, -1, x.NewNextExecutionAsDateTime) as NewLastExecutionAsDateTime
    into #tmpRecurringJobDailyWithHourMinuteTodayRunTimeAndNewTimes
    from #tmpRecurringJobDailyWithHourMinuteTodayRunTimeAndNewExecutionTime x

    if object_id('tempdb.dbo.#tmpRecurringJobDailyWithHourMinuteTodayRunTimeAndNewTimesWithStrings') is not null drop table #tmpRecurringJobDailyWithHourMinuteTodayRunTimeAndNewTimesWithStrings
    select x.RecurringJobName, x.TimeZoneId, x.Cron, x.CronMinute, x.CronHour,
        x.NowAsDateTime,
        x.TodayRunTime,
        x.NextExecution,
        x.NextExecutionAsDateTime,
        x.NewNextExecutionAsDateTime,
        format(x.NewNextExecutionAsDateTime at time zone x.TimeZoneId at time zone 'UTC', N'yyyy-MM-ddTHH:mm:ss.fffffffZ') as NewNextExecution,
        x.LastExecution,
        x.LastExecutionAsDateTime,
        x.NewLastExecutionAsDateTime,
        format(x.NewLastExecutionAsDateTime at time zone x.TimeZoneId at time zone 'UTC', N'yyyy-MM-ddTHH:mm:ss.fffffffZ') as NewLastExecution
    into #tmpRecurringJobDailyWithHourMinuteTodayRunTimeAndNewTimesWithStrings
    from #tmpRecurringJobDailyWithHourMinuteTodayRunTimeAndNewTimes x

    if object_id('tempdb.dbo.#tmpRecurringJobDailyWithHourMinuteTodayRunTimeAndNewTimesWithStringsAndUpdateBit') is not null drop table #tmpRecurringJobDailyWithHourMinuteTodayRunTimeAndNewTimesWithStringsAndUpdateBit
    select x.RecurringJobName, x.TimeZoneId, x.Cron, x.CronMinute, x.CronHour,
        x.NowAsDateTime,
        x.TodayRunTime,
        x.NextExecution,
        x.NextExecutionAsDateTime,
        x.NewNextExecutionAsDateTime,
        x.NewNextExecution,
        cast(sign(abs(datediff(minute, x.NextExecutionAsDateTime, x.NewNextExecutionAsDateTime))) as bit) as DoesNextExecutionNeedUpdate,
        x.LastExecution,
        x.LastExecutionAsDateTime,
        x.NewLastExecutionAsDateTime,
        x.NewLastExecution,
        cast(sign(abs(datediff(minute, x.LastExecutionAsDateTime, x.NewLastExecutionAsDateTime))) as bit) as DoesLastExecutionNeedUpdate
    into #tmpRecurringJobDailyWithHourMinuteTodayRunTimeAndNewTimesWithStringsAndUpdateBit
    from #tmpRecurringJobDailyWithHourMinuteTodayRunTimeAndNewTimesWithStrings x

    print 'Status of HangFire.Hash table schedules'
    select x.RecurringJobName, x.NextExecutionAsDateTime, x.NewNextExecutionAsDateTime, x.DoesNextExecutionNeedUpdate, x.LastExecutionAsDateTime, x.NewLastExecutionAsDateTime, x.DoesLastExecutionNeedUpdate
    from #tmpRecurringJobDailyWithHourMinuteTodayRunTimeAndNewTimesWithStringsAndUpdateBit x

    print 'Updating HangFire.Hash table NextExecution'
    update HangFire.Hash
    set Value = t.NewNextExecution
    from HangFire.Hash h
    join #tmpRecurringJobDailyWithHourMinuteTodayRunTimeAndNewTimesWithStringsAndUpdateBit t on h.[Key] = t.RecurringJobName
    where h.Field = 'NextExecution' and t.DoesNextExecutionNeedUpdate = 1

    print 'Updating HangFire.Hash table LastExecution'
    update HangFire.Hash
    set Value = t.NewLastExecution
    from HangFire.Hash h
    join #tmpRecurringJobDailyWithHourMinuteTodayRunTimeAndNewTimesWithStringsAndUpdateBit t on h.[Key] = t.RecurringJobName
    where h.Field = 'LastExecution' and t.DoesLastExecutionNeedUpdate = 1

    commit tran
go
