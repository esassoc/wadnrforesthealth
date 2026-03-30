CREATE TABLE dbo.AgreementProject
(
    AgreementProjectID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_AgreementProject_AgreementProjectID PRIMARY KEY,
    AgreementID int NOT NULL CONSTRAINT FK_AgreementProject_Agreement_AgreementID FOREIGN KEY REFERENCES dbo.Agreement(AgreementID),
    ProjectID int NOT NULL CONSTRAINT FK_AgreementProject_Project_ProjectID FOREIGN KEY REFERENCES dbo.Project(ProjectID)
)
GO