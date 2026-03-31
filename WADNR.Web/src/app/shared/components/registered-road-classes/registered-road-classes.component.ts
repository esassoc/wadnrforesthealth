import { Component, Input } from "@angular/core";
import { CommonModule } from "@angular/common";
import { RouterLink } from "@angular/router";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";
import { RoadClassDetail } from "src/app/shared/generated/model/road-class-detail";
import { RoadRegistrationScenarioRoadClassDetail } from "src/app/shared/generated/model/road-registration-scenario-road-class-detail";
import {
    filterRoadClassesByScore as utilFilterRoadClassesByScore,
    getTotalImperviousAreaOfRoadRegistrationScenarioRoadClasses as utilGetTotalImperviousArea,
    getNumberOfRoadSegmentsRequiredForImperviousArea as utilGetNumberOfRoadSegmentsRequired,
} from "src/app/shared/utils/road-class-utils";

@Component({
    selector: "registered-road-classes",
    standalone: true,
    templateUrl: "./registered-road-classes.component.html",
    styleUrls: ["./registered-road-classes.component.scss"],
    imports: [CommonModule, RouterLink, FieldDefinitionComponent],
})
export class RegisteredRoadClassesComponent {
    @Input() public roadClasses: RoadClassDetail[] = [];
    @Input() public roadRegistrationScenarioRoadClasses: RoadRegistrationScenarioRoadClassDetail[] = [];
    @Input() public totalNumberOfRoadSegmentsRequired: number = 0;

    // Child owns helper implementations by delegating to shared utils
    public filterRoadClassesByScore(roadRegistrationScenarioRoadClasses: RoadRegistrationScenarioRoadClassDetail[], expectedScore: number) {
        return utilFilterRoadClassesByScore(roadRegistrationScenarioRoadClasses, expectedScore);
    }

    public getTotalImperviousAreaOfRoadRegistrationScenarioRoadClasses(roadClass: RoadClassDetail, roadRegistrationScenarioRoadClasses: RoadRegistrationScenarioRoadClassDetail[]) {
        return utilGetTotalImperviousArea(roadClass, roadRegistrationScenarioRoadClasses);
    }

    public getNumberOfRoadSegmentsRequiredForImperviousArea(roadClass: RoadClassDetail, roadRegistrationScenarioRoadClasses: RoadRegistrationScenarioRoadClassDetail[]) {
        return utilGetNumberOfRoadSegmentsRequired(roadClass, roadRegistrationScenarioRoadClasses);
    }
}
