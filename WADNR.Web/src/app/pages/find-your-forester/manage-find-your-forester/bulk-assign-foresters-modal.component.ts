import { Component, inject } from "@angular/core";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { AsyncPipe } from "@angular/common";
import { DialogRef } from "@ngneat/dialog";
import { shareReplay, tap } from "rxjs";

import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { FindYourForesterService } from "src/app/shared/generated/api/find-your-forester.service";
import { ForesterWorkUnitGridRow } from "src/app/shared/generated/model/forester-work-unit-grid-row";

export interface BulkAssignModalData {
    selectedWorkUnits: ForesterWorkUnitGridRow[];
    roleDisplayName: string;
}

@Component({
    selector: "bulk-assign-foresters-modal",
    standalone: true,
    imports: [ReactiveFormsModule, AsyncPipe, ModalAlertsComponent, FormFieldComponent, ButtonLoadingDirective, LoadingDirective],
    template: `
        <div class="modal">
            <div class="modal-header">
                <h4>Manage Forester Assignments</h4>
            </div>
            <div class="modal-body" [loadingSpinner]="{ isLoading: !(people$ | async), loadingHeight: 200 }">
                <modal-alerts [alerts]="localAlerts" (onClosed)="removeLocalAlert($event)"></modal-alerts>

                @if (people$ | async) {
                    <p>Assign the following {{ data.roleDisplayName }} Forester Work Units:</p>
                    <div class="selected-work-units-list">
                        <div class="work-unit-header">
                            <span>Forester Work Unit Name</span>
                            <span>Current Forester Assignment</span>
                        </div>
                        @for (wu of data.selectedWorkUnits; track wu.ForesterWorkUnitID) {
                            <div class="work-unit-item">
                                <span>{{ wu.ForesterWorkUnitName }}</span>
                                <span class="text-muted">
                                    {{ wu.AssignedPersonName || 'unassigned' }}
                                </span>
                            </div>
                        }
                    </div>

                    <div class="mt-3">
                        <form-field
                            [fieldLabel]="'To:'"
                            [type]="FormFieldType.Select"
                            [formInputOptions]="personOptions"
                            [formControl]="selectedPersonControl">
                        </form-field>
                        <p class="text-muted mt-1">All the above work units will be reassigned to this selection</p>
                    </div>
                }
            </div>
            <div class="modal-footer">
                <button class="btn btn-secondary" (click)="cancel()">Cancel</button>
                <button class="btn btn-primary" (click)="save()" [buttonLoading]="isSubmitting" [disabled]="!(people$ | async)">
                    Save
                </button>
            </div>
        </div>
    `,
    styles: [`
        .selected-work-units-list {
            max-height: 200px;
            overflow-y: auto;
            border: 1px solid #ddd;
            border-radius: 4px;
            padding: 0.5rem;
        }
        .work-unit-header {
            padding: 0.25rem 0;
            display: flex;
            justify-content: space-between;
            font-weight: bold;
        }
        .work-unit-item {
            padding: 0.25rem 0;
            border-bottom: 1px solid #eee;
            display: flex;
            justify-content: space-between;
            &:last-child { border-bottom: none; }
        }
    `],
})
export class BulkAssignForestersModalComponent extends BaseModal {
    FormFieldType = FormFieldType;
    public ref: DialogRef<BulkAssignModalData, boolean> = inject(DialogRef);

    personOptions: FormInputOption[] = [];
    selectedPersonControl = new FormControl<number | null>(null);
    isSubmitting = false;

    private findYourForesterService = inject(FindYourForesterService);

    constructor(alertService: AlertService) {
        super(alertService);
    }

    data: BulkAssignModalData = this.ref.data;

    people$ = this.findYourForesterService.listAssignablePeopleFindYourForester().pipe(
        tap((people) => {
            this.personOptions = [
                { Value: null, Label: "Unassigned", disabled: false },
                ...people.map((p) => ({
                    Value: p.PersonID,
                    Label: p.FullName,
                    disabled: false,
                })),
            ];
        }),
        shareReplay({ bufferSize: 1, refCount: true }),
    );

    save(): void {
        this.isSubmitting = true;

        const request = {
            ForesterWorkUnitIDList: this.data.selectedWorkUnits.map((wu) => wu.ForesterWorkUnitID),
            SelectedForesterPersonID: this.selectedPersonControl.value,
        };

        this.findYourForesterService.bulkAssignForestersFindYourForester(request).subscribe({
            next: () => {
                this.pushGlobalSuccess("Forester assignments updated successfully.");
                this.ref.close(true);
            },
            error: (err) => {
                this.isSubmitting = false;
                this.addLocalAlert(err?.error?.message ?? "An error occurred while saving assignments.", AlertContext.Danger, true);
            },
        });
    }

    cancel(): void {
        this.ref.close(false);
    }
}
