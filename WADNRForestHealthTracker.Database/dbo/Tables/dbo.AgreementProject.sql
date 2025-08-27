SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AgreementProject](
	[AgreementProjectID] [int] IDENTITY(1,1) NOT NULL,
	[AgreementID] [int] NOT NULL,
	[ProjectID] [int] NOT NULL,
 CONSTRAINT [PK_AgreementProject_AgreementProjectID] PRIMARY KEY CLUSTERED 
(
	[AgreementProjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_AgreementProject_AgreementID_ProjectID] UNIQUE NONCLUSTERED 
(
	[AgreementID] ASC,
	[ProjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[AgreementProject]  WITH CHECK ADD  CONSTRAINT [FK_AgreementProject_Agreement_AgreementID] FOREIGN KEY([AgreementID])
REFERENCES [dbo].[Agreement] ([AgreementID])
GO
ALTER TABLE [dbo].[AgreementProject] CHECK CONSTRAINT [FK_AgreementProject_Agreement_AgreementID]
GO
ALTER TABLE [dbo].[AgreementProject]  WITH CHECK ADD  CONSTRAINT [FK_AgreementProject_Project_ProjectID] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[Project] ([ProjectID])
GO
ALTER TABLE [dbo].[AgreementProject] CHECK CONSTRAINT [FK_AgreementProject_Project_ProjectID]