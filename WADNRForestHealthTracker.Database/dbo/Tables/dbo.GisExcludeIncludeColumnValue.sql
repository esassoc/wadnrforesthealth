SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GisExcludeIncludeColumnValue](
	[GisExcludeIncludeColumnValueID] [int] IDENTITY(1,1) NOT NULL,
	[GisExcludeIncludeColumnID] [int] NOT NULL,
	[GisExcludeIncludeColumnValueForFiltering] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_GisExcludeIncludeColumnValue_GisExcludeIncludeColumnValueID] PRIMARY KEY CLUSTERED 
(
	[GisExcludeIncludeColumnValueID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[GisExcludeIncludeColumnValue]  WITH CHECK ADD  CONSTRAINT [FK_GisExcludeIncludeColumnValue_GisExcludeIncludeColumn_GisExcludeIncludeColumnID] FOREIGN KEY([GisExcludeIncludeColumnID])
REFERENCES [dbo].[GisExcludeIncludeColumn] ([GisExcludeIncludeColumnID])
GO
ALTER TABLE [dbo].[GisExcludeIncludeColumnValue] CHECK CONSTRAINT [FK_GisExcludeIncludeColumnValue_GisExcludeIncludeColumn_GisExcludeIncludeColumnID]