SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Classification](
	[ClassificationID] [int] IDENTITY(1,1) NOT NULL,
	[ClassificationDescription] [varchar](300) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ThemeColor] [varchar](7) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[DisplayName] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[GoalStatement] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[KeyImageFileResourceID] [int] NULL,
	[ClassificationSystemID] [int] NOT NULL,
	[ClassificationSortOrder] [int] NULL,
 CONSTRAINT [PK_Classification_ClassificationID] PRIMARY KEY CLUSTERED 
(
	[ClassificationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[Classification]  WITH CHECK ADD  CONSTRAINT [FK_Classification_ClassificationSystem_ClassificationSystemID] FOREIGN KEY([ClassificationSystemID])
REFERENCES [dbo].[ClassificationSystem] ([ClassificationSystemID])
GO
ALTER TABLE [dbo].[Classification] CHECK CONSTRAINT [FK_Classification_ClassificationSystem_ClassificationSystemID]
GO
ALTER TABLE [dbo].[Classification]  WITH CHECK ADD  CONSTRAINT [FK_Classification_FileResource_KeyImageFileResourceID_FileResourceID] FOREIGN KEY([KeyImageFileResourceID])
REFERENCES [dbo].[FileResource] ([FileResourceID])
GO
ALTER TABLE [dbo].[Classification] CHECK CONSTRAINT [FK_Classification_FileResource_KeyImageFileResourceID_FileResourceID]