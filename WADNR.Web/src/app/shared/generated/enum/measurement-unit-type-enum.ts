//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[MeasurementUnitType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum MeasurementUnitTypeEnum {
  Acres = 1,
  Miles = 2,
  SquareFeet = 3,
  LinearFeet = 4,
  Kilogram = 5,
  Number = 6,
  Pounds = 7,
  Tons = 8,
  Dollars = 9,
  Parcels = 10,
  Percent = 11,
  Therms = 12,
  PartsPerMillion = 13,
  PartsPerBillion = 14,
  MilligamsPerLiter = 15,
  NephlometricTurbidityUnit = 16,
  Meters = 17,
  PeriphytonBiomassIndex = 18,
  AcreFeet = 19,
  Gallon = 20,
  CubicYards = 21,
  MetricTons = 22,
  Hours = 23
}

export const MeasurementUnitTypes: LookupTableEntry[]  = [
  { Name: "Acres", DisplayName: "acres", Value: 1, SortOrder: 10 },
  { Name: "Miles", DisplayName: "miles", Value: 2, SortOrder: 20 },
  { Name: "SquareFeet", DisplayName: "square feet", Value: 3, SortOrder: 30 },
  { Name: "LinearFeet", DisplayName: "linear feet", Value: 4, SortOrder: 40 },
  { Name: "Kilogram", DisplayName: "kg", Value: 5, SortOrder: 50 },
  { Name: "Number", DisplayName: "number", Value: 6, SortOrder: 60 },
  { Name: "Pounds", DisplayName: "pounds", Value: 7, SortOrder: 70 },
  { Name: "Tons", DisplayName: "tons", Value: 8, SortOrder: 80 },
  { Name: "Dollars", DisplayName: "dollars", Value: 9, SortOrder: 90 },
  { Name: "Parcels", DisplayName: "parcels", Value: 10, SortOrder: 100 },
  { Name: "Percent", DisplayName: "%", Value: 11, SortOrder: 110 },
  { Name: "Therms", DisplayName: "therms", Value: 12, SortOrder: 120 },
  { Name: "PartsPerMillion", DisplayName: "ppm", Value: 13, SortOrder: 130 },
  { Name: "PartsPerBillion", DisplayName: "ppb", Value: 14, SortOrder: 140 },
  { Name: "MilligamsPerLiter", DisplayName: "mg/L", Value: 15, SortOrder: 150 },
  { Name: "NephlometricTurbidityUnit", DisplayName: "NTU", Value: 16, SortOrder: 160 },
  { Name: "Meters", DisplayName: "meters", Value: 17, SortOrder: 170 },
  { Name: "PeriphytonBiomassIndex", DisplayName: "PBI", Value: 18, SortOrder: 180 },
  { Name: "AcreFeet", DisplayName: "acre-feet", Value: 19, SortOrder: 190 },
  { Name: "Gallon", DisplayName: "gallons", Value: 20, SortOrder: 200 },
  { Name: "CubicYards", DisplayName: "cubic yards", Value: 21, SortOrder: 210 },
  { Name: "MetricTons", DisplayName: "metric tons", Value: 22, SortOrder: 220 },
  { Name: "Hours", DisplayName: "hours ", Value: 23, SortOrder: 230 }
];
export const MeasurementUnitTypesAsSelectDropdownOptions = MeasurementUnitTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
