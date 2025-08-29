CREATE TABLE [dbo].[OrganizationCode](
    [OrganizationCodeID] [int] NOT NULL CONSTRAINT [PK_OrganizationCode_OrganizationCodeID] PRIMARY KEY,
    [OrganizationCodeName] [varchar](250) NULL,
    [OrganizationCodeValue] [varchar](20) NULL
)
GO
