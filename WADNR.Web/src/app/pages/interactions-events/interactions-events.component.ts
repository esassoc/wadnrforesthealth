import { Component, ViewContainerRef } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { Router } from "@angular/router";
import { ColDef } from "ag-grid-community";
import { forkJoin, map, Observable } from "rxjs";
import { DialogService } from "@ngneat/dialog";

import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { AuthenticationService } from "src/app/services/authentication.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";

import { InteractionEventService } from "src/app/shared/generated/api/interaction-event.service";
import { PersonService } from "src/app/shared/generated/api/person.service";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { InteractionEventGridRow } from "src/app/shared/generated/model/interaction-event-grid-row";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";

@Component({
    selector: "interactions-events",
    imports: [PageHeaderComponent, WADNRGridComponent, AsyncPipe],
    templateUrl: "./interactions-events.component.html",
})
export class InteractionsEventsComponent {
    public interactionEvents$: Observable<InteractionEventGridRow[]>;
    public columnDefs: ColDef[];
    public customRichTextTypeID = FirmaPageTypeEnum.InteractionEventList;
    public canEditInteractionEvents$: Observable<boolean>;

    constructor(
        private interactionEventService: InteractionEventService,
        private personService: PersonService,
        private projectService: ProjectService,
        private utilityFunctions: UtilityFunctionsService,
        private dialogService: DialogService,
        private alertService: AlertService,
        private confirmService: ConfirmService,
        private authService: AuthenticationService,
        private viewContainerRef: ViewContainerRef,
        private router: Router,
    ) {}

    ngOnInit(): void {
        this.canEditInteractionEvents$ = this.authService.currentUserSetObservable.pipe(
            map((user) => this.authService.canEditInteractionEvents(user)),
        );

        this.initColumnDefs();
        this.interactionEvents$ = this.interactionEventService.listInteractionEvent();
    }

    private initColumnDefs(): void {
        this.authService.currentUserSetObservable.pipe(
            map((user) => this.authService.canEditInteractionEvents(user)),
        ).subscribe((canEdit) => {
            const actionsColumn = canEdit
                ? [this.utilityFunctions.createActionsColumnDef((params) => {
                    const row = params.data as InteractionEventGridRow;
                    return [
                        { ActionName: "Edit", ActionHandler: () => this.openEditModal(row), ActionIcon: "fa fa-edit" },
                        { ActionName: "Delete", ActionHandler: () => this.confirmDeleteInteractionEvent(row), ActionIcon: "fa fa-trash" },
                    ];
                })]
                : [];

            this.columnDefs = [
                ...actionsColumn,
                this.utilityFunctions.createLinkColumnDef("Title", "InteractionEventTitle", "InteractionEventID", {
                    InRouterLink: "/interactions-events/",
                }),
                this.utilityFunctions.createBasicColumnDef("Description", "InteractionEventDescription"),
                this.utilityFunctions.createDateColumnDef("Date", "InteractionEventDate", "M/d/yyyy"),
                this.utilityFunctions.createBasicColumnDef("Type", "InteractionEventType.InteractionEventTypeDisplayName", {
                    FieldDefinitionType: "InteractionEventType",
                    CustomDropdownFilterField: "InteractionEventType.InteractionEventTypeDisplayName",
                }),
                this.utilityFunctions.createLinkColumnDef("Staff Person", "StaffPerson.FullName", "StaffPerson.PersonID", {
                    InRouterLink: "/people/",
                    CustomDropdownFilterField: "StaffPerson.FullName",
                    RequiresAuth: true,
                }),
            ];
        });
    }

