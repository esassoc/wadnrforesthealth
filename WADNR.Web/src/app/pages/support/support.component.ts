import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { DialogService } from "@ngneat/dialog";
import { FeedbackModalComponent, FeedbackModalData } from "src/app/shared/components/feedback-modal/feedback-modal.component";

@Component({
    selector: "app-support",
    standalone: true,
    template: "",
})
export class SupportComponent implements OnInit {
    constructor(
        private router: Router,
        private dialogService: DialogService
    ) {}

    ngOnInit(): void {
        const data: FeedbackModalData = { currentPageUrl: window.location.href };
        this.dialogService.open(FeedbackModalComponent, { data, width: "600px" });
        this.router.navigate(["/"]);
    }
}
