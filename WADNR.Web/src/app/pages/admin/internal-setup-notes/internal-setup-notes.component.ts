import { Component } from "@angular/core";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";

@Component({
  selector: "internal-setup-notes",
  standalone: true,
  imports: [PageHeaderComponent, CustomRichTextComponent],
  templateUrl: "./internal-setup-notes.component.html",
})
export class InternalSetupNotesComponent {
  public customRichTextTypeID = FirmaPageTypeEnum.InternalSetupNotes;
}
