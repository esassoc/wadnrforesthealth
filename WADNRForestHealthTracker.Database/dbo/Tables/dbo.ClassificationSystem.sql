SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ClassificationSystem](
	[ClassificationSystemID] [int] IDENTITY(1,1) NOT NULL,
	[ClassificationSystemName] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ClassificationSystemDefinition] [dbo].[html] NULL,
	[ClassificationSystemListPageContent] [dbo].[html] NULL,
 CONSTRAINT [PK_ClassificationSystem_ClassificationSystemID] PRIMARY KEY CLUSTERED 
(
	[ClassificationSystemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
