import { Component, Input, OnInit } from "@angular/core";
import { Observable, map, forkJoin, combineLatest, startWith, Subscription, of, concat, defer } from "rxjs";
import { shareReplay, tap } from "rxjs/operators";
import { AsyncPipe, CommonModule } from "@angular/common";
import { ReactiveFormsModule, FormBuilder, FormGroup } from "@angular/forms";
import { SimpleTreeComponent } from "src/app/shared/components/simple-tree/simple-tree.component";
import { IFeature } from "src/app/shared/generated/model/i-feature";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { ProjectLocationFilterTypes } from "src/app/shared/generated/enum/project-location-filter-type-enum";
import { ProjectColorByTypes } from "src/app/shared/generated/enum/project-color-by-type-enum";
import { SelectDropdownOption, FormInputOption, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { LegendColors } from "src/app/shared/models/legend-colors";
import { NgSelectModule } from "@ng-select/ng-select";
import { Router, ActivatedRoute } from "@angular/router";
import { MarkerHelper } from "src/app/shared/helpers/marker-helper";
import { ProjectStagesAsSelectDropdownOptions } from "src/app/shared/generated/enum/project-stage-enum";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";
import { ClassificationGridRow, OrganizationGridRow, ProgramGridRow, ProjectSimpleTree, ProjectTypeGridRow } from "src/app/shared/generated/model/models";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { ProgramService } from "src/app/shared/generated/api/program.service";
import { OrganizationService } from "src/app/shared/generated/api/organization.service";
import { ProjectTypeService } from "src/app/shared/generated/api/project-type.service";
import { ClassificationService } from "src/app/shared/generated/api/classification.service";
import { WADNRMapComponent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { ProjectsLayerComponent } from "src/app/shared/components/leaflet/layers/projects-layer/projects-layer.component";
import * as L from "leaflet";
import { DomSanitizer, SafeHtml } from "@angular/platform-browser";
// ButtonLoadingDirective removed from imports because it's not used in this template

@Component({
    selector: "projects-map",
    standalone: true,
    templateUrl: "./projects-map.component.html",
    styleUrls: ["./projects-map.component.scss"],
    imports: [AsyncPipe, LoadingDirective, CommonModule, ReactiveFormsModule, NgSelectModule, PageHeaderComponent, SimpleTreeComponent, WADNRMapComponent, ProjectsLayerComponent],
})
export class ProjectsMapComponent implements OnInit {
    public customRichTextTypeID: number = FirmaPageTypeEnum.ProjectMap;
    public projectPoints$: Observable<IFeature[]>;
    public mapHeight = "700px";
    // legend colors keyed by property name. Each inner map is id -> color (string keys)
    public legendColorsToUse: LegendColors = {
        ProjectStageID: { "2": "#80B2FF", "3": "#1975FF", "4": "#000066", "5": "#D6D6D6" },
    };
    public cluster: boolean = false;
    public FormFieldType = FormFieldType;

    public map: L.Map;
    public mapIsReady: boolean = false;
    public layerControl: any;

    // Color-by options (use generated ProjectColorByTypes)
    public colorByOptions: SelectDropdownOption[] = [];

    // Filter types (use property name strings matching feature properties)
    public filterTypeOptions = [
        { Value: "ProjectTypeID", Label: "Project Type" },
        // Section divider: First
        { Value: "__divider_first", Label: "", disabled: true },
        { Value: "ClassificationID", Label: "Project Themes" },
        { Value: "ProjectStageID", Label: "Project Stage" },
        // Section divider: Second
        { Value: "__divider_second", Label: "", disabled: true },
        { Value: "OrganizationID", Label: "Lead Implementer" },
        { Value: "ProgramID", Label: "Program" },
    ];
    public ProjectLocationFilterTypes = ProjectLocationFilterTypes;

    public get currentFilterPropertyName(): string | undefined {
        const raw = this.form?.get("filterType")?.value ?? this.selectedFilterType;
        const val = String(raw);
        // find by Value matching the property name
        const entry = this.filterTypeOptions.find((x) => String(x.Value) === val);
        return entry ? entry.Value : undefined;
    }
    // Default to Project Stage on load; query params (if present) will override in ngOnInit.
    public selectedFilterType: string = "ProjectStageID";

    // Multi-select filter values derived from projectPoints
    public filterValueOptions: FormInputOption[] = [];
    public filterValueOptions$: Observable<FormInputOption[]>;

    // Reactive form
    public form: FormGroup;

    // cached selected filter values (stable reference)
    private _selectedFilterValuesCache: string[] = [];
    private _filterValuesSub: Subscription | null = null;

    // projects with no simple location list visibility
    public showProjectWithNoSimpleLocationList = false;

    constructor(
        private fb: FormBuilder,
        private projectService: ProjectService,
        private projectTypeService: ProjectTypeService,
        private programService: ProgramService,
        private classificationService: ClassificationService,
        private organizationService: OrganizationService,
        private sanitizer: DomSanitizer,
        private router: Router,
        private route: ActivatedRoute
    ) {
        this.form = this.fb.group({
            colorBy: ["ProjectStageID"],
            filterType: [this.selectedFilterType],
            filterValues: [[]],
            cluster: [this.cluster],
        });
    }

    ngOnInit(): void {
        // Read any incoming query params and apply to form before domains load
        const qp = this.route.snapshot.queryParamMap;
        const qpColorBy = qp.get("colorBy");
        const qpFilterType = qp.get("filterType");
        const qpFilterValues = qp.get("filterValues");
        const qpCluster = qp.get("cluster");
        if (qpColorBy) {
            this.form.get("colorBy")?.setValue(qpColorBy);
        }
        if (qpFilterType) {
            // keep selectedFilterType in sync for initial load
            this.selectedFilterType = qpFilterType;
            this.form.get("filterType")?.setValue(qpFilterType);
        }
        if (qpFilterValues) {
            // expected as comma-separated values
            const arr = qpFilterValues
                .split(",")
                .map((s) => s)
                .filter((s) => s !== "");
            this.form.get("filterValues")?.setValue(arr);
            this._selectedFilterValuesCache = [...arr];
        }
        if (qpCluster !== null) {
            // treat any truthy value as true
            const c = qpCluster === "1" || qpCluster.toLowerCase() === "true";
            this.form.get("cluster")?.setValue(c);
            this.cluster = c;
        }
        // derive color-by options from generated ProjectColorByTypes (map to property names)
        this.colorByOptions = ProjectColorByTypes.filter((x) => x.DisplayName == "Stage").map(
            (x: any) => ({ Value: String((x.Name ?? x.Name) + "ID"), Label: x.DisplayName } as SelectDropdownOption)
        );

        this.projectPoints$ = this.projectService.listMappedPointsFeatureCollectionProject().pipe(shareReplay(1));

        // create a domains$ observable that loads all three domain lists and caches the result
        this.domains$ = forkJoin({
            projectTypes: this.projectTypeService.listProjectType(),
            programs: this.programService.listProgram(),
            classifications: this.classificationService.listClassification(),
            organizations: this.organizationService.listOrganization(),
        }).pipe(
            map((r: any) => ({
                projectTypes: (r.projectTypes || []).map((i: ProjectTypeGridRow) => ({
                    Value: String(i.ProjectTypeID),
                    Label: i.ProjectTypeName,
                })),
                programs: (r.programs || []).map((i: ProgramGridRow) => ({
                    Value: String(i.ProgramID),
                    Label: i.ProgramName,
                })),
                classifications: (r.classifications || []).map((i: ClassificationGridRow) => ({
                    Value: String(i.ClassificationID),
                    Label: i.DisplayName,
                })),
                organizations: (r.organizations || []).map((i: OrganizationGridRow) => ({
                    Value: String(i.OrganizationID),
                    Label: i.OrganizationName,
                })),
            })),
            tap((d: any) => {
                // populate caches when the domains observable emits (template async will subscribe)
                this._projectTypeCache = d.projectTypes;
                this._programCache = d.programs;
                this._classificationCache = d.classifications;
                this._organizationCache = d.organizations;
                this.loadDomainForSelectedFilterType(this.selectedFilterType);

                // set initial form values based on caches, but don't overwrite any values
                const currentColorBy = this.form.get("colorBy")?.value;
                if (!currentColorBy) {
                    this.form.patchValue({ colorBy: this.colorByOptions[0].Value, cluster: this.cluster });
                } else {
                    // ensure cluster default is applied if nothing set
                    this.form.patchValue({ cluster: this.cluster });
                }
                const currentFilterVals = this.form.get("filterValues")?.value;
                if (!currentFilterVals || (Array.isArray(currentFilterVals) && currentFilterVals.length === 0)) {
                    this.form.get("filterValues")?.setValue(this.filterValueOptions.map((f) => String(f.Value)));
                }

                // If query params provided filter values earlier, re-apply them now that domain lists are available.
                // Intersect cached values with the available options for the currently selected filterType
                if (this._selectedFilterValuesCache && this._selectedFilterValuesCache.length) {
                    const selType = String(this.form.get("filterType")?.value ?? this.selectedFilterType);
                    let availableValues: string[] = [];
                    if (selType === "ProjectTypeID") availableValues = (this._projectTypeCache || []).map((o) => String(o.Value));
                    else if (selType === "ProgramID") availableValues = (this._programCache || []).map((o) => String(o.Value));
                    else if (selType === "ClassificationID") availableValues = (this._classificationCache || []).map((o) => String(o.Value));
                    else if (selType === "ProjectStageID") availableValues = (this._projectStageCache || []).map((o) => String(o.Value));
                    else if (selType === "OrganizationID") availableValues = (this._organizationCache || []).map((o) => String(o.Value));
                    const intersection = this._selectedFilterValuesCache.filter((v) => availableValues.includes(String(v)));
                    if (intersection.length) {
                        const ctrlVals = this.form.get("filterValues")?.value;
                        const ctrlArr = Array.isArray(ctrlVals) ? ctrlVals : [];
                        if (!this.arraysEqual(ctrlArr, intersection)) {
                            this.form.get("filterValues")?.setValue([...intersection]);
                        }
                    }
                }
            }),
            shareReplay(1)
        );

        this.filterValueOptions$ = combineLatest([this.domains$, this.form.get("filterType")!.valueChanges.pipe(startWith(this.form.get("filterType")!.value))]).pipe(
            map(([d, type]: any) => {
                const t = String(type);
                if (t === "ProjectTypeID") return d.projectTypes || [];
                if (t === "ProgramID") return d.programs || [];
                if (t === "ClassificationID") return d.classifications || [];
                if (t === "ProjectStageID") return this._projectStageCache || [];
                if (t === "OrganizationID") return d.organizations || [];
                return [];
            }),
            tap((opts) => {
                // update local snapshot and default-select all values
                this.filterValueOptions = opts;
                const vals = (opts || []).map((o: any) => String(o.Value));
                // Only set defaults when the form control is empty (don't overwrite query-supplied values)
                const currentVals = this.form.get("filterValues")?.value;
                if (!currentVals || (Array.isArray(currentVals) && currentVals.length === 0)) {
                    this.form.get("filterValues")?.setValue([...vals]);
                }
            }),
            shareReplay(1)
        );

        // When the user explicitly changes the filterType, clear and default-select all values for the new type.
        this.form.get("filterType")!.valueChanges.subscribe((val) => {
            // force defaults for the newly selected type
            this.loadDomainForSelectedFilterType(val, true);
        });

        // subscribe once to filterValues and cache the array to avoid creating new references on each CD cycle
        this._filterValuesSub = this.form.get("filterValues")!.valueChanges.subscribe((vals: any[]) => {
            const arr = Array.isArray(vals) ? vals : [];
            // only update cache when values actually change
            if (!this.arraysEqual(arr, this._selectedFilterValuesCache)) {
                this._selectedFilterValuesCache = [...arr];
            }
        });
    }

    // expose domains$ for potential template usage (async) if needed
    public domains$: Observable<{
        projectTypes: any[];
        programs: any[];
        classifications: any[];
        organizations?: any[];
    }>;

    // simple local caches populated at startup
    private _projectTypeCache: SelectDropdownOption[] = [];
    private _programCache: SelectDropdownOption[] = [];
    private _classificationCache: SelectDropdownOption[] = [];
    private _organizationCache: SelectDropdownOption[] = [];
    private _projectStageCache: SelectDropdownOption[] = ProjectStagesAsSelectDropdownOptions.filter((x) => x.Label !== "Terminated" && x.Label !== "Deferred");

    public get selectedFilterValues(): string[] {
        // return the cached array reference (stable unless values actually change)
        return this._selectedFilterValuesCache;
    }

    // Share URL state
    public shareUrl: string = "";
    public copySuccess: boolean = false;
    public showSharePopup: boolean = false;

    // Build a shareable URL containing current filters as query parameters
    public buildShareUrl(): string {
        try {
            const url = new URL(window.location.href);
            const params = url.searchParams;
            // colorBy
            const colorBy = this.form.get("colorBy")?.value;
            if (colorBy) params.set("colorBy", String(colorBy));
            else params.delete("colorBy");
            // filterType
            const filterType = this.form.get("filterType")?.value;
            if (filterType) params.set("filterType", String(filterType));
            else params.delete("filterType");
            // filterValues (comma separated)
            const filterValues = Array.isArray(this.form.get("filterValues")?.value) ? this.form.get("filterValues")?.value : [];
            if (filterValues && filterValues.length) params.set("filterValues", filterValues.map((v: any) => String(v)).join(","));
            else params.delete("filterValues");
            // cluster
            const clusterVal = this.form.get("cluster")?.value;
            if (clusterVal) params.set("cluster", clusterVal ? "1" : "0");
            else params.delete("cluster");

            // return full href
            return url.toString();
        } catch (e) {
            // fallback: build relative url
            const qp: string[] = [];
            const colorBy = this.form.get("colorBy")?.value;
            if (colorBy) qp.push(`colorBy=${encodeURIComponent(String(colorBy))}`);
            const filterType = this.form.get("filterType")?.value;
            if (filterType) qp.push(`filterType=${encodeURIComponent(String(filterType))}`);
            const filterValues = Array.isArray(this.form.get("filterValues")?.value) ? this.form.get("filterValues")?.value : [];
            if (filterValues && filterValues.length) qp.push(`filterValues=${encodeURIComponent(filterValues.map((v: any) => String(v)).join(","))}`);
            const clusterVal = this.form.get("cluster")?.value;
            if (clusterVal) qp.push(`cluster=${clusterVal ? "1" : "0"}`);
            return `${window.location.pathname}${qp.length ? "?" + qp.join("&") : ""}`;
        }
    }

    // Copy share URL to clipboard and update UI state
    public async copyShareUrl() {
        this.shareUrl = this.buildShareUrl();
        try {
            if (navigator && (navigator as any).clipboard && typeof (navigator as any).clipboard.writeText === "function") {
                await (navigator as any).clipboard.writeText(this.shareUrl);
                this.copySuccess = true;
                setTimeout(() => (this.copySuccess = false), 2500);
            } else {
                // fallback: create temporary input
                const ta = document.createElement("textarea");
                ta.value = this.shareUrl;
                ta.style.position = "fixed";
                ta.style.left = "-9999px";
                document.body.appendChild(ta);
                ta.select();
                try {
                    document.execCommand("copy");
                    this.copySuccess = true;
                    setTimeout(() => (this.copySuccess = false), 2500);
                } finally {
                    document.body.removeChild(ta);
                }
            }
        } catch (e) {
            this.copySuccess = false;
        }
    }

    // Toggle the share popup and ensure URL is up to date
    public toggleSharePopup() {
        this.showSharePopup = !this.showSharePopup;
        if (this.showSharePopup) {
            this.shareUrl = this.buildShareUrl();
        }
    }

    private arraysEqual(a: any[], b: any[]) {
        if (a === b) return true;
        if (!a || !b) return false;
        if (a.length !== b.length) return false;
        for (let i = 0; i < a.length; i++) {
            if (String(a[i]) !== String(b[i])) return false;
        }
        return true;
    }

    private loadDomainForSelectedFilterType(filterTypeValue: string | number, forceDefaults: boolean = false) {
        const t = String(filterTypeValue);
        const currentVals = this.form.get("filterValues")?.value;
        const isEmpty = !currentVals || (Array.isArray(currentVals) && currentVals.length === 0);
        const shouldSetDefaults = forceDefaults || isEmpty;
        if (t === "ProjectTypeID") {
            this.filterValueOptions = this._projectTypeCache;
            // update form control when caches are ready (only if empty)
            if (shouldSetDefaults) this.form.get("filterValues")?.setValue(this._projectTypeCache.map((o) => String(o.Value)));
        } else if (t === "ProgramID") {
            this.filterValueOptions = this._programCache;
            if (shouldSetDefaults) this.form.get("filterValues")?.setValue(this._programCache.map((o) => String(o.Value)));
        } else if (t === "ClassificationID") {
            this.filterValueOptions = this._classificationCache;
            if (shouldSetDefaults) this.form.get("filterValues")?.setValue(this._classificationCache.map((o) => String(o.Value)));
        } else if (t === "ProjectStageID") {
            this.filterValueOptions = this._projectStageCache;
            if (shouldSetDefaults) this.form.get("filterValues")?.setValue(this._projectStageCache.map((o) => String(o.Value)));
        } else if (t === "OrganizationID") {
            this.filterValueOptions = this._organizationCache;
            if (shouldSetDefaults) this.form.get("filterValues")?.setValue(this._organizationCache.map((o) => String(o.Value)));
        } else {
            this.filterValueOptions = [];
            if (shouldSetDefaults) this.form.get("filterValues")?.setValue([]);
        }
    }

    onFilterTypeChange(value: string) {
        // keep legacy handler but actual state is driven from the reactive form's valueChanges subscription
        this.form.get("filterType")?.setValue(value);
    }

    toggleProjectsWithNoSimpleLocationList() {
        this.showProjectWithNoSimpleLocationList = !this.showProjectWithNoSimpleLocationList;
    }

    // Projects with no simple location arranged as a tree grouped by ProjectType -> projects
    // Each group node includes a `count` of descendant projects.
    public noSimpleLocationTree$: Observable<any[]> = of([]);

    public handleMapReady(event: any): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
    }

    ngAfterViewInit(): void {
        // Build a tree structure suitable for the Angular tree component.
        this.noSimpleLocationTree$ = this.projectService.listProjectsWithNoSimpleLocationProject().pipe(
            map((results) => {
                // group by ProjectTypeLookupItem (prefer stable ID when present)
                const groups = new Map<
                    string,
                    {
                        title: string;
                        projectTypeId?: number;
                        projects: ProjectSimpleTree[];
                    }
                >();

                const getGroupKey = (projectTypeId: number | undefined, title: string) => {
                    if (projectTypeId !== null && projectTypeId !== undefined) return `id:${projectTypeId}`;
                    return `name:${String(title || "")
                        .trim()
                        .toLowerCase()}`;
                };

                (results || []).forEach((p) => {
                    const title = String(p?.ProjectType?.ProjectTypeName || "(Unspecified)");
                    const projectTypeId = p?.ProjectType?.ProjectTypeID;
                    const key = getGroupKey(projectTypeId, title);
                    const existing = groups.get(key);
                    if (existing) {
                        existing.projects.push(p);
                    } else {
                        groups.set(key, { title, projectTypeId, projects: [p] });
                    }
                });

                // Sort groups by title and build nodes
                const sortedGroups = Array.from(groups.values()).sort((a, b) => a.title.localeCompare(b.title));
                const tree: any[] = sortedGroups.map((g, gi) => {
                    const grp = g.title;
                    const projectTypeId = g.projectTypeId;
                    const projects = g.projects || [];

                    // sort projects by name, then ID for stability
                    const projectsSorted = projects.slice().sort((p1, p2) => {
                        const name1 = String(p1?.ProjectName || "");
                        const name2 = String(p2?.ProjectName || "");
                        const nameCmp = name1.localeCompare(name2);
                        if (nameCmp !== 0) return nameCmp;
                        const id1 = Number(p1?.ProjectID ?? 0);
                        const id2 = Number(p2?.ProjectID ?? 0);
                        return id1 - id2;
                    });

                    const sanitized = (str: string) =>
                        String(str || "")
                            .replace(/[^a-z0-9_-]/gi, "-")
                            .toLowerCase();
                    const groupKeyBase = sanitized(grp);

                    const projectChildren = projectsSorted.map((p, pi) => {
                        const projName = String(p?.ProjectName || "Unnamed project");
                        const projId = p?.ProjectID;

                        const parts: any[] = [];
                        parts.push({ text: projName, routerLink: ["/projects/fact-sheet", projId] });

                        return {
                            title: projName,
                            key: `proj-${groupKeyBase}__${String(projId ?? pi)}`,
                            folder: false,
                            titleParts: parts,
                        };
                    });

                    return {
                        title: `${grp}`,
                        key: `group-${String(projectTypeId ?? String(grp)).replace(/[^a-z0-9_-]/gi, "-")}-${gi}`,
                        routerLink: projectTypeId !== null && projectTypeId !== undefined ? ["/project-types", projectTypeId] : undefined,
                        children: projectChildren,
                        count: projectChildren.length,
                        folder: true,
                    };
                });

                return tree;
            }),
            shareReplay(1)
        );
    }
    ngOnDestroy(): void {
        if (this._filterValuesSub) {
            this._filterValuesSub.unsubscribe();
            this._filterValuesSub = null;
        }
    }

    // Resolve a legend label (id -> human label) for a given property name using
    // the cached domain lists. Returns the key itself when no label is found.
    public legendLabel(propertyName: string | undefined, key: string): string {
        if (!propertyName || !key) return key;
        const k = String(key);
        if (propertyName === "ProjectTypeID") {
            const found = this._projectTypeCache.find((x) => String(x.Value) === k);
            return found ? found.Label : k;
        }
        if (propertyName === "ProjectStageID") {
            const found = this._projectStageCache.find((x) => String(x.Value) === k);
            return found ? found.Label : k;
        }
        if (propertyName === "OrganizationID") {
            const found = this._organizationCache.find((x) => String(x.Value) === k);
            return found ? found.Label : k;
        }
        return k;
    }

    // Return legend entries for a given propertyName as an array of { id, html, label }
    public legendEntries(propertyName: string | undefined): Array<{ id: string; html: SafeHtml; label: string }> {
        if (!propertyName) return [];
        const palette = (this.legendColorsToUse && (this.legendColorsToUse as any)[propertyName]) || {};
        return Object.keys(palette || {}).map((k) => {
            const color = palette[k];
            const icon = MarkerHelper.circleDivIcon(color) as any;
            const rawHtml: string = String(icon?.options?.html ?? "");
            return { id: k, html: this.sanitizer.bypassSecurityTrustHtml(rawHtml), label: this.legendLabel(propertyName, k) };
        });
    }

    // Select all available filter values (useful for ng-select "Select All")
    public selectAllFilterValues() {
        const vals = (this.filterValueOptions || []).map((o: any) => o.Value);
        this.form.get("filterValues")?.setValue([...vals]);
    }

    // Deselect all filter values
    public deselectAllFilterValues() {
        this.form.get("filterValues")?.setValue([]);
    }

    // Toggle a single filter value in the form control (keeps stable cache updates)
    public toggleFilterValue(value: string) {
        const ctrl = this.form.get("filterValues");
        if (!ctrl) return;
        const arr = Array.isArray(ctrl.value) ? [...ctrl.value] : [];
        const s = String(value);
        const idx = arr.findIndex((v) => String(v) === s);
        if (idx >= 0) {
            arr.splice(idx, 1);
        } else {
            arr.push(s);
        }
        ctrl.setValue(arr);
    }

    // Return whether a given filter value is currently selected in the form control
    public isFilterValueSelected(value: string): boolean {
        const ctrl = this.form.get("filterValues");
        if (!ctrl) return false;
        const arr = Array.isArray(ctrl.value) ? ctrl.value : [];
        return arr.some((v) => String(v) === String(value));
    }
}
