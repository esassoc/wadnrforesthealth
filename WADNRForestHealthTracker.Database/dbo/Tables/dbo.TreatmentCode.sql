CREATE TABLE [dbo].[TreatmentCode](
    [TreatmentCodeID] [int] NOT NULL CONSTRAINT [PK_TreatmentCode_TreatmentCodeID] PRIMARY KEY,
    [TreatmentCodeName] [varchar](100) NOT NULL,
    [TreatmentCodeDisplayName] [varchar](100) NOT NULL,
    CONSTRAINT [AK_TreatmentCode_TreatmentCodeDisplayName] UNIQUE ([TreatmentCodeDisplayName]),
    CONSTRAINT [AK_TreatmentCode_TreatmentCodeName] UNIQUE ([TreatmentCodeName])
)
GO
