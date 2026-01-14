/**
 * Leaflet popup helpers ("two-phase" rendering).
 *
 * Why this exists:
 * - Leaflet measures popup size when it opens.
 * - When popup content is a custom element (Angular Element) that renders async and/or shows a loading spinner,
 *   Leaflet will often size the popup to the *spinner* state. When the real content renders later, it can overflow
 *   or appear constrained.
 *
 * What this accomplishes:
 * - Opens a lightweight loading popup first.
 * - Prefetches data into `PopupDataCacheService`.
 * - Closes the loading popup and opens a *fresh* popup with the final HTML/custom element, so Leaflet measures the
 *   correct content state.
 *
 * How to use:
 * - For custom elements, prefer `bindTwoPhaseCustomElementPopup` or `openTwoPhaseCustomElementPopupAt`.
 * - Provide `cacheId` (or `cacheKey`) and a `fetcher`; the helper will build the cache key and inject `cache-key`
 *   into the custom element attributes automatically.
 */
import * as L from "leaflet";
import { Observable } from "rxjs";
import { take } from "rxjs";
import { buildPopupCacheKey, PopupDataCacheService } from "src/app/shared/services/popup-data-cache.service";

export interface TwoPhasePopupConfig {
    popupOptions: L.PopupOptions;
    buildLoadingHtml: () => string;
    buildLoadedHtml: () => string;
    prefetch$: () => Observable<unknown>;
    getMap: () => L.Map | null | undefined;
}

export interface TwoPhaseCustomElementPopupConfig<T> {
    popupOptions?: L.PopupOptions;
    spinnerMinWidthPx?: number;
    spinnerMinHeightPx?: number;

    customElementTagName: string;
    customElementAttributes?: Record<string, string | number | boolean | null | undefined>;

    // If omitted, provide cacheId and the helper will build `${cacheTagName ?? customElementTagName}:${cacheId}`.
    cacheKey?: string;
    cacheId?: string | number;
    cacheTagName?: string;
    cache: PopupDataCacheService;
    fetcher: () => Observable<T>;

    getMap: () => L.Map | null | undefined;
}

function resolveCacheKey(config: TwoPhaseCustomElementPopupConfig<unknown>): string | null {
    if (config.cacheKey) {
        return config.cacheKey;
    }
    if (config.cacheId === undefined || config.cacheId === null) {
        return null;
    }
    const tag = config.cacheTagName ?? config.customElementTagName;
    return buildPopupCacheKey(tag, config.cacheId);
}

function resolveCustomElementAttributes(
    config: TwoPhaseCustomElementPopupConfig<unknown>,
    cacheKey: string | null
): Record<string, string | number | boolean | null | undefined> | undefined {
    if (!config.customElementAttributes && !cacheKey) {
        return config.customElementAttributes;
    }

    const attrs: Record<string, string | number | boolean | null | undefined> = {
        ...(config.customElementAttributes ?? {}),
    };

    // Safe to include even if the custom element doesn't declare this input.
    if (cacheKey && attrs["cache-key"] === undefined) {
        attrs["cache-key"] = cacheKey;
    }

    return attrs;
}

export const DEFAULT_LEAFLET_POPUP_OPTIONS: L.PopupOptions = {
    maxWidth: 475,
    keepInView: false,
    autoPan: false,
};

export function buildSpinnerPopupHtml(minWidthPx: number = 110, minHeightPx: number = 90): string {
    // Uses existing global spinner styles in src/scss/utilities/_spinner.scss
    return `
        <div class="has-spinner" style="min-width: ${minWidthPx}px; min-height: ${minHeightPx}px;">
            <div class="spinner-container">
                <div class="circle"><div class="wave"></div></div>
            </div>
        </div>
    `.trim();
}

export function scheduleLeafletPopupUpdate(popup: L.Popup): void {
    const update = () => {
        try {
            popup.update();
        } catch {
            // best-effort
        }
    };

    // With the two-phase approach we open the "loaded" popup only after data is prefetched,
    // so sizing is usually correct immediately. A couple of deferred updates still help because
    // custom elements may render on the next microtask / animation frame.
    Promise.resolve().then(update);
    if (typeof requestAnimationFrame !== "undefined") {
        requestAnimationFrame(update);
    }
}

