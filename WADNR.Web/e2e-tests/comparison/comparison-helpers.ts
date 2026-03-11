import { Page } from "@playwright/test";
import * as fs from "fs";
import * as path from "path";

// ─── Types ──────────────────────────────────────────────────────────────────

export interface PageElement {
    tag: string;
    text: string;
    classes: string[];
    attributes: Record<string, string>;
    children: PageElement[];
}

export interface PageSnapshot {
    url: string;
    title: string;
    timestamp: string;
    elements: PageElement[];
    screenshot?: string; // base64
}

export interface ComparisonDiff {
    type: "added" | "removed" | "changed" | "structural";
    path: string;
    description: string;
    oldValue?: string;
    newValue?: string;
    severity: "info" | "warning" | "error";
}

export interface PageComparisonResult {
    pageName: string;
    angularUrl: string;
    legacyUrl: string;
    status: "pass" | "fail" | "error" | "skipped" | "accepted";
    diffs: ComparisonDiff[];
    angularScreenshot?: string;
    legacyScreenshot?: string;
    angularSnapshot?: PageSnapshot;
    legacySnapshot?: PageSnapshot;
    error?: string;
    duration: number;
}

// ─── DOM Scraping ───────────────────────────────────────────────────────────

/**
 * Scrapes the visible DOM structure of a page, focusing on content-bearing elements.
 * Skips scripts, styles, hidden elements, and framework-internal nodes.
 */
export async function scrapePageStructure(page: Page): Promise<PageElement[]> {
    return await page.evaluate(() => {
        const SKIP_TAGS = new Set([
            "SCRIPT", "STYLE", "LINK", "META", "NOSCRIPT", "SVG", "PATH",
            "IFRAME", "TEMPLATE", "HEAD",
        ]);

        const CONTENT_TAGS = new Set([
            "H1", "H2", "H3", "H4", "H5", "H6",
            "P", "SPAN", "A", "BUTTON", "LABEL",
            "TABLE", "THEAD", "TBODY", "TR", "TH", "TD",
            "UL", "OL", "LI",
            "DL", "DT", "DD",
            "INPUT", "SELECT", "TEXTAREA",
            "IMG", "FORM",
            "DIV", "SECTION", "ARTICLE", "NAV", "HEADER", "FOOTER", "MAIN",
        ]);

        const STRUCTURAL_CLASSES = [
            "card", "card-header", "card-body", "card-footer", "card-title",
            "panel", "panel-heading", "panel-body", "panel-footer", "panel-title",
            "grid-12", "g-col-1", "g-col-2", "g-col-3", "g-col-4", "g-col-5", "g-col-6",
            "g-col-7", "g-col-8", "g-col-9", "g-col-10", "g-col-11", "g-col-12",
            "row", "col-sm-", "col-md-", "col-lg-", "col-xs-",
            "nav", "nav-tabs", "nav-pills", "tab-content", "tab-pane",
            "modal", "modal-header", "modal-body", "modal-footer",
            "btn", "btn-primary", "btn-secondary", "btn-danger",
            "form-group", "form-control",
            "table", "table-striped", "table-bordered",
            "ag-root", "ag-header", "ag-body",
            "alert", "alert-info", "alert-warning", "alert-danger", "alert-success",
        ];

        function getStructuralClasses(el: Element): string[] {
            const classList = Array.from(el.classList);
            return classList.filter(c =>
                STRUCTURAL_CLASSES.some(sc => c === sc || c.startsWith(sc))
            );
        }

        function getRelevantAttributes(el: Element): Record<string, string> {
            const attrs: Record<string, string> = {};
            const tag = el.tagName;

            if (tag === "A") {
                const href = el.getAttribute("href");
                if (href) attrs["href"] = href;
            }
            if (tag === "IMG") {
                const alt = el.getAttribute("alt");
                if (alt) attrs["alt"] = alt;
            }
            if (tag === "INPUT" || tag === "SELECT" || tag === "TEXTAREA") {
                const type = el.getAttribute("type");
                if (type) attrs["type"] = type;
                const name = el.getAttribute("name");
                if (name) attrs["name"] = name;
                const placeholder = el.getAttribute("placeholder");
                if (placeholder) attrs["placeholder"] = placeholder;
            }
            if (el.getAttribute("role")) {
                attrs["role"] = el.getAttribute("role")!;
            }
            if (el.getAttribute("data-testid")) {
                attrs["data-testid"] = el.getAttribute("data-testid")!;
            }
            return attrs;
        }

        function isVisible(el: Element): boolean {
            const style = window.getComputedStyle(el);
            return style.display !== "none"
                && style.visibility !== "hidden"
                && style.opacity !== "0"
                && el.getAttribute("aria-hidden") !== "true";
        }

        function scrapeElement(el: Element, depth: number = 0): PageElement | null {
            if (depth > 20) return null;
            if (SKIP_TAGS.has(el.tagName)) return null;
            if (!isVisible(el)) return null;

            // Skip Angular-internal elements with no meaningful content
            if (el.tagName.startsWith("NG-") || el.tagName === "ROUTER-OUTLET") {
                const children: PageElement[] = [];
                for (const child of el.children) {
                    const scraped = scrapeElement(child, depth);
                    if (scraped) children.push(scraped);
                }
                // Flatten — return children directly for structural components
                if (children.length === 1) return children[0];
                if (children.length > 1) {
                    return {
                        tag: el.tagName.toLowerCase(),
                        text: "",
                        classes: [],
                        attributes: {},
                        children,
                    };
                }
                return null;
            }

            const structuralClasses = getStructuralClasses(el);
            const isContentTag = CONTENT_TAGS.has(el.tagName);
            const hasStructuralClass = structuralClasses.length > 0;

            // Get direct text content (not from children)
            let directText = "";
            for (const node of el.childNodes) {
                if (node.nodeType === Node.TEXT_NODE) {
                    const t = node.textContent?.trim() ?? "";
                    if (t) directText += (directText ? " " : "") + t;
                }
            }

            // Scrape children
            const children: PageElement[] = [];
            for (const child of el.children) {
                const scraped = scrapeElement(child, depth + 1);
                if (scraped) children.push(scraped);
            }

            // Only include elements that are content-bearing or structural
            if (!isContentTag && !hasStructuralClass && !directText && children.length === 0) {
                return null;
            }

            // Collapse wrapper divs that add no semantic value
            if (el.tagName === "DIV" && !hasStructuralClass && !directText && children.length === 1) {
                return children[0];
            }

            return {
                tag: el.tagName.toLowerCase(),
                text: directText.substring(0, 200), // Cap text length
                classes: structuralClasses,
                attributes: getRelevantAttributes(el),
                children,
            };
        }

        const body = document.querySelector("body");
        if (!body) return [];

        const results: PageElement[] = [];
        for (const child of body.children) {
            const scraped = scrapeElement(child, 0);
            if (scraped) results.push(scraped);
        }
        return results;
    });
}

