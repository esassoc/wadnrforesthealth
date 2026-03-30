CREATE TABLE dbo.AgreementPerson
(
    AgreementPersonID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_AgreementPerson_AgreementPersonID PRIMARY KEY,
    AgreementID int NOT NULL CONSTRAINT FK_AgreementPerson_Agreement_AgreementID FOREIGN KEY REFERENCES dbo.Agreement(AgreementID),
    PersonID int NOT NULL CONSTRAINT FK_AgreementPerson_Person_PersonID FOREIGN KEY REFERENCES dbo.Person(PersonID),
    AgreementPersonRoleID int NOT NULL CONSTRAINT FK_AgreementPerson_AgreementPersonRole_AgreementPersonRoleID FOREIGN KEY REFERENCES dbo.AgreementPersonRole(AgreementPersonRoleID)
)
GO