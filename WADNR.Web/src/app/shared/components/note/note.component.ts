import { Component, Input } from "@angular/core";
import { NgClass } from "@angular/common";

@Component({
    selector: "note",
    templateUrl: "./note.component.html",
    styleUrls: ["./note.component.scss"],
    imports: [NgClass],
})
export class NoteComponent {
    @Input() noteType: typeof NoteType = "default";
}

export let NoteType: "default" | "danger" | "info" | "success" | "warning";
