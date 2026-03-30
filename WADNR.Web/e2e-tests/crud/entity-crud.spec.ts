/**
 * Data-driven CRUD tests for all modal-based admin entities.
 *
 * Each entity definition specifies the page URL, create button text,
 * expected modal title, and API details. Tests verify:
 * 1. Create modal opens with correct title
 * 2. Empty form submission shows validation errors
 * 3. Cancel closes modal without side effects
 * 4. Full CRUD: create entity, verify in grid, delete via API
 */

import { test, expect } from "../fixtures/base-test";
import {
    testCrudModalOpen,
    testModalValidation,
    testModalCancel,
    waitForPageHeading,
    interceptCreatedId,
} from "../fixtures/crud-helpers";
import { waitForModalOpen, clickModalSave, waitForModalClose } from "../fixtures/modal-helpers";

interface CrudEntityConfig {
    /** Test group name */
    name: string;
    /** Page URL */
    url: string;
    /** Text of the button that opens the create modal */
    createButtonText: string;
    /** Expected modal title */
    expectedModalTitle: string;
    /** API path fragment for POST intercept (e.g., "/api/project-types") */
    apiPath: string;
    /** PascalCase ID field name in the POST response (e.g., "ProjectTypeID") */
    idField: string;
    /** Function to fill required fields in the modal and return a unique name for grid verification */
    fillFields: (page: import("@playwright/test").Page, uniquePrefix: string) => Promise<string>;
}

const MODAL = ".ngneat-dialog-content";

// ─── Entity Configurations ─────────────────────────────────────────────────────

const entities: CrudEntityConfig[] = [
    {
        name: "Project Types",
        url: "/project-types",
        createButtonText: "Create New Project Type",
        expectedModalTitle: "Create Project Type",
        apiPath: "/api/project-types",
        idField: "ProjectTypeID",
        fillFields: async (page, prefix) => {
            const name = `${prefix} ProjType`;
            const modal = page.locator(MODAL);
            await modal.locator("input[type='text']").first().fill(name);
            const rte = modal.locator(".ql-editor, [contenteditable='true'], textarea").first();
            if (await rte.isVisible({ timeout: 2000 }).catch(() => false)) {
                await rte.fill("E2E test description");
            }
            return name;
        },
    },
    {
        name: "Project Themes",
        url: "/project-themes",
        createButtonText: "Create New Theme",
        expectedModalTitle: "Create Theme",
        apiPath: "/api/classification",
        idField: "ClassificationID",
        fillFields: async (page, prefix) => {
            const name = `${prefix} Theme`;
            const modal = page.locator(MODAL);
            await modal.locator("input[type='text']").first().fill(name);
            const rte = modal.locator(".ql-editor, [contenteditable='true'], textarea").first();
            if (await rte.isVisible({ timeout: 2000 }).catch(() => false)) {
                await rte.fill("E2E theme description");
            }
            const colorInput = modal.locator("input[type='color']").first();
            if (await colorInput.isVisible({ timeout: 1000 }).catch(() => false)) {
                await colorInput.fill("#AA33CC");
            }
            return name;
        },
    },
    {
        name: "Map Layers",
        url: "/map-layers",
        createButtonText: "Add External Map Layer",
        expectedModalTitle: "Add External Map Layer",
        apiPath: "/api/external-map-layers",
        idField: "ExternalMapLayerID",
        fillFields: async (page, prefix) => {
            const name = `${prefix} MapLayer`;
            const modal = page.locator(MODAL);
            const textInputs = modal.locator("input[type='text']");
            await textInputs.first().fill(name);
            await textInputs.nth(1).fill("https://example.com/e2e-test-layer");
            return name;
        },
    },
    {
        name: "Tags",
        url: "/tags",
        createButtonText: "Create New Tag",
        expectedModalTitle: "Create Tag",
        apiPath: "/api/tags",
        idField: "TagID",
        fillFields: async (page, prefix) => {
            const name = `${prefix} Tag`;
            const modal = page.locator(MODAL);
            await modal.locator("input[type='text']").first().fill(name);
            return name;
        },
    },
];

// ─── Data-Driven Tests ─────────────────────────────────────────────────────────

for (const entity of entities) {
    test.describe(`${entity.name} CRUD`, () => {
        test.beforeEach(async ({ authedPage: page }) => {
            await page.goto(entity.url);
            await waitForPageHeading(page);
        });

        test(`Create button opens modal with title "${entity.expectedModalTitle}"`, async ({ authedPage: page }) => {
            await testCrudModalOpen(page, entity.createButtonText, entity.expectedModalTitle);
        });

        test("Empty form shows validation errors", async ({ authedPage: page }) => {
            await testModalValidation(page, entity.createButtonText);
        });

        test("Cancel closes modal", async ({ authedPage: page }) => {
            await testModalCancel(page, entity.createButtonText);
        });

        test("Full CRUD: create and delete", async ({ authedPage: page }) => {
            const uniquePrefix = `E2E ${Date.now()}`;
            const getCreatedId = interceptCreatedId(page, entity.apiPath, entity.idField);

            let entityName: string | undefined;
            try {
                await page.locator("button", { hasText: entity.createButtonText }).click();
                await waitForModalOpen(page);

                entityName = await entity.fillFields(page, uniquePrefix);

                await clickModalSave(page);
                await waitForModalClose(page, 10000);

                // Verify the new row appears in the grid
                await expect(
                    page.locator(".ag-row", { hasText: entityName }).first(),
                ).toBeVisible({ timeout: 15000 });
            } finally {
                // Cleanup: delete via API
                const id = getCreatedId();
                if (id) {
                    await page.request.delete(`${entity.apiPath}/${id}`).catch(() => {});
                }
            }
        });
    });
}
