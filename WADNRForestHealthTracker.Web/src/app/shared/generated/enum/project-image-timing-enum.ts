//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[ProjectImageTiming]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum ProjectImageTimingEnum {
  After = 1,
  Before = 2,
  During = 3,
  Unknown = 4,
  DesiredFutureConditions = 5
}

export const ProjectImageTimings: LookupTableEntry[]  = [
  { Name: "After", DisplayName: "After", Value: 1, SortOrder: 10 },
  { Name: "Before", DisplayName: "Before", Value: 2, SortOrder: 20 },
  { Name: "During", DisplayName: "During", Value: 3, SortOrder: 30 },
  { Name: "Unknown", DisplayName: "Unknown", Value: 4, SortOrder: 40 },
  { Name: "DesiredFutureConditions", DisplayName: "Desired Future Conditions", Value: 5, SortOrder: 50 }
];
export const ProjectImageTimingsAsSelectDropdownOptions = ProjectImageTimings.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
