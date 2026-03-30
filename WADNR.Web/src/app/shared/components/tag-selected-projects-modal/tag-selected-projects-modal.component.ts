import { Component, inject, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { DialogRef } from "@ngneat/dialog";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { Observable, combineLatest, map, startWith } from "rxjs";
import { BulkTagProjectsRequest } from "src/app/shared/generated/model/bulk-tag-projects-request";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { TagService } from "src/app/shared/generated/api/tag.service";
import { TagGridRow } from "src/app/shared/generated/model/tag-grid-row";

import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { BaseModal } from "src/app/shared/components/modal/base-modal";

@Component({
    selector: "tag-selected-projects-modal",
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent],
    templateUrl: "./tag-selected-projects-modal.component.html",
    styleUrls: ["./tag-selected-projects-modal.component.scss"],
})
export class TagSelectedProjectsModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<{ projects: any[] }, boolean> = inject(DialogRef);
    public form = new FormGroup({ TagName: new FormControl<string>(undefined, { validators: [Validators.required] }) });
    public FormFieldType = FormFieldType;
    public projects: any[] = [];
    public filteredTags$: Observable<TagGridRow[]>;

    constructor(private tagService: TagService, alertService: AlertService) {
        super(alertService);
    }

    ngOnInit(): void {
        this.projects = this.ref.data?.projects ?? [];

        const allTags$ = this.tagService.listTag();
        const typedValue$ = this.form.controls.TagName.valueChanges.pipe(startWith(""));

        this.filteredTags$ = combineLatest([allTags$, typedValue$]).pipe(
            map(([tags, query]) => {
                const q = (query ?? "").trim().toLowerCase();
                if (!q) return [];
                return tags
                    .filter((t) => t.TagName?.toLowerCase().includes(q) && t.TagName?.toLowerCase() !== q)
                    .slice(0, 10);
            }),
        );
    }

    selectTag(tagName: string): void {
        this.form.controls.TagName.setValue(tagName);
    }

    save(): void {
        if (this.form.invalid) return;
        const projectIDs = this.projects?.map((p) => p.ProjectID).filter((x) => x != null) ?? [];
        const dto = new BulkTagProjectsRequest({
            TagName: this.form.value.TagName,
            ProjectIDs: projectIDs,
        });

        this.tagService.bulkTagProjectsTag(dto).subscribe({
            next: () => {
                const count = projectIDs.length;
                const message = count > 0 ? `Successfully tagged ${count} project${count > 1 ? "s" : ""}.` : "Tag created.";
                // push success to global alert service (page-level) and close
                this.pushGlobalSuccess(message);
                this.ref.close(true);
            },
            error: (err) => {
                // keep error local to the modal so the user can retry
                const message = err?.message ?? "Failed to create tag.";
                this.addLocalAlert(message, AlertContext.Danger, true);
                // also ensure any global clearing doesn't remove these local modal alerts
                // (modal-level alert display will render `localAlerts`)
            },
        });
    }

    cancel(): void {
        this.ref.close(null);
    }
}
