/**
 * Comparison configuration for WADNR Forest Health Tracker.
 *
 * Maps Angular routes to their legacy MVC equivalents for visual comparison testing.
 * WADNR is single-domain (no subdomain routing).
 *
 * Data is derived from the central page registry — do not add pages here directly.
 * Add them to e2e-tests/registry/page-registry.ts instead.
 */

import { getComparablePages } from "../registry/registry-helpers";

export interface ComparisonPage {
    /** Human-readable page name */
    name: string;
    /** Angular route path (may contain {token} placeholders) */
    angularPath: string;
    /** Legacy MVC route path (may contain {token} placeholders) */
    legacyPath: string;
    /** Tokens required for this page (resolved from test-data.ts) */
    tokens?: string[];
    /** Whether to wait extra time for this page (maps, heavy grids) */
    slowPage?: boolean;
}

/**
 * Get all comparison pages as a flat array.
 *
 * Derived from the page registry: returns all entries with status "migrated",
 * both paths present, and pageType "page".
 */
export function getAllComparisonPages(): ComparisonPage[] {
    return getComparablePages().map((entry) => ({
        name: entry.name,
        angularPath: entry.modernPath!,
        legacyPath: entry.legacyPath,
        tokens: entry.tokens,
        slowPage: entry.slowPage,
    }));
}

/**
 * Get comparison pages filtered by area.
 */
export function getComparisonPagesByArea(area: string): ComparisonPage[] {
    return getComparablePages()
        .filter((entry) => entry.area === area)
        .map((entry) => ({
            name: entry.name,
            angularPath: entry.modernPath!,
            legacyPath: entry.legacyPath,
            tokens: entry.tokens,
            slowPage: entry.slowPage,
        }));
}
