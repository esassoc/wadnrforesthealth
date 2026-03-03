import { Component } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { ColDef } from "ag-grid-community";
import { BehaviorSubject, Observable, switchMap } from "rxjs";
import { DialogService } from "@ngneat/dialog";

import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { AuthenticationService } from "src/app/services/authentication.service";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

import { TagService } from "src/app/shared/generated/api/tag.service";
import { TagGridRow } from "src/app/shared/generated/model/tag-grid-row";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";
import { TagModalComponent, TagModalData } from "./tag-modal.component";

@Component({
    selector: "tags",
    imports: [PageHeaderComponent, WADNRGridComponent, AsyncPipe],
    templateUrl: "./tags.component.html",
})
export class TagsComponent {
    public tags$: Observable<TagGridRow[]>;
    public columnDefs: ColDef[] = [];
    public customRichTextTypeID = FirmaPageTypeEnum.TagList;
    public isAdmin = false;

    private refreshTags$ = new BehaviorSubject<void>(undefined);

    constructor(
        private tagService: TagService,
        private utilityFunctions: UtilityFunctionsService,
        private authenticationService: AuthenticationService,
        private dialogService: DialogService,
        private confirmService: ConfirmService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.authenticationService.getCurrentUser().subscribe(user => {
            this.isAdmin = this.authenticationService.isUserAnAdministrator(user);
            this.buildColumnDefs();
        });

        this.tags$ = this.refreshTags$.pipe(
            switchMap(() => this.tagService.listTag())
        );
    }

    private buildColumnDefs(): void {
        this.columnDefs = [];

        if (this.isAdmin) {
            this.columnDefs.push(
                this.utilityFunctions.createActionsColumnDef((params) => {
                    const tag = params.data as TagGridRow;
                    return [
                        { ActionName: "Delete", ActionHandler: () => this.confirmDelete(tag), ActionIcon: "fa fa-trash" },
                    ];
                })
            );
        }

        this.columnDefs.push(
            this.utilityFunctions.createLinkColumnDef("Tag Name", "TagName", "TagID", {
                InRouterLink: "/tags/",
                FieldDefinitionType: "TagName",
            }),
            this.utilityFunctions.createBasicColumnDef("Description", "TagDescription", {
                FieldDefinitionType: "TagDescription",
            }),
            this.utilityFunctions.createYearColumnDef("# of Projects", "ProjectCount", { Width: 130 }),
        );
    }

    openCreateTag(): void {
        const dialogRef = this.dialogService.open(TagModalComponent, {
            data: { mode: "create" } as TagModalData,
            width: "600px",
        });
        dialogRef.afterClosed$.subscribe(result => {
            if (result) {
                this.refreshTags$.next();
            }
        });
    }

    async confirmDelete(tag: TagGridRow): Promise<void> {
        const confirmed = await this.confirmService.confirm({
            title: "Delete Tag",
            message: `Are you sure you want to delete Tag "${tag.TagName}"?`,
            buttonTextYes: "Delete",
            buttonClassYes: "btn-danger",
            buttonTextNo: "Cancel",
        });

        if (confirmed) {
            this.tagService.deleteTag(tag.TagID).subscribe({
                next: () => {
                    this.alertService.pushAlert(new Alert("Tag deleted successfully.", AlertContext.Success));
                    this.refreshTags$.next();
                },
                error: (err) => {
                    const message = err?.error ?? err?.message ?? "An error occurred.";
                    this.alertService.pushAlert(new Alert(message, AlertContext.Danger));
                },
            });
        }
    }
}
