CREATE TABLE [HangFire].[AggregatedCounter](
    [Id] [int] IDENTITY(1,1) NOT NULL CONSTRAINT PK_AggregatedCounter_Id PRIMARY KEY,
    [Key] [varchar](100) NOT NULL,
    [Value] [bigint] NOT NULL,
    [ExpireAt] [datetime] NULL
)
GO
CREATE UNIQUE INDEX UX_HangFire_CounterAggregated_Key ON [HangFire].[AggregatedCounter]([Key]) INCLUDE([Value]);