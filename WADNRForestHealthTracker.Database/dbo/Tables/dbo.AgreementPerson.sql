SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AgreementPerson](
	[AgreementPersonID] [int] IDENTITY(1,1) NOT NULL,
	[AgreementID] [int] NOT NULL,
	[PersonID] [int] NOT NULL,
	[AgreementPersonRoleID] [int] NOT NULL,
 CONSTRAINT [PK_AgreementPerson_AgreementPersonID] PRIMARY KEY CLUSTERED 
(
	[AgreementPersonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_AgreementPerson_AgreementID_PersonID_AgreementPersonRoleID] UNIQUE NONCLUSTERED 
(
	[AgreementID] ASC,
	[PersonID] ASC,
	[AgreementPersonRoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[AgreementPerson]  WITH CHECK ADD  CONSTRAINT [FK_AgreementPerson_Agreement_AgreementID] FOREIGN KEY([AgreementID])
REFERENCES [dbo].[Agreement] ([AgreementID])
GO
ALTER TABLE [dbo].[AgreementPerson] CHECK CONSTRAINT [FK_AgreementPerson_Agreement_AgreementID]
GO
ALTER TABLE [dbo].[AgreementPerson]  WITH CHECK ADD  CONSTRAINT [FK_AgreementPerson_AgreementPersonRole_AgreementPersonRoleID] FOREIGN KEY([AgreementPersonRoleID])
REFERENCES [dbo].[AgreementPersonRole] ([AgreementPersonRoleID])
GO
ALTER TABLE [dbo].[AgreementPerson] CHECK CONSTRAINT [FK_AgreementPerson_AgreementPersonRole_AgreementPersonRoleID]
GO
ALTER TABLE [dbo].[AgreementPerson]  WITH CHECK ADD  CONSTRAINT [FK_AgreementPerson_Person_PersonID] FOREIGN KEY([PersonID])
REFERENCES [dbo].[Person] ([PersonID])
GO
ALTER TABLE [dbo].[AgreementPerson] CHECK CONSTRAINT [FK_AgreementPerson_Person_PersonID]