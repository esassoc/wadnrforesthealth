import { Injectable } from "@angular/core";
import { Subject, ReplaySubject, Observable, Subscription } from "rxjs";
import { RoadRegistrationService } from "../shared/generated/api/road-registration.service";
import { RoadRegistrationWorkflowProgressDto } from "../shared/generated/model/road-registration-workflow-progress-dto";

@Injectable({
    providedIn: "root",
})
export class RoadRegistrationWorkflowProgressService {
    private progressSubject: Subject<RoadRegistrationWorkflowProgressDto> = new ReplaySubject();
    public progressObservable$: Observable<RoadRegistrationWorkflowProgressDto> = this.progressSubject.asObservable();

    private progressSubscription = Subscription.EMPTY;

    constructor(private roadRegistrationService: RoadRegistrationService) {}

    updateProgress(registrationID: number): void {
        this.progressSubscription.unsubscribe();
        this.getProgress(registrationID);
    }

    getProgress(registrationID: number) {
        // If a registrationID is provided, call the backend workflow progress endpoint and emit its result.
        if (registrationID) {
            this.progressSubscription = this.roadRegistrationService.getWorkflowProgressRoadRegistration(registrationID).subscribe({
                next: (dto) => this.progressSubject.next(dto),
            });
        } else {
            // no registrationID: emit client-side defaults
            this.progressSubject.next({
                Steps: {
                    RegistrationInformation: { Completed: true, Disabled: false },
                    RegistrationArea: { Completed: false, Disabled: true },
                    UploadPLRMProjects: { Completed: false, Disabled: true },
                    RegisterRoadClasses: { Completed: false, Disabled: true },
                    ProposeRoadSegments: { Completed: false, Disabled: true },
                    RegisterCredits: { Completed: false, Disabled: true },
                },
            });
        }
    }
}
