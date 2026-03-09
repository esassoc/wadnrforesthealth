import { Injectable } from "@angular/core";
import { BehaviorSubject } from "rxjs";

@Injectable({ providedIn: "root" })
export class CustomPageNavService {
    private refresh$ = new BehaviorSubject<void>(undefined);
    public refreshSignal$ = this.refresh$.asObservable();

    triggerRefresh(): void {
        this.refresh$.next();
    }
}
