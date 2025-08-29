CREATE TABLE dbo.AgreementType
(
    AgreementTypeID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_AgreementType_AgreementTypeID PRIMARY KEY,
    AgreementTypeName varchar(100) NOT NULL,
    CONSTRAINT AK_AgreementType_AgreementTypeName UNIQUE (AgreementTypeName)
)
GO