/**
 * Takes a full page snapshot including DOM structure and screenshot.
 */
export async function takePageSnapshot(page: Page, takeScreenshot = true): Promise<PageSnapshot> {
    const elements = await scrapePageStructure(page);
    let screenshot: string | undefined;
    if (takeScreenshot) {
        const buffer = await page.screenshot({ fullPage: true, type: "png" });
        screenshot = buffer.toString("base64");
    }

    return {
        url: page.url(),
        title: await page.title(),
        timestamp: new Date().toISOString(),
        elements,
        screenshot,
    };
}

// ─── Comparison Logic ───────────────────────────────────────────────────────

/**
 * Compares two page snapshots and returns a list of differences.
 */
export function compareSnapshots(
    angularSnapshot: PageSnapshot,
    legacySnapshot: PageSnapshot,
): ComparisonDiff[] {
    const diffs: ComparisonDiff[] = [];

    // Compare titles
    if (normalizeText(angularSnapshot.title) !== normalizeText(legacySnapshot.title)) {
        diffs.push({
            type: "changed",
            path: "page.title",
            description: "Page title differs",
            oldValue: legacySnapshot.title,
            newValue: angularSnapshot.title,
            severity: "info",
        });
    }

    // Compare element trees
    compareElementTrees(
        angularSnapshot.elements,
        legacySnapshot.elements,
        "body",
        diffs,
    );

    return diffs;
}

function normalizeText(text: string): string {
    return text.replace(/\s+/g, " ").trim().toLowerCase();
}

function compareElementTrees(
    angularElements: PageElement[],
    legacyElements: PageElement[],
    parentPath: string,
    diffs: ComparisonDiff[],
): void {
    // Compare cards/panels (structural sections)
    const angularCards = findStructuralSections(angularElements);
    const legacyCards = findStructuralSections(legacyElements);

    if (angularCards.length !== legacyCards.length) {
        diffs.push({
            type: "structural",
            path: parentPath,
            description: `Different number of card/panel sections: Angular has ${angularCards.length}, Legacy has ${legacyCards.length}`,
            severity: "warning",
        });
    }

    // Compare grids/tables
    const angularGrids = findGridElements(angularElements);
    const legacyGrids = findGridElements(legacyElements);

    if (angularGrids.length !== legacyGrids.length) {
        diffs.push({
            type: "structural",
            path: parentPath,
            description: `Different number of data grids/tables: Angular has ${angularGrids.length}, Legacy has ${legacyGrids.length}`,
            severity: "warning",
        });
    }

    // Compare headings
    const angularHeadings = findHeadings(angularElements);
    const legacyHeadings = findHeadings(legacyElements);

    compareTextLists(angularHeadings, legacyHeadings, `${parentPath} > headings`, diffs);

    // Compare form fields
    const angularFields = findFormFields(angularElements);
    const legacyFields = findFormFields(legacyElements);

    if (angularFields.length !== legacyFields.length) {
        diffs.push({
            type: "structural",
            path: `${parentPath} > form-fields`,
            description: `Different number of form fields: Angular has ${angularFields.length}, Legacy has ${legacyFields.length}`,
            severity: "warning",
        });
    }

    // Compare definition lists (dt/dd pairs)
    const angularDefs = findDefinitionPairs(angularElements);
    const legacyDefs = findDefinitionPairs(legacyElements);

    if (angularDefs.length !== legacyDefs.length) {
        diffs.push({
            type: "structural",
            path: `${parentPath} > definitions`,
            description: `Different number of dt/dd pairs: Angular has ${angularDefs.length}, Legacy has ${legacyDefs.length}`,
            severity: "info",
        });
    }

    // Compare buttons/actions
    const angularButtons = findButtons(angularElements);
    const legacyButtons = findButtons(legacyElements);

    compareLabelLists(angularButtons, legacyButtons, `${parentPath} > buttons`, diffs);

    // ─── Information parity checks ──────────────────────────────────────

    // Compare card/panel section titles (match sections by name)
    compareCardTitles(angularCards, legacyCards, `${parentPath} > card-titles`, diffs);

    // Compare dt/dd pair content (labels + values, not just counts)
    compareDefinitionPairContent(angularDefs, legacyDefs, `${parentPath} > definitions`, diffs);

    // Compare grid/table column headers
    compareGridColumnHeaders(angularElements, legacyElements, `${parentPath} > grid-headers`, diffs);

    // Compare grid/table row counts
    compareGridRowCounts(angularElements, legacyElements, `${parentPath} > grid-rows`, diffs);

    // Compare link text
    compareLinkText(angularElements, legacyElements, `${parentPath} > links`, diffs);

    // Compare text content within matched cards
    compareCardBodyContent(angularCards, legacyCards, `${parentPath} > card-content`, diffs);
}

