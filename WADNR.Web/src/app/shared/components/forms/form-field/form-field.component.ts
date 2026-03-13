import { Component, EventEmitter, HostBinding, Input, OnDestroy, OnInit, Output, ViewChild } from "@angular/core";
import { FormControl, NG_VALUE_ACCESSOR, FormsModule, ReactiveFormsModule, FormArray } from "@angular/forms";
import { Subscription } from "rxjs";
import { TinyMceConfigPipe } from "src/app/shared/pipes/tiny-mce-config.pipe";
import { RequiredPipe } from "src/app/shared/pipes/required.pipe";
import { InputErrorsComponent } from "src/app/shared/components/inputs/input-errors/input-errors.component";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";
import { EditorComponent, TINYMCE_SCRIPT_SRC } from "@tinymce/tinymce-angular";

import { NgxMaskDirective, provideNgxMask } from "ngx-mask";
import { NgSelectModule } from "@ng-select/ng-select";

@Component({
    selector: "form-field",
    templateUrl: "./form-field.component.html",
    styleUrls: ["./form-field.component.scss"],
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            useExisting: FormFieldComponent,
            multi: true,
        },
        {
            provide: TINYMCE_SCRIPT_SRC,
            useValue: "tinymce/tinymce.min.js",
        },
        provideNgxMask(),
    ],
    imports: [NgxMaskDirective, FormsModule, ReactiveFormsModule, EditorComponent, FieldDefinitionComponent, InputErrorsComponent, RequiredPipe, TinyMceConfigPipe, NgSelectModule],
})
export class FormFieldComponent implements OnInit, OnDestroy {
    public FormFieldType = FormFieldType;
    @Output() change = new EventEmitter<any>();
    @Input() formControl: FormControl;
    @Input() fieldLabel: string = "";
    @Input() placeholder: string = "";
    @Input() type: FormFieldType = FormFieldType.Text;
    @Input() toggleTrue: string = "On";
    @Input() toggleFalse: string = "Off";
    @Input() checkLabel: string;
    @Input() units: string;
    @Input() name: string;
    @Input() fieldDefinitionName: string;
    @Input() fieldDefinitionLabelOverride: string;
    @Input() toggleHeight: string = "";
    @Input() mask: string;
    @Input() horizontal: boolean = false;

    // for select dropdown
    @Input() formInputOptions: FormInputOption[];
    @Input() formInputOptionLabel: string = "Label";
    @Input() formInputOptionValue: string = "Value";
    @Input() multiple: boolean = false;
    @Input() searchable: boolean = true;
    @Input() clearable: boolean = true;
    @Input() appendTo: string = "#dropdown-host";

    @Input() readOnly: boolean = false;

    /**
     * comma separated list of file types: https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input/file#accept
     */
    @Input() uploadFileAccepts: string;

    @ViewChild("fileUploadField") fileUploadField: any;
    public fileName: string = null;
    public fileExtension: string = null;
    public selectedFiles: File[] = [];

    public val: any;
    set value(val) {
        this.val = val;
        this.change.emit(val);

        this.onChange(val);
        this.onTouch(val);
    }

    @HostBinding("class.horizontal") get isHorizontal() {
        return this.horizontal;
    }

    public isDisabled: boolean = false;

    onChange: any = () => {};
    onTouch: any = () => {};

    writeValue(value: any): void {
        this.val = value;
        if (this.multiple && this.type === FormFieldType.File) {
            this.selectedFiles = Array.isArray(value) ? [...value] : [];
        }
        if (this.type === FormFieldType.Currency && !this.currencyFocused) {
            this.updateCurrencyDisplay();
        }
    }

    registerOnChange(fn: any): void {
        this.onChange = fn;
    }

    registerOnTouched(fn: any): void {
        this.onTouch = fn;
    }

    setDisabledState?(isDisabled: boolean): void {
        this.isDisabled = isDisabled;
    }

