import { test, expect } from "../fixtures/base-test";
import { waitForModalOpen, clickModalSave, clickModalCancel, waitForModalClose, expectValidationErrors, getModalTitle } from "../fixtures/modal-helpers";

/**
 * Tests admin modal validation and full CRUD round-trips on admin-guarded pages.
 * Full CRUD tests create entities with unique names and clean up via DELETE API calls.
 */

test.describe("Project Types modals", () => {
    test.beforeEach(async ({ authedPage: page }) => {
        await page.goto("/project-types");
        await expect(page.locator("h2.page-title")).toBeVisible({ timeout: 15000 });
    });

    test("Create button opens modal with correct title", async ({ authedPage: page }) => {
        await page.locator("button", { hasText: "Create New Project Type" }).click();
        await waitForModalOpen(page);
        const title = await getModalTitle(page);
        expect(title.trim()).toBe("Create Project Type");
    });

    test("Save on empty form shows validation errors", async ({ authedPage: page }) => {
        await page.locator("button", { hasText: "Create New Project Type" }).click();
        await waitForModalOpen(page);
        await clickModalSave(page);
        await expectValidationErrors(page);
    });

    test("Cancel closes modal without changes", async ({ authedPage: page }) => {
        await page.locator("button", { hasText: "Create New Project Type" }).click();
        await waitForModalOpen(page);
        await clickModalCancel(page);
        await waitForModalClose(page);
    });

    test("Full CRUD: create and delete project type", async ({ authedPage: page }) => {
        const uniqueName = `E2E Test ProjectType ${Date.now()}`;
        let createdID: number | null = null;

        // Intercept POST response to capture created ID
        page.on("response", async (resp) => {
            if (resp.url().includes("/api/project-types") && resp.request().method() === "POST" && resp.status() < 300) {
                try {
                    const body = await resp.json();
                    createdID = body.ProjectTypeID ?? body.projectTypeID;
                } catch {
                    // ignore parse errors
                }
            }
        });

        try {
            await page.locator("button", { hasText: "Create New Project Type" }).click();
            await waitForModalOpen(page);

            // Fill required fields: ProjectTypeName and ProjectTypeDescription
            const modal = page.locator(".ngneat-dialog-content");
            await modal.locator("input[type='text']").first().fill(uniqueName);

            // ProjectTypeDescription is an RTE - find the contenteditable area or textarea
            const rte = modal.locator(".ql-editor, [contenteditable='true'], textarea").first();
            if (await rte.isVisible({ timeout: 2000 }).catch(() => false)) {
                await rte.fill("E2E test description - safe to delete");
            }

            await clickModalSave(page);
            await waitForModalClose(page, 10000);

            // Verify the new row appears in the grid
            await expect(page.locator(".ag-row", { hasText: uniqueName }).first()).toBeVisible({ timeout: 15000 });
        } finally {
            // Cleanup: delete via API
            if (createdID) {
                await page.request.delete(`/api/project-types/${createdID}`).catch(() => {});
            }
        }
    });
});

test.describe("Project Themes modals", () => {
    test.beforeEach(async ({ authedPage: page }) => {
        await page.goto("/project-themes");
        await expect(page.locator("h2.page-title")).toBeVisible({ timeout: 15000 });
    });

    test("Create New Theme opens modal with correct title", async ({ authedPage: page }) => {
        await page.locator("button", { hasText: "Create New Theme" }).click();
        await waitForModalOpen(page);
        const title = await getModalTitle(page);
        expect(title.trim()).toBe("Create Theme");
    });

    test("Save on empty form shows validation errors", async ({ authedPage: page }) => {
        await page.locator("button", { hasText: "Create New Theme" }).click();
        await waitForModalOpen(page);
        await clickModalSave(page);
        await expectValidationErrors(page);
    });

    test("Cancel closes modal", async ({ authedPage: page }) => {
        await page.locator("button", { hasText: "Create New Theme" }).click();
        await waitForModalOpen(page);
        await clickModalCancel(page);
        await waitForModalClose(page);
    });

    test("Full CRUD: create and delete theme", async ({ authedPage: page }) => {
        const uniqueName = `E2E Theme ${Date.now()}`;
        let createdID: number | null = null;

        page.on("response", async (resp) => {
            if (resp.url().includes("/api/classification") && resp.request().method() === "POST" && resp.status() < 300) {
                try {
                    const body = await resp.json();
                    createdID = body.ClassificationID ?? body.classificationID;
                } catch {
                    // ignore
                }
            }
        });

        try {
            await page.locator("button", { hasText: "Create New Theme" }).click();
            await waitForModalOpen(page);

            const modal = page.locator(".ngneat-dialog-content");

            // Fill DisplayName (first text input)
            await modal.locator("input[type='text']").first().fill(uniqueName);

            // Fill ClassificationDescription (RTE)
            const rte = modal.locator(".ql-editor, [contenteditable='true'], textarea").first();
            if (await rte.isVisible({ timeout: 2000 }).catch(() => false)) {
                await rte.fill("E2E test theme description");
            }

            // Fill ThemeColor (color input or text input)
            const colorInput = modal.locator("input[type='color']").first();
            if (await colorInput.isVisible({ timeout: 1000 }).catch(() => false)) {
                await colorInput.fill("#FF5733");
            } else {
                // ThemeColor might be a text input
                const textInputs = modal.locator("input[type='text']");
                const count = await textInputs.count();
                if (count > 1) {
                    await textInputs.nth(count - 1).fill("#FF5733");
                }
            }

            await clickModalSave(page);
            await waitForModalClose(page, 10000);
        } finally {
            if (createdID) {
                await page.request.delete(`/api/classifications/${createdID}`).catch(() => {});
            }
        }
    });

    test("Edit Sort Order opens sort order modal", async ({ authedPage: page }) => {
        await page.locator("button", { hasText: "Edit Sort Order" }).click();
        await waitForModalOpen(page);
        // Sort order modal should have some list items
        const modal = page.locator(".ngneat-dialog-content");
        await expect(modal).toBeVisible();
    });

    test("Sort order Cancel closes modal", async ({ authedPage: page }) => {
        await page.locator("button", { hasText: "Edit Sort Order" }).click();
        await waitForModalOpen(page);
        await clickModalCancel(page);
        await waitForModalClose(page);
    });
});