function findStructuralSections(elements: PageElement[]): PageElement[] {
    const results: PageElement[] = [];
    function walk(els: PageElement[]) {
        for (const el of els) {
            if (
                el.classes.some(c => c === "card" || c === "panel" || c === "panel-default")
            ) {
                results.push(el);
            }
            walk(el.children);
        }
    }
    walk(elements);
    return results;
}

function findGridElements(elements: PageElement[]): PageElement[] {
    const results: PageElement[] = [];
    function walk(els: PageElement[]) {
        for (const el of els) {
            if (
                el.tag === "table" ||
                el.classes.some(c => c.startsWith("ag-root") || c.startsWith("ag-"))
            ) {
                results.push(el);
            }
            walk(el.children);
        }
    }
    walk(elements);
    return results;
}

function findHeadings(elements: PageElement[]): string[] {
    const results: string[] = [];
    function walk(els: PageElement[]) {
        for (const el of els) {
            if (/^h[1-6]$/.test(el.tag) || el.classes.includes("card-title") || el.classes.includes("panel-title")) {
                const text = getFullText(el);
                if (text) results.push(text);
            }
            walk(el.children);
        }
    }
    walk(elements);
    return results;
}

function findFormFields(elements: PageElement[]): PageElement[] {
    const results: PageElement[] = [];
    function walk(els: PageElement[]) {
        for (const el of els) {
            if (["input", "select", "textarea"].includes(el.tag)) {
                results.push(el);
            }
            walk(el.children);
        }
    }
    walk(elements);
    return results;
}

function findDefinitionPairs(elements: PageElement[]): { term: string; def: string }[] {
    const results: { term: string; def: string }[] = [];
    function walk(els: PageElement[]) {
        for (let i = 0; i < els.length; i++) {
            if (els[i].tag === "dt" && i + 1 < els.length && els[i + 1].tag === "dd") {
                results.push({
                    term: getFullText(els[i]),
                    def: getFullText(els[i + 1]),
                });
            }
            walk(els[i].children);
        }
    }
    walk(elements);
    return results;
}

function findButtons(elements: PageElement[]): string[] {
    const results: string[] = [];
    function walk(els: PageElement[]) {
        for (const el of els) {
            if (el.tag === "button" || el.classes.some(c => c.startsWith("btn"))) {
                const text = getFullText(el);
                if (text) results.push(text);
            }
            walk(el.children);
        }
    }
    walk(elements);
    return results;
}

// ─── Information Parity Comparison Functions ────────────────────────────────

/**
 * Extract title text from a card/panel element.
 * Looks for .card-title, .panel-title, or h3/h4 inside card-header/panel-heading.
 */
function getCardTitle(card: PageElement): string {
    let title = "";
    function walk(el: PageElement) {
        if (
            el.classes.includes("card-title") ||
            el.classes.includes("panel-title") ||
            ((el.tag === "h3" || el.tag === "h4") &&
                el.classes.some(c => c === "card-header" || c === "panel-heading"))
        ) {
            title = getFullText(el);
            return;
        }
        // Also check if a parent is card-header/panel-heading and child is h3/h4/span
        if (
            el.classes.some(c => c === "card-header" || c === "panel-heading")
        ) {
            for (const child of el.children) {
                if (
                    child.classes.includes("card-title") ||
                    child.classes.includes("panel-title") ||
                    child.tag === "h3" || child.tag === "h4"
                ) {
                    title = getFullText(child);
                    return;
                }
            }
            // Fallback: direct text of the header
            const headerText = getFullText(el);
            if (headerText) title = headerText;
            return;
        }
        for (const child of el.children) {
            walk(child);
            if (title) return;
        }
    }
    walk(card);
    return title.trim();
}

/**
 * Compare card/panel sections by title. Reports missing or extra sections.
 */
function compareCardTitles(
    angularCards: PageElement[],
    legacyCards: PageElement[],
    pathPrefix: string,
    diffs: ComparisonDiff[],
): void {
    const angularTitles = angularCards.map(getCardTitle).filter(Boolean);
    const legacyTitles = legacyCards.map(getCardTitle).filter(Boolean);

    const angularSet = new Set(angularTitles.map(normalizeText));
    const legacySet = new Set(legacyTitles.map(normalizeText));

    for (const title of legacyTitles) {
        if (!angularSet.has(normalizeText(title))) {
            diffs.push({
                type: "removed",
                path: pathPrefix,
                description: `Section missing from Angular: "${title}"`,
                oldValue: title,
                severity: "warning",
            });
        }
    }

    for (const title of angularTitles) {
        if (!legacySet.has(normalizeText(title))) {
            diffs.push({
                type: "added",
                path: pathPrefix,
                description: `New section in Angular: "${title}"`,
                newValue: title,
                severity: "info",
            });
        }
    }
}

/**
 * Compare dt/dd pair content — match by term text and compare values.
 */
