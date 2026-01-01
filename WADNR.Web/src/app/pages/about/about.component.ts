import { Component, OnInit } from "@angular/core";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";
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

    public customPageID: number = FirmaPageTypeEnum.FindYourForester;

    ngOnInit(): void {}
}
