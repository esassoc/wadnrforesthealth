import { Component, OnInit } from "@angular/core";
import { environment } from "src/environments/environment";
import { RouterLink, RouterLinkActive } from "@angular/router";
import { CommonModule } from "@angular/common";

@Component({
    selector: "header-nav",
    templateUrl: "./header-nav.component.html",
    styleUrls: ["./header-nav.component.scss"],
    imports: [CommonModule, RouterLink, RouterLinkActive],
})
export class HeaderNavComponent implements OnInit {
    constructor() {}

    ngOnInit() {}

    public showTestingWarning(): boolean {
        return environment.staging || environment.dev;
    }

    public testingWarningText(): string {
        return environment.staging ? "Environment: <strong>QA</strong>" : "Environment: <strong>DEV</strong>";
    }
}