function compareDefinitionPairContent(
    angularDefs: { term: string; def: string }[],
    legacyDefs: { term: string; def: string }[],
    pathPrefix: string,
    diffs: ComparisonDiff[],
): void {
    // Build lookup by normalized term
    const angularByTerm = new Map<string, string>();
    for (const d of angularDefs) {
        const key = normalizeText(d.term);
        if (key) angularByTerm.set(key, d.def);
    }

    const legacyByTerm = new Map<string, string>();
    for (const d of legacyDefs) {
        const key = normalizeText(d.term);
        if (key) legacyByTerm.set(key, d.def);
    }

    // Report missing terms (in legacy but not Angular)
    for (const [key, legacyDef] of legacyByTerm) {
        if (!angularByTerm.has(key)) {
            const originalTerm = legacyDefs.find(d => normalizeText(d.term) === key)?.term ?? key;
            diffs.push({
                type: "removed",
                path: `${pathPrefix}[${originalTerm}]`,
                description: `Data field missing from Angular: "${originalTerm}"`,
                oldValue: legacyDef.substring(0, 200),
                severity: "warning",
            });
        } else {
            // Compare values (fuzzy — normalize whitespace)
            const angularDef = angularByTerm.get(key)!;
            const normAngular = normalizeText(angularDef);
            const normLegacy = normalizeText(legacyDef);

            if (normAngular !== normLegacy && normAngular && normLegacy) {
                const originalTerm = legacyDefs.find(d => normalizeText(d.term) === key)?.term ?? key;
                diffs.push({
                    type: "changed",
                    path: `${pathPrefix}[${originalTerm}]`,
                    description: `Value differs for "${originalTerm}"`,
                    oldValue: legacyDef.substring(0, 200),
                    newValue: angularDef.substring(0, 200),
                    severity: "info",
                });
            }
        }
    }

    // Report extra terms (in Angular but not legacy)
    for (const [key] of angularByTerm) {
        if (!legacyByTerm.has(key)) {
            const originalTerm = angularDefs.find(d => normalizeText(d.term) === key)?.term ?? key;
            diffs.push({
                type: "added",
                path: `${pathPrefix}[${originalTerm}]`,
                description: `New data field in Angular: "${originalTerm}"`,
                newValue: angularByTerm.get(key)?.substring(0, 200),
                severity: "info",
            });
        }
    }
}

/**
 * Extract grid/table column header text from elements.
 */
function findGridColumnHeaders(elements: PageElement[]): string[][] {
    const grids: string[][] = [];

    function walk(els: PageElement[]) {
        for (const el of els) {
            // AG Grid headers
            if (el.classes.some(c => c.startsWith("ag-header"))) {
                const headers: string[] = [];
                function findHeaderCells(e: PageElement) {
                    // ag-header-cell-text contains the column label
                    if (e.text && e.classes.some(c => c === "ag-header")) {
                        // Look deeper for cell text
                    }
                    if (e.tag === "span" || e.tag === "div") {
                        const text = e.text.trim();
                        if (text && text.length > 0 && text.length < 100) {
                            headers.push(text);
                        }
                    }
                    for (const child of e.children) {
                        findHeaderCells(child);
                    }
                }
                findHeaderCells(el);
                if (headers.length > 0) grids.push(headers);
                continue; // Don't walk children again
            }

            // HTML table headers
            if (el.tag === "table") {
                const headers: string[] = [];
                function findTH(e: PageElement) {
                    if (e.tag === "th") {
                        const text = getFullText(e).trim();
                        if (text) headers.push(text);
                    }
                    for (const child of e.children) {
                        findTH(child);
                    }
                }
                findTH(el);
                if (headers.length > 0) grids.push(headers);
                continue;
            }

            walk(el.children);
        }
    }
    walk(elements);
    return grids;
}

/**
 * Compare grid/table column headers between Angular and Legacy.
 */
function compareGridColumnHeaders(
    angularElements: PageElement[],
    legacyElements: PageElement[],
    pathPrefix: string,
    diffs: ComparisonDiff[],
): void {
    const angularGridHeaders = findGridColumnHeaders(angularElements);
    const legacyGridHeaders = findGridColumnHeaders(legacyElements);

    const pairCount = Math.min(angularGridHeaders.length, legacyGridHeaders.length);
    for (let i = 0; i < pairCount; i++) {
        const angularSet = new Set(angularGridHeaders[i].map(normalizeText));
        const legacySet = new Set(legacyGridHeaders[i].map(normalizeText));

        for (const header of legacyGridHeaders[i]) {
            if (!angularSet.has(normalizeText(header))) {
                diffs.push({
                    type: "removed",
                    path: `${pathPrefix}[grid-${i}]`,
                    description: `Grid column missing from Angular: "${header}"`,
                    oldValue: header,
                    severity: "warning",
                });
            }
        }

        for (const header of angularGridHeaders[i]) {
            if (!legacySet.has(normalizeText(header))) {
                diffs.push({
                    type: "added",
                    path: `${pathPrefix}[grid-${i}]`,
                    description: `New grid column in Angular: "${header}"`,
                    newValue: header,
                    severity: "info",
                });
            }
        }
    }
}

/**
 * Count rows in grids/tables.
 */
function findGridRowCounts(elements: PageElement[]): number[] {
    const counts: number[] = [];

    function walk(els: PageElement[]) {
        for (const el of els) {
            // AG Grid rows
            if (el.classes.some(c => c === "ag-body" || c.startsWith("ag-body"))) {
                let rowCount = 0;
                function countRows(e: PageElement) {
                    if (e.classes.some(c => c.startsWith("ag-row"))) {
                        rowCount++;
                        return; // Don't count nested
                    }
                    for (const child of e.children) {
                        countRows(child);
                    }
                }
                countRows(el);
                if (rowCount > 0) counts.push(rowCount);
                continue;
            }

            // HTML table rows
            if (el.tag === "table") {
                let rowCount = 0;
                function countTR(e: PageElement) {
                    if (e.tag === "tr" && e.children.some(c => c.tag === "td")) {
                        rowCount++;
                    }
                    for (const child of e.children) {
                        countTR(child);
                    }
                }
                countTR(el);
                if (rowCount > 0) counts.push(rowCount);
                continue;
            }

            walk(el.children);
        }
    }
    walk(elements);
    return counts;
}

