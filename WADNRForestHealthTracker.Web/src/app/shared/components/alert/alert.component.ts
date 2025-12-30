import { Component, Input, OnInit, Output, EventEmitter } from "@angular/core";
import { Alert } from "../../models/alert";
import { AlertContext } from "../../models/enums/alert-context.enum";
import { AlertService } from "../../services/alert.service";

@Component({
    selector: "alert",
    templateUrl: "./alert.component.html",
    styleUrls: ["./alert.component.scss"],
    imports: [],
})
export class AlertComponent implements OnInit {
    @Input() alert: Alert;
    @Output() closed: EventEmitter<Alert> = new EventEmitter<Alert>();

    public AlertContext: AlertContext;

    constructor(private alertService: AlertService) {}

    ngOnInit(): void {}

    alertClass(): string {
        switch (this.alert.context) {
            case AlertContext.Danger:
                return "alert-danger";
            case AlertContext.Info:
                return "alert-info";
            case AlertContext.Primary:
                return "alert-primary";
            case AlertContext.Secondary:
                return "alert-secondary";
            case AlertContext.Success:
                return "alert-success";
            case AlertContext.Warning:
                return "alert-warning";
            default:
                return "";
        }
    }

    closeAlert(): void {
        // Emit closed event so parent can decide how to remove the alert (local vs global)
        // this.closed.emit(this.alert);
        this.alertService.removeAlert(this.alert);
    }
}