    createNewInteractionEvent(): void {
        forkJoin({
            people: this.personService.listLookupPerson(),
            wadnrPeople: this.personService.listWadnrLookupPerson(),
            projects: this.projectService.listLookupProject(),
        }).subscribe(({ people, wadnrPeople, projects }) => {
            const personOptions: SelectDropdownOption[] = people.map(p => ({
                Value: p.PersonID,
                Label: p.FullName,
                disabled: false,
            } as SelectDropdownOption));

            const staffOptions: SelectDropdownOption[] = wadnrPeople.map(p => ({
                Value: p.PersonID,
                Label: p.FullName,
                disabled: false,
            } as SelectDropdownOption));

            const projectOptions: SelectDropdownOption[] = projects.map(p => ({
                Value: p.ProjectID,
                Label: p.ProjectName,
                disabled: false,
            } as SelectDropdownOption));

            import("../projects/interaction-event-modal/interaction-event-modal.component").then(({ InteractionEventModalComponent }) => {
                const dialogRef = this.dialogService.open(InteractionEventModalComponent, {
                    data: {
                        mode: "create" as const,
                        projectID: 0,
                        staffPersonOptions: staffOptions,
                        contactOptions: personOptions,
                        projectOptions: projectOptions,
                    },
                    width: "600px",
                });
                dialogRef.afterClosed$.subscribe((result) => {
                    if (result) {
                        this.router.navigate(["/interactions-events", result.InteractionEventID]);
                    }
                });
            });
        });
    }

    openEditModal(row: InteractionEventGridRow): void {
        forkJoin({
            people: this.personService.listLookupPerson(),
            wadnrPeople: this.personService.listWadnrLookupPerson(),
            projects: this.projectService.listLookupProject(),
            existingProjects: this.interactionEventService.listProjectsForInteractionEventIDInteractionEvent(row.InteractionEventID!),
            existingContacts: this.interactionEventService.listContactsForInteractionEventIDInteractionEvent(row.InteractionEventID!),
        }).subscribe(({ people, wadnrPeople, projects, existingProjects, existingContacts }) => {
            const personOptions: SelectDropdownOption[] = people.map(p => ({
                Value: p.PersonID,
                Label: p.FullName,
                disabled: false,
            } as SelectDropdownOption));

            const staffOptions: SelectDropdownOption[] = wadnrPeople.map(p => ({
                Value: p.PersonID,
                Label: p.FullName,
                disabled: false,
            } as SelectDropdownOption));

            const projectOptions: SelectDropdownOption[] = projects.map(p => ({
                Value: p.ProjectID,
                Label: p.ProjectName,
                disabled: false,
            } as SelectDropdownOption));

            import("../projects/interaction-event-modal/interaction-event-modal.component").then(({ InteractionEventModalComponent }) => {
                const dialogRef = this.dialogService.open(InteractionEventModalComponent, {
                    data: {
                        mode: "edit" as const,
                        projectID: 0,
                        staffPersonOptions: staffOptions,
                        contactOptions: personOptions,
                        projectOptions: projectOptions,
                        interactionEvent: {
                            ...row,
                            InteractionEventDate: row.InteractionEventDate?.substring(0, 10),
                        } as any,
                        existingProjectIDs: existingProjects.map(p => p.ProjectID),
                        existingContactIDs: existingContacts.map(c => c.PersonID),
                    },
                    width: "600px",
                });
                dialogRef.afterClosed$.subscribe((result) => {
                    if (result) {
                        this.interactionEvents$ = this.interactionEventService.listInteractionEvent();
                    }
                });
            });
        });
    }

    async confirmDeleteInteractionEvent(row: InteractionEventGridRow): Promise<void> {
        const confirmed = await this.confirmService.confirm(
            {
                title: "Delete Interaction/Event",
                message: `Are you sure you want to delete interaction/event "<strong>${row.InteractionEventTitle}</strong>"?`,
                buttonTextYes: "Delete",
                buttonClassYes: "btn-danger",
                buttonTextNo: "Cancel",
            },
            this.viewContainerRef,
        );

        if (confirmed) {
            this.interactionEventService.deleteInteractionEvent(row.InteractionEventID!).subscribe({
                next: () => {
                    this.alertService.pushAlert(new Alert("Interaction/Event deleted successfully.", AlertContext.Success, true));
                    this.interactionEvents$ = this.interactionEventService.listInteractionEvent();
                },
                error: (err) => {
                    const message = err?.error?.message ?? err?.error ?? "An error occurred while deleting.";
                    this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                },
            });
        }
    }
}
