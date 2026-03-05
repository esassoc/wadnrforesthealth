import { AsyncPipe } from "@angular/common";
import { Component } from "@angular/core";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { BehaviorSubject, combineLatest, distinctUntilChanged, filter, forkJoin, map, Observable, shareReplay, switchMap, take } from "rxjs";
import { toLoadingState } from "src/app/shared/interfaces/page-loading.interface";
import { ColDef } from "ag-grid-community";
import { DialogService } from "@ngneat/dialog";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { PersonLinkComponent } from "src/app/shared/components/person-link/person-link.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { environment } from "src/environments/environment";

import { ProgramService } from "src/app/shared/generated/api/program.service";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { OrganizationService } from "src/app/shared/generated/api/organization.service";
import { RelationshipTypeService } from "src/app/shared/generated/api/relationship-type.service";
import { ProjectStagesAsSelectDropdownOptions } from "src/app/shared/generated/enum/project-stage-enum";
import { ProgramDetail } from "src/app/shared/generated/model/program-detail";
import { PersonWithOrganizationLookupItem } from "src/app/shared/generated/model/person-with-organization-lookup-item";
import { ProjectProgramDetailGridRow } from "src/app/shared/generated/model/project-program-detail-grid-row";
import { ProgramNotificationGridRow } from "src/app/shared/generated/model/program-notification-grid-row";
import { ProjectImportBlockListGridRow } from "src/app/shared/generated/model/project-import-block-list-grid-row";
import { GdbDefaultMappingItem } from "src/app/shared/generated/model/gdb-default-mapping-item";
import { GdbCrosswalkItem } from "src/app/shared/generated/model/gdb-crosswalk-item";
import { ProgramModalComponent, ProgramModalData } from "../program-modal/program-modal.component";
import { EditImportBasicsModalComponent, EditImportBasicsModalData } from "../edit-import-basics-modal/edit-import-basics-modal.component";
import { EditDefaultMappingsModalComponent, EditDefaultMappingsModalData } from "../edit-default-mappings-modal/edit-default-mappings-modal.component";
import { EditCrosswalkValuesModalComponent, EditCrosswalkValuesModalData } from "../edit-crosswalk-values-modal/edit-crosswalk-values-modal.component";
import { EditProgramEditorsModalComponent, EditProgramEditorsModalData } from "../edit-program-editors-modal/edit-program-editors-modal.component";
import { AddToBlockListModalComponent, AddToBlockListModalData } from "../add-to-block-list-modal/add-to-block-list-modal.component";
import { CreateBlockListEntryModalComponent, CreateBlockListEntryModalData } from "../create-block-list-entry-modal/create-block-list-entry-modal.component";
import { ProgramNotificationModalComponent, ProgramNotificationModalData } from "../program-notification-modal/program-notification-modal.component";
import { AsyncConfirmModalComponent, AsyncConfirmModalData } from "src/app/shared/components/async-confirm-modal/async-confirm-modal.component";
import { FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { GdbImportBasics } from "src/app/shared/generated/model/gdb-import-basics";
import { IconComponent } from "src/app/shared/components/icon/icon.component";

@Component({
    selector: "program-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, BreadcrumbComponent, WADNRGridComponent, RouterLink, PersonLinkComponent, LoadingDirective, IconComponent],
    templateUrl: "./program-detail.component.html",
    styleUrls: ["./program-detail.component.scss"],
})
export class ProgramDetailComponent {
    public programID$: Observable<number>;
    public program$: Observable<ProgramDetail>;
    public projects$: Observable<ProjectProgramDetailGridRow[]>;
    public notifications$: Observable<ProgramNotificationGridRow[]>;
    public blockListEntries$: Observable<ProjectImportBlockListGridRow[]>;

    public projectsIsLoading$: Observable<boolean>;
    public notificationsIsLoading$: Observable<boolean>;
    public blockListEntriesIsLoading$: Observable<boolean>;

    public projectColumnDefs: ColDef<ProjectProgramDetailGridRow>[] = [];
    public notificationColumnDefs: ColDef<ProgramNotificationGridRow>[] = [];
    public blockListColumnDefs: ColDef<ProjectImportBlockListGridRow>[] = [];

    private refreshData$ = new BehaviorSubject<void>(undefined);
    private refreshNotifications$ = new BehaviorSubject<void>(undefined);
    private refreshBlockList$ = new BehaviorSubject<void>(undefined);

    constructor(
        private route: ActivatedRoute,
        private programService: ProgramService,
        private projectService: ProjectService,
        private organizationService: OrganizationService,
        private relationshipTypeService: RelationshipTypeService,
        private utilityFunctions: UtilityFunctionsService,
        private dialogService: DialogService,
        private confirmService: ConfirmService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.programID$ = this.route.paramMap.pipe(
            map((p) => (p.get("programID") ? Number(p.get("programID")) : null)),
            filter((programID): programID is number => programID != null && !Number.isNaN(programID)),
            distinctUntilChanged(),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.program$ = combineLatest([this.programID$, this.refreshData$]).pipe(
            switchMap(([programID]) => this.programService.getProgram(programID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projects$ = combineLatest([this.programID$, this.refreshData$]).pipe(
            switchMap(([programID]) => this.programService.listProjectsProgram(programID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.notifications$ = combineLatest([this.programID$, this.refreshNotifications$]).pipe(
            switchMap(([programID]) => this.programService.listNotificationsProgram(programID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.blockListEntries$ = combineLatest([this.programID$, this.refreshBlockList$]).pipe(
            switchMap(([programID]) => this.programService.listBlockListEntriesProgram(programID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projectsIsLoading$ = toLoadingState(this.projects$);
        this.notificationsIsLoading$ = toLoadingState(this.notifications$);
        this.blockListEntriesIsLoading$ = toLoadingState(this.blockListEntries$);

        this.projectColumnDefs = this.createProjectColumnDefs();
        this.notificationColumnDefs = this.createNotificationColumnDefs();
        this.blockListColumnDefs = this.createBlockListColumnDefs();
    }

    getFileUrl(fileResourceUrl: string | null | undefined): string | null {
        if (!fileResourceUrl) return null;
        return `${environment.mainAppApiUrl}/file-resources/${fileResourceUrl}`;
    }

    downloadGdb(): void {
        combineLatest([this.programID$, this.program$]).pipe(take(1)).subscribe(([programID, program]) => {
            this.programService.downloadProjectsAsGdbProgram(programID, "body", false, { httpHeaderAccept: "application/octet-stream" as any }).subscribe({
                next: (blob) => {
                    const url = window.URL.createObjectURL(blob);
                    const a = document.createElement("a");
                    a.href = url;
                    const date = new Date().toISOString().slice(0, 10);
                    a.download = `ProjectsInProgram-${program.ProgramName}-${date}.gdb.zip`;
                    a.click();
                    window.URL.revokeObjectURL(url);
                },
                error: () => {
                    this.alertService.pushAlert(new Alert("An error occurred while downloading the GDB.", AlertContext.Danger, true));
                },
            });
        });
    }

    private createProjectColumnDefs(): ColDef<ProjectProgramDetailGridRow>[] {
        return [
            this.utilityFunctions.createActionsColumnDef((params) => {
                const row = params.data as ProjectProgramDetailGridRow;
                return [
                    { ActionName: "Delete", ActionHandler: () => this.confirmDeleteProject(row), ActionIcon: "fa fa-trash" },
                    { ActionName: "Add to Block List", ActionHandler: () => this.openAddToBlockListModal(row), ActionIcon: "fa fa-ban" },
                ];
            }),
            this.utilityFunctions.createBasicColumnDef("Project Identifier", "ProjectGisIdentifier", {
                FieldDefinitionType: "ProjectIdentifier",
            }),
            this.utilityFunctions.createLinkColumnDef("FHT Project Number", "FhtProjectNumber", "ProjectID", {
                InRouterLink: "/projects/fact-sheet/",
                FieldDefinitionType: "FhtProjectNumber",
            }),
            this.utilityFunctions.createLinkColumnDef("Project", "ProjectName", "ProjectID", {
                InRouterLink: "/projects/fact-sheet/",
                FieldDefinitionType: "ProjectName",
            }),
            this.utilityFunctions.createBasicColumnDef("Project Type", "ProjectTypeName", {
                FieldDefinitionType: "ProjectType",
            }),
            this.utilityFunctions.createBasicColumnDef("Project Stage", "ProjectStage.ProjectStageName", {
                FieldDefinitionType: "ProjectStage",
                CustomDropdownFilterField: "ProjectStage.ProjectStageName",
            }),
            this.utilityFunctions.createBasicColumnDef("Programs", "Programs", {
                FieldDefinitionType: "Program",
            }),
        ];
    }

    private createNotificationColumnDefs(): ColDef<ProgramNotificationGridRow>[] {
        const emailTextCol = this.utilityFunctions.createBasicColumnDef("Notification Email Text", "NotificationEmailText");
        emailTextCol.valueFormatter = (params) => {
            if (!params.value) return "";
            const doc = new DOMParser().parseFromString(params.value, "text/html");
            return doc.body.textContent || "";
        };

        return [
            this.utilityFunctions.createActionsColumnDef((params) => {
                const row = params.data as ProgramNotificationGridRow;
                return [
                    { ActionName: "Edit", ActionHandler: () => this.openEditNotificationModal(row), ActionIcon: "fa fa-edit" },
                ];
            }),
            this.utilityFunctions.createBasicColumnDef("Program Notification Type", "ProgramNotificationTypeDisplayName"),
            this.utilityFunctions.createBasicColumnDef("Recurrence Interval in Years", "RecurrenceIntervalInYears"),
            emailTextCol,
        ];
    }

    openEditModal(program: ProgramDetail): void {
        const dialogRef = this.dialogService.open(ProgramModalComponent, {
            data: {
                mode: "edit",
                program: program,
            } as ProgramModalData,
            width: "55vw"
        });

        dialogRef.afterClosed$.subscribe(result => {
            if (result) {
                this.refreshData$.next();
            }
        });
    }

    getEditorDisplayText(editor: PersonWithOrganizationLookupItem): string {
        let text = editor.FullName ?? "";
        if (editor.OrganizationName) {
            text += ` - ${editor.OrganizationName}`;
            if (editor.OrganizationShortName) {
                text += ` (${editor.OrganizationShortName})`;
            }
        }
        return text;
    }

    openEditEditorsModal(program: ProgramDetail): void {
        const dialogRef = this.dialogService.open(EditProgramEditorsModalComponent, {
            data: {
                programID: program.ProgramID,
                currentEditors: program.ProgramEditors ?? [],
            } as EditProgramEditorsModalData,
        });

        dialogRef.afterClosed$.subscribe(result => {
            if (result) {
                this.refreshData$.next();
            }
        });
    }

    getMappingColumnName(mappings: GdbDefaultMappingItem[] | undefined, fieldDisplayName: string): string {
        if (!mappings) return "—";
        const mapping = mappings.find(m => m.FieldDefinitionDisplayName === fieldDisplayName);
        return mapping?.GisDefaultMappingColumnName || "—";
    }

    getCrosswalksByField(crosswalks: GdbCrosswalkItem[] | undefined, fieldDisplayName: string): GdbCrosswalkItem[] {
        if (!crosswalks) return [];
        return crosswalks.filter(c => c.FieldDefinitionDisplayName === fieldDisplayName);
    }

    openEditImportBasicsModal(program: ProgramDetail): void {
        if (!program.GdbImportBasics) return;

        forkJoin({
            organizations: this.organizationService.listOrganization(),
            relationshipTypes: this.relationshipTypeService.listLookupRelationshipType(),
        }).subscribe(({ organizations, relationshipTypes }) => {
            const projectStageOptions: FormInputOption[] = ProjectStagesAsSelectDropdownOptions;

            const organizationOptions: FormInputOption[] = organizations.map(o => ({
                Value: o.OrganizationID,
                Label: o.OrganizationName,
                disabled: false,
            }));

            const relationshipTypeOptions: FormInputOption[] = relationshipTypes.map(rt => ({
                Value: rt.RelationshipTypeID,
                Label: rt.RelationshipTypeName,
                disabled: false,
            }));

            const dialogRef = this.dialogService.open(EditImportBasicsModalComponent, {
                data: {
                    programID: program.ProgramID,
                    basics: program.GdbImportBasics,
                    projectStageOptions,
                    organizationOptions,
                    relationshipTypeOptions,
                } as EditImportBasicsModalData,
                size: "lg",
            });

            dialogRef.afterClosed$.subscribe(result => {
                if (result) {
                    this.alertService.pushAlert(new Alert("Import basics updated.", AlertContext.Success, true));
                    this.refreshData$.next();
                }
            });
        });
    }

    openEditDefaultMappingsModal(program: ProgramDetail): void {
        const dialogRef = this.dialogService.open(EditDefaultMappingsModalComponent, {
            data: {
                programID: program.ProgramID,
                mappings: program.GdbDefaultMappings ?? [],
                isFlattened: program.GdbImportBasics?.ImportIsFlattened ?? false,
            } as EditDefaultMappingsModalData,
            size: "lg",
        });

        dialogRef.afterClosed$.subscribe(result => {
            if (result) {
                this.alertService.pushAlert(new Alert("Default mappings updated.", AlertContext.Success, true));
                this.refreshData$.next();
            }
        });
    }

    openEditCrosswalkValuesModal(program: ProgramDetail): void {
        const dialogRef = this.dialogService.open(EditCrosswalkValuesModalComponent, {
            data: {
                programID: program.ProgramID,
                crosswalks: program.GdbCrosswalkValues ?? [],
                isFlattened: program.GdbImportBasics?.ImportIsFlattened ?? false,
            } as EditCrosswalkValuesModalData,
            size: "lg",
        });

        dialogRef.afterClosed$.subscribe(result => {
            if (result) {
                this.alertService.pushAlert(new Alert("Crosswalk values updated.", AlertContext.Success, true));
                this.refreshData$.next();
            }
        });
    }

    // --- Project Actions ---

    confirmDeleteProject(row: ProjectProgramDetailGridRow): void {
        const data: AsyncConfirmModalData = {
            title: "Confirm Delete",
            message: `Are you sure you want to delete Project "${row.ProjectName}"? This action cannot be undone.`,
            buttonTextYes: "Delete",
            buttonClassYes: "btn-danger",
            buttonTextNo: "Cancel",
            actionFn: () => this.projectService.deleteProject(row.ProjectID),
        };

        this.dialogService
            .open(AsyncConfirmModalComponent, { data, size: "md" })
            .afterClosed$.subscribe((result) => {
                if (result) {
                    this.alertService.pushAlert(new Alert("Project deleted successfully.", AlertContext.Success, true));
                    this.refreshData$.next();
                }
            });
    }

    openAddToBlockListModal(row: ProjectProgramDetailGridRow): void {
        this.programID$.subscribe((programID) => {
            const dialogRef = this.dialogService.open(AddToBlockListModalComponent, {
                data: {
                    programID,
                    projectID: row.ProjectID,
                    projectName: row.ProjectName ?? "",
                    projectGisIdentifier: row.ProjectGisIdentifier ?? "",
                } as AddToBlockListModalData,
            });

            dialogRef.afterClosed$.subscribe((result) => {
                if (result) {
                    this.alertService.pushAlert(new Alert("Project added to block list.", AlertContext.Success, true));
                    this.refreshBlockList$.next();
                }
            });
        });
    }

    // --- Notifications ---

    openCreateNotificationModal(programID: number): void {
        const dialogRef = this.dialogService.open(ProgramNotificationModalComponent, {
            data: { programID } as ProgramNotificationModalData,
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.alertService.pushAlert(new Alert("Notification configuration created.", AlertContext.Success, true));
                this.refreshNotifications$.next();
            }
        });
    }

    openEditNotificationModal(row: ProgramNotificationGridRow): void {
        this.programID$.pipe(take(1)).subscribe((programID) => {
            const dialogRef = this.dialogService.open(ProgramNotificationModalComponent, {
                data: { programID, notification: row } as ProgramNotificationModalData,
            });

            dialogRef.afterClosed$.subscribe((result) => {
                if (result) {
                    this.alertService.pushAlert(new Alert("Notification configuration updated.", AlertContext.Success, true));
                    this.refreshNotifications$.next();
                }
            });
        });
    }

    // --- Block List ---

    private createBlockListColumnDefs(): ColDef<ProjectImportBlockListGridRow>[] {
        return [
            this.utilityFunctions.createActionsColumnDef((params) => {
                const row = params.data as ProjectImportBlockListGridRow;
                return [
                    { ActionName: "Remove", ActionHandler: () => this.confirmDeleteBlockListEntry(row), ActionIcon: "fa fa-trash" },
                ];
            }),
            this.utilityFunctions.createBasicColumnDef("Project Name", "ProjectName"),
            this.utilityFunctions.createBasicColumnDef("Project GIS Identifier", "ProjectGisIdentifier"),
            this.utilityFunctions.createBasicColumnDef("Notes", "Notes"),
        ];
    }

    openCreateBlockListEntryModal(programID: number): void {
        const dialogRef = this.dialogService.open(CreateBlockListEntryModalComponent, {
            data: { programID } as CreateBlockListEntryModalData,
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.alertService.pushAlert(new Alert("Block list entry created.", AlertContext.Success, true));
                this.refreshBlockList$.next();
            }
        });
    }

    async confirmDeleteBlockListEntry(row: ProjectImportBlockListGridRow): Promise<void> {
        const confirmed = await this.confirmService.confirm({
            title: "Confirm Remove",
            message: `Are you sure you want to remove "${row.ProjectName || row.ProjectGisIdentifier}" from the block list?`,
            buttonTextYes: "Remove",
            buttonClassYes: "btn-danger",
            buttonTextNo: "Cancel",
        });

        if (confirmed) {
            this.programService.deleteBlockListEntryProgram(row.ProgramID, row.ProjectImportBlockListID).subscribe({
                next: () => {
                    this.alertService.pushAlert(new Alert("Block list entry removed.", AlertContext.Success, true));
                    this.refreshBlockList$.next();
                },
                error: (err) => {
                    this.alertService.pushAlert(new Alert(err?.error?.message ?? "Failed to remove block list entry.", AlertContext.Danger, true));
                },
            });
        }
    }
}
