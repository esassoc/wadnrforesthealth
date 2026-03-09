import { Component, OnInit } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { DomSanitizer, SafeHtml } from "@angular/platform-browser";
import { ColDef, GridOptions } from "ag-grid-community";
import { BehaviorSubject, Observable, of, switchMap, map, startWith, shareReplay } from "rxjs";
import { DialogService } from "@ngneat/dialog";

import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";

import { CustomPageService } from "src/app/shared/generated/api/custom-page.service";
import { CustomPageGridRow } from "src/app/shared/generated/model/custom-page-grid-row";
import { CustomPageModalComponent, CustomPageModalData } from "./custom-page-modal/custom-page-modal.component";
import { CustomPageContentModalComponent, CustomPageContentModalData } from "./custom-page-content-modal/custom-page-content-modal.component";
import { CustomPageNavService } from "src/app/shared/services/custom-page-nav.service";

interface PreviewState {
    loading: boolean;
    html: SafeHtml | null;
}

@Component({
    selector: "manage-custom-pages",
    standalone: true,
    imports: [PageHeaderComponent, WADNRGridComponent, AsyncPipe, LoadingDirective],
    templateUrl: "./manage-custom-pages.component.html",
    styleUrl: "./manage-custom-pages.component.scss",
})
export class ManageCustomPagesComponent implements OnInit {
    public customPages$: Observable<CustomPageGridRow[]>;
    public columnDefs: ColDef<CustomPageGridRow>[] = [];
    public gridOptions: GridOptions = {};

    public selectedPage: CustomPageGridRow | null = null;
    public previewContent$: Observable<PreviewState>;

    private refreshPages$ = new BehaviorSubject<void>(undefined);
    private selectedPageID$ = new BehaviorSubject<number | null>(null);

    constructor(
        private customPageService: CustomPageService,
        private utilityFunctions: UtilityFunctionsService,
        private dialogService: DialogService,
        private confirmService: ConfirmService,
        private alertService: AlertService,
        private sanitizer: DomSanitizer,
        private customPageNavService: CustomPageNavService
    ) {}

    ngOnInit(): void {
        this.customPages$ = this.refreshPages$.pipe(
            switchMap(() => this.customPageService.listCustomPage())
        );
        this.buildColumnDefs();
        this.gridOptions = {
            onRowClicked: (event) => this.onRowClicked(event),
        };

        this.previewContent$ = this.selectedPageID$.pipe(
            switchMap(id => {
                if (id == null) return of(null);
                return this.customPageService.getByIDCustomPage(id).pipe(
                    map(detail => ({
                        loading: false,
                        html: detail.CustomPageContent
                            ? this.sanitizer.bypassSecurityTrustHtml(detail.CustomPageContent)
                            : null,
                    } as PreviewState)),
                    startWith({ loading: true, html: null } as PreviewState)
                );
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }

    private buildColumnDefs(): void {
        this.columnDefs = [
            this.utilityFunctions.createActionsColumnDef((params) => {
                const page = params.data as CustomPageGridRow;
                return [
                    { ActionName: "Edit", ActionHandler: () => this.openEdit(page), ActionIcon: "fa fa-pencil" },
                    { ActionName: "Edit Content", ActionHandler: () => this.openEditContent(page), ActionIcon: "fa fa-file-text" },
                    { ActionName: "Delete", ActionHandler: () => this.confirmDelete(page), ActionIcon: "fa fa-trash" },
                ];
            }),
            this.utilityFunctions.createBasicColumnDef("Page Name", "CustomPageDisplayName", { Width: 200 }),
            this.utilityFunctions.createBasicColumnDef("Vanity URL", "CustomPageVanityUrl", { Width: 150 }),
            this.utilityFunctions.createBooleanColumnDef("Has Content", "HasContent", { Width: 100, CustomDropdownFilterField: "HasContent" }),
            this.utilityFunctions.createBasicColumnDef("Display Type", "CustomPageDisplayTypeName", { Width: 120, FieldDefinitionType: "CustomPageDisplayType" }),
            this.utilityFunctions.createBasicColumnDef("Navigation Section", "CustomPageNavigationSectionName", { Width: 150 }),
        ];
    }

    onRowClicked(event: any): void {
        const page = event.data as CustomPageGridRow;
        if (!page) return;

        this.selectedPage = page;
        this.selectedPageID$.next(page.CustomPageID);
    }

    openCreate(): void {
        const dialogRef = this.dialogService.open(CustomPageModalComponent, {
            data: { mode: "create" } as CustomPageModalData,
            width: "600px",
        });
        dialogRef.afterClosed$.subscribe(result => {
            if (result) {
                this.refreshPages$.next();
                this.customPageNavService.triggerRefresh();
            }
        });
    }

    openEdit(page: CustomPageGridRow): void {
        const dialogRef = this.dialogService.open(CustomPageModalComponent, {
            data: { mode: "edit", customPage: page } as CustomPageModalData,
            width: "600px",
        });
        dialogRef.afterClosed$.subscribe(result => {
            if (result) {
                this.refreshPages$.next();
                this.customPageNavService.triggerRefresh();
                if (this.selectedPage?.CustomPageID === page.CustomPageID) {
                    this.selectedPageID$.next(page.CustomPageID);
                }
            }
        });
    }

    openEditContent(page: CustomPageGridRow): void {
        const dialogRef = this.dialogService.open(CustomPageContentModalComponent, {
            data: { customPageID: page.CustomPageID, currentContent: page.CustomPageContent ?? "" } as CustomPageContentModalData,
            width: "900px",
        });
        dialogRef.afterClosed$.subscribe(result => {
            if (result) {
                this.refreshPages$.next();
                this.customPageNavService.triggerRefresh();
                if (this.selectedPage?.CustomPageID === page.CustomPageID) {
                    this.selectedPageID$.next(page.CustomPageID);
                }
            }
        });
    }

    async confirmDelete(page: CustomPageGridRow): Promise<void> {
        const confirmed = await this.confirmService.confirm({
            title: "Delete Custom Page",
            message: `Are you sure you want to delete "${page.CustomPageDisplayName}"? This action cannot be undone.`,
            buttonTextYes: "Delete",
            buttonClassYes: "btn-danger",
            buttonTextNo: "Cancel",
        });

        if (confirmed) {
            this.customPageService.deleteCustomPage(page.CustomPageID).subscribe({
                next: () => {
                    this.alertService.pushAlert(new Alert("Custom page deleted successfully.", AlertContext.Success));
                    this.refreshPages$.next();
                    this.customPageNavService.triggerRefresh();
                    if (this.selectedPage?.CustomPageID === page.CustomPageID) {
                        this.selectedPage = null;
                        this.selectedPageID$.next(null);
                    }
                },
                error: (err) => {
                    const message = err?.error ?? err?.message ?? "An error occurred.";
                    this.alertService.pushAlert(new Alert(message, AlertContext.Danger));
                },
            });
        }
    }
}
