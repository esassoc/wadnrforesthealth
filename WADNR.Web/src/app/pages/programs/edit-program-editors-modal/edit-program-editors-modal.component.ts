import { Component, inject, OnDestroy, OnInit, signal } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { Subscription } from "rxjs";

import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";

import { ProgramService } from "src/app/shared/generated/api/program.service";
import { PersonLookupItem } from "src/app/shared/generated/model/person-lookup-item";

export interface EditProgramEditorsModalData {
    programID: number;
    currentEditors: PersonLookupItem[];
}

interface PersonLookupItemWithDisplay extends PersonLookupItem {
    DisplayName: string;
}

@Component({
    selector: "edit-program-editors-modal",
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, ModalAlertsComponent, LoadingDirective, ButtonLoadingDirective, IconComponent, FormFieldComponent],
    templateUrl: "./edit-program-editors-modal.component.html",
})
export class EditProgramEditorsModalComponent extends BaseModal implements OnInit, OnDestroy {
    public ref: DialogRef<EditProgramEditorsModalData, boolean> = inject(DialogRef);
    public FormFieldType = FormFieldType;

    public isLoading = signal(true);
    public isSubmitting = signal(false);

    public eligiblePeople: PersonLookupItemWithDisplay[] = [];
    public selectedEditors: PersonLookupItemWithDisplay[] = [];
    public availableOptions: FormInputOption[] = [];
    public editorSelectControl = new FormControl<number | null>(null);

    private programService = inject(ProgramService);
    private selectSub: Subscription;

    constructor() {
        super();
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.selectedEditors = data.currentEditors.map((p) => ({ ...p, DisplayName: this.getDisplayName(p) }));

        this.selectSub = this.editorSelectControl.valueChanges.subscribe((personID) => {
            if (personID == null) return;
            const person = this.eligiblePeople.find((p) => p.PersonID === personID);
            if (person && !this.selectedEditors.some((e) => e.PersonID === person.PersonID)) {
                this.selectedEditors.push(person);
            }
            this.updateAvailableOptions();
            setTimeout(() => this.editorSelectControl.reset());
        });

        this.programService.listEligibleEditorsProgram().subscribe({
            next: (people) => {
                this.eligiblePeople = people.map((p) => ({ ...p, DisplayName: this.getDisplayName(p) }));
                this.updateAvailableOptions();
                this.isLoading.set(false);
            },
            error: () => {
                this.addLocalAlert("Failed to load eligible editors.", AlertContext.Danger);
                this.isLoading.set(false);
            },
        });
    }

    ngOnDestroy(): void {
        this.selectSub?.unsubscribe();
    }

    private updateAvailableOptions(): void {
        const selectedIDs = new Set(this.selectedEditors.map((e) => e.PersonID));
        this.availableOptions = this.eligiblePeople
            .filter((p) => !selectedIDs.has(p.PersonID))
            .map((p) => ({ Value: p.PersonID, Label: p.DisplayName, disabled: false }));
    }

    get sortedEditors(): PersonLookupItemWithDisplay[] {
        return [...this.selectedEditors].sort((a, b) => {
            const aLast = a.FullName?.split(" ").slice(1).join(" ") ?? "";
            const bLast = b.FullName?.split(" ").slice(1).join(" ") ?? "";
            return aLast.localeCompare(bLast);
        });
    }

    getDisplayName(person: PersonLookupItem): string {
        return person.OrganizationName ? `${person.FullName} - ${person.OrganizationName}` : person.FullName;
    }

    removeEditor(personID: number): void {
        this.selectedEditors = this.selectedEditors.filter((e) => e.PersonID !== personID);
        this.updateAvailableOptions();
    }

    save(): void {
        this.isSubmitting.set(true);
        const programID = this.ref.data.programID;
        const request = { PersonIDList: this.selectedEditors.map((e) => e.PersonID) };

        this.programService.updateEditorsProgram(programID, request).subscribe({
            next: () => {
                this.isSubmitting.set(false);
                this.ref.close(true);
            },
            error: () => {
                this.addLocalAlert("Failed to update program editors.", AlertContext.Danger);
                this.isSubmitting.set(false);
            },
        });
    }

    cancel(): void {
        this.ref.close(false);
    }
}
