CREATE TABLE [HangFire].[Hash](
    [Id] [int] IDENTITY(1,1) NOT NULL CONSTRAINT PK_Hash_Id PRIMARY KEY,
    [Key] [varchar](100) NOT NULL,
    [Field] [varchar](100) NOT NULL,
    [Value] [varchar](max) NULL,
    [ExpireAt] [datetime2](7) NULL
)
GO
CREATE UNIQUE INDEX UX_HangFire_Hash_Key_Field ON [HangFire].[Hash]([Key], [Field]);