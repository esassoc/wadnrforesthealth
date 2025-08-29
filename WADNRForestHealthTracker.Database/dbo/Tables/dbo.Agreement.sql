CREATE TABLE dbo.Agreement
(
    AgreementID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Agreement_AgreementID PRIMARY KEY,
    AgreementTypeID int NOT NULL,
    AgreementNumber varchar(100) NULL,
    StartDate datetime NULL,
    EndDate datetime NULL,
    AgreementAmount money NULL,
    ExpendedAmount money NULL,
    BalanceAmount money NULL,
    DNRUplandRegionID int NULL,
    FirstBillDueOn datetime NULL,
    Notes varchar(max) NULL,
    AgreementTitle varchar(256) NOT NULL,
    OrganizationID int NOT NULL,
    AgreementStatusID int NULL,
    AgreementFileResourceID int NULL,
    tmpAgreement2ID int NULL,
    CONSTRAINT FK_Agreement_AgreementStatus_AgreementStatusID FOREIGN KEY (AgreementStatusID) REFERENCES dbo.AgreementStatus (AgreementStatusID),
    CONSTRAINT FK_Agreement_AgreementType_AgreementTypeID FOREIGN KEY (AgreementTypeID) REFERENCES dbo.AgreementType (AgreementTypeID),
    CONSTRAINT FK_Agreement_DNRUplandRegion_DNRUplandRegionID FOREIGN KEY (DNRUplandRegionID) REFERENCES dbo.DNRUplandRegion (DNRUplandRegionID),
    CONSTRAINT FK_Agreement_FileResource_AgreementFileResourceID FOREIGN KEY (AgreementFileResourceID) REFERENCES dbo.FileResource (FileResourceID),
    CONSTRAINT FK_Agreement_Organization_OrganizationID FOREIGN KEY (OrganizationID) REFERENCES dbo.Organization (OrganizationID),
    CONSTRAINT FK_Agreement_tmpAgreement2_tmpAgreement2ID FOREIGN KEY (tmpAgreement2ID) REFERENCES dbo.tmpAgreement2 (TmpAgreement2ID)
)
GO