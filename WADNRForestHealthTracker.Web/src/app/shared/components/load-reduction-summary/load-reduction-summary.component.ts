import { Component, Input } from "@angular/core";
import { CommonModule } from "@angular/common";
import { RoadRegistrationDetail } from "src/app/shared/generated/model/road-registration-detail";

@Component({
    selector: "load-reduction-summary",
    standalone: true,
    imports: [CommonModule],
    templateUrl: "./load-reduction-summary.component.html",
    styleUrls: ["./load-reduction-summary.component.scss"],
})
export class LoadReductionSummaryComponent {
    @Input() public roadRegistration?: RoadRegistrationDetail = null;

    public hasPlrm(): boolean {
        return !!this.roadRegistration?.PlrmZipFileResourceInfoID;
    }
}
