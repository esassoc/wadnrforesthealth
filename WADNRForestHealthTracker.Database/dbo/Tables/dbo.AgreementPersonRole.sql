CREATE TABLE dbo.AgreementPersonRole
(
    AgreementPersonRoleID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_AgreementPersonRole_AgreementPersonRoleID PRIMARY KEY,
    AgreementPersonRoleName varchar(100) NOT NULL,
    CONSTRAINT AK_AgreementPersonRole_AgreementPersonRoleName UNIQUE (AgreementPersonRoleName)
)
GO
