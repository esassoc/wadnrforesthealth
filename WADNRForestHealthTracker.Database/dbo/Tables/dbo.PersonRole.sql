CREATE TABLE [dbo].[PersonRole](
	[PersonRoleID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_PersonRole_PersonRoleID] PRIMARY KEY,
	[PersonID] [int] NOT NULL CONSTRAINT [FK_PersonRole_Person_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
	[RoleID] [int] NOT NULL CONSTRAINT [FK_PersonRole_Role_RoleID] FOREIGN KEY REFERENCES [dbo].[Role]([RoleID]),
 CONSTRAINT [AK_PersonRole_PersonID_RoleID] UNIQUE ([PersonID], [RoleID])
)
GO