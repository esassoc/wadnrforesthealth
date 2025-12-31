//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[FirmaPageRenderType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum FirmaPageRenderTypeEnum {
  IntroductoryText = 1,
  PageContent = 2
}

export const FirmaPageRenderTypes: LookupTableEntry[]  = [
  { Name: "IntroductoryText", DisplayName: "Introductory Text", Value: 1, SortOrder: 10 },
  { Name: "PageContent", DisplayName: "Page Content", Value: 2, SortOrder: 20 }
];
export const FirmaPageRenderTypesAsSelectDropdownOptions = FirmaPageRenderTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
