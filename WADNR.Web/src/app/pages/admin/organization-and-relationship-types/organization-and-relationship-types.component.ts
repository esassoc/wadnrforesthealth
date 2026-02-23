import { Component, OnInit } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { ColDef } from "ag-grid-community";
import { BehaviorSubject, Observable, switchMap } from "rxjs";
import { DialogService } from "@ngneat/dialog";

import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { AuthenticationService } from "src/app/services/authentication.service";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

import { OrganizationTypeService } from "src/app/shared/generated/api/organization-type.service";
import { RelationshipTypeService } from "src/app/shared/generated/api/relationship-type.service";
import { OrganizationTypeLookupItem } from "src/app/shared/generated/model/organization-type-lookup-item";
import { OrganizationTypeGridRow } from "src/app/shared/generated/model/organization-type-grid-row";
import { RelationshipTypeGridRow } from "src/app/shared/generated/model/relationship-type-grid-row";
import { OrganizationTypeModalComponent, OrganizationTypeModalData } from "./organization-type-modal/organization-type-modal.component";
import { RelationshipTypeModalComponent, RelationshipTypeModalData } from "./relationship-type-modal/relationship-type-modal.component";

@Component({
    selector: "organization-and-relationship-types",
    standalone: true,
    imports: [PageHeaderComponent, WADNRGridComponent, AsyncPipe],
    templateUrl: "./organization-and-relationship-types.component.html",
})
export class OrganizationAndRelationshipTypesComponent implements OnInit {
    public orgTypes$: Observable<OrganizationTypeGridRow[]>;
    public relTypes$: Observable<RelationshipTypeGridRow[]>;

    public orgTypeColumnDefs: ColDef<OrganizationTypeGridRow>[] = [];
    public relTypeColumnDefs: ColDef<RelationshipTypeGridRow>[] = [];
    public isAdmin = false;

    private refreshOrgTypes$ = new BehaviorSubject<void>(undefined);
    private refreshRelTypes$ = new BehaviorSubject<void>(undefined);

    constructor(
        private organizationTypeService: OrganizationTypeService,
        private relationshipTypeService: RelationshipTypeService,
        private utilityFunctions: UtilityFunctionsService,
        private authenticationService: AuthenticationService,
        private dialogService: DialogService,
        private confirmService: ConfirmService,
        private alertService: AlertService
    ) {}

    private allOrgTypes: OrganizationTypeLookupItem[] = [];

    ngOnInit(): void {
        this.authenticationService.getCurrentUser().subscribe(user => {
            this.isAdmin = this.authenticationService.isUserAnAdministrator(user);
            this.buildOrgTypeColumnDefs();
        });

        this.orgTypes$ = this.refreshOrgTypes$.pipe(
            switchMap(() => this.organizationTypeService.listOrganizationType())
        );

        this.relTypes$ = this.refreshRelTypes$.pipe(
            switchMap(() => this.relationshipTypeService.listRelationshipType())
        );

        // Load all org types for dynamic relationship type columns
        this.organizationTypeService.listLookupOrganizationType().subscribe(orgTypes => {
            this.allOrgTypes = orgTypes;
            this.buildRelTypeColumnDefs();
        });
    }

    private buildOrgTypeColumnDefs(): void {
        this.orgTypeColumnDefs = [];

        if (this.isAdmin) {
            this.orgTypeColumnDefs.push(
                this.utilityFunctions.createActionsColumnDef((params) => {
                    const orgType = params.data as OrganizationTypeGridRow;
                    return [
                        { ActionName: "Edit", ActionHandler: () => this.openEditOrgType(orgType), ActionIcon: "fa fa-pencil" },
                        { ActionName: "Delete", ActionHandler: () => this.confirmDeleteOrgType(orgType), ActionIcon: "fa fa-trash" },
                    ];
                })
            );
        }

        this.orgTypeColumnDefs.push(
            this.utilityFunctions.createBasicColumnDef("Organization Type Name", "OrganizationTypeName", { Width: 240 }),
            this.utilityFunctions.createBasicColumnDef("Abbreviation", "OrganizationTypeAbbreviation", { Width: 200 }),
            this.utilityFunctions.createBooleanColumnDef("Is Default?", "IsDefaultOrganizationType", { Width: 80, CustomDropdownFilterField: "IsDefaultOrganizationType" }),
            this.utilityFunctions.createBooleanColumnDef("Is Funding Type?", "IsFundingType", { Width: 80, CustomDropdownFilterField: "IsFundingType" }),
            this.utilityFunctions.createBooleanColumnDef("Show on Project Map?", "ShowOnProjectMaps", { Width: 150, CustomDropdownFilterField: "ShowOnProjectMaps" }),
            {
                headerName: "Legend Color",
                field: "LegendColor",
                width: 90,
                filter: false,
                cellRenderer: (params: any) => {
                    if (!params.value) return "";
                    return `<div style="background-color: ${params.value}; height: 1em; width: 1em; display: block; margin: auto;"></div>`;
                },
            },
        );
    }

