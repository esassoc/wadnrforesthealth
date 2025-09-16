CREATE TABLE [HangFire].[JobParameter](
                                          [Id] [int] IDENTITY(1,1) NOT NULL CONSTRAINT PK_JobParameter_Id PRIMARY KEY,
                                          [JobId] [int] NOT NULL CONSTRAINT FK_JobParameter_Job_Id FOREIGN KEY REFERENCES [HangFire].[Job]([Id]) ON UPDATE CASCADE ON DELETE CASCADE,
                                          [Name] [varchar](40) NOT NULL,
                                          [Value] [varchar](max) NULL
)
GO
CREATE INDEX IX_HangFire_JobParameter_JobIdAndName ON [HangFire].[JobParameter]([JobId], [Name]);
GO