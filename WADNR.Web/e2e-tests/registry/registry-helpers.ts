/**
 * Query utilities for the page registry.
 *
 * These functions derive filtered views and statistics from the central
 * page registry without duplicating any data.
 */

import { type PageRegistryEntry, type MigrationStatus, pageRegistry } from "./page-registry";

// ─── Query Functions ───────────────────────────────────────────────────────────

/**
 * Pages that can be visually compared between legacy and modern:
 * - status = "migrated"
 * - has both legacyPath and modernPath
 * - pageType = "page" (not modal)
 */
export function getComparablePages(): PageRegistryEntry[] {
    return pageRegistry.filter(
        (e) => e.status === "migrated" && e.modernPath != null && e.pageType === "page",
    );
}

/**
 * Pages that have not yet been migrated.
 */
export function getGaps(): PageRegistryEntry[] {
    return pageRegistry.filter((e) => e.status === "not-yet-migrated");
}

/**
 * Pages explicitly excluded from migration (dead code).
 */
export function getExcluded(): PageRegistryEntry[] {
    return pageRegistry.filter((e) => e.status === "excluded");
}

/**
 * Pages restructured into a different UX pattern (workflow, modal, etc.).
 */
export function getRestructured(): PageRegistryEntry[] {
    return pageRegistry.filter((e) => e.status === "restructured");
}

/**
 * All pages with a modern route (migrated + restructured with a modernPath).
 */
export function getModernPages(): PageRegistryEntry[] {
    return pageRegistry.filter((e) => e.modernPath != null);
}

/**
 * Pages filtered by area.
 */
export function getByArea(area: string): PageRegistryEntry[] {
    return pageRegistry.filter((e) => e.area === area);
}

/**
 * Pages filtered by auth requirement.
 */
export function getByAuth(auth: PageRegistryEntry["auth"]): PageRegistryEntry[] {
    return pageRegistry.filter((e) => e.auth === auth);
}

// ─── Coverage Statistics ───────────────────────────────────────────────────────

export interface CoverageStats {
    total: number;
    migrated: number;
    restructured: number;
    notYet: number;
    excluded: number;
    /** (migrated + restructured + excluded) / total × 100 */
    coveragePercent: number;
}

/**
 * Calculate migration coverage statistics.
 *
 * Coverage = pages that are done (migrated + restructured + excluded) / total.
 * "Not yet migrated" pages are the gap.
 */
export function getCoverageStats(): CoverageStats {
    const counts: Record<MigrationStatus, number> = {
        migrated: 0,
        restructured: 0,
        excluded: 0,
        "not-yet-migrated": 0,
    };

    for (const entry of pageRegistry) {
        counts[entry.status]++;
    }

    const total = pageRegistry.length;
    const done = counts.migrated + counts.restructured + counts.excluded;
    const coveragePercent = total > 0 ? (done / total) * 100 : 0;

    return {
        total,
        migrated: counts.migrated,
        restructured: counts.restructured,
        notYet: counts["not-yet-migrated"],
        excluded: counts.excluded,
        coveragePercent,
    };
}

// ─── URL Resolution ────────────────────────────────────────────────────────────

/**
 * Resolve a registry entry's URL by replacing {token} placeholders with values
 * from the provided token map.
 *
 * @param entry - The page registry entry
 * @param side - "modern" or "legacy"
 * @param tokenMap - Map of token names to values (e.g., { projectID: 12688 })
 * @returns Resolved URL string, or null if the entry has no path for the requested side
 */
export function resolveUrl(
    entry: PageRegistryEntry,
    side: "modern" | "legacy",
    tokenMap: Record<string, string | number>,
): string | null {
    const path = side === "modern" ? entry.modernPath : entry.legacyPath;
    if (path == null) return null;

    let resolved = path;
    for (const [key, value] of Object.entries(tokenMap)) {
        resolved = resolved.replace(`{${key}}`, String(value));
    }
    return resolved;
}

// ─── Unique Areas ──────────────────────────────────────────────────────────────

/**
 * Get all unique area names in the registry.
 */
export function getAreas(): string[] {
    return [...new Set(pageRegistry.map((e) => e.area))];
}
