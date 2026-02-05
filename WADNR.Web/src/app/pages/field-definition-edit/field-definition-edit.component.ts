import { Component, OnInit, ChangeDetectorRef, ViewChild, Input } from "@angular/core";
import { Router, ActivatedRoute, RouterLink } from "@angular/router";
import { AlertService } from "src/app/shared/services/alert.service";
import { FieldDefinitionDatumDetail, FieldDefinitionDatumUpsertRequest, PersonDetail } from "src/app/shared/generated/model/models";
import { FieldDefinitionService } from "src/app/shared/generated/api/field-definition.service";
import { EditorComponent, EditorModule, TINYMCE_SCRIPT_SRC } from "@tinymce/tinymce-angular";
import TinyMCEHelpers from "src/app/shared/helpers/tiny-mce-helpers";
import { FormsModule } from "@angular/forms";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

import { PageHeaderComponent } from "../../shared/components/page-header/page-header.component";

@Component({
    selector: "field-definition-edit",
    templateUrl: "./field-definition-edit.component.html",
    styleUrls: ["./field-definition-edit.component.scss"],
    imports: [RouterLink, EditorModule, FormsModule, PageHeaderComponent],
    providers: [{ provide: TINYMCE_SCRIPT_SRC, useValue: "tinymce/tinymce.min.js" }],
})
export class FieldDefinitionEditComponent implements OnInit {
    @Input() definitionID: number;
    private currentUser: PersonDetail;

    public fieldDefinitionDatum: FieldDefinitionDatumDetail;
    public editor;
    @ViewChild("tinyMceEditor") tinyMceEditor: EditorComponent;
    public tinyMceConfig: object;

    public isLoadingSubmit: boolean;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private alertService: AlertService,
        private fieldDefinitionService: FieldDefinitionService,
        private cdr: ChangeDetectorRef
    ) {}

    ngOnInit() {
        if (this.definitionID) {
            this.fieldDefinitionService.getFieldDefinition(this.definitionID).subscribe((fieldDefinitionDatum) => {
                this.fieldDefinitionDatum = fieldDefinitionDatum;
            });
        }
    }

    ngAfterViewInit(): void {
        // We need to use ngAfterViewInit because the image upload needs a reference to the component
        // to setup the blobCache for image base64 encoding
        this.tinyMceConfig = TinyMCEHelpers.DefaultInitConfig(this.tinyMceEditor);
    }

    ngOnDestroy() {
        this.cdr.detach();
    }

    saveDefinition(): void {
        this.isLoadingSubmit = true;
        const upsertRequest: FieldDefinitionDatumUpsertRequest = {
            FieldDefinitionDatumValue: this.fieldDefinitionDatum.FieldDefinitionDatumValue
        };
        this.fieldDefinitionService.updateFieldDefinition(
            this.fieldDefinitionDatum.FieldDefinition.FieldDefinitionID,
            upsertRequest
        ).subscribe({
            next: (response) => {
                this.isLoadingSubmit = false;
                this.router.navigateByUrl("/labels-and-definitions").then(() => {
                    this.alertService.pushAlert(
                        new Alert(`The definition for ${this.fieldDefinitionDatum.FieldDefinition.FieldDefinitionDisplayName} was successfully updated.`, AlertContext.Success)
                    );
                });
            },
            error: (error) => {
                this.isLoadingSubmit = false;
                this.cdr.detectChanges();
            }
        });
    }
}