/**
 * Compare grid/table row counts (±10% tolerance).
 */
function compareGridRowCounts(
    angularElements: PageElement[],
    legacyElements: PageElement[],
    pathPrefix: string,
    diffs: ComparisonDiff[],
): void {
    const angularCounts = findGridRowCounts(angularElements);
    const legacyCounts = findGridRowCounts(legacyElements);

    const pairCount = Math.min(angularCounts.length, legacyCounts.length);
    for (let i = 0; i < pairCount; i++) {
        const a = angularCounts[i];
        const l = legacyCounts[i];
        const tolerance = Math.max(l * 0.1, 1); // ±10% or at least 1

        if (Math.abs(a - l) > tolerance) {
            diffs.push({
                type: "changed",
                path: `${pathPrefix}[grid-${i}]`,
                description: `Grid row count differs significantly: Angular has ${a}, Legacy has ${l}`,
                oldValue: String(l),
                newValue: String(a),
                severity: "warning",
            });
        }
    }
}

/**
 * Extract meaningful link text from elements.
 */
function findLinkTexts(elements: PageElement[]): string[] {
    const results: string[] = [];
    function walk(els: PageElement[]) {
        for (const el of els) {
            if (el.tag === "a") {
                const href = el.attributes["href"] ?? "";
                // Skip empty links, anchor-only links, and javascript: links
                if (href && href !== "#" && !href.startsWith("javascript:")) {
                    const text = getFullText(el).trim();
                    if (text && text.length > 1) {
                        results.push(text);
                    }
                }
            }
            walk(el.children);
        }
    }
    walk(elements);
    return results;
}

/**
 * Compare link text between Angular and Legacy (as sets).
 */
function compareLinkText(
    angularElements: PageElement[],
    legacyElements: PageElement[],
    pathPrefix: string,
    diffs: ComparisonDiff[],
): void {
    const angularLinks = findLinkTexts(angularElements);
    const legacyLinks = findLinkTexts(legacyElements);

    const angularSet = new Set(angularLinks.map(normalizeText));
    const legacySet = new Set(legacyLinks.map(normalizeText));

    for (const link of legacyLinks) {
        if (!angularSet.has(normalizeText(link))) {
            diffs.push({
                type: "removed",
                path: pathPrefix,
                description: `Link missing from Angular: "${link}"`,
                oldValue: link,
                severity: "info",
            });
        }
    }

    for (const link of angularLinks) {
        if (!legacySet.has(normalizeText(link))) {
            diffs.push({
                type: "added",
                path: pathPrefix,
                description: `New link in Angular: "${link}"`,
                newValue: link,
                severity: "info",
            });
        }
    }
}

/**
 * Extract card body text content.
 */
function getCardBodyText(card: PageElement): string {
    let bodyText = "";
    function walk(el: PageElement) {
        if (el.classes.some(c => c === "card-body" || c === "panel-body")) {
            bodyText = getFullText(el);
            return;
        }
        for (const child of el.children) {
            walk(child);
            if (bodyText) return;
        }
    }
    walk(card);
    return bodyText.substring(0, 500).trim();
}

/**
 * Compare text content within matched card sections.
 * Matches cards by title, then compares body text.
 */
function compareCardBodyContent(
    angularCards: PageElement[],
    legacyCards: PageElement[],
    pathPrefix: string,
    diffs: ComparisonDiff[],
): void {
    // Build title → card body map
    const angularByTitle = new Map<string, string>();
    for (const card of angularCards) {
        const title = normalizeText(getCardTitle(card));
        if (title) angularByTitle.set(title, getCardBodyText(card));
    }

    const legacyByTitle = new Map<string, string>();
    for (const card of legacyCards) {
        const title = normalizeText(getCardTitle(card));
        if (title) legacyByTitle.set(title, getCardBodyText(card));
    }

    // Compare body text for matched cards
    for (const [title, legacyBody] of legacyByTitle) {
        const angularBody = angularByTitle.get(title);
        if (angularBody == null) continue; // Missing card already reported by compareCardTitles

        const normAngular = normalizeText(angularBody);
        const normLegacy = normalizeText(legacyBody);

        if (normAngular !== normLegacy && normLegacy.length > 0) {
            diffs.push({
                type: "changed",
                path: `${pathPrefix}[${title}]`,
                description: `Card body text differs for "${title}"`,
                oldValue: legacyBody.substring(0, 200),
                newValue: angularBody.substring(0, 200),
                severity: "info",
            });
        }
    }
}

function getFullText(el: PageElement): string {
    let text = el.text;
    for (const child of el.children) {
        const childText = getFullText(child);
        if (childText) text += (text ? " " : "") + childText;
    }
    return text.trim();
}

function compareTextLists(
    angularItems: string[],
    legacyItems: string[],
    pathPrefix: string,
    diffs: ComparisonDiff[],
): void {
    const normalizedAngular = angularItems.map(normalizeText);
    const normalizedLegacy = legacyItems.map(normalizeText);

    for (let i = 0; i < Math.max(normalizedAngular.length, normalizedLegacy.length); i++) {
        const a = normalizedAngular[i];
        const l = normalizedLegacy[i];

        if (a && !l) {
            diffs.push({
                type: "added",
                path: `${pathPrefix}[${i}]`,
                description: `New item in Angular: "${angularItems[i]}"`,
                newValue: angularItems[i],
                severity: "info",
            });
        } else if (!a && l) {
            diffs.push({
                type: "removed",
                path: `${pathPrefix}[${i}]`,
                description: `Missing from Angular: "${legacyItems[i]}"`,
                oldValue: legacyItems[i],
                severity: "warning",
            });
        } else if (a !== l) {
            diffs.push({
                type: "changed",
                path: `${pathPrefix}[${i}]`,
                description: `Text differs`,
                oldValue: legacyItems[i],
                newValue: angularItems[i],
                severity: "info",
            });
        }
    }
}

