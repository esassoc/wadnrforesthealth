CREATE TABLE [dbo].[Tag](
    [TagID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Tag_TagID] PRIMARY KEY,
    [TagName] [varchar](100) NOT NULL CONSTRAINT [AK_Tag_TagName] UNIQUE,
    [TagDisplayName] [varchar](100) NOT NULL CONSTRAINT [AK_Tag_TagDisplayName] UNIQUE,
    [TagDescription] [varchar](250) NULL
)
GO
