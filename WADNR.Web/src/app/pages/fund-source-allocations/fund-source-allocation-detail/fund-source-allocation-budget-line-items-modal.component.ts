import { Component } from "@angular/core";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";

import { FundSourceAllocationService } from "src/app/shared/generated/api/fund-source-allocation.service";
import { FundSourceAllocationBudgetLineItemGridRow } from "src/app/shared/generated/model/fund-source-allocation-budget-line-item-grid-row";
import { CostTypes } from "src/app/shared/generated/enum/cost-type-enum";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

interface BudgetLineItemsModalInput {
    fundSourceAllocationID: number;
    existingItems: FundSourceAllocationBudgetLineItemGridRow[];
}

@Component({
    selector: "fund-source-allocation-budget-line-items-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent, ButtonLoadingDirective],
    template: `
        <div class="modal">
            <div class="modal-header">
                <h3>Edit Budget Line Items</h3>
            </div>
            <div class="modal-body">
                <modal-alerts [alerts]="localAlerts" (onClosed)="removeLocalAlert($event)"></modal-alerts>
                <table class="table">
                    <thead>
                        <tr>
                            <th>Cost Type</th>
                            <th>Amount</th>
                            <th>Note</th>
                        </tr>
                    </thead>
                    <tbody>
                        @for (row of rows; track row.costTypeID) {
                            <tr>
                                <td>{{ row.costTypeName }}</td>
                                <td style="width: 200px">
                                    <form-field
                                        [formControl]="row.amountControl"
                                        [type]="FormFieldType.Number"
                                        placeholder="0">
                                    </form-field>
                                </td>
                                <td>
                                    <form-field
                                        [formControl]="row.noteControl"
                                        placeholder="Note">
                                    </form-field>
                                </td>
                            </tr>
                        }
                    </tbody>
                </table>
            </div>
            <div class="modal-footer">
                <button class="btn btn-primary" [disabled]="isSubmitting" [buttonLoading]="isSubmitting" (click)="save()">Save</button>
                <button class="btn btn-secondary" (click)="ref.close(null)">Cancel</button>
            </div>
        </div>
    `,
})
export class FundSourceAllocationBudgetLineItemsModalComponent extends BaseModal {
    FormFieldType = FormFieldType;

    data: BudgetLineItemsModalInput;
    isSubmitting = false;

    rows: { costTypeID: number; costTypeName: string; amountControl: FormControl<number>; noteControl: FormControl<string> }[];

    constructor(
        public ref: DialogRef<BudgetLineItemsModalInput, boolean>,
        private fundSourceAllocationService: FundSourceAllocationService,
    ) {
        super();
        this.data = ref.data!;

        const existingByType = new Map(this.data.existingItems.map(i => [i.CostTypeID, i]));

        this.rows = CostTypes.map(ct => {
            const existing = existingByType.get(ct.Value);
            return {
                costTypeID: ct.Value,
                costTypeName: ct.DisplayName,
                amountControl: new FormControl<number>(existing?.FundSourceAllocationBudgetLineItemAmount ?? 0, { nonNullable: true }),
                noteControl: new FormControl<string>(existing?.FundSourceAllocationBudgetLineItemNote ?? "", { nonNullable: true }),
            };
        });
    }

    save(): void {
        if (this.isSubmitting) return;
        this.isSubmitting = true;

        const items = this.rows
            .filter(r => r.amountControl.value !== 0 || r.noteControl.value)
            .map(r => ({
                CostTypeID: r.costTypeID,
                Amount: r.amountControl.value,
                Note: r.noteControl.value || undefined,
            }));

        this.fundSourceAllocationService.saveBudgetLineItemsFundSourceAllocation(
            this.data.fundSourceAllocationID,
            { Items: items },
        ).subscribe({
            next: () => this.ref.close(true),
            error: (err) => {
                this.isSubmitting = false;
                this.addLocalAlert(err?.error || "An error occurred.", AlertContext.Danger, true);
            },
        });
    }
}
