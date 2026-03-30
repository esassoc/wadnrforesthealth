import { signal } from "@angular/core";
import { Alert } from "src/app/shared/models/alert";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

// Lightweight base class for modal components to share alert handling.
// Not an Angular component — simple class to extend from.
export class BaseModal {
    public localAlerts = signal<Alert[]>([]);

    constructor(protected alertService?: AlertService) {}

    addLocalAlert(message: string, context: AlertContext = AlertContext.Danger, dismissible = true) {
        const alert = new Alert(message, context, dismissible);
        this.localAlerts.update((alerts) => {
            if (alerts.some((a) => a.message === alert.message && a.context === alert.context)) {
                return alerts;
            }
            return [...alerts, alert];
        });
    }

    removeLocalAlert(alert: Alert): void {
        this.localAlerts.update((alerts) => alerts.filter((a) => a !== alert));
    }

    pushGlobalSuccess(message: string) {
        if (this.alertService) {
            this.alertService.pushAlert(new Alert(message, AlertContext.Success, true));
        }
    }
}
