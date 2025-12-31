CREATE TABLE [dbo].[ProjectApprovalStatus](
	[ProjectApprovalStatusID] [int] NOT NULL CONSTRAINT [PK_ProjectApprovalStatus_ProjectApprovalStatusID] PRIMARY KEY,
	[ProjectApprovalStatusName] [varchar](50) NOT NULL CONSTRAINT [AK_ProjectApprovalStatus_ProjectApprovalStatusName] UNIQUE,
	[ProjectApprovalStatusDisplayName] [varchar](50) NOT NULL CONSTRAINT [AK_ProjectApprovalStatus_ProjectApprovalStatusDisplayName] UNIQUE
)
GO
