import { Component, OnInit, Input, ViewChild, AfterViewChecked, ChangeDetectorRef, OnDestroy } from "@angular/core";
import { AlertService } from "../../services/alert.service";
import { Alert } from "../../models/alert";
import { AlertContext } from "../../models/enums/alert-context.enum";
import { EditorComponent, TINYMCE_SCRIPT_SRC } from "@tinymce/tinymce-angular";
import TinyMCEHelpers from "../../helpers/tiny-mce-helpers";
import { DomSanitizer, SafeHtml } from "@angular/platform-browser";
import { CustomRichTextService } from "src/app/shared/generated/api/custom-rich-text.service";
import { FormsModule } from "@angular/forms";

import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { FirmaPageDetail, PersonDetail } from "../../generated/model/models";

@Component({
    selector: "custom-rich-text",
    templateUrl: "./custom-rich-text.component.html",
    styleUrls: ["./custom-rich-text.component.scss"],
    imports: [LoadingDirective, FormsModule, EditorComponent],
    providers: [{ provide: TINYMCE_SCRIPT_SRC, useValue: "tinymce/tinymce.min.js" }],
})
export class CustomRichTextComponent implements OnInit, AfterViewChecked, OnDestroy {
    @Input() customRichTextTypeID: number;
    @Input() showLoading: boolean = true;
    @Input() showInfoIcon: boolean = true;
    @ViewChild("tinyMceEditor") tinyMceEditor: EditorComponent;
    public tinyMceConfig: object;

    private currentUser: PersonDetail;

    public customRichTextTitle: string;
    public editedTitle: string;
    public showTitle: boolean = false;

    public customRichTextContent: SafeHtml;
    public editedContent: string;
    public isEmptyContent: boolean = false;

    public isLoading: boolean = true;
    public isEditing: boolean = false;

    constructor(
        private customRichTextService: CustomRichTextService,
        private publicService: CustomRichTextService,
        private alertService: AlertService,
        private sanitizer: DomSanitizer,
        private cdr: ChangeDetectorRef
    ) {}

    ngAfterViewChecked(): void {
        // We need to use ngAfterViewInit because the image upload needs a reference to the component
        // to setup the blobCache for image base64 encoding
        this.tinyMceConfig = TinyMCEHelpers.DefaultInitConfig(this.tinyMceEditor);
    }

    ngOnInit() {
        this.publicService.getCustomRichText(this.customRichTextTypeID).subscribe((x) => {
            this.loadCustomRichText(x);
        });
    }

    ngOnDestroy(): void {
        this.cdr.detach();
    }

    private loadCustomRichText(customRichText: FirmaPageDetail) {
        this.editedTitle = this.customRichTextTitle;

        this.customRichTextContent = this.sanitizer.bypassSecurityTrustHtml(customRichText.FirmaPageContent);
        this.editedContent = customRichText.FirmaPageContent || "";
        this.isEmptyContent = customRichText.FirmaPageContent === undefined || customRichText.FirmaPageContent === null || customRichText.FirmaPageContent?.trim().length === 0;
        this.isLoading = false;
        this.cdr.detectChanges();
    }

    public showEditButton(): boolean {
        return true;
        // return this.authenticationService.isUserAnAdministrator(this.currentUser);
    }

    public enterEdit(): void {
        this.isEditing = true;
    }

    public cancelEdit(): void {
        this.isEditing = false;
    }

    public saveEdit(): void {
        // this.isEditing = false;
        // this.isLoading = true;
        // const updateDto = new FirmaPageDetail({ FirmaPageContent: this.editedContent });
        // this.customRichTextService.updateCustomRichText(this.customRichTextTypeID, updateDto).subscribe(
        //     (x) => {
        //         this.loadCustomRichText(x);
        //     },
        //     (error) => {
        //         this.isLoading = false;
        //         this.alertService.pushAlert(new Alert("There was an error updating the rich text content", AlertContext.Danger, true));
        //     }
        // );
    }
}
