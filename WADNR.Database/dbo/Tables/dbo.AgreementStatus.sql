CREATE TABLE dbo.AgreementStatus
(
    AgreementStatusID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_AgreementStatus_AgreementStatusID PRIMARY KEY,
    AgreementStatusName varchar(100) NOT NULL,
    CONSTRAINT AK_AgreementStatus_AgreementStatusName UNIQUE (AgreementStatusName)
)
GO
