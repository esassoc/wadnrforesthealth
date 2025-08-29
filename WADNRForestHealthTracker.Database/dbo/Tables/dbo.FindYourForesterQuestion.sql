CREATE TABLE [dbo].[FindYourForesterQuestion](
	[FindYourForesterQuestionID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FindYourForesterQuestion_FindYourForesterQuestionID] PRIMARY KEY,
	[QuestionText] [varchar](500) NOT NULL,
	[ParentQuestionID] [int] NULL CONSTRAINT [FK_FindYourForesterQuestion_FindYourForesterQuestion_ParentQuestionID_FindYourForesterQuestionID] FOREIGN KEY REFERENCES [dbo].[FindYourForesterQuestion]([FindYourForesterQuestionID]),
	[ForesterRoleID] [int] NULL CONSTRAINT [FK_FindYourForesterQuestion_ForesterRole_ForesterRoleID] FOREIGN KEY REFERENCES [dbo].[ForesterRole]([ForesterRoleID]),
	[ResultsBonusContent] [dbo].[html] NULL
)
GO