function compareLabelLists(
    angularItems: string[],
    legacyItems: string[],
    pathPrefix: string,
    diffs: ComparisonDiff[],
): void {
    const angularSet = new Set(angularItems.map(normalizeText));
    const legacySet = new Set(legacyItems.map(normalizeText));

    for (const item of legacyItems) {
        if (!angularSet.has(normalizeText(item))) {
            diffs.push({
                type: "removed",
                path: pathPrefix,
                description: `Button/action missing from Angular: "${item}"`,
                oldValue: item,
                severity: "warning",
            });
        }
    }

    for (const item of angularItems) {
        if (!legacySet.has(normalizeText(item))) {
            diffs.push({
                type: "added",
                path: pathPrefix,
                description: `New button/action in Angular: "${item}"`,
                newValue: item,
                severity: "info",
            });
        }
    }
}

// ─── Acceptance ─────────────────────────────────────────────────────────────

const ACCEPTED_PAGES_PATH = path.resolve(__dirname, "accepted-pages.json");

export function loadAcceptedPages(): Record<string, string> {
    try {
        const content = fs.readFileSync(ACCEPTED_PAGES_PATH, "utf-8");
        return JSON.parse(content);
    } catch {
        return {};
    }
}

export function saveAcceptedPages(accepted: Record<string, string>): void {
    fs.writeFileSync(ACCEPTED_PAGES_PATH, JSON.stringify(accepted, null, 2) + "\n");
}

export function isPageAccepted(pageName: string, diffs: ComparisonDiff[]): boolean {
    const accepted = loadAcceptedPages();
    if (!accepted[pageName]) return false;

    // Check if the acceptance hash still matches current diffs
    const currentHash = hashDiffs(diffs);
    return accepted[pageName] === currentHash;
}

export function acceptPage(pageName: string, diffs: ComparisonDiff[]): void {
    const accepted = loadAcceptedPages();
    accepted[pageName] = hashDiffs(diffs);
    saveAcceptedPages(accepted);
}

function hashDiffs(diffs: ComparisonDiff[]): string {
    const content = JSON.stringify(diffs.map(d => ({
        type: d.type,
        path: d.path,
        description: d.description,
    })));
    // Simple hash — not cryptographic, just for change detection
    let hash = 0;
    for (let i = 0; i < content.length; i++) {
        const chr = content.charCodeAt(i);
        hash = ((hash << 5) - hash) + chr;
        hash |= 0;
    }
    return hash.toString(36);
}

// ─── Report Generation ──────────────────────────────────────────────────────