function buildCustomElementHtml(tagName: string, attrs?: Record<string, string | number | boolean | null | undefined>): string {
    const parts: string[] = [];
    if (attrs) {
        for (const [key, raw] of Object.entries(attrs)) {
            if (raw === undefined || raw === null) {
                continue;
            }
            const value = typeof raw === "boolean" ? String(raw) : String(raw);
            // Attributes are injected into an HTML string for Leaflet; keep escaping minimal but safe.
            const escaped = value.replaceAll('"', "&quot;");
            parts.push(`${key}="${escaped}"`);
        }
    }
    const attrString = parts.length ? " " + parts.join(" ") : "";
    return `<${tagName}${attrString}></${tagName}>`;
}

export function bindTwoPhaseCustomElementPopup<T>(layer: L.Layer, config: TwoPhaseCustomElementPopupConfig<T>): void {
    const bindable = layer as any;
    if (typeof bindable.bindPopup !== "function" || typeof bindable.on !== "function") {
        return;
    }

    const cacheKey = resolveCacheKey(config);
    if (!cacheKey) {
        return;
    }

    const popupOptions = config.popupOptions ?? DEFAULT_LEAFLET_POPUP_OPTIONS;
    const loadingHtml = buildSpinnerPopupHtml(config.spinnerMinWidthPx, config.spinnerMinHeightPx);
    const loadedHtml = buildCustomElementHtml(config.customElementTagName, resolveCustomElementAttributes(config, cacheKey));

    bindTwoPhasePopup(layer, {
        popupOptions,
        buildLoadingHtml: () => loadingHtml,
        buildLoadedHtml: () => loadedHtml,
        prefetch$: () => config.cache.getOrFetch(cacheKey, config.fetcher),
        getMap: config.getMap,
    });
}

export function openTwoPhaseCustomElementPopupAt<T>(
    latlng: L.LatLng,
    config: Omit<TwoPhaseCustomElementPopupConfig<T>, "getMap"> & { getMap: () => L.Map | null | undefined }
): void {
    const map = config.getMap();
    if (!map) {
        return;
    }

    const cacheKey = resolveCacheKey(config);
    if (!cacheKey) {
        return;
    }

    const popupOptions = config.popupOptions ?? DEFAULT_LEAFLET_POPUP_OPTIONS;
    const loadingHtml = buildSpinnerPopupHtml(config.spinnerMinWidthPx, config.spinnerMinHeightPx);
    const loadedHtml = buildCustomElementHtml(config.customElementTagName, resolveCustomElementAttributes(config, cacheKey));

    const loadingPopup = L.popup(popupOptions).setLatLng(latlng).setContent(loadingHtml).openOn(map);

    config.cache
        .getOrFetch(cacheKey, config.fetcher)
        .pipe(take(1))
        .subscribe({
            next: () => {
                if (!map || !(map as any).hasLayer?.(loadingPopup)) {
                    return;
                }
                try {
                    loadingPopup.remove();
                } catch {
                    // ignore
                }

                const loadedPopup = L.popup(popupOptions).setLatLng(latlng).setContent(loadedHtml).openOn(map);
                scheduleLeafletPopupUpdate(loadedPopup);
            },
            error: () => {
                // Leave the loading popup if fetch fails.
            },
        });
}

export function bindTwoPhasePopup(layer: L.Layer, config: TwoPhasePopupConfig): void {
    const bindable = layer as any;
    if (typeof bindable.bindPopup !== "function" || typeof bindable.on !== "function") {
        return;
    }

    bindable.bindPopup(config.buildLoadingHtml(), config.popupOptions);

    bindable.on("popupopen", (ev: any) => {
        const popup = ev?.popup as L.Popup | undefined;
        const map = config.getMap();
        if (!popup || !map) {
            return;
        }

        const latlng = popup.getLatLng();

        config
            .prefetch$()
            .pipe(take(1))
            .subscribe({
                next: () => {
                    // If the user closed it already, don't reopen.
                    if (!map || !(map as any).hasLayer?.(popup)) {
                        return;
                    }

                    try {
                        popup.remove();
                    } catch {
                        // ignore
                    }

                    const loadedPopup = L.popup(config.popupOptions).setLatLng(latlng).setContent(config.buildLoadedHtml()).openOn(map);
                    scheduleLeafletPopupUpdate(loadedPopup);
                },
                error: () => {
                    // Leave the loading popup if prefetch fails.
                },
            });
    });
}
