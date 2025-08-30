CREATE TABLE [dbo].[ProjectUpdateState](
	[ProjectUpdateStateID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectUpdateState_ProjectUpdateStateID] PRIMARY KEY,
	[ProjectUpdateStateName] [varchar](100) NOT NULL CONSTRAINT [AK_ProjectUpdateState_ProjectUpdateStateName] UNIQUE,
	[ProjectUpdateStateDisplayName] [varchar](100) NOT NULL CONSTRAINT [AK_ProjectUpdateState_ProjectUpdateStateDisplayName] UNIQUE
)
GO
