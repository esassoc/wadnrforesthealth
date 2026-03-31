//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[TabularDataImportTableType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum TabularDataImportTableTypeEnum {
  LoaNortheast = 1,
  LoaSoutheast = 2
}

export const TabularDataImportTableTypes: LookupTableEntry[]  = [
  { Name: "LoaNortheast", DisplayName: "LoaNortheast", Value: 1, SortOrder: 10 },
  { Name: "LoaSoutheast", DisplayName: "LoaSoutheast", Value: 2, SortOrder: 20 }
];
export const TabularDataImportTableTypesAsSelectDropdownOptions = TabularDataImportTableTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
