CREATE TABLE [dbo].[TreatmentDetailedActivityType](
    [TreatmentDetailedActivityTypeID] [int] NOT NULL CONSTRAINT [PK_TreatmentDetailedActivityType_TreatmentDetailedActivityTypeID] PRIMARY KEY,
    [TreatmentDetailedActivityTypeName] [varchar](50) NOT NULL,
    [TreatmentDetailedActivityTypeDisplayName] [varchar](50) NOT NULL,
    CONSTRAINT [AK_TreatmentDetailedActivityType_TreatmentDetailedActivityTypeDisplayName] UNIQUE ([TreatmentDetailedActivityTypeDisplayName]),
    CONSTRAINT [AK_TreatmentDetailedActivityType_TreatmentDetailedActivityTypeName] UNIQUE ([TreatmentDetailedActivityTypeName])
)
GO
