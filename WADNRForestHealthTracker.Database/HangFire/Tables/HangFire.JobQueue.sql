CREATE TABLE [HangFire].[JobQueue](
	[Id] [int] IDENTITY(1,1) NOT NULL CONSTRAINT PK_JobQueue_Id PRIMARY KEY,
	[JobId] [int] NOT NULL,
	[Queue] [varchar](20) NOT NULL,
	[FetchedAt] [datetime] NULL
)
GO
CREATE INDEX IX_HangFire_JobQueue_QueueAndFetchedAt ON [HangFire].[JobQueue]([Queue], [FetchedAt]);