import { Component } from "@angular/core";
import { AgFilterComponent } from "ag-grid-angular";
import { IDoesFilterPassParams, RowNode } from "ag-grid-community";
import { FormsModule } from "@angular/forms";
import { NgClass } from "@angular/common";

@Component({
    selector: "qanat-custom-dropdown-filter",
    templateUrl: "./custom-dropdown-filter.component.html",
    styleUrls: ["./custom-dropdown-filter.component.scss"],
    imports: [FormsModule, NgClass],
})
export class CustomDropdownFilterComponent implements AgFilterComponent {
    params;
    field: string;
    dropdownValues = [];
    columnContainsMultipleValues: boolean = false;

    state = {
        selectAll: true,
        deselectAll: false,
        strict: false,
        filterOptions: {},
    };

    agInit(params): void {
        this.params = params;

        if (params.colDef.filterParams) {
            this.field = params.colDef.filterParams.field;
            //MP 1/8/26 This is mostly just an optional override, but we can probably get rid of it at some point. The function is array-aware.
            this.columnContainsMultipleValues = params.colDef.filterParams.columnContainsMultipleValues;
        }

        this.initFilter();
    }

    private initFilter(): void {
        this.dropdownValues = [];
        this.state.filterOptions = {};

        let encounteredArray = false;

        this.params.api.forEachNode((rowNode) => {
            const nodeValue = this.getNodeValue(rowNode);
            const values = this.normalizeToArrayIfNeeded(nodeValue);

            if (Array.isArray(nodeValue) || values.length > 1) {
                encounteredArray = true;
            }

            values.forEach((value) => {
                const extracted = this.ensurePrimitiveValue(value);
                if (!this.dropdownValues.includes(extracted)) {
                    this.dropdownValues.push(extracted);
                }
            });
        });

        // If the resolved field value is an array anywhere (e.g., Programs.ProgramName), treat the column as multi-value.
        this.columnContainsMultipleValues = !!this.columnContainsMultipleValues || encounteredArray;

        if (this.columnContainsMultipleValues) {
            this.state.selectAll = false;
            this.state.deselectAll = true;
            this.dropdownValues.forEach((element) => {
                this.state.filterOptions[this.getOptionKey(element)] = false;
            });
        } else {
            this.state.selectAll = true;
            this.state.deselectAll = false;
            this.dropdownValues.forEach((element) => {
                this.state.filterOptions[this.getOptionKey(element)] = true;
            });
        }
    }

    initSingleValueColumnFilter(): void {
        this.params.api.forEachNode((rowNode, i) => {
            let columnValue = this.getNodeValue(rowNode);
            if (!this.dropdownValues.includes(columnValue)) {
                this.dropdownValues.push(columnValue);
            }
        });

        // Initialize the checked state for each option.
        this.dropdownValues.forEach((element) => {
            this.state.filterOptions[this.getOptionKey(element)] = true;
        });
    }

    initMultipleValueColumnFilter(): void {
        this.state.selectAll = false;
        this.state.deselectAll = true;

        this.params.api.forEachNode((rowNode, i) => {
            let columnValue = this.getNodeValue(rowNode);
            if (!Array.isArray(columnValue)) {
                throw "Value getter for multiple column filter needs to return an array";
            }

            this.extractMultipleValues(columnValue).forEach((value) => {
                if (!this.dropdownValues.includes(value)) {
                    this.dropdownValues.push(value);
                }
            });
        });

        // Initialize the unchecked state for multiple value columns
        this.dropdownValues.forEach((element) => {
            this.state.filterOptions[this.getOptionKey(element)] = false;
        });
    }

    // If the filter is NOT active, the filter will pass for every row.
    // the filter paradigm for a multiple value column
    isFilterActive(): boolean {
        return this.columnContainsMultipleValues ? !this.state.deselectAll : !this.state.selectAll;
    }

    doesFilterPass(filterParams: IDoesFilterPassParams): boolean {
        if (this.columnContainsMultipleValues) {
            return this.doesfilterPassMultipleValues(filterParams);
        } else {
            return this.doesFilterPassSingleValue(filterParams);
        }
    }

    doesfilterPassMultipleValues(filterParams: IDoesFilterPassParams): boolean {
        const rawValue = this.getNodeValue(filterParams.node as RowNode<any>);
        const rawValueArray = this.normalizeToArrayIfNeeded(rawValue);
        const valueArray = this.extractMultipleValues(rawValueArray);
        let filterPasses: boolean;

        // if strict we need to compare all true filter options and see if the valueArray contains them all
        if (this.state.strict) {
            const checkedOptionKeys = Object.entries(this.state.filterOptions)
                .filter(([_, isChecked]) => !!isChecked)
                .map(([key]) => key);
            const valueKeySet = new Set(valueArray.map((v) => this.getOptionKey(v)));
            filterPasses = checkedOptionKeys.every((key) => valueKeySet.has(key));
        } else {
            // if not strict we can just see if the valueArray contains at least one of the selected filter options
            filterPasses = valueArray.some((value) => this.state.filterOptions[this.getOptionKey(value)]);
        }

        return filterPasses;
    }

