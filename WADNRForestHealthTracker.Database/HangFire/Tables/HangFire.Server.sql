CREATE TABLE [HangFire].[Server](
    [Id] [varchar](100) NOT NULL CONSTRAINT [PK_Server_Id] PRIMARY KEY,
    [Data] [varchar](max) NULL,
    [LastHeartbeat] [datetime] NOT NULL
)
GO
