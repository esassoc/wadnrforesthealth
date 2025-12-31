export class LookupTableEntry {
    Name: string;
    DisplayName: string;
    SortOrder?: number;
    Value?: number;
    constructor(name: string, displayName: string, value: number, sortOrder?: number) {
        this.Name = name;
        this.DisplayName = displayName;
        this.Value = value;
        this.SortOrder = sortOrder;
    }
}