export function generateComparisonReport(
    results: PageComparisonResult[],
    outputDir: string,
): string {
    const reportDir = path.resolve(outputDir);
    if (!fs.existsSync(reportDir)) {
        fs.mkdirSync(reportDir, { recursive: true });
    }

    const reportPath = path.join(reportDir, "index.html");

    const passed = results.filter(r => r.status === "pass" || r.status === "accepted").length;
    const failed = results.filter(r => r.status === "fail").length;
    const errors = results.filter(r => r.status === "error").length;
    const skipped = results.filter(r => r.status === "skipped").length;
    const total = results.length;

    const html = `<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>WADNR Forest Health Tracker Comparison Report</title>
    <style>
        :root {
            --pass: #22c55e;
            --fail: #ef4444;
            --error: #f97316;
            --skip: #94a3b8;
            --accepted: #3b82f6;
            --bg: #0f172a;
            --surface: #1e293b;
            --surface-hover: #334155;
            --text: #f1f5f9;
            --text-muted: #94a3b8;
            --border: #334155;
        }
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body {
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
            background: var(--bg);
            color: var(--text);
            line-height: 1.6;
        }
        .container { max-width: 1400px; margin: 0 auto; padding: 2rem; }
        h1 { font-size: 1.75rem; margin-bottom: 0.5rem; }
        .subtitle { color: var(--text-muted); margin-bottom: 2rem; }
        .summary {
            display: flex; gap: 1rem; margin-bottom: 2rem; flex-wrap: wrap;
        }
        .summary-card {
            background: var(--surface);
            border-radius: 8px;
            padding: 1rem 1.5rem;
            min-width: 120px;
            text-align: center;
        }
        .summary-card .count { font-size: 2rem; font-weight: bold; }
        .summary-card .label { color: var(--text-muted); font-size: 0.875rem; }
        .summary-card.pass .count { color: var(--pass); }
        .summary-card.fail .count { color: var(--fail); }
        .summary-card.error .count { color: var(--error); }
        .summary-card.skip .count { color: var(--skip); }
        .summary-card.accepted .count { color: var(--accepted); }

        .filter-bar {
            display: flex; gap: 0.5rem; margin-bottom: 1.5rem; flex-wrap: wrap;
        }
        .filter-btn {
            background: var(--surface);
            border: 1px solid var(--border);
            color: var(--text);
            padding: 0.4rem 1rem;
            border-radius: 6px;
            cursor: pointer;
            font-size: 0.875rem;
            transition: all 0.2s;
        }
        .filter-btn:hover { background: var(--surface-hover); }
        .filter-btn.active {
            background: var(--surface-hover);
            border-color: var(--text-muted);
        }

        .result-card {
            background: var(--surface);
            border-radius: 8px;
            margin-bottom: 1rem;
            border: 1px solid var(--border);
            overflow: hidden;
        }
        .result-header {
            padding: 1rem 1.5rem;
            display: flex;
            align-items: center;
            justify-content: space-between;
            cursor: pointer;
            user-select: none;
        }
        .result-header:hover { background: var(--surface-hover); }
        .result-info { display: flex; align-items: center; gap: 0.75rem; }
        .status-badge {
            padding: 0.2rem 0.6rem;
            border-radius: 4px;
            font-size: 0.75rem;
            font-weight: 600;
            text-transform: uppercase;
        }
        .status-badge.pass { background: var(--pass); color: #000; }
        .status-badge.fail { background: var(--fail); color: #fff; }
        .status-badge.error { background: var(--error); color: #000; }
        .status-badge.skipped { background: var(--skip); color: #000; }
        .status-badge.accepted { background: var(--accepted); color: #fff; }

        .page-name { font-weight: 600; }
        .urls { color: var(--text-muted); font-size: 0.8rem; }
        .duration { color: var(--text-muted); font-size: 0.8rem; }
        .chevron { transition: transform 0.2s; }
        .chevron.open { transform: rotate(90deg); }

        .result-body {
            display: none;
            padding: 1rem 1.5rem;
            border-top: 1px solid var(--border);
        }
        .result-body.open { display: block; }

        .screenshots {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 1rem;
            margin-bottom: 1rem;
        }
        .screenshot-col h4 {
            font-size: 0.875rem;
            color: var(--text-muted);
            margin-bottom: 0.5rem;
        }
        .screenshot-col img {
            width: 100%;
            border-radius: 4px;
            border: 1px solid var(--border);
        }

        .diff-list { margin-top: 1rem; }
        .diff-item {
            padding: 0.75rem;
            margin-bottom: 0.5rem;
            border-radius: 6px;
            font-size: 0.875rem;
        }
        .diff-item.info { background: rgba(59, 130, 246, 0.1); border-left: 3px solid #3b82f6; }
        .diff-item.warning { background: rgba(249, 115, 22, 0.1); border-left: 3px solid #f97316; }
        .diff-item.error { background: rgba(239, 68, 68, 0.1); border-left: 3px solid #ef4444; }
        .diff-path { color: var(--text-muted); font-size: 0.75rem; font-family: monospace; }
        .diff-values { margin-top: 0.25rem; font-size: 0.8rem; }
        .diff-values .old { color: #f87171; }
        .diff-values .new { color: #4ade80; }

        .action-bar {
            display: flex; gap: 0.5rem; margin-top: 1rem; padding-top: 1rem;
            border-top: 1px solid var(--border);
        }
        .accept-btn {
            background: var(--accepted);
            color: #fff;
            border: none;
            padding: 0.4rem 1rem;
            border-radius: 6px;
            cursor: pointer;
            font-size: 0.8rem;
        }
        .accept-btn:hover { opacity: 0.9; }
        .copy-btn {
            background: var(--surface-hover);
            color: var(--text);
            border: 1px solid var(--border);
            padding: 0.4rem 1rem;
            border-radius: 6px;
            cursor: pointer;
            font-size: 0.8rem;
        }
        .copy-btn:hover { background: var(--border); }
    </style>
</head>
<body>
    <div class="container">
        <h1>WADNR Forest Health Tracker Comparison Report</h1>
        <p class="subtitle">Generated ${new Date().toLocaleString()} &mdash; ${total} pages compared</p>

        <div class="summary">
            <div class="summary-card pass">
                <div class="count">${passed}</div>
                <div class="label">Passed</div>
            </div>
            <div class="summary-card fail">
                <div class="count">${failed}</div>
                <div class="label">Failed</div>
            </div>
            <div class="summary-card error">
                <div class="count">${errors}</div>
                <div class="label">Errors</div>
            </div>
            <div class="summary-card skip">
                <div class="count">${skipped}</div>
                <div class="label">Skipped</div>
            </div>
        </div>

        <div class="filter-bar">
            <button class="filter-btn active" onclick="filterResults('all')">All (${total})</button>
            <button class="filter-btn" onclick="filterResults('pass')">Pass (${passed})</button>
            <button class="filter-btn" onclick="filterResults('fail')">Fail (${failed})</button>
            <button class="filter-btn" onclick="filterResults('error')">Error (${errors})</button>
            <button class="filter-btn" onclick="filterResults('skipped')">Skipped (${skipped})</button>
        </div>

        <div id="results">
${results.map((r, i) => generateResultCard(r, i)).join("\n")}
        </div>
    </div>

    <script>
        function toggleResult(id) {
            const body = document.getElementById('body-' + id);
            const chevron = document.getElementById('chevron-' + id);
            body.classList.toggle('open');
            chevron.classList.toggle('open');
        }

        function filterResults(status) {
            document.querySelectorAll('.filter-btn').forEach(b => b.classList.remove('active'));
            event.target.classList.add('active');

            document.querySelectorAll('.result-card').forEach(card => {
                if (status === 'all' || card.dataset.status === status
                    || (status === 'pass' && card.dataset.status === 'accepted')) {
                    card.style.display = '';
                } else {
                    card.style.display = 'none';
                }
            });
        }

        function acceptPage(pageName, index) {
            const cmd = 'cd WADNR.Web/ && npx ts-node -e "' +
                "import {acceptPage} from './e2e-tests/comparison/comparison-helpers';" +
                "const r = require('./comparison-reports/results.json');" +
                "acceptPage('" + pageName + "', r[" + index + "].diffs)" +
                '"';
            navigator.clipboard.writeText(cmd).then(() => {
                const btn = event.target;
                btn.textContent = 'Copied!';
                setTimeout(() => { btn.textContent = 'Accept'; }, 2000);
            });
        }
    </script>
</body>
</html>`;

    fs.writeFileSync(reportPath, html);

    // Also save raw results as JSON for programmatic use
    const resultsPath = path.join(reportDir, "results.json");
    fs.writeFileSync(resultsPath, JSON.stringify(results, null, 2));

    return reportPath;
}

