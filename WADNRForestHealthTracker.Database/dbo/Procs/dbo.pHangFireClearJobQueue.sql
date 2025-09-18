

create procedure dbo.pHangFireClearJobQueue
as
begin

    declare @JobCount int = (select count(*) from HangFire.JobQueue)

    insert into HangFire.State 
        (JobId, [Name], Reason, CreatedAt, [Data])
    select JobId, 'Deleted', 'Triggered via ScheduledBackgroundJobBootstrapper startup cleanup', getutcdate(), '{"DeletedAt":"' + convert(varchar, sysutcdatetime(), 127) + 'Z"}' 
    from HangFire.JobQueue

    ;with LatestStateFromJobQueue as
    (
        select distinct s.JobId, max(s.Id) over(partition by s.JobId) as LatestStateID
        from HangFire.JobQueue q
            inner join HangFire.[State] s on s.JobId = q.JobId
    )
    update HangFire.Job
    set StateId = s.Id, StateName = s.[Name], ExpireAt = dateadd(Day, 7, getutcdate())
    from HangFire.Job j
        join LatestStateFromJobQueue ls on ls.JobId = j.Id
        join HangFire.[State] s on s.Id = ls.LatestStateID

    delete from HangFire.JobQueue

    insert into HangFire.[Counter]
        ([Key], [Value], ExpireAt)
    values ('stats:deleted', @JobCount, null)

end
go