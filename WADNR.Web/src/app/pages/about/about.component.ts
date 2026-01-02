import { Component, OnInit } from "@angular/core";
import { CustomPageComponent } from "src/app/shared/components/custom-page/custom-page.component";

@Component({
    selector: "about",
    standalone: true,
    templateUrl: "./about.component.html",
    styleUrls: ["./about.component.scss"],
    imports: [CustomPageComponent],
})
export class AboutComponent implements OnInit {
    constructor() {}

    ngOnInit(): void {}
}
