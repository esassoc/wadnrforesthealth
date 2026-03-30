import { catchError, map, Observable, of, shareReplay, startWith } from "rxjs";

export function toLoadingState(source$: Observable<any>): Observable<boolean> {
    return source$.pipe(
        map(() => false),
        catchError(() => of(false)),
        startWith(true),
        shareReplay({ bufferSize: 1, refCount: true })
    );
}
