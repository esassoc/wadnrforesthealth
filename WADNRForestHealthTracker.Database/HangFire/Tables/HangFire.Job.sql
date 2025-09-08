CREATE TABLE [HangFire].[Job](
    [Id] [int] IDENTITY(1,1) NOT NULL CONSTRAINT PK_Job_Id PRIMARY KEY,
    [StateId] [int] NULL,
    [StateName] [varchar](20) NULL,
    [InvocationData] [varchar](max) NOT NULL,
    [Arguments] [varchar](max) NOT NULL,
    [CreatedAt] [datetime] NOT NULL,
    [ExpireAt] [datetime] NULL
)
GO
CREATE INDEX IX_HangFire_Job_StateName ON [HangFire].[Job]([StateName]);