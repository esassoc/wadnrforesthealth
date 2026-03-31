import { Component, OnInit } from "@angular/core";
import { PageHeaderComponent } from "../../components/page-header/page-header.component";

@Component({
    selector: "not-found",
    templateUrl: "./not-found.component.html",
    styleUrls: ["./not-found.component.scss"],
    imports: [PageHeaderComponent],
})
export class NotFoundComponent implements OnInit {
    constructor() {}

    ngOnInit() {}
}
