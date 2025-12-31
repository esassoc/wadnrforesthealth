import { Component, OnInit } from "@angular/core";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";

@Component({
    selector: "about",
    templateUrl: "./about.component.html",
    styleUrls: ["./about.component.scss"],
    imports: [PageHeaderComponent],
})
export class AboutComponent implements OnInit {
    constructor() {}

    public customRichTextTypeID: number = FirmaPageTypeEnum.FindYourForester;

    ngOnInit(): void {}
}