    onFileChange(event: any): void {
        if (this.multiple) {
            let newFiles: File[] = Array.from(event.target.files ?? []);
            if (newFiles.length && this.uploadFileAccepts) {
                const allowedExts = this.uploadFileAccepts.split(",").map(e => e.trim().toLowerCase());
                newFiles = newFiles.filter(f => {
                    const ext = f.name.substring(f.name.lastIndexOf(".")).toLowerCase();
                    return allowedExts.includes(ext);
                });
                if (!newFiles.length) {
                    event.target.value = "";
                    // Set errors after clearing input so Angular's re-validation doesn't clear them
                    this.formControl.setErrors({ invalidFileType: { allowed: this.uploadFileAccepts } });
                    this.formControl.markAsTouched();
                    return;
                }
            }
            if (newFiles.length) {
                this.selectedFiles = [...this.selectedFiles, ...newFiles];
                this.value = [...this.selectedFiles];
            }
            // Reset native input so the same file can be re-added
            event.target.value = "";
        } else {
            let file = event.target.files[0];
            if (file && this.uploadFileAccepts) {
                const allowedExts = this.uploadFileAccepts.split(",").map(e => e.trim().toLowerCase());
                const ext = file.name.substring(file.name.lastIndexOf(".")).toLowerCase();
                if (!allowedExts.includes(ext)) {
                    event.target.value = "";
                    this.value = null;
                    this.fileName = null;
                    this.fileExtension = null;
                    // Set errors after value change so Angular's re-validation doesn't clear them
                    this.formControl.setErrors({ invalidFileType: { allowed: this.uploadFileAccepts } });
                    this.formControl.markAsTouched();
                    return;
                }
            }
            this.value = file;
            if (file) {
                const name = file.name;
                const i = name.lastIndexOf(".");
                this.fileName = i > 0 ? name.slice(0, i) : name;
                this.fileExtension = i > 0 ? name.slice(i) : "";
            } else {
                this.fileName = null;
                this.fileExtension = null;
            }
        }
    }

    removeFile(index: number): void {
        this.selectedFiles.splice(index, 1);
        this.value = [...this.selectedFiles];
    }

    onClickFileUpload(event: any): void {
        const fileUploadInput = this.fileUploadField.nativeElement;
        fileUploadInput.click();
    }

    onNgSelectChange(event: any): void {
        this.change.emit(event);
    }

    // Currency formatting
    private currencyFormatter = new Intl.NumberFormat("en-US", { style: "currency", currency: "USD" });
    public currencyDisplayValue: string = "";
    private currencySubscription: Subscription;

    ngOnInit(): void {
        if (this.type === FormFieldType.Currency && this.formControl) {
            this.updateCurrencyDisplay();
            this.currencySubscription = this.formControl.valueChanges.subscribe(() => {
                if (!this.currencyFocused) {
                    this.updateCurrencyDisplay();
                }
            });
        }
    }

    ngOnDestroy(): void {
        this.currencySubscription?.unsubscribe();
    }

    private currencyFocused = false;

    updateCurrencyDisplay(): void {
        const raw = this.formControl?.value;
        if (raw != null && raw !== "") {
            this.currencyDisplayValue = this.currencyFormatter.format(Number(raw));
        } else {
            this.currencyDisplayValue = "";
        }
    }

    onCurrencyFocus(): void {
        this.currencyFocused = true;
        const raw = this.formControl?.value;
        this.currencyDisplayValue = (raw != null && raw !== "") ? String(raw) : "";
    }

    onCurrencyBlur(): void {
        this.currencyFocused = false;
        const parsed = parseFloat(this.currencyDisplayValue.replace(/[^0-9.\-]/g, ""));
        if (!isNaN(parsed)) {
            this.formControl.setValue(parsed);
            this.formControl.markAsDirty();
        } else {
            this.formControl.setValue(null);
        }
        this.updateCurrencyDisplay();
    }

    onCurrencyInput(event: Event): void {
        this.currencyDisplayValue = (event.target as HTMLInputElement).value;
    }
}

export enum FormFieldType {
    Text = "text",
    Textarea = "textarea",
    Check = "check",
    Toggle = "toggle",
    Date = "date",
    Select = "select",
    Number = "number",
    Radio = "radio",
    RTE = "rte",
    File = "file",
    Currency = "currency",
}

export interface FormInputOption {
    Value: any;
    Label: string;
    SortOrder?: number | null | undefined;
    Group?: string | null | undefined;

    disabled: boolean | null | undefined; // Needs to be lowercase to match ng-select's disabled property
}

export interface SelectDropdownOption extends FormInputOption {}
