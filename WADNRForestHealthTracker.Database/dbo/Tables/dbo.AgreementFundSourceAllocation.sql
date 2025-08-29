CREATE TABLE dbo.AgreementFundSourceAllocation
(
    AgreementFundSourceAllocationID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_AgreementFundSourceAllocation_AgreementFundSourceAllocationID PRIMARY KEY,
    AgreementID int NOT NULL CONSTRAINT FK_AgreementFundSourceAllocation_Agreement_AgreementID FOREIGN KEY REFERENCES dbo.Agreement(AgreementID),
    FundSourceAllocationID int NOT NULL CONSTRAINT FK_AgreementFundSourceAllocation_FundSourceAllocation_FundSourceAllocationID FOREIGN KEY REFERENCES dbo.FundSourceAllocation(FundSourceAllocationID),
    CONSTRAINT AK_AgreementFundSourceAllocation_AgreementID_FundSourceAllocationID UNIQUE (AgreementID, FundSourceAllocationID)
)
GO