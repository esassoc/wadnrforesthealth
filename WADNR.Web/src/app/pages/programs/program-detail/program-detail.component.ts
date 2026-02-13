import { AsyncPipe } from "@angular/common";
import { Component } from "@angular/core";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { BehaviorSubject, combineLatest, distinctUntilChanged, filter, forkJoin, map, Observable, shareReplay, switchMap } from "rxjs";
import { ColDef } from "ag-grid-community";
import { DialogService } from "@ngneat/dialog";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { PersonLinkComponent } from "src/app/shared/components/person-link/person-link.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { environment } from "src/environments/environment";

import { ProgramService } from "src/app/shared/generated/api/program.service";
import { OrganizationService } from "src/app/shared/generated/api/organization.service";
import { PersonService } from "src/app/shared/generated/api/person.service";
import { RelationshipTypeService } from "src/app/shared/generated/api/relationship-type.service";
import { ProjectStagesAsSelectDropdownOptions } from "src/app/shared/generated/enum/project-stage-enum";
import { ProgramDetail } from "src/app/shared/generated/model/program-detail";
import { ProjectProgramDetailGridRow } from "src/app/shared/generated/model/project-program-detail-grid-row";
import { ProgramNotificationGridRow } from "src/app/shared/generated/model/program-notification-grid-row";
import { GdbDefaultMappingItem } from "src/app/shared/generated/model/gdb-default-mapping-item";
import { GdbCrosswalkItem } from "src/app/shared/generated/model/gdb-crosswalk-item";
import { ProgramModalComponent, ProgramModalData } from "../program-modal/program-modal.component";
import { EditImportBasicsModalComponent, EditImportBasicsModalData } from "../edit-import-basics-modal/edit-import-basics-modal.component";
import { EditDefaultMappingsModalComponent, EditDefaultMappingsModalData } from "../edit-default-mappings-modal/edit-default-mappings-modal.component";
import { EditCrosswalkValuesModalComponent, EditCrosswalkValuesModalData } from "../edit-crosswalk-values-modal/edit-crosswalk-values-modal.component";
import { FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { GdbImportBasics } from "src/app/shared/generated/model/gdb-import-basics";

@Component({
    selector: "program-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, BreadcrumbComponent, WADNRGridComponent, RouterLink, PersonLinkComponent],
    templateUrl: "./program-detail.component.html",
    styleUrls: ["./program-detail.component.scss"],
})
export class ProgramDetailComponent {
    public programID$: Observable<number>;
    public program$: Observable<ProgramDetail>;
    public projects$: Observable<ProjectProgramDetailGridRow[]>;
    public notifications$: Observable<ProgramNotificationGridRow[]>;

    public projectColumnDefs: ColDef<ProjectProgramDetailGridRow>[] = [];
    public notificationColumnDefs: ColDef<ProgramNotificationGridRow>[] = [];

    private refreshData$ = new BehaviorSubject<void>(undefined);

    constructor(
        private route: ActivatedRoute,
        private programService: ProgramService,
        private organizationService: OrganizationService,
        private personService: PersonService,
        private relationshipTypeService: RelationshipTypeService,
        private utilityFunctions: UtilityFunctionsService,
        private dialogService: DialogService
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

        this.projects$ = this.programID$.pipe(
            switchMap((programID) => this.programService.listProjectsProgram(programID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.notifications$ = this.programID$.pipe(
            switchMap((programID) => this.programService.listNotificationsProgram(programID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projectColumnDefs = this.createProjectColumnDefs();
        this.notificationColumnDefs = this.createNotificationColumnDefs();
    }

    getFileUrl(fileResourceUrl: string | null | undefined): string | null {
        if (!fileResourceUrl) return null;
        return `${environment.mainAppApiUrl}/file-resources/${fileResourceUrl}`;
    }

    private createProjectColumnDefs(): ColDef<ProjectProgramDetailGridRow>[] {
        return [
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
        return [
            this.utilityFunctions.createBasicColumnDef("Type", "ProgramNotificationTypeDisplayName"),
            this.utilityFunctions.createBasicColumnDef("Recurrence Interval (Years)", "RecurrenceIntervalInYears"),
            this.utilityFunctions.createBasicColumnDef("Email Text", "NotificationEmailText"),
        ];
    }

    openEditModal(program: ProgramDetail): void {
        forkJoin({
            organizations: this.organizationService.listOrganization(),
            people: this.personService.listPerson()
        }).subscribe(({ organizations, people }) => {
            const dialogRef = this.dialogService.open(ProgramModalComponent, {
                data: {
                    mode: "edit",
                    program: program,
                    organizations: organizations,
                    people: people
                } as ProgramModalData,
                size: "md"
            });

            dialogRef.afterClosed$.subscribe(result => {
                if (result) {
                    this.refreshData$.next();
                }
            });
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
                this.refreshData$.next();
            }
        });
    }
}