    private buildRelTypeColumnDefs(): void {
        this.relTypeColumnDefs = [];

        if (this.isAdmin) {
            this.relTypeColumnDefs.push(
                this.utilityFunctions.createActionsColumnDef((params) => {
                    const relType = params.data as RelationshipTypeGridRow;
                    return [
                        { ActionName: "Edit", ActionHandler: () => this.openEditRelType(relType), ActionIcon: "fa fa-pencil" },
                        { ActionName: "Delete", ActionHandler: () => this.confirmDeleteRelType(relType), ActionIcon: "fa fa-trash" },
                    ];
                })
            );
        }

        this.relTypeColumnDefs.push(
            this.utilityFunctions.createBasicColumnDef("Project Relationship Type Name", "RelationshipTypeName", { Width: 240 }),
            this.utilityFunctions.createBooleanColumnDef("Can Steward Projects?", "CanStewardProjects", { Width: 90, CustomDropdownFilterField: "CanStewardProjects" }),
            this.utilityFunctions.createBooleanColumnDef("Serves as Primary Contact?", "IsPrimaryContact", { Width: 90, CustomDropdownFilterField: "IsPrimaryContact" }),
            this.utilityFunctions.createBooleanColumnDef("Must be Related to a Project Once?", "CanOnlyBeRelatedOnceToAProject", { Width: 90, CustomDropdownFilterField: "CanOnlyBeRelatedOnceToAProject" }),
            this.utilityFunctions.createBooleanColumnDef("Show on Project Fact Sheet", "ShowOnFactSheet", { Width: 90, CustomDropdownFilterField: "ShowOnFactSheet" }),
        );

        // Dynamic columns: one per organization type
        for (const orgType of this.allOrgTypes) {
            this.relTypeColumnDefs.push({
                headerName: orgType.OrganizationTypeName,
                width: 90,
                valueGetter: (params) => {
                    const row = params.data as RelationshipTypeGridRow;
                    return row?.AssociatedOrganizationTypeNames?.includes(orgType.OrganizationTypeName) ?? false;
                },
                valueFormatter: (params) => this.utilityFunctions.booleanValueGetter(params.value),
                filter: true,
            });
        }
    }

    // Organization Type CRUD
    openCreateOrgType(): void {
        const dialogRef = this.dialogService.open(OrganizationTypeModalComponent, {
            data: { mode: "create" } as OrganizationTypeModalData,
            width: "600px",
        });
        dialogRef.afterClosed$.subscribe(result => {
            if (result) {
                this.refreshOrgTypes$.next();
            }
        });
    }

    openEditOrgType(orgType: OrganizationTypeGridRow): void {
        const dialogRef = this.dialogService.open(OrganizationTypeModalComponent, {
            data: { mode: "edit", organizationType: orgType } as OrganizationTypeModalData,
            width: "600px",
        });
        dialogRef.afterClosed$.subscribe(result => {
            if (result) {
                this.refreshOrgTypes$.next();
            }
        });
    }

    async confirmDeleteOrgType(orgType: OrganizationTypeGridRow): Promise<void> {
        const confirmed = await this.confirmService.confirm({
            title: "Delete Organization Type",
            message: `Are you sure you want to delete "${orgType.OrganizationTypeName}"? This action cannot be undone.`,
            buttonTextYes: "Delete",
            buttonClassYes: "btn-danger",
            buttonTextNo: "Cancel",
        });

        if (confirmed) {
            this.organizationTypeService.deleteOrganizationType(orgType.OrganizationTypeID).subscribe({
                next: () => {
                    this.alertService.pushAlert(new Alert("Organization Type deleted successfully.", AlertContext.Success));
                    this.refreshOrgTypes$.next();
                },
                error: (err) => {
                    const message = err?.error ?? err?.message ?? "An error occurred.";
                    this.alertService.pushAlert(new Alert(message, AlertContext.Danger));
                },
            });
        }
    }

    // Relationship Type CRUD
    openCreateRelType(): void {
        this.organizationTypeService.listLookupOrganizationType().subscribe(orgTypes => {
            const dialogRef = this.dialogService.open(RelationshipTypeModalComponent, {
                data: {
                    mode: "create",
                    organizationTypes: orgTypes,
                    orgTypeNameToID: new Map(orgTypes.map(ot => [ot.OrganizationTypeName, ot.OrganizationTypeID])),
                } as RelationshipTypeModalData,
                width: "600px",
            });
            dialogRef.afterClosed$.subscribe(result => {
                if (result) {
                    this.refreshRelTypes$.next();
                }
            });
        });
    }

    openEditRelType(relType: RelationshipTypeGridRow): void {
        this.organizationTypeService.listLookupOrganizationType().subscribe(orgTypes => {
            const dialogRef = this.dialogService.open(RelationshipTypeModalComponent, {
                data: {
                    mode: "edit",
                    relationshipType: relType,
                    organizationTypes: orgTypes,
                    orgTypeNameToID: new Map(orgTypes.map(ot => [ot.OrganizationTypeName, ot.OrganizationTypeID])),
                } as RelationshipTypeModalData,
                width: "600px",
            });
            dialogRef.afterClosed$.subscribe(result => {
                if (result) {
                    this.refreshRelTypes$.next();
                }
            });
        });
    }

    async confirmDeleteRelType(relType: RelationshipTypeGridRow): Promise<void> {
        const confirmed = await this.confirmService.confirm({
            title: "Delete Relationship Type",
            message: `Are you sure you want to delete "${relType.RelationshipTypeName}"? This action cannot be undone.`,
            buttonTextYes: "Delete",
            buttonClassYes: "btn-danger",
            buttonTextNo: "Cancel",
        });

        if (confirmed) {
            this.relationshipTypeService.deleteRelationshipType(relType.RelationshipTypeID).subscribe({
                next: () => {
                    this.alertService.pushAlert(new Alert("Relationship Type deleted successfully.", AlertContext.Success));
                    this.refreshRelTypes$.next();
                },
                error: (err) => {
                    const message = err?.error ?? err?.message ?? "An error occurred.";
                    this.alertService.pushAlert(new Alert(message, AlertContext.Danger));
                },
            });
        }
    }
}
