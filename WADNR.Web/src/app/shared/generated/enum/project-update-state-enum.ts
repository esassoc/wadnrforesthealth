//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[ProjectUpdateState]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum ProjectUpdateStateEnum {
  Created = 1,
  Submitted = 2,
  Returned = 3,
  Approved = 4
}

export const ProjectUpdateStates: LookupTableEntry[]  = [
  { Name: "Created", DisplayName: "Created", Value: 1, SortOrder: 10 },
  { Name: "Submitted", DisplayName: "Submitted", Value: 2, SortOrder: 20 },
  { Name: "Returned", DisplayName: "Returned", Value: 3, SortOrder: 30 },
  { Name: "Approved", DisplayName: "Approved", Value: 4, SortOrder: 40 }
];
export const ProjectUpdateStatesAsSelectDropdownOptions = ProjectUpdateStates.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
