CREATE TABLE [HangFire].[State](
	[Id] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_State_Id] PRIMARY KEY,
	[JobId] [int] NOT NULL CONSTRAINT [FK_State_Job_JobId_Id] FOREIGN KEY REFERENCES [HangFire].[Job]([Id]) ON UPDATE CASCADE ON DELETE CASCADE,
	[Name] [varchar](20) NOT NULL,
	[Reason] [varchar](100) NULL,
	[CreatedAt] [datetime] NOT NULL,
	[Data] [varchar](max) NULL
)
GO
CREATE NONCLUSTERED INDEX [IX_HangFire_State_JobId] ON [HangFire].[State]([JobId])
GO