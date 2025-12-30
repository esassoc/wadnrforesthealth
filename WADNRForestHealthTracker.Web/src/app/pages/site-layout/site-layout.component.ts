import { Component, OnInit } from "@angular/core";
import { RouterLink, RouterLinkActive, RouterOutlet } from "@angular/router";
import { DropdownToggleDirective } from "src/app/shared/directives/dropdown-toggle.directive";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { HeaderNavComponent } from "../../shared/components/header-nav/header-nav.component";

@Component({
    selector: "site-layout",
    templateUrl: "./site-layout.component.html",
    styleUrls: ["./site-layout.component.scss"],
    imports: [RouterLink, RouterLinkActive, RouterOutlet, DropdownToggleDirective, IconComponent, HeaderNavComponent],
})
export class SiteLayoutComponent implements OnInit {
    public userHasProjectPermission: boolean = true;

    constructor() {}

    ngOnInit() {}
}
