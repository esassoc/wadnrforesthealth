CREATE TABLE [dbo].[GisExcludeIncludeColumnValue](
    [GisExcludeIncludeColumnValueID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_GisExcludeIncludeColumnValue_GisExcludeIncludeColumnValueID] PRIMARY KEY,
    [GisExcludeIncludeColumnID] [int] NOT NULL CONSTRAINT [FK_GisExcludeIncludeColumnValue_GisExcludeIncludeColumn_GisExcludeIncludeColumnID] FOREIGN KEY REFERENCES [dbo].[GisExcludeIncludeColumn]([GisExcludeIncludeColumnID]),
    [GisExcludeIncludeColumnValueForFiltering] [varchar](200) NOT NULL
)
GO