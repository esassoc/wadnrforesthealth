import { Component } from "@angular/core";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { AsyncPipe } from "@angular/common";
import { ProgramService } from "src/app/shared/generated/api/program.service";
import { ProgramDetail } from "src/app/shared/generated/model/program-detail";
import { ColDef } from "ag-grid-community";
import { Observable } from "rxjs";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
//import { ProgramModalComponent } from "./program-modal/program-modal.component";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { DialogService } from "@ngneat/dialog";
import { AlertService } from "src/app/shared/services/alert.service";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";

@Component({
    selector: "programs",
    imports: [PageHeaderComponent, AlertDisplayComponent, WADNRGridComponent, AsyncPipe],
    templateUrl: "./programs.component.html",
})
export class ProgramsComponent {
    public programs$: Observable<ProgramDetail[]>;
    public columnDefs: ColDef[];
    public customRichTextTypeID = FirmaPageTypeEnum.ProgramsList;

    constructor(
        private ProgramService: ProgramService,
        private utilityFunctions: UtilityFunctionsService,
        private dialogService: DialogService,
        private alertService: AlertService,
        private confirmService: ConfirmService
    ) {}

    ngOnInit(): void {
        this.columnDefs = [
            // Program name as a link to the program detail using ProgramID
            this.utilityFunctions.createLinkColumnDef("Name", "ProgramName", "ProgramID", {
                InRouterLink: "/programs/",
                FieldDefinitionType: "Program",
            }),
            // Short name
            this.utilityFunctions.createBasicColumnDef("Short Name", "ProgramShortName"),
            // Organization - nested object, display OrganizationName
            this.utilityFunctions.createBasicColumnDef("Organization", "Organization.OrganizationName"),
            // Project count as a numeric (right aligned)
            this.utilityFunctions.createYearColumnDef("Project Count", "ProjectCount", { Width: 120 }),
            // Flags
            this.utilityFunctions.createBooleanColumnDef("Active?", "IsActive"),
            this.utilityFunctions.createBooleanColumnDef("Default For Import Only?", "IsDefaultProgramForImportOnly"),
        ];
        this.programs$ = this.ProgramService.listProgram();
    }

    // openAddModal() {
    //     const dialogRef = this.dialogService.open(ProgramModalComponent, {
    //         data: {
    //             mode: "add",
    //             Program: null,
    //         },
    //     });
    //     dialogRef.afterClosed$.subscribe((result) => {
    //         if (result) {
    //             this.alertService.clearAlerts();
    //             this.alertService.pushAlert(new Alert("Funding source added successfully.", AlertContext.Success));
    //             this.programs$ = this.ProgramService.listProgram();
    //         }
    //     });
    // }

    // openEditModal(Program: ProgramDto) {
    //     const dialogRef = this.dialogService.open(ProgramModalComponent, {
    //         data: {
    //             mode: "edit",
    //             Program,
    //         },
    //     });
    //     dialogRef.afterClosed$.subscribe((result) => {
    //         if (result) {
    //             this.alertService.clearAlerts();
    //             this.alertService.pushAlert(new Alert("Funding source updated successfully.", AlertContext.Success));
    //             this.programs$ = this.ProgramService.listProgram();
    //         }
    //     });
    // }

    // deleteProgram(Program: ProgramDto) {
    //     this.confirmService
    //         .confirm({
    //             title: "Delete Funding Source",
    //             message: `Are you sure you want to delete funding source '<strong>${Program.ProgramName}</strong>'?`,
    //             buttonTextYes: "Delete",
    //             buttonTextNo: "Cancel",
    //             buttonClassYes: "btn-danger",
    //         })
    //         .then((confirmed) => {
    //             if (confirmed) {
    //                 this.ProgramService.deleteProgram(Program.ProgramID).subscribe(() => {
    //                     this.alertService.clearAlerts();
    //                     this.alertService.pushAlert(new Alert("Funding source deleted successfully.", AlertContext.Success));
    //                     this.programs$ = this.ProgramService.listProgram();
    //                 });
    //             }
    //         });
    // }
}
