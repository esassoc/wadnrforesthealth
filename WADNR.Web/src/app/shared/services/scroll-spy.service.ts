import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";

@Injectable({ providedIn: "root" })
export class ScrollSpyService {
    private _active = new BehaviorSubject<string | null>(null);
    public active$: Observable<string | null> = this._active.asObservable();

    setActive(id: string | null) {
        this._active.next(id);
    }
}
