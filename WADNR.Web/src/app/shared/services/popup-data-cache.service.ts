import { Injectable } from "@angular/core";
import { Observable, of } from "rxjs";
import { shareReplay, tap } from "rxjs/operators";

@Injectable({
    providedIn: "root",
})
export class PopupDataCacheService {
    private readonly valueCache = new Map<string, unknown>();
    private readonly observableCache = new Map<string, Observable<unknown>>();

    getCachedValue<T>(key: string): T | undefined {
        return this.valueCache.get(key) as T | undefined;
    }

    set<T>(key: string, value: T): void {
        this.valueCache.set(key, value);
        this.observableCache.set(key, of(value));
    }

    getOrFetch<T>(key: string, fetcher: () => Observable<T>): Observable<T> {
        const cachedValue = this.valueCache.get(key) as T | undefined;
        if (cachedValue !== undefined) {
            return of(cachedValue);
        }

        const cachedObs = this.observableCache.get(key) as Observable<T> | undefined;
        if (cachedObs) {
            return cachedObs;
        }

        const obs = fetcher().pipe(
            tap((value) => this.valueCache.set(key, value)),
            shareReplay(1)
        );

        this.observableCache.set(key, obs as Observable<unknown>);
        return obs;
    }

    clear(key: string): void {
        this.valueCache.delete(key);
        this.observableCache.delete(key);
    }

    clearAll(): void {
        this.valueCache.clear();
        this.observableCache.clear();
    }
}

export function buildPopupCacheKey(tagName: string, id: string | number): string {
    return `${String(tagName)}:${String(id)}`;
}