function generateResultCard(result: PageComparisonResult, index: number): string {
    const statusClass = result.status === "pass" ? "pass"
        : result.status === "accepted" ? "accepted"
        : result.status === "fail" ? "fail"
        : result.status === "error" ? "error"
        : "skipped";

    let body = "";

    if (result.error) {
        body += `<div class="diff-item error"><strong>Error:</strong> ${escapeHtml(result.error)}</div>`;
    }

    if (result.angularScreenshot && result.legacyScreenshot) {
        body += `
        <div class="screenshots">
            <div class="screenshot-col">
                <h4>Angular (New)</h4>
                <img src="data:image/png;base64,${result.angularScreenshot}" alt="Angular screenshot" loading="lazy" />
            </div>
            <div class="screenshot-col">
                <h4>Legacy (MVC)</h4>
                <img src="data:image/png;base64,${result.legacyScreenshot}" alt="Legacy screenshot" loading="lazy" />
            </div>
        </div>`;
    }

    if (result.diffs.length > 0) {
        body += `<div class="diff-list">`;
        for (const diff of result.diffs) {
            body += `
            <div class="diff-item ${diff.severity}">
                <div><strong>${escapeHtml(diff.description)}</strong></div>
                <div class="diff-path">${escapeHtml(diff.path)}</div>
                ${diff.oldValue || diff.newValue ? `
                <div class="diff-values">
                    ${diff.oldValue ? `<div class="old">- ${escapeHtml(diff.oldValue)}</div>` : ""}
                    ${diff.newValue ? `<div class="new">+ ${escapeHtml(diff.newValue)}</div>` : ""}
                </div>` : ""}
            </div>`;
        }
        body += `</div>`;
    }

    if (result.status === "fail") {
        body += `
        <div class="action-bar">
            <button class="accept-btn" onclick="acceptPage('${escapeHtml(result.pageName)}', ${index})">Accept</button>
            <button class="copy-btn" onclick="navigator.clipboard.writeText('${escapeHtml(result.pageName)}')">Copy Name</button>
        </div>`;
    }

    return `
            <div class="result-card" data-status="${statusClass}">
                <div class="result-header" onclick="toggleResult(${index})">
                    <div class="result-info">
                        <span class="status-badge ${statusClass}">${result.status}</span>
                        <span class="page-name">${escapeHtml(result.pageName)}</span>
                        <span class="urls">${escapeHtml(result.angularUrl)} ↔ ${escapeHtml(result.legacyUrl)}</span>
                    </div>
                    <div style="display:flex;align-items:center;gap:1rem;">
                        <span class="duration">${result.duration}ms</span>
                        <span class="chevron" id="chevron-${index}">▶</span>
                    </div>
                </div>
                <div class="result-body" id="body-${index}">
                    ${body}
                </div>
            </div>`;
}

function escapeHtml(text: string): string {
    return text
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}

// ─── Utilities ──────────────────────────────────────────────────────────────

/**
 * Wait for a page to be fully loaded (network idle + no loading spinners).
 */
export async function waitForPageReady(page: Page, timeout = 30000): Promise<void> {
    await page.waitForLoadState("networkidle", { timeout });

    // Wait for Angular loading spinners to disappear
    try {
        await page.waitForSelector("app-loading-spinner", { state: "detached", timeout: 5000 });
    } catch {
        // No spinner found, page is ready
    }

    // Wait for ag-grid to finish loading if present
    try {
        const hasGrid = await page.locator(".ag-root").count();
        if (hasGrid > 0) {
            await page.waitForSelector(".ag-overlay-loading-center", { state: "detached", timeout: 10000 });
        }
    } catch {
        // Grid loaded or not present
    }

    // Small delay for any final renders
    await page.waitForTimeout(500);
}

/**
 * Wait for a legacy MVC page to be fully loaded.
 */
export async function waitForLegacyPageReady(page: Page, timeout = 30000): Promise<void> {
    await page.waitForLoadState("networkidle", { timeout });

    // Wait for jQuery document ready if jQuery is present
    try {
        await page.evaluate(() => {
            return new Promise<void>((resolve) => {
                if ((window as any).jQuery) {
                    (window as any).jQuery(resolve);
                } else {
                    resolve();
                }
            });
        });
    } catch {
        // No jQuery or timeout
    }

    // Wait for DataTables to finish if present
    try {
        await page.waitForSelector(".dataTables_processing", { state: "hidden", timeout: 10000 });
    } catch {
        // No DataTables processing indicator
    }

    // Small delay for any final renders
    await page.waitForTimeout(500);
}

/**
 * Resolve token placeholders in a URL path.
 * e.g., "/projects/{projectID}" with { projectID: 123 } → "/projects/123"
 */
export function resolvePath(
    urlPath: string,
    tokens: Record<string, string | number>,
): string {
    let resolved = urlPath;
    for (const [key, value] of Object.entries(tokens)) {
        resolved = resolved.replace(`{${key}}`, String(value));
    }
    return resolved;
}
