
CREATE TABLE [HangFire].[Set](
    [Id] [int] IDENTITY(1,1) NOT NULL CONSTRAINT PK_Set_Id PRIMARY KEY,
    [Key] [varchar](100) NOT NULL,
    [Score] [float] NOT NULL,
    [Value] [varchar](256) NOT NULL,
    [ExpireAt] [datetime] NULL
)
GO
CREATE UNIQUE INDEX UX_HangFire_Set_KeyAndValue ON [HangFire].[Set]([Key], [Value]);