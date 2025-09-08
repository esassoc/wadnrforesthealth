CREATE TABLE [HangFire].[Counter](
    [Id] [int] IDENTITY(1,1) NOT NULL CONSTRAINT PK_Counter_Id PRIMARY KEY,
    [Key] [varchar](100) NOT NULL,
    [Value] [smallint] NOT NULL,
    [ExpireAt] [datetime] NULL
)
GO
CREATE INDEX IX_HangFire_Counter_Key ON [HangFire].[Counter]([Key]) INCLUDE([Value]);