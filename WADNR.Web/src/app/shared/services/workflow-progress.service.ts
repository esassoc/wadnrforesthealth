import { Injectable } from "@angular/core";
import { BehaviorSubject, Subject } from "rxjs";

/**
 * Shared service that bridges workflow step components and outlet components.
 * Step components call triggerRefresh() after save/revert to notify the outlet
 * to re-fetch workflow progress (updating the sidebar nav state).
 */
@Injectable({ providedIn: "root" })
export class WorkflowProgressService {
    private _refreshProgress$ = new Subject<void>();
    public refreshProgress$ = this._refreshProgress$.asObservable();

    /** Tracks whether the current step has unsaved form changes. */
    private _formDirty$ = new BehaviorSubject<boolean>(false);
    public formDirty$ = this._formDirty$.asObservable();

    triggerRefresh(): void {
        this._refreshProgress$.next();
    }

    get isFormDirty(): boolean {
        return this._formDirty$.value;
    }

    setFormDirty(dirty: boolean): void {
        this._formDirty$.next(dirty);
    }
}