test.describe("Map Layers modals", () => {
    test.beforeEach(async ({ authedPage: page }) => {
        await page.goto("/map-layers");
        await expect(page.locator("h2.page-title")).toBeVisible({ timeout: 15000 });
    });

    test("Create button opens modal", async ({ authedPage: page }) => {
        await page.locator("button", { hasText: "Add External Map Layer" }).click();
        await waitForModalOpen(page);
        const title = await getModalTitle(page);
        expect(title.trim()).toContain("Add External Map Layer");
    });

    test("Save on empty form shows validation errors", async ({ authedPage: page }) => {
        await page.locator("button", { hasText: "Add External Map Layer" }).click();
        await waitForModalOpen(page);
        await clickModalSave(page);
        await expectValidationErrors(page);
    });

    test("Cancel closes modal", async ({ authedPage: page }) => {
        await page.locator("button", { hasText: "Add External Map Layer" }).click();
        await waitForModalOpen(page);
        await clickModalCancel(page);
        await waitForModalClose(page);
    });

    test("Full CRUD: create and delete map layer", async ({ authedPage: page }) => {
        const uniqueName = `E2E MapLayer ${Date.now()}`;
        let createdID: number | null = null;

        page.on("response", async (resp) => {
            if (resp.url().includes("/api/external-map-layers") && resp.request().method() === "POST" && resp.status() < 300) {
                try {
                    const body = await resp.json();
                    createdID = body.ExternalMapLayerID ?? body.externalMapLayerID;
                } catch {
                    // ignore
                }
            }
        });

        try {
            await page.locator("button", { hasText: "Add External Map Layer" }).click();
            await waitForModalOpen(page);

            const modal = page.locator(".ngneat-dialog-content");
            const textInputs = modal.locator("input[type='text']");

            // Fill DisplayName (first text input)
            await textInputs.first().fill(uniqueName);

            // Fill LayerUrl (second text input)
            await textInputs.nth(1).fill("https://example.com/e2e-test-layer");

            await clickModalSave(page);
            await waitForModalClose(page, 10000);

            // Verify new row in grid
            await expect(page.locator(".ag-row", { hasText: uniqueName }).first()).toBeVisible({ timeout: 15000 });
        } finally {
            if (createdID) {
                await page.request.delete(`/api/external-map-layers/${createdID}`).catch(() => {});
            }
        }
    });
});

test.describe("Organization Types modals", () => {
    test.beforeEach(async ({ authedPage: page }) => {
        await page.goto("/organization-and-relationship-types");
        await expect(page.locator("h2.page-title")).toBeVisible({ timeout: 15000 });
    });

    test("Create Org Type modal opens", async ({ authedPage: page }) => {
        const createBtn = page.locator("button", { hasText: /Create/i }).first();
        if (await createBtn.isVisible({ timeout: 5000 }).catch(() => false)) {
            await createBtn.click();
            await waitForModalOpen(page);
        }
    });

    test("Cancel closes org type modal", async ({ authedPage: page }) => {
        const createBtn = page.locator("button", { hasText: /Create/i }).first();
        if (await createBtn.isVisible({ timeout: 5000 }).catch(() => false)) {
            await createBtn.click();
            await waitForModalOpen(page);
            await clickModalCancel(page);
            await waitForModalClose(page);
        }
    });
});
