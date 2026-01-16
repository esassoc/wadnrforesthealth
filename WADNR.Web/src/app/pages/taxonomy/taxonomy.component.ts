import { Component } from "@angular/core";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";
import { AsyncPipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { SimpleTreeComponent, SimpleTreeNode } from "src/app/shared/components/simple-tree/simple-tree.component";
import { Router } from "@angular/router";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";
import { ProjectTypeService } from "src/app/shared/generated/api/project-type.service";
import { ProjectTypeTaxonomy } from "src/app/shared/generated/model/project-type-taxonomy";
import { ProjectTypeLookupItem } from "src/app/shared/generated/model/project-type-lookup-item";
import { ProjectLookupItem } from "src/app/shared/generated/model/project-lookup-item";

@Component({
    selector: "taxonomy",
    standalone: true,
    imports: [PageHeaderComponent, AlertDisplayComponent, SimpleTreeComponent, AsyncPipe],
    templateUrl: "./taxonomy.component.html",
    styleUrls: ["./taxonomy.component.scss"],
})
export class TaxonomyComponent {
    public customRichTextTypeID: number = FirmaPageTypeEnum.Taxonomy;

    public nodes$: Observable<SimpleTreeNode[]>;

    constructor(private projectTypeService: ProjectTypeService, private router: Router) {}

    ngOnInit(): void {
        this.nodes$ = this.projectTypeService.taxonomyProjectType().pipe(map((areas) => this.mapAreasToNodes(areas || [])));
    }

    private mapAreasToNodes(areas: ProjectTypeTaxonomy[]): SimpleTreeNode[] {
        return areas.map((a) => {
            // project type base and parts
            const ptName = a.ProjectTypeName;
            const ptParts: any[] = [];
            ptParts.push({ text: ptName, routerLink: ["/project-types", a.ProjectTypeID], additionalCssClasses: ["no-flex"] });
            ptParts.push({
                text: "Map",
                href: [`/projects/map?filterType=ProjectTypeID&filterValues=${a.ProjectTypeID}`],
                icon: "MapMarker",
                additionalCssClasses: ["btn", "btn-sm", "map-btn"],
            });

            const sortedProjects = (a.Projects || [])
                .slice()
                .sort((x: ProjectLookupItem, y: ProjectLookupItem) => (x?.ProjectName ?? "").localeCompare(y?.ProjectName ?? "", undefined, { sensitivity: "base" }));
            const children = sortedProjects.map((proj: any) => {
                const key = `project-${proj.ProjectID ?? Math.random().toString(36).slice(2)}`;
                const nameText = proj.ProjectName;

                const parts: any[] = [];
                parts.push({ text: nameText, routerLink: ["/projects/fact-sheet", proj.ProjectID] });

                return {
                    title: nameText,
                    key,
                    data: proj,
                    titleParts: parts,
                    count: 1,
                } as any;
            });

            const ptCount = children.reduce((s: number, c: any) => s + (c.count || 0), 0);
            return {
                title: a.ProjectTypeName || "",
                key: `project-type-${a.ProjectTypeID ?? Math.random().toString(36).slice(2)}`,
                titleParts: ptParts,
                children,
                count: ptCount,
            } as SimpleTreeNode;
        });
    }

    public onSelect(feature: any | null) {
        if (!feature) return;
        // if the node contains a project DTO with ProjectID, navigate to project detail
        const proj = feature as any;
        if (proj && proj.ProjectID) {
            this.router.navigate([`/projects/fact-sheet/${proj.ProjectID}`]);
            return;
        }
        // otherwise no-op for non-project taxonomy nodes
    }
}
