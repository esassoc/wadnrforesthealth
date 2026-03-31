import { Component, inject, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormControl, FormGroup, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ProjectUpdateReturnRequest } from "src/app/shared/generated/model/project-update-return-request";

export interface ReturnWithCommentsModalData {
    projectName: string;
}

@Component({
    selector: "return-with-comments-modal",
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, FormFieldComponent],
    templateUrl: "./return-with-comments-modal.component.html",
})
export class ReturnWithCommentsModalComponent implements OnInit {
    private dialogRef = inject(DialogRef<ReturnWithCommentsModalData, ProjectUpdateReturnRequest | null>);

    FormFieldType = FormFieldType;
    projectName: string = "";

    form = new FormGroup({
        BasicsComment: new FormControl<string>(""),
        LocationSimpleComment: new FormControl<string>(""),
        LocationDetailedComment: new FormControl<string>(""),
        ExpectedFundingComment: new FormControl<string>(""),
        ContactsComment: new FormControl<string>(""),
        OrganizationsComment: new FormControl<string>(""),
    });

    ngOnInit(): void {
        this.projectName = this.dialogRef.data?.projectName ?? "";
    }

    submit(): void {
        const v = this.form.value;
        const request: ProjectUpdateReturnRequest = {
            BasicsComment: v.BasicsComment || null,
            LocationSimpleComment: v.LocationSimpleComment || null,
            LocationDetailedComment: v.LocationDetailedComment || null,
            ExpectedFundingComment: v.ExpectedFundingComment || null,
            ContactsComment: v.ContactsComment || null,
            OrganizationsComment: v.OrganizationsComment || null,
        };
        this.dialogRef.close(request);
    }

    cancel(): void {
        this.dialogRef.close(null);
    }
}
