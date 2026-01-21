import { AsyncPipe } from "@angular/common";
import { Component } from "@angular/core";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { BehaviorSubject, distinctUntilChanged, filter, map, Observable, shareReplay, switchMap } from "rxjs";
import { ColDef } from "ag-grid-community";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { environment } from "src/environments/environment";

import { ProgramService } from "src/app/shared/generated/api/program.service";
import { ProgramDetail } from "src/app/shared/generated/model/program-detail";
import { ProjectProgramDetailGridRow } from "src/app/shared/generated/model/project-program-detail-grid-row";

@Component({
    selector: "program-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, BreadcrumbComponent, WADNRGridComponent, RouterLink],
    templateUrl: "./program-detail.component.html",
    styleUrls: ["./program-detail.component.scss"],
})
export class ProgramDetailComponent {
    public programID$: Observable<number>;
    public program$: Observable<ProgramDetail>;
    public projects$: Observable<ProjectProgramDetailGridRow[]>;

    public projectColumnDefs: ColDef<ProjectProgramDetailGridRow>[] = [];

    private refreshData$ = new BehaviorSubject<void>(undefined);

    constructor(
        private route: ActivatedRoute,
        private programService: ProgramService,
        private utilityFunctions: UtilityFunctionsService
    ) {}

    ngOnInit(): void {
        this.programID$ = this.route.paramMap.pipe(
            map((p) => (p.get("programID") ? Number(p.get("programID")) : null)),
            filter((programID): programID is number => programID != null && !Number.isNaN(programID)),
            distinctUntilChanged(),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.program$ = this.programID$.pipe(
            switchMap((programID) => this.programService.getProgram(programID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projects$ = this.programID$.pipe(
            switchMap((programID) => this.programService.listProjectsProgram(programID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projectColumnDefs = this.createProjectColumnDefs();
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
}
