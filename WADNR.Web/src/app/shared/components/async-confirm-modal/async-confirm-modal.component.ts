import { Component, inject } from "@angular/core";
import { Observable } from "rxjs";
import { DialogRef } from "@ngneat/dialog";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

export interface AsyncConfirmModalData {
    title: string;
    message: string;
    buttonTextYes: string;
    buttonTextNo?: string;
    buttonClassYes?: string;
    actionFn: () => Observable<any>;
}

@Component({
    selector: "async-confirm-modal",
    standalone: true,
    imports: [ButtonLoadingDirective],
    templateUrl: "./async-confirm-modal.component.html",
})
export class AsyncConfirmModalComponent {
    private dialogRef = inject(DialogRef<AsyncConfirmModalData, any>);
    private alertService = inject(AlertService);

    data = this.dialogRef.data;
    isLoading = false;

    execute(): void {
        this.isLoading = true;

        this.data.actionFn().subscribe({
            next: (result) => {
                this.dialogRef.close(result ?? true);
            },
            error: (err) => {
                const message = err?.error?.ErrorMessage ?? err?.error ?? err?.message ?? "An unexpected error occurred.";
                this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                this.dialogRef.close(undefined);
            },
        });
    }

    cancel(): void {
        this.dialogRef.close(undefined);
    }
}
