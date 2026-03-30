//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[GoogleChartType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum GoogleChartTypeEnum {
  ColumnChart = 1,
  LineChart = 2,
  ComboChart = 3,
  AreaChart = 4,
  PieChart = 5,
  ImageChart = 6,
  BarChart = 7,
  Histogram = 8,
  BubbleChart = 9,
  ScatterChart = 10,
  SteppedAreaChart = 11
}

export const GoogleChartTypes: LookupTableEntry[]  = [
  { Name: "ColumnChart", DisplayName: "ColumnChart", Value: 1, SortOrder: 10 },
  { Name: "LineChart", DisplayName: "LineChart", Value: 2, SortOrder: 20 },
  { Name: "ComboChart", DisplayName: "ComboChart", Value: 3, SortOrder: 30 },
  { Name: "AreaChart", DisplayName: "AreaChart", Value: 4, SortOrder: 40 },
  { Name: "PieChart", DisplayName: "PieChart", Value: 5, SortOrder: 50 },
  { Name: "ImageChart", DisplayName: "ImageChart", Value: 6, SortOrder: 60 },
  { Name: "BarChart", DisplayName: "BarChart", Value: 7, SortOrder: 70 },
  { Name: "Histogram", DisplayName: "Histogram", Value: 8, SortOrder: 80 },
  { Name: "BubbleChart", DisplayName: "BubbleChart", Value: 9, SortOrder: 90 },
  { Name: "ScatterChart", DisplayName: "ScatterChart", Value: 10, SortOrder: 100 },
  { Name: "SteppedAreaChart", DisplayName: "SteppedAreaChart", Value: 11, SortOrder: 110 }
];
export const GoogleChartTypesAsSelectDropdownOptions = GoogleChartTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
