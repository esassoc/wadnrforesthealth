import { Component } from "@angular/core";
import { CommonModule, DatePipe } from "@angular/common";
import { DialogRef } from "@ngneat/dialog";
import { ProjectUpdateHistoryEntry } from "src/app/shared/generated/model/project-update-history-entry";

export interface UpdateHistoryModalData {
    entries: ProjectUpdateHistoryEntry[];
}

@Component({
    selector: "update-history-modal",
    standalone: true,
    imports: [CommonModule, DatePipe],
    template: `
        <div class="modal">
            <div class="modal-header">
                <h4>History</h4>
            </div>
            <div class="modal-body">
                <p>The following is the high-level summary of the history for this Project update.</p>
                @if (lastEntry) {
                    <p>
                        Last Updated: {{ lastEntry.TransitionDate | date : "M/d/yyyy h:mm a" }}
                        - {{ lastEntry.UpdatePersonName }}
                    </p>
                }
                <hr />
                <table class="table table-sm">
                    <thead>
                        <tr>
                            <th>Date</th>
                            <th>Action</th>
                            <th>User</th>
                        </tr>
                    </thead>
                    <tbody>
                        @for (entry of entries; track entry.TransitionDate) {
                            <tr>
                                <td>{{ entry.TransitionDate | date : "M/d/yyyy h:mm a" }}</td>
                                <td>{{ entry.ProjectUpdateStateName }}</td>
                                <td>{{ entry.UpdatePersonName }}</td>
                            </tr>
                        }
                    </tbody>
                </table>
            </div>
            <div class="modal-footer">
                <button class="btn btn-secondary" (click)="dialogRef.close()">Close</button>
            </div>
        </div>
    `,
})
export class UpdateHistoryModalComponent {
    entries: ProjectUpdateHistoryEntry[];
    lastEntry: ProjectUpdateHistoryEntry | null;

    constructor(public dialogRef: DialogRef<UpdateHistoryModalData>) {
        this.entries = dialogRef.data.entries;
        this.lastEntry = this.entries.length > 0 ? this.entries[this.entries.length - 1] : null;
    }
}
