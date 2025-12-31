import { RoadClassDetail } from "src/app/shared/generated/model/road-class-detail";
import { RoadRegistrationScenarioRoadClassDetail } from "src/app/shared/generated/model/road-registration-scenario-road-class-detail";

export function filterRoadClassesByScore(
    roadRegistrationScenarioRoadClasses: RoadRegistrationScenarioRoadClassDetail[],
    expectedScore: number
): RoadRegistrationScenarioRoadClassDetail[] {
    return roadRegistrationScenarioRoadClasses.filter((x) => x.PlrmRoadClassScore === expectedScore);
}

export function getTotalImperviousAreaOfRoadRegistrationScenarioRoadClasses(
    roadClass: RoadClassDetail,
    roadRegistrationScenarioRoadClasses: RoadRegistrationScenarioRoadClassDetail[]
): number {
    return filterRoadClassesByScore(roadRegistrationScenarioRoadClasses, roadClass.ExpectedScore)
        .map((x) => x.PlrmRoadClassArea)
        .reduce((acc, curr) => acc + curr, 0);
}

export function getNumberOfRoadSegmentsRequiredForImperviousArea(
    roadClass: RoadClassDetail,
    roadRegistrationScenarioRoadClasses: RoadRegistrationScenarioRoadClassDetail[]
): number {
    const totalImperviousArea = getTotalImperviousAreaOfRoadRegistrationScenarioRoadClasses(roadClass, roadRegistrationScenarioRoadClasses);
    let numberOfSegments = 0;
    if (0 < totalImperviousArea && totalImperviousArea <= 8) {
        numberOfSegments = 4;
    }
    if (8 < totalImperviousArea && totalImperviousArea <= 17) {
        numberOfSegments = 8;
    } else if (17 < totalImperviousArea && totalImperviousArea <= 42) {
        numberOfSegments = 12;
    } else if (42 < totalImperviousArea && totalImperviousArea <= 85) {
        numberOfSegments = 16;
    } else if (85 < totalImperviousArea) {
        numberOfSegments = 20;
    }

    return numberOfSegments;
}
