import { Component, Input } from "@angular/core";
import { RouterLink } from "@angular/router";

import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";

@Component({
    selector: "gis-instructions-step",
    standalone: true,
    imports: [RouterLink, CustomRichTextComponent],
    templateUrl: "./instructions-step.component.html",
})
export class InstructionsStepComponent {
    @Input() attemptID: string;

    public instructionsTypeID = FirmaPageTypeEnum.GisUploadAttemptInstructions;

    getUploadLink(): string[] {
        return ["/gis-bulk-import", this.attemptID, "upload"];
    }
}
