import { Component, Input } from "@angular/core";
import { FieldDefinitionComponent } from "../field-definition/field-definition.component";

@Component({
    selector: "app-forester-popup",
    standalone: true,
    imports: [FieldDefinitionComponent],
    template: `
        @if (roleName) {
            <field-definition [fieldDefinition]="roleName"></field-definition>
        } @else if (roleDisplayName) {
            <strong>{{ roleDisplayName }}</strong>
        }
        @if (firstName) {
            <div>{{ firstName }} {{ lastName }}</div>
            @if (phone) {
                <div><a [href]="'tel:' + phone">{{ phone }}</a></div>
            }
            @if (email) {
                <div><a [href]="'mailto:' + email">{{ email }}</a></div>
            }
        } @else {
            <div><em>This role is unassigned for this region.</em></div>
        }
    `,
})
export class ForesterPopupComponent {
    @Input() roleName: string;
    @Input() roleDisplayName: string;
    @Input() firstName: string;
    @Input() lastName: string;
    @Input() phone: string;
    @Input() email: string;
}
