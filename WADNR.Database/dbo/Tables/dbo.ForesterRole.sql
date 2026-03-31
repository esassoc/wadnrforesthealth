CREATE TABLE [dbo].[ForesterRole](
    [ForesterRoleID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ForesterRole_ForesterRoleID] PRIMARY KEY,
    [ForesterRoleDisplayName] [varchar](100) NOT NULL,
    [ForesterRoleName] [varchar](100) NOT NULL,
    [SortOrder] [int] NOT NULL
)
GO
