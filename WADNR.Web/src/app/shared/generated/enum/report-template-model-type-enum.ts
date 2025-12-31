//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[ReportTemplateModelType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum ReportTemplateModelTypeEnum {
  SingleModel = 1,
  MultipleModels = 2
}

export const ReportTemplateModelTypes: LookupTableEntry[]  = [
  { Name: "SingleModel", DisplayName: "Single Model", Value: 1, SortOrder: 10 },
  { Name: "MultipleModels", DisplayName: "Multiple Models", Value: 2, SortOrder: 20 }
];
export const ReportTemplateModelTypesAsSelectDropdownOptions = ReportTemplateModelTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
