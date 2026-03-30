//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[ReportTemplateModel]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum ReportTemplateModelEnum {
  Project = 1,
  InvoicePaymentRequest = 2
}

export const ReportTemplateModels: LookupTableEntry[]  = [
  { Name: "Project", DisplayName: "Project", Value: 1, SortOrder: 10 },
  { Name: "InvoicePaymentRequest", DisplayName: "Invoice Payment Request", Value: 2, SortOrder: 20 }
];
export const ReportTemplateModelsAsSelectDropdownOptions = ReportTemplateModels.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
