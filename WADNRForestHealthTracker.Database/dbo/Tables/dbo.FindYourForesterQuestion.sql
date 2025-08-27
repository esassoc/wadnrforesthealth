SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FindYourForesterQuestion](
	[FindYourForesterQuestionID] [int] IDENTITY(1,1) NOT NULL,
	[QuestionText] [varchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ParentQuestionID] [int] NULL,
	[ForesterRoleID] [int] NULL,
	[ResultsBonusContent] [dbo].[html] NULL,
 CONSTRAINT [PK_FindYourForesterQuestion_FindYourForesterQuestionID] PRIMARY KEY CLUSTERED 
(
	[FindYourForesterQuestionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[FindYourForesterQuestion]  WITH CHECK ADD  CONSTRAINT [FK_FindYourForesterQuestion_FindYourForesterQuestion_ParentQuestionID_FindYourForesterQuestionID] FOREIGN KEY([ParentQuestionID])
REFERENCES [dbo].[FindYourForesterQuestion] ([FindYourForesterQuestionID])
GO
ALTER TABLE [dbo].[FindYourForesterQuestion] CHECK CONSTRAINT [FK_FindYourForesterQuestion_FindYourForesterQuestion_ParentQuestionID_FindYourForesterQuestionID]
GO
ALTER TABLE [dbo].[FindYourForesterQuestion]  WITH CHECK ADD  CONSTRAINT [FK_FindYourForesterQuestion_ForesterRole_ForesterRoleID] FOREIGN KEY([ForesterRoleID])
REFERENCES [dbo].[ForesterRole] ([ForesterRoleID])
GO
ALTER TABLE [dbo].[FindYourForesterQuestion] CHECK CONSTRAINT [FK_FindYourForesterQuestion_ForesterRole_ForesterRoleID]