import { Component } from "@angular/core";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AsyncPipe } from "@angular/common";
import { ProgramService } from "src/app/shared/generated/api/program.service";
import { ProgramDetail } from "src/app/shared/generated/model/program-detail";
import { ColDef } from "ag-grid-community";
import { firstValueFrom, map, Observable, take } from "rxjs";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { ProgramModalComponent, ProgramModalData } from "./program-modal/program-modal.component";
import { AsyncConfirmModalComponent, AsyncConfirmModalData } from "src/app/shared/components/async-confirm-modal/async-confirm-modal.component";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { DialogService } from "@ngneat/dialog";
import { AlertService } from "src/app/shared/services/alert.service";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";
import { AuthenticationService } from "src/app/services/authentication.service";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";

@Component({
    selector: "programs",
    imports: [PageHeaderComponent, WADNRGridComponent, AsyncPipe],
    templateUrl: "./programs.component.html",
})
export class ProgramsComponent {
    public programs$: Observable<ProgramDetail[]>;
    public columnDefs: ColDef[];
    public customRichTextTypeID = FirmaPageTypeEnum.ProgramsList;
    public canManagePrograms$: Observable<boolean>;

    constructor(
        private programService: ProgramService,
        private utilityFunctions: UtilityFunctionsService,
        private dialogService: DialogService,
        private alertService: AlertService,
        private authenticationService: AuthenticationService
    ) {}

    ngOnInit(): void {
        this.canManagePrograms$ = this.authenticationService.currentUserSetObservable.pipe(
            map(user => this.authenticationService.canManagePrograms(user))
        );
        this.canManagePrograms$.pipe(take(1)).subscribe(canManage => {
            const actionsColumn = canManage
                ? [this.utilityFunctions.createActionsColumnDef((params) => {
                    const program = params.data as ProgramDetail;
                    return [
                        { ActionName: "Edit", ActionHandler: () => this.openEditModal(program), ActionIcon: "fa fa-pencil" },
                        { ActionName: "Delete", ActionHandler: () => this.deleteProgram(program), ActionIcon: "fa fa-trash" },
                    ];
                })]
                : [];

            this.columnDefs = [
                ...actionsColumn,
                this.utilityFunctions.createLinkColumnDef("Program", "ProgramName", "ProgramID", {
                    InRouterLink: "/programs/",
                }),
                this.utilityFunctions.createLinkColumnDef("Organization", "Organization.OrganizationName", "Organization.OrganizationID", {
                    InRouterLink: "/organizations/",
                    FieldDefinitionType: "Organization",
                    FieldDefinitionLabelOverride: "Parent Organization",
                }),
                this.utilityFunctions.createBasicColumnDef("Short Name", "ProgramShortName"),
                this.utilityFunctions.createYearColumnDef("Project Count", "ProjectCount", { Width: 120 }),
                // Flags
                this.utilityFunctions.createBooleanColumnDef("Active?", "IsActive", { CustomDropdownFilterField: "IsActive" }),
                this.utilityFunctions.createBooleanColumnDef("Default For Import Only?", "IsDefaultProgramForImportOnly", { CustomDropdownFilterField: "IsDefaultProgramForImportOnly" }),
            ];
        });
        this.programs$ = this.programService.listProgram();
    }

    openCreateModal() {
        const dialogRef = this.dialogService.open(ProgramModalComponent, {
            data: {
                mode: "create",
            } as ProgramModalData,
            width: "55vw",
        });
        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Program created successfully.", AlertContext.Success));
                this.programs$ = this.programService.listProgram();
            }
        });
    }

    openEditModal(program: ProgramDetail) {
        const dialogRef = this.dialogService.open(ProgramModalComponent, {
            data: {
                mode: "edit",
                programID: program.ProgramID,
            } as ProgramModalData,
            width: "55vw",
        });
        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Program updated successfully.", AlertContext.Success));
                this.programs$ = this.programService.listProgram();
            }
        });
    }

    async deleteProgram(program: ProgramDetail) {
        let message = `Are you sure you want to delete program '<strong>${program.ProgramName}</strong>'?`;

        try {
            const info = await firstValueFrom(this.programService.getDeleteInfoProgram(program.ProgramID));
            message += `<br/><br/><strong>This will also delete:</strong><ul>`
                + `<li>${info.TreatmentCount} treatment${info.TreatmentCount !== 1 ? "s" : ""}</li>`
                + `<li>${info.TreatmentUpdateCount} treatment update${info.TreatmentUpdateCount !== 1 ? "s" : ""}</li>`
                + `<li>${info.ProjectLocationCount} project location${info.ProjectLocationCount !== 1 ? "s" : ""}</li>`
                + `</ul>`;
            if (info.ProjectCount > 0) {
                message += `<em>across ${info.ProjectCount} linked project${info.ProjectCount !== 1 ? "s" : ""}. This may take several minutes.</em>`;
            }
        } catch {
            // If delete-info fails, proceed with basic confirmation
        }

        const data: AsyncConfirmModalData = {
            title: "Delete Program",
            message,
            htmlMessage: true,
            buttonTextYes: "Delete",
            buttonTextNo: "Cancel",
            buttonClassYes: "btn-danger",
            actionFn: () => this.programService.deleteProgram(program.ProgramID),
        };

        this.dialogService.open(AsyncConfirmModalComponent, { data, size: "sm" })
            .afterClosed$.subscribe((result) => {
                if (result) {
                    this.alertService.clearAlerts();
                    this.alertService.pushAlert(new Alert("Program deleted successfully.", AlertContext.Success));
                    this.programs$ = this.programService.listProgram();
                }
            });
    }
}
