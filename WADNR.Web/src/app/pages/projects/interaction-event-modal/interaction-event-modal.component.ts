import { Component, inject, OnInit } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { concat, of } from "rxjs";
import { last, switchMap } from "rxjs/operators";

import { FormFieldComponent, FormFieldType, SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

import { InteractionEventService } from "src/app/shared/generated/api/interaction-event.service";
import { InteractionEventUpsertRequest, InteractionEventUpsertRequestFormControls } from "src/app/shared/generated/model/interaction-event-upsert-request";
import { InteractionEventDetail } from "src/app/shared/generated/model/interaction-event-detail";
import { InteractionEventGridRow } from "src/app/shared/generated/model/interaction-event-grid-row";
import { InteractionEventTypesAsSelectDropdownOptions } from "src/app/shared/generated/enum/interaction-event-type-enum";

export interface InteractionEventModalData {
    mode: "create" | "edit";
    projectID: number;
    staffPersonOptions: SelectDropdownOption[];
    contactOptions: SelectDropdownOption[];
    projectOptions: SelectDropdownOption[];
    interactionEvent?: InteractionEventGridRow;
    existingProjectIDs?: number[];
    existingContactIDs?: number[];
}

@Component({
    selector: "interaction-event-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, IconComponent, ButtonLoadingDirective, ModalAlertsComponent],
    templateUrl: "./interaction-event-modal.component.html",
    styleUrls: ["./interaction-event-modal.component.scss"],
})
export class InteractionEventModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<InteractionEventModalData, InteractionEventDetail | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public isSubmitting = false;
    public mode: "create" | "edit" = "create";
    public projectID: number;
    public interactionEventID: number;

    // Dropdown options
    public interactionEventTypeOptions: SelectDropdownOption[] = InteractionEventTypesAsSelectDropdownOptions;
    public staffPersonOptions: SelectDropdownOption[] = [];

    // All options (for lookups when re-adding removed items)
    private allProjectOptions: SelectDropdownOption[] = [];
    private allContactOptions: SelectDropdownOption[] = [];

    // Select-and-chips: single-select controls
    public projectSelectControl = new FormControl<number | null>(null);
    public contactSelectControl = new FormControl<number | null>(null);

    // Select-and-chips: selected items and available dropdown options
    public selectedProjects: SelectDropdownOption[] = [];
    public selectedContacts: SelectDropdownOption[] = [];
    public availableProjectOptions: SelectDropdownOption[] = [];
    public availableContactOptions: SelectDropdownOption[] = [];

    // File upload
    public fileControl = new FormControl<File[]>([]);
    public allowedFileExtensions = ".pdf,.zip,.doc,.docx,.xls,.xlsx,.ppt,.pptx,.txt,.jpg,.jpeg,.png";

    public form = new FormGroup({
        InteractionEventTitle: InteractionEventUpsertRequestFormControls.InteractionEventTitle("", {
            validators: [Validators.required],
        }),
        InteractionEventTypeID: InteractionEventUpsertRequestFormControls.InteractionEventTypeID(null, {
            validators: [Validators.required],
        }),
        InteractionEventDate: InteractionEventUpsertRequestFormControls.InteractionEventDate("", {
            validators: [Validators.required],
        }),
        StaffPersonID: InteractionEventUpsertRequestFormControls.StaffPersonID(null),
        InteractionEventDescription: InteractionEventUpsertRequestFormControls.InteractionEventDescription(""),
    });

    constructor(
        private interactionEventService: InteractionEventService,
        alertService: AlertService,
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.mode = data?.mode ?? "create";
        this.projectID = data?.projectID;
        this.staffPersonOptions = data?.staffPersonOptions ?? [];
        this.allProjectOptions = data?.projectOptions ?? [];
        this.allContactOptions = data?.contactOptions ?? [];

        if (this.mode === "edit" && data?.interactionEvent) {
            const event = data.interactionEvent;
            this.interactionEventID = event.InteractionEventID;
            this.form.patchValue({
                InteractionEventTitle: event.InteractionEventTitle,
                InteractionEventTypeID: event.InteractionEventType?.InteractionEventTypeID,
                InteractionEventDate: event.InteractionEventDate,
                StaffPersonID: event.StaffPerson?.PersonID ?? null,
                InteractionEventDescription: event.InteractionEventDescription,
            });

            // Pre-populate selected projects from existing IDs
            const existingProjectIDs = data.existingProjectIDs ?? [];
            this.selectedProjects = existingProjectIDs
                .map(id => this.allProjectOptions.find(o => o.Value === id))
                .filter((o): o is SelectDropdownOption => o != null);
            this.availableProjectOptions = this.allProjectOptions.filter(
                o => !existingProjectIDs.includes(o.Value as number)
            );

            // Pre-populate selected contacts from existing IDs
            const existingContactIDs = data.existingContactIDs ?? [];
            this.selectedContacts = existingContactIDs
                .map(id => this.allContactOptions.find(o => o.Value === id))
                .filter((o): o is SelectDropdownOption => o != null);
            this.availableContactOptions = this.allContactOptions.filter(
                o => !existingContactIDs.includes(o.Value as number)
            );
        } else {
            // Create mode: pre-select the current project
            const currentProjectOption = this.allProjectOptions.find(o => o.Value === this.projectID);
            if (currentProjectOption) {
                this.selectedProjects = [currentProjectOption];
                this.availableProjectOptions = this.allProjectOptions.filter(o => o.Value !== this.projectID);
            } else {
                this.availableProjectOptions = [...this.allProjectOptions];
            }
            this.availableContactOptions = [...this.allContactOptions];
        }
    }

    onAddProject(event: any): void {
        const value = event?.Value ?? event;
        if (value == null) return;

        const option = this.allProjectOptions.find(o => o.Value === value);
        if (!option || this.selectedProjects.some(p => p.Value === value)) {
            this.projectSelectControl.reset();
            return;
        }

        this.selectedProjects = [...this.selectedProjects, option];
        this.availableProjectOptions = this.availableProjectOptions.filter(o => o.Value !== value);
        this.projectSelectControl.reset();
    }

    removeProject(value: number | string): void {
        this.selectedProjects = this.selectedProjects.filter(p => p.Value !== value);
        const option = this.allProjectOptions.find(o => o.Value === value);
        if (option) {
            this.availableProjectOptions = [...this.availableProjectOptions, option];
        }
    }

    onAddContact(event: any): void {
        const value = event?.Value ?? event;
        if (value == null) return;

        const option = this.allContactOptions.find(o => o.Value === value);
        if (!option || this.selectedContacts.some(c => c.Value === value)) {
            this.contactSelectControl.reset();
            return;
        }

        this.selectedContacts = [...this.selectedContacts, option];
        this.availableContactOptions = this.availableContactOptions.filter(o => o.Value !== value);
        this.contactSelectControl.reset();
    }

    removeContact(value: number | string): void {
        this.selectedContacts = this.selectedContacts.filter(c => c.Value !== value);
        const option = this.allContactOptions.find(o => o.Value === value);
        if (option) {
            this.availableContactOptions = [...this.availableContactOptions, option];
        }
    }

    save(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        this.isSubmitting = true;
        this.localAlerts = [];

        const projectIDs = this.selectedProjects.map(p => p.Value as number);
        const contactIDs = this.selectedContacts.map(c => c.Value as number);

        const dto = new InteractionEventUpsertRequest({
            InteractionEventTitle: this.form.value.InteractionEventTitle,
            InteractionEventTypeID: this.form.value.InteractionEventTypeID,
            InteractionEventDate: this.form.value.InteractionEventDate,
            StaffPersonID: this.form.value.StaffPersonID || null,
            InteractionEventDescription: this.form.value.InteractionEventDescription || null,
            ProjectIDs: projectIDs.length ? projectIDs : null,
            ContactIDs: contactIDs.length ? contactIDs : null,
        });

        if (this.mode === "edit") {
            this.interactionEventService.updateInteractionEvent(this.interactionEventID, dto).subscribe({
                next: (result) => {
                    this.pushGlobalSuccess("Interaction event updated successfully.");
                    this.ref.close(result);
                },
                error: (err) => {
                    this.isSubmitting = false;
                    const message = err?.error ?? err?.message ?? "An error occurred while updating the interaction event.";
                    this.addLocalAlert(message, AlertContext.Danger, true);
                },
            });
        } else {
            this.interactionEventService.createInteractionEvent(dto).pipe(
                switchMap((result) => {
                    const files = this.fileControl.value ?? [];
                    if (files.length === 0) {
                        return of(result);
                    }
                    // Upload each file sequentially, return the original result at the end
                    const uploads$ = files.map(file =>
                        this.interactionEventService.createFileResourceInteractionEvent(result.InteractionEventID, file.name, null, file)
                    );
                    return concat(...uploads$).pipe(
                        last(),
                        switchMap(() => of(result))
                    );
                })
            ).subscribe({
                next: (result) => {
                    this.pushGlobalSuccess("Interaction event added successfully.");
                    this.ref.close(result);
                },
                error: (err) => {
                    this.isSubmitting = false;
                    const message = err?.error ?? err?.message ?? "An error occurred while adding the interaction event.";
                    this.addLocalAlert(message, AlertContext.Danger, true);
                },
            });
        }
    }

    cancel(): void {
        this.ref.close(null);
    }
}
