import { AsyncPipe } from "@angular/common";
import { Component } from "@angular/core";
import { RouterModule } from "@angular/router";
import { map, Observable } from "rxjs";

import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { TruncateWordsPipe } from "src/app/shared/pipes/truncate-words.pipe";
import { ClassificationService } from "src/app/shared/generated/api/classification.service";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";
import { ClassificationWithProjectCount } from "src/app/shared/generated/model/classification-with-project-count";

@Component({
    selector: "classifications",
    standalone: true,
    imports: [PageHeaderComponent, AlertDisplayComponent, RouterModule, AsyncPipe, TruncateWordsPipe],
    templateUrl: "./classifications.component.html",
    styleUrls: ["./classifications.component.scss"],
})
export class ClassificationsComponent {
    public classifications$: Observable<ClassificationWithProjectCount[]>;

    public customRichTextTypeID: number = FirmaPageTypeEnum.Classifications;

    constructor(private classificationService: ClassificationService) {}

    ngOnInit(): void {
        this.classifications$ = this.classificationService.listWithProjectCountClassification();
    }

    public tileColor(classification: ClassificationWithProjectCount): string {
        return classification?.ThemeColor || "var(--card-body-bg-color)";
    }
}
