//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[TreatmentType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum TreatmentTypeEnum {
  Commercial = 1,
  PrescribedFire = 2,
  NonCommercial = 3,
  Other = 4
}

export const TreatmentTypes: LookupTableEntry[]  = [
  { Name: "Commercial", DisplayName: "Commercial", Value: 1, SortOrder: 10 },
  { Name: "PrescribedFire", DisplayName: "Prescribed Fire", Value: 2, SortOrder: 20 },
  { Name: "NonCommercial", DisplayName: "Non-Commercial", Value: 3, SortOrder: 30 },
  { Name: "Other", DisplayName: "Other", Value: 4, SortOrder: 40 }
];
export const TreatmentTypesAsSelectDropdownOptions = TreatmentTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
