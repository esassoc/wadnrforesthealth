import { Component, OnInit, Input, ViewChild, AfterViewChecked, inject, DestroyRef } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { AlertService } from "../../services/alert.service";
import { Alert } from "../../models/alert";
import { AlertContext } from "../../models/enums/alert-context.enum";
import { EditorComponent, TINYMCE_SCRIPT_SRC } from "@tinymce/tinymce-angular";
import TinyMCEHelpers from "../../helpers/tiny-mce-helpers";
import { DomSanitizer, SafeHtml } from "@angular/platform-browser";
import { CustomRichTextService } from "src/app/shared/generated/api/custom-rich-text.service";
import { FormsModule } from "@angular/forms";
import { AsyncPipe } from "@angular/common";
import { BehaviorSubject, map } from "rxjs";

import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { FirmaPageUpsertRequest } from "../../generated/model/models";
import { AuthenticationService } from "src/app/services/authentication.service";

@Component({
    selector: "custom-rich-text",
    templateUrl: "./custom-rich-text.component.html",
    styleUrls: ["./custom-rich-text.component.scss"],
    imports: [LoadingDirective, FormsModule, EditorComponent, AsyncPipe],
    providers: [{ provide: TINYMCE_SCRIPT_SRC, useValue: "tinymce/tinymce.min.js" }],
})
export class CustomRichTextComponent implements OnInit, AfterViewChecked {
    @Input() customRichTextTypeID: number;
    @Input() showLoading: boolean = true;
    @Input() showInfoIcon: boolean = true;
    @ViewChild("tinyMceEditor") tinyMceEditor: EditorComponent;
    public tinyMceConfig: object;

    private destroyRef = inject(DestroyRef);

    private isLoadingSubject = new BehaviorSubject<boolean>(true);
    public isLoading$ = this.isLoadingSubject.asObservable();

    private isEditingSubject = new BehaviorSubject<boolean>(false);
    public isEditing$ = this.isEditingSubject.asObservable();

    private customRichTextContentSubject = new BehaviorSubject<SafeHtml>(null);
    public customRichTextContent$ = this.customRichTextContentSubject.asObservable();

    private isEmptyContentSubject = new BehaviorSubject<boolean>(false);
    public isEmptyContent$ = this.isEmptyContentSubject.asObservable();

    public editedContent: string;

    public canEdit$ = this.authenticationService.currentUserSetObservable.pipe(
        map(user => this.authenticationService.canManagePageContent(user))
    );

    constructor(
        private customRichTextService: CustomRichTextService,
        private alertService: AlertService,
        private sanitizer: DomSanitizer,
        private authenticationService: AuthenticationService
    ) {}

    ngAfterViewChecked(): void {
        this.tinyMceConfig = TinyMCEHelpers.DefaultInitConfig(this.tinyMceEditor);
    }

    ngOnInit() {
        this.customRichTextService.getCustomRichText(this.customRichTextTypeID)
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe({
                next: (customRichText) => this.loadCustomRichText(customRichText),
                error: () => this.isLoadingSubject.next(false)
            });
    }

    private loadCustomRichText(customRichText: { FirmaPageContent?: string }) {
        const content = customRichText.FirmaPageContent || "";
        this.customRichTextContentSubject.next(this.sanitizer.bypassSecurityTrustHtml(content));
        this.editedContent = content;
        this.isEmptyContentSubject.next(!content || content.trim().length === 0);
        this.isLoadingSubject.next(false);
    }

    public enterEdit(): void {
        this.isEditingSubject.next(true);
    }

    public cancelEdit(): void {
        this.isEditingSubject.next(false);
    }

    public saveEdit(): void {
        this.isEditingSubject.next(false);
        this.isLoadingSubject.next(true);
        const upsertRequest: FirmaPageUpsertRequest = {
            FirmaPageContent: this.editedContent
        };
        this.customRichTextService.updateCustomRichText(this.customRichTextTypeID, upsertRequest)
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe({
                next: (x) => this.loadCustomRichText(x),
                error: () => {
                    this.isLoadingSubject.next(false);
                    this.alertService.pushAlert(new Alert("There was an error updating the rich text content", AlertContext.Danger, true));
                }
            });
    }
}
