import { Injectable } from "@angular/core";
import { Subject } from "rxjs";

/**
 * Shared service that bridges workflow step components and outlet components.
 * Step components call triggerRefresh() after save/revert to notify the outlet
 * to re-fetch workflow progress (updating the sidebar nav state).
 */
@Injectable({ providedIn: "root" })
export class WorkflowProgressService {
    private _refreshProgress$ = new Subject<void>();
    public refreshProgress$ = this._refreshProgress$.asObservable();

    triggerRefresh(): void {
        this._refreshProgress$.next();
    }
}
