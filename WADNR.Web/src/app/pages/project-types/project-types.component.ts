import { Component } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { ColDef } from "ag-grid-community";
import { BehaviorSubject, Observable, switchMap, take } from "rxjs";
import { DialogService } from "@ngneat/dialog";

import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { AuthenticationService } from "src/app/services/authentication.service";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

import { ProjectTypeService } from "src/app/shared/generated/api/project-type.service";
import { ProjectTypeGridRow } from "src/app/shared/generated/model/project-type-grid-row";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";
import { ProjectTypeModalComponent, ProjectTypeModalData } from "./project-type-modal/project-type-modal.component";
import { SortOrderModalComponent, SortOrderModalData } from "src/app/shared/components/sort-order-modal/sort-order-modal.component";

@Component({
    selector: "project-types",
    standalone: true,
    imports: [PageHeaderComponent, WADNRGridComponent, AsyncPipe],
    templateUrl: "./project-types.component.html",
})
export class ProjectTypesComponent {
    public projectTypes$: Observable<ProjectTypeGridRow[]>;
    public columnDefs: ColDef[] = [];
    public isAdmin = false;
    public customRichTextTypeID = FirmaPageTypeEnum.ProjectTypeList;

    private refreshProjectTypes$ = new BehaviorSubject<void>(undefined);

    constructor(
        private projectTypeService: ProjectTypeService,
        private utilityFunctions: UtilityFunctionsService,
        private authenticationService: AuthenticationService,
        private dialogService: DialogService,
        private confirmService: ConfirmService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.authenticationService.getCurrentUser().subscribe(user => {
            this.isAdmin = this.authenticationService.isUserAnAdministrator(user);
            this.buildColumnDefs();
        });

        this.projectTypes$ = this.refreshProjectTypes$.pipe(
            switchMap(() => this.projectTypeService.listProjectType())
        );
    }

    private buildColumnDefs(): void {
        this.columnDefs = [];

        if (this.isAdmin) {
            this.columnDefs.push(
                this.utilityFunctions.createActionsColumnDef((params) => {
                    const row = params.data as ProjectTypeGridRow;
                    return [
                        { ActionName: "Delete", ActionHandler: () => this.confirmDelete(row), ActionIcon: "fa fa-trash" },
                    ];
                })
            );
        }

        this.columnDefs.push(
            this.utilityFunctions.createLinkColumnDef("Name", "ProjectTypeName", "ProjectTypeID", {
                InRouterLink: "/project-types/",
                FieldDefinitionType: "ProjectType",
            }),
            this.utilityFunctions.createBasicColumnDef("Description", "ProjectTypeDescription", {
                FieldDefinitionType: "ProjectTypeDescription",
                ValueFormatter: (params) => this.utilityFunctions.stripHtml(params.value),
            }),
            this.utilityFunctions.createYearColumnDef("# of Projects", "ProjectCount", { Width: 130 }),
            this.utilityFunctions.createYearColumnDef("Sort Order", "ProjectTypeSortOrder", { Width: 120 }),
            this.utilityFunctions.createBasicColumnDef("Admin Only", "LimitVisibilityToAdmin", {
                ValueFormatter: (params: any) => (params.value ? "Yes" : "No"),
                Width: 120,
            }),
        );
    }

    openEditSortOrder(): void {
        this.projectTypes$.pipe(take(1)).subscribe(projectTypes => {
            const dialogRef = this.dialogService.open(SortOrderModalComponent, {
                data: {
                    items: projectTypes.map(t => ({ id: t.ProjectTypeID, displayName: t.ProjectTypeName })),
                    entityLabel: "project types",
                } as SortOrderModalData,
                width: "500px",
            });
            dialogRef.afterClosed$.subscribe(result => {
                if (result) {
                    this.projectTypeService.updateSortOrderProjectType(result).subscribe({
                        next: () => {
                            this.alertService.pushAlert(new Alert("Sort order updated successfully.", AlertContext.Success));
                            this.refreshProjectTypes$.next();
                        },
                        error: (err) => {
                            const message = err?.error ?? err?.message ?? "An error occurred.";
                            this.alertService.pushAlert(new Alert(message, AlertContext.Danger));
                        },
                    });
                }
            });
        });
    }

    openCreateProjectType(): void {
        const dialogRef = this.dialogService.open(ProjectTypeModalComponent, {
            data: { mode: "create" } as ProjectTypeModalData,
            width: "600px",
        });
        dialogRef.afterClosed$.subscribe(result => {
            if (result) {
                this.refreshProjectTypes$.next();
            }
        });
    }

    async confirmDelete(row: ProjectTypeGridRow): Promise<void> {
        if (row.ProjectCount > 0) {
            await this.confirmService.confirm({
                title: "Cannot Delete Project Type",
                message: `Cannot delete "${row.ProjectTypeName}" because it has ${row.ProjectCount} associated project(s). Click <a href="/project-types/${row.ProjectTypeID}">here</a> to view it.`,
                buttonTextYes: "OK",
                buttonClassYes: "btn-primary",
                buttonTextNo: "Cancel",
            });
            return;
        }

        const confirmed = await this.confirmService.confirm({
            title: "Delete Project Type",
            message: `Are you sure you want to delete Project Type "${row.ProjectTypeName}"?`,
            buttonTextYes: "Delete",
            buttonClassYes: "btn-danger",
            buttonTextNo: "Cancel",
        });

        if (confirmed) {
            this.projectTypeService.deleteProjectType(row.ProjectTypeID).subscribe({
                next: () => {
                    this.alertService.pushAlert(new Alert("Project Type deleted successfully.", AlertContext.Success));
                    this.refreshProjectTypes$.next();
                },
                error: (err) => {
                    const message = err?.error ?? err?.message ?? "An error occurred.";
                    this.alertService.pushAlert(new Alert(message, AlertContext.Danger));
                },
            });
        }
    }
}
