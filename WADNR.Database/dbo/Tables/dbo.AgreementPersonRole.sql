CREATE TABLE dbo.AgreementPersonRole
(
    AgreementPersonRoleID int NOT NULL CONSTRAINT PK_AgreementPersonRole_AgreementPersonRoleID PRIMARY KEY,
    AgreementPersonRoleName varchar(100) NOT NULL,
    AgreementPersonRoleDisplayName varchar(50) NOT NULL,
    CONSTRAINT AK_AgreementPersonRole_AgreementPersonRoleName UNIQUE (AgreementPersonRoleName),
    CONSTRAINT AK_AgreementPersonRole_AgreementPersonRoleDisplayName UNIQUE (AgreementPersonRoleDisplayName)
)
GO
