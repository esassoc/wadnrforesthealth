CREATE TABLE [HangFire].[List](
    [Id] [int] IDENTITY(1,1) NOT NULL CONSTRAINT PK_List_Id PRIMARY KEY,
    [Key] [varchar](100) NOT NULL,
    [Value] [varchar](max) NULL,
    [ExpireAt] [datetime] NULL
)
GO
