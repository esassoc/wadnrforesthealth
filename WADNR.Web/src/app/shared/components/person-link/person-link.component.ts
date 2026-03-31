import { Component, Input, inject } from "@angular/core";
import { RouterLink } from "@angular/router";
import { AuthenticationService } from "src/app/services/authentication.service";

/**
 * A component that conditionally renders a link to a person's detail page.
 * If the user is authenticated, renders a clickable link.
 * If not authenticated, renders plain text.
 *
 * Usage:
 * <person-link [personID]="person.PersonID" [displayText]="person.FullName"></person-link>
 */
@Component({
    selector: "person-link",
    standalone: true,
    imports: [RouterLink],
    template: `
        @if (isAuthenticated && personID) {
            <a [routerLink]="['/people', personID]">{{ displayText }}</a>
        } @else {
            <span>{{ displayText || "—" }}</span>
        }
    `,
})
export class PersonLinkComponent {
    private authenticationService = inject(AuthenticationService);

    @Input() personID: number | null | undefined;
    @Input() displayText: string | null | undefined;

    get isAuthenticated(): boolean {
        return this.authenticationService.isAuthenticated();
    }
}
