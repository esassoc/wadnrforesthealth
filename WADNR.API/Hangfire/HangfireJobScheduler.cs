using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Hangfire;
using Hangfire.Storage;

namespace WADNR.API.Hangfire
{
    public class HangfireJobScheduler
    {
        public static void ScheduleRecurringJobs()
        {
            var recurringJobIds = new List<string>();

            // Blob File Transfer - daily at 1:00 AM UTC
            //AddRecurringJob<BlobFileTransferJob>(BlobFileTransferJob.JobName, x => x.RunJob(JobCancellationToken.Null), "0 1 * * *", recurringJobIds);

            // Finance API Jobs - every 15 minutes
            AddRecurringJob<VendorImportJob>(VendorImportJob.JobName, x => x.RunJob(JobCancellationToken.Null), "*/15 * * * *", recurringJobIds);
            AddRecurringJob<ProjectCodeImportJob>(ProjectCodeImportJob.JobName, x => x.RunJob(JobCancellationToken.Null), "*/15 * * * *", recurringJobIds);
            AddRecurringJob<ProgramIndexImportJob>(ProgramIndexImportJob.JobName, x => x.RunJob(JobCancellationToken.Null), "*/15 * * * *", recurringJobIds);
            AddRecurringJob<FundSourceExpenditureImportJob>(FundSourceExpenditureImportJob.JobName, x => x.RunJob(JobCancellationToken.Null), "*/15 * * * *", recurringJobIds);

            // GIS Data Import Jobs - daily (times in UTC; roughly 10-11 PM PST)
            AddRecurringJob<UsfsNepaBoundaryDataImportJob>(UsfsNepaBoundaryDataImportJob.JobName, x => x.RunJob(JobCancellationToken.Null), "15 6 * * *", recurringJobIds);
            AddRecurringJob<UsfsDataImportJob>(UsfsDataImportJob.JobName, x => x.RunJob(JobCancellationToken.Null), "45 6 * * *", recurringJobIds);
            AddRecurringJob<LoaDataImportJob>(LoaDataImportJob.JobName, x => x.RunJob(JobCancellationToken.Null), "15 7 * * *", recurringJobIds);

            // Program Notification Job - daily at 7:00 AM UTC (11:00 PM PST)
            AddRecurringJob<ProgramNotificationJob>(ProgramNotificationJob.JobName, x => x.RunJob(JobCancellationToken.Null), "0 7 * * *", recurringJobIds);

            // Remove any jobs we haven't explicitly scheduled
            RemoveExtraneousJobs(recurringJobIds);
        }
       
        public static void EnqueueRecurringJob(string jobName)
        {
            RecurringJob.TriggerJob(jobName);
        }

        private static void AddRecurringJob<T>(string jobName, Expression<Action<T>> methodCallExpression,
            string cronExpression, ICollection<string> recurringJobIds)
        {
            RecurringJob.AddOrUpdate<T>(jobName, methodCallExpression, cronExpression);
            recurringJobIds.Add(jobName);
        }


        private static void RemoveExtraneousJobs(List<string> recurringJobIds)
        {
            using var connection = JobStorage.Current.GetConnection();
            var recurringJobs = connection.GetRecurringJobs();
            var jobsToRemove = recurringJobs.Where(x => !recurringJobIds.Contains(x.Id)).ToList();
            foreach (var job in jobsToRemove)
            {
                RecurringJob.RemoveIfExists(job.Id);
            }
        }
    }
}
