import { Component, OnInit, Input, ChangeDetectorRef, ViewChild, AfterViewInit, OnDestroy } from "@angular/core";
import { AlertService } from "../../services/alert.service";
import { EditorComponent, TINYMCE_SCRIPT_SRC } from "@tinymce/tinymce-angular";
import TinyMCEHelpers from "../../helpers/tiny-mce-helpers";
import { FieldDefinitionService } from "src/app/shared/generated/api/field-definition.service";
import { FieldDefinitionEnum } from "src/app/shared/generated/enum/field-definition-enum";
import { PopperDirective } from "src/app/shared/directives/popper.directive";
import { FormsModule } from "@angular/forms";
import { FieldDefinitionDatumDetail } from "../../generated/model/models";

@Component({
    selector: "field-definition",
    templateUrl: "./field-definition.component.html",
    styleUrls: ["./field-definition.component.scss"],
    imports: [EditorComponent, FormsModule, PopperDirective],
    providers: [{ provide: TINYMCE_SCRIPT_SRC, useValue: "tinymce/tinymce.min.js" }],
})
export class FieldDefinitionComponent implements OnInit, AfterViewInit, OnDestroy {
    @Input() fieldDefinition: string;
    @Input() labelOverride: string;
    @Input() inline: boolean = false;
    @Input() useBodyContainer: boolean = false;

    @ViewChild("tinyMceEditor") tinyMceEditor: EditorComponent;
    public tinyMceConfig: object;

    public fieldDefinitionDatum: FieldDefinitionDatumDetail;
    public isLoading: boolean = true;
    public isEditing: boolean = false;
    public emptyContent: boolean = false;

    public editedContent: string;

    constructor(private fieldDefinitionService: FieldDefinitionService, private cdr: ChangeDetectorRef, private alertService: AlertService) {}

    ngAfterViewInit(): void {
        // We need to use ngAfterViewInit because the image upload needs a reference to the component
        // to setup the blobCache for image base64 encoding
        this.tinyMceConfig = TinyMCEHelpers.DefaultInitConfig(this.tinyMceEditor);
    }

    ngOnInit() {
        this.fieldDefinitionService.getFieldDefinition(FieldDefinitionEnum[this.fieldDefinition]).subscribe((x) => this.loadFieldDefinition(x));
    }

    ngOnDestroy() {
        this.cdr.detach();
    }

    private loadFieldDefinition(fieldDefinition: FieldDefinitionDatumDetail) {
        this.fieldDefinitionDatum = fieldDefinition;
        this.emptyContent = fieldDefinition.FieldDefinitionDatumValue?.length == 0;
        this.isLoading = false;
        this.cdr.detectChanges();
    }

    public getLabelText() {
        if (this.labelOverride !== null && this.labelOverride !== undefined) {
            return this.labelOverride;
        }

        return this.fieldDefinitionDatum.FieldDefinition.FieldDefinitionDisplayName;
    }

    public showEditButton(): boolean {
        return true;
        // return this.authenticationService.isCurrentUserAnAdministrator();
    }

    public enterEdit(event: any): void {
        event.preventDefault();

        this.editedContent = this.fieldDefinitionDatum.FieldDefinitionDatumValue;
        this.isEditing = true;
    }

    public cancelEdit(): void {
        this.isEditing = false;
    }

    public saveEdit(): void {
        this.isEditing = false;
        this.isLoading = true;

        this.fieldDefinitionDatum.FieldDefinitionDatumValue = this.editedContent;
        // this.fieldDefinitionService.updateFieldDefinition(this.fieldDefinition.FieldDefinitionType.FieldDefinitionTypeID, this.fieldDefinition).subscribe(
        //     (x) => this.loadFieldDefinition(x),
        //     (error) => {
        //         this.isLoading = false;
        //         this.alertService.pushAlert(new Alert("There was an error updating the field definition", AlertContext.Danger));
        //     }
        // );
    }
}
