import { Component, EventEmitter, Input, Output } from "@angular/core";
import { Alert } from "src/app/shared/models/alert";
import { AlertComponent } from "src/app/shared/components/alert/alert.component";


@Component({
    selector: "modal-alerts",
    standalone: true,
    imports: [AlertComponent],
    template: `
        @if (alerts?.length > 0) {
        <div class="mt-2">
            @for (a of alerts; track a; let i = $index) {
            <alert [alert]="a" (closed)="onClosed.emit(a)">
                <span [innerHTML]="a.message" style="width: 100%"> </span>
            </alert>
            }
        </div>
        }
    `,
})
export class ModalAlertsComponent {
    @Input() alerts: Alert[] = [];
    @Output() onClosed: EventEmitter<Alert> = new EventEmitter<Alert>();
}
