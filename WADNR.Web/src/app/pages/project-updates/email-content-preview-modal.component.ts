import { Component, inject, OnInit, signal } from "@angular/core";
import { DialogRef } from "@ngneat/dialog";

import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { ProjectUpdateConfigurationService } from "src/app/shared/generated/api/project-update-configuration.service";

@Component({
    selector: "email-content-preview-modal",
    standalone: true,
    imports: [LoadingDirective],
    templateUrl: "./email-content-preview-modal.component.html",
})
export class EmailContentPreviewModalComponent implements OnInit {
    public ref: DialogRef<{ emailType: string; emailTypeLabel: string }> = inject(DialogRef);
    public isLoading = signal(true);
    public previewHtml = signal<string | null>(null);
    public emailTypeLabel = "";

    private projectUpdateConfigurationService = inject(ProjectUpdateConfigurationService);

    ngOnInit(): void {
        this.emailTypeLabel = this.ref.data?.emailTypeLabel ?? "Email";
        const emailType = this.ref.data?.emailType;
        if (emailType) {
            this.projectUpdateConfigurationService.getEmailContentPreviewProjectUpdateConfiguration(emailType).subscribe({
                next: (result) => {
                    this.previewHtml.set(result.EmailContentHtml ?? null);
                    this.isLoading.set(false);
                },
                error: () => {
                    this.previewHtml.set("<p>Failed to load email preview.</p>");
                    this.isLoading.set(false);
                },
            });
        }
    }
}
