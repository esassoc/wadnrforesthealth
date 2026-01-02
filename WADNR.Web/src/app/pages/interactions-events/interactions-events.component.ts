import { Component } from "@angular/core";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { AsyncPipe } from "@angular/common";
import { InteractionEventService } from "src/app/shared/generated/api/interaction-event.service";
import { InteractionEventGridRow } from "src/app/shared/generated/model/interaction-event-grid-row";
import { ColDef } from "ag-grid-community";
import { Observable } from "rxjs";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
//import { InteractionEventModalComponent } from "./interaction-event-modal/interaction-event-modal.component";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { DialogService } from "@ngneat/dialog";
import { AlertService } from "src/app/shared/services/alert.service";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";

@Component({
    selector: "interactions-events",
    imports: [PageHeaderComponent, AlertDisplayComponent, WADNRGridComponent, AsyncPipe],
    templateUrl: "./interactions-events.component.html",
})
export class InteractionsEventsComponent {
    public interactionEvents$: Observable<InteractionEventGridRow[]>;
    public columnDefs: ColDef[];
    public customRichTextTypeID = FirmaPageTypeEnum.InteractionEventList;

    constructor(
        private InteractionEventService: InteractionEventService,
        private utilityFunctions: UtilityFunctionsService,
        private dialogService: DialogService,
        private alertService: AlertService,
        private confirmService: ConfirmService
    ) {}

    ngOnInit(): void {
        this.columnDefs = [
            this.utilityFunctions.createLinkColumnDef("Title", "InteractionEventTitle", "InteractionEventID", {
                InRouterLink: "/interactions-events/",
            }),
            this.utilityFunctions.createBasicColumnDef("Description", "InteractionEventDescription"),
            this.utilityFunctions.createDateColumnDef("Date", "InteractionEventDate", "M/d/yyyy"),
            this.utilityFunctions.createBasicColumnDef("Type", "InteractionEventType.InteractionEventTypeDisplayName", {
                FieldDefinitionType: "InteractionEventType",
            }),
            this.utilityFunctions.createBasicColumnDef("Staff Person", "StaffPerson.FullName"),
        ];

        this.interactionEvents$ = this.InteractionEventService.listInteractionEvent();
    }

    // openAddModal() {
    //     const dialogRef = this.dialogService.open(InteractionEventModalComponent, {
    //         data: {
    //             mode: "add",
    //             InteractionEvent: null,
    //         },
    //     });
    //     dialogRef.afterClosed$.subscribe((result) => {
    //         if (result) {
    //             this.alertService.clearAlerts();
    //             this.alertService.pushAlert(new Alert("Interaction/Event added successfully.", AlertContext.Success));
    //             this.interactionEvents$ = this.InteractionEventService.listInteractionEvent();
    //         }
    //     });
    // }

    // openEditModal(InteractionEvent: InteractionEventDto) {
    //     const dialogRef = this.dialogService.open(InteractionEventModalComponent, {
    //         data: {
    //             mode: "edit",
    //             InteractionEvent,
    //         },
    //     });
    //     dialogRef.afterClosed$.subscribe((result) => {
    //         if (result) {
    //             this.alertService.clearAlerts();
    //             this.alertService.pushAlert(new Alert("Interaction/Event updated successfully.", AlertContext.Success));
    //             this.interactionEvents$ = this.InteractionEventService.listInteractionEvent();
    //         }
    //     });
    // }

    // deleteInteractionEvent(InteractionEvent: InteractionEventDto) {
    //     this.confirmService
    //         .confirm({
    //             title: "Delete Interaction/Event",
    //             message: `Are you sure you want to delete interaction/event '<strong>${InteractionEvent.InteractionEventTitle}</strong>'?`,
    //             buttonTextYes: "Delete",
    //             buttonTextNo: "Cancel",
    //             buttonClassYes: "btn-danger",
    //         })
    //         .then((confirmed) => {
    //             if (confirmed) {
    //                 this.InteractionEventService.deleteInteractionEvent(InteractionEvent.InteractionEventID).subscribe(() => {
    //                     this.alertService.clearAlerts();
    //                     this.alertService.pushAlert(new Alert("Interaction/Event deleted successfully.", AlertContext.Success));
    //                     this.interactionEvents$ = this.InteractionEventService.listInteractionEvent();
    //                 });
    //             }
    //         });
    // }
}
