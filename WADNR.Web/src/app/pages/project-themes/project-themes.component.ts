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

import { ClassificationService } from "src/app/shared/generated/api/classification.service";
import { ClassificationGridRow } from "src/app/shared/generated/model/classification-grid-row";
import { ClassificationModalComponent, ClassificationModalData } from "src/app/pages/classifications/classification-modal/classification-modal.component";
import { SortOrderModalComponent, SortOrderModalData } from "src/app/shared/components/sort-order-modal/sort-order-modal.component";

@Component({
    selector: "project-themes",
    standalone: true,
    imports: [PageHeaderComponent, WADNRGridComponent, AsyncPipe],
    templateUrl: "./project-themes.component.html",
})
export class ProjectThemesComponent {
    public themes$: Observable<ClassificationGridRow[]>;
    public columnDefs: ColDef[] = [];
    public isAdmin = false;

    private refreshThemes$ = new BehaviorSubject<void>(undefined);

    constructor(
        private classificationService: ClassificationService,
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

        this.themes$ = this.refreshThemes$.pipe(
            switchMap(() => this.classificationService.listClassification())
        );
    }

    private buildColumnDefs(): void {
        this.columnDefs = [];

        if (this.isAdmin) {
            this.columnDefs.push(
                this.utilityFunctions.createActionsColumnDef((params) => {
                    const row = params.data as ClassificationGridRow;
                    return [
                        { ActionName: "Delete", ActionHandler: () => this.confirmDelete(row), ActionIcon: "fa fa-trash" },
                    ];
                })
            );
        }

        this.columnDefs.push(
            this.utilityFunctions.createLinkColumnDef("Name", "DisplayName", "ClassificationID", {
                InRouterLink: "/classifications/",
                FieldDefinitionType: "ClassificationName",
                FieldDefinitionLabelOverride: "Project Theme",
            }),
            this.utilityFunctions.createBasicColumnDef("Description", "ClassificationDescription", {
                FieldDefinitionType: "ClassificationDescription",
                FieldDefinitionLabelOverride: "Description",
                ValueFormatter: (params) => this.utilityFunctions.stripHtml(params.value),
            }),
            this.utilityFunctions.createBasicColumnDef("Goal Statement", "GoalStatement", {
                FieldDefinitionType: "ClassificationGoalStatement",
                FieldDefinitionLabelOverride: "Goal Statement",
                ValueFormatter: (params) => this.utilityFunctions.stripHtml(params.value),
            }),
            this.utilityFunctions.createYearColumnDef("# of Projects", "ProjectCount", { Width: 130 }),
            this.utilityFunctions.createYearColumnDef("Sort Order", "ClassificationSortOrder", { Width: 120 }),
        );
    }

    openCreateTheme(): void {
        const dialogRef = this.dialogService.open(ClassificationModalComponent, {
            data: { mode: "create" } as ClassificationModalData,
            width: "600px",
        });
        dialogRef.afterClosed$.subscribe(result => {
            if (result) {
                this.refreshThemes$.next();
            }
        });
    }

    openEditSortOrder(): void {
        this.themes$.pipe(take(1)).subscribe(themes => {
            const dialogRef = this.dialogService.open(SortOrderModalComponent, {
                data: {
                    items: themes.map(t => ({ id: t.ClassificationID, displayName: t.DisplayName })),
                    entityLabel: "themes",
                } as SortOrderModalData,
                width: "500px",
            });
            dialogRef.afterClosed$.subscribe(result => {
                if (result) {
                    this.classificationService.updateSortOrderClassification(result).subscribe({
                        next: () => {
                            this.alertService.pushAlert(new Alert("Sort order updated successfully.", AlertContext.Success));
                            this.refreshThemes$.next();
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

    async confirmDelete(row: ClassificationGridRow): Promise<void> {
        if (row.ProjectCount > 0) {
            await this.confirmService.confirm({
                title: "Cannot Delete Theme",
                message: `Cannot delete "${row.DisplayName}" because it has ${row.ProjectCount} associated project(s). Click <a href="/classifications/${row.ClassificationID}">here</a> to view it.`,
                buttonTextYes: "OK",
                buttonClassYes: "btn-primary",
                buttonTextNo: "Cancel",
            });
            return;
        }

        const confirmed = await this.confirmService.confirm({
            title: "Delete Theme",
            message: `Are you sure you want to delete Theme "${row.DisplayName}"?`,
            buttonTextYes: "Delete",
            buttonClassYes: "btn-danger",
            buttonTextNo: "Cancel",
        });

        if (confirmed) {
            this.classificationService.deleteClassification(row.ClassificationID).subscribe({
                next: () => {
                    this.alertService.pushAlert(new Alert("Theme deleted successfully.", AlertContext.Success));
                    this.refreshThemes$.next();
                },
                error: (err) => {
                    const message = err?.error ?? err?.message ?? "An error occurred.";
                    this.alertService.pushAlert(new Alert(message, AlertContext.Danger));
                },
            });
        }
    }
}
