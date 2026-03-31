import { Component } from "@angular/core";
import { RouterLink } from "@angular/router";
import { AgRendererComponent } from "ag-grid-angular";

@Component({
    selector: "person-org-link-renderer",
    standalone: true,
    imports: [RouterLink],
    template: `
        @if (personName) {
            <a [routerLink]="['/people', personID]">{{ personName }}</a>
            @if (orgShortName) {
                (<a [routerLink]="['/organizations', orgID]">{{ orgShortName }}</a>)
            }
        }
    `,
})
export class PersonOrgLinkRendererComponent implements AgRendererComponent {
    personID: number;
    personName: string;
    orgID: number;
    orgShortName: string;

    agInit(params: any): void {
        this.personID = params.data?.PersonID;
        this.personName = params.data?.PersonName;
        this.orgID = params.data?.OrganizationID;
        this.orgShortName = params.data?.OrganizationShortName;
    }

    refresh(params: any): boolean {
        return false;
    }
}
