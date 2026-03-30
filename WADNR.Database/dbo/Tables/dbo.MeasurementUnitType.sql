CREATE TABLE [dbo].[MeasurementUnitType](
    [MeasurementUnitTypeID] [int] NOT NULL CONSTRAINT [PK_MeasurementUnitType_MeasurementUnitTypeID] PRIMARY KEY,
    [MeasurementUnitTypeName] [varchar](100) NOT NULL,
    [MeasurementUnitTypeDisplayName] [varchar](100) NOT NULL,
    [LegendDisplayName] [varchar](50) NULL,
    [SingularDisplayName] [varchar](50) NULL,
    [NumberOfSignificantDigits] [int] NOT NULL,
    CONSTRAINT [AK_MeasurementUnitType_MeasurementUnitTypeDisplayName] UNIQUE ([MeasurementUnitTypeDisplayName]),
    CONSTRAINT [AK_MeasurementUnitType_MeasurementUnitTypeName] UNIQUE ([MeasurementUnitTypeName])
)
GO
