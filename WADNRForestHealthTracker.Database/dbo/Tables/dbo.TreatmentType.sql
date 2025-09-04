CREATE TABLE [dbo].[TreatmentType](
    [TreatmentTypeID] [int] NOT NULL CONSTRAINT [PK_TreatmentType_TreatmentTypeID] PRIMARY KEY,
    [TreatmentTypeName] [varchar](50) NOT NULL,
    [TreatmentTypeDisplayName] [varchar](50) NOT NULL,
    CONSTRAINT [AK_TreatmentType_TreatmentTypeDisplayName] UNIQUE ([TreatmentTypeDisplayName]),
    CONSTRAINT [AK_TreatmentType_TreatmentTypeName] UNIQUE ([TreatmentTypeName])
)
GO