    doesFilterPassSingleValue(filterParams: IDoesFilterPassParams): boolean {
        let value = this.getNodeValue(filterParams.node as RowNode<any>);
        const key = this.getOptionKey(value);
        if (this.state.filterOptions[key] == null) {
            return false;
        }
        return this.state.filterOptions[key] ? true : false;
    }

    private getNodeValue(rowNode: RowNode) {
        if (this.field) {
            return this.getPropertyValue(rowNode.data, this.field, null);
        }
        if (this.params?.colDef?.valueGetter) {
            return this.params.colDef.valueGetter(rowNode);
        }

        return rowNode.data;
    }

    private getPropertyValue(object, path, defaultValue) {
        if (!path) {
            return defaultValue;
        }
        return this.getPropertyValueFromParts(object, path.split("."), defaultValue);
    }

    private getPropertyValueFromParts(current: any, pathParts: string[], defaultValue: any): any {
        if (current === undefined) {
            return defaultValue;
        }
        if (current === null) {
            return null;
        }
        if (pathParts.length === 0) {
            return current;
        }

        // If we hit an array while traversing, apply the remaining path to each item.
        if (Array.isArray(current)) {
            const mapped = current.map((item) => this.getPropertyValueFromParts(item, pathParts, defaultValue)).filter((v) => v !== undefined);
            return mapped.flatMap((v) => (Array.isArray(v) ? v : [v]));
        }

        const [head, ...tail] = pathParts;
        return this.getPropertyValueFromParts(current?.[head], tail, defaultValue);
    }

    private extractMultipleValues(valueArray: any[]): any[] {
        return valueArray.map((item) => this.ensurePrimitiveValue(item)).filter((v) => v !== undefined);
    }

    private ensurePrimitiveValue(item: any): any {
        if (item === undefined) {
            return undefined;
        }
        if (item === null) {
            return null;
        }
        if (this.isPrimitive(item)) {
            return item;
        }

        throw "Custom dropdown filter expected primitive values. If your field resolves to an array of objects, include the nested property in the field path (e.g., Programs.ProgramName)";
    }

    private normalizeToArrayIfNeeded(value: any): any[] {
        if (Array.isArray(value)) {
            return value;
        }
        // Split comma-separated strings into individual values
        if (typeof value === "string" && value.includes(",")) {
            return value.split(",").map((v) => v.trim()).filter((v) => v.length > 0);
        }
        return [value];
    }

    private isPrimitive(value: any): boolean {
        const t = typeof value;
        return t === "string" || t === "number" || t === "boolean";
    }

    getOptionKey(value: any): string {
        if (value === null) {
            return "null";
        }
        if (value === undefined) {
            return "undefined";
        }
        const t = typeof value;
        if (t === "string") {
            return `string:${value}`;
        }
        if (t === "number") {
            return `number:${value}`;
        }
        if (t === "boolean") {
            return `boolean:${value}`;
        }
        return `other:${String(value)}`;
    }

    isBlankOption(value: any): boolean {
        return value == null || (typeof value === "string" && value.trim().length === 0);
    }

    getOptionLabel(value: any): string {
        if (this.isBlankOption(value)) {
            return "(Blank)";
        }
        if (typeof value === "boolean") {
            return value ? "Yes" : "No";
        }
        return String(value);
    }

    getModel() {
        return { filtersActive: this.state };
    }

    // one place this gets called is when the clear-grid-filters-button component clears the filter model
    setModel(model: any) {
        if (model === null) {
            // when we reset the model, for a multiple values filter we need to deselect all instead of selecting all.
            this.columnContainsMultipleValues ? this.onDeselectAll() : this.onSelectAll();
        } else {
            this.state = model.filtersActive;
        }
    }

    getDropdownValues() {
        // sort numbers numerically (not as strings). Check the first and last item of the array because there can be at most one "null" element
        if (this.dropdownValues.length > 0 && (typeof this.dropdownValues[0] == "number" || typeof this.dropdownValues[this.dropdownValues.length - 1] == "number")) {
            return this.dropdownValues.sort(function (a, b) {
                if (a != null && b != null) {
                    return a - b;
                }
                // sort the null/blank item first
                if (a == null) {
                    return -1;
                }
                return 1;
            });
        }
        return this.dropdownValues.sort();
    }

    updateFilter() {
        this.state.selectAll = true;
        this.state.deselectAll = true;
        for (let element of this.dropdownValues) {
            if (this.state.filterOptions[this.getOptionKey(element)]) {
                this.state.deselectAll = false;
            } else {
                this.state.selectAll = false;
            }

            if (!this.state.selectAll && !this.state.deselectAll) {
                break;
            }
        }

        this.params.filterChangedCallback();
    }

    onSelectAll() {
        this.state.selectAll = true;
        this.state.deselectAll = false;

        this.updateFilterSelection();
    }

    onDeselectAll() {
        this.state.selectAll = false;
        this.state.deselectAll = true;

        this.updateFilterSelection();
    }

    onSelectStrict() {
        this.state.strict = true;
        this.params.filterChangedCallback();
    }

    onSelectLoose() {
        this.state.strict = false;
        this.params.filterChangedCallback();
    }

    private updateFilterSelection() {
        this.dropdownValues.forEach((element) => {
            this.state.filterOptions[this.getOptionKey(element)] = this.state.selectAll;
        });

        this.params.filterChangedCallback();
    }
}
