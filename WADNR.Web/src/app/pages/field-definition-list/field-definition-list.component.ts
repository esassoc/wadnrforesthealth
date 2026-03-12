import { Component, OnInit } from "@angular/core";
import { Observable, BehaviorSubject, switchMap, take, first } from "rxjs";
import { ColDef } from "ag-grid-community";
import { AsyncPipe } from "@angular/common";
import { DialogService } from "@ngneat/dialog";
import { FieldDefinitionDatumDetail } from "src/app/shared/generated/model/models";
import { FieldDefinitionService } from "src/app/shared/generated/api/field-definition.service";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { AuthenticationService } from "src/app/services/authentication.service";
import { FieldDefinitionEditComponent, FieldDefinitionEditModalData } from "../field-definition-edit/field-definition-edit.component";

@Component({
    selector: "field-definition-list",
    templateUrl: "./field-definition-list.component.html",
    styleUrls: ["./field-definition-list.component.scss"],
    imports: [AsyncPipe, WADNRGridComponent, PageHeaderComponent],
})
export class FieldDefinitionListComponent implements OnInit {
    public fieldDefinitions$: Observable<FieldDefinitionDatumDetail[]>;
    public columnDefs: ColDef[];
    public canManagePageContent = false;

    private refreshData$ = new BehaviorSubject<void>(undefined);

    constructor(
        private fieldDefinitionService: FieldDefinitionService,
        private utilityFunctions: UtilityFunctionsService,
        private dialogService: DialogService,
        private authenticationService: AuthenticationService,
    ) {}

    ngOnInit() {
        this.authenticationService.currentUserSetObservable.pipe(first()).subscribe((user) => {
            this.canManagePageContent = this.authenticationService.canManagePageContent(user);
            this.buildColumnDefs();
        });

        this.fieldDefinitions$ = this.refreshData$.pipe(
            switchMap(() => this.fieldDefinitionService.listFieldDefinition()),
        );
    }

    private buildColumnDefs(): void {
        this.columnDefs = [
            ...(this.canManagePageContent
                ? [
                      this.utilityFunctions.createActionsColumnDef((params) => {
                          const row = params.data as FieldDefinitionDatumDetail;
                          return [{ ActionName: "Edit", ActionHandler: () => this.openEdit(row), ActionIcon: "fa fa-pencil" }];
                      }),
                  ]
                : []),
            this.utilityFunctions.createBasicColumnDef("Custom Label", "FieldDefinitionLabel"),
            this.utilityFunctions.createBasicColumnDef("Default Label", "FieldDefinition.FieldDefinitionDisplayName"),
            this.utilityFunctions.createBooleanColumnDef("Has Custom Field Name?", "FieldDefinitionLabel", {
                UseCustomDropdownFilter: true,
                FilterValueGetter: (params) => !!params.data?.FieldDefinitionLabel,
            }),
            this.utilityFunctions.createBooleanColumnDef("Has Custom Field Definition?", "FieldDefinitionDatumValue", {
                UseCustomDropdownFilter: true,
                FilterValueGetter: (params) => !!params.data?.FieldDefinitionDatumValue,
            }),
            {
                headerName: "Custom Definition",
                field: "FieldDefinitionDatumValue",
                cellRenderer: (params: any) => params.value ?? "",
                autoHeight: true,
                sortable: true,
                filter: true,
                cellStyle: { "white-space": "normal" },
                hide: true,
            },
            {
                headerName: "Default Definition",
                field: "FieldDefinition.DefaultDefinition",
                valueGetter: (params: any) => params.data?.FieldDefinition?.DefaultDefinition,
                cellRenderer: (params: any) => params.value ?? "",
                autoHeight: true,
                sortable: true,
                filter: true,
                cellStyle: { "white-space": "normal" },
                hide: true,
            },
        ];
    }

    openEdit(row: FieldDefinitionDatumDetail): void {
        const dialogRef = this.dialogService.open(FieldDefinitionEditComponent, {
            data: {
                fieldDefinitionID: row.FieldDefinition.FieldDefinitionID,
                fieldDefinitionDisplayName: row.FieldDefinition.FieldDefinitionDisplayName,
            } as FieldDefinitionEditModalData,
            width: "900px",
        });
        dialogRef.afterClosed$.pipe(take(1)).subscribe(result => {
            if (result) this.refreshData$.next();
        });
    }
}
