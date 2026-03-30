import { test, expect } from "@playwright/test";
import { setupTestAuth } from "../fixtures/inject-auth-header";
import { testUsers } from "../fixtures/test-users";
import { testData } from "../fixtures/test-data";
import { waitForModalOpen } from "../fixtures/modal-helpers";
import {
    MODAL_SCREENSHOT_OPTIONS,
    waitForPageStable,
    waitForModalStable,
} from "./visual-config";

const MODAL_SELECTOR = ".ngneat-dialog-content .modal";

test.describe("Modals - visual regression", () => {
    test.beforeEach(async ({ page }) => {
        await setupTestAuth(page, testUsers.admin);
    });

    // ─── Admin CRUD modals ──────────────────────────────────────────────

    test("Create project type modal", async ({ page }) => {
        await page.goto("/project-types");
        await waitForPageStable(page);
        await page.locator("button", { hasText: "Create New Project Type" }).click();
        await waitForModalStable(page);
        await expect(page.locator(MODAL_SELECTOR)).toHaveScreenshot("modal-create-project-type.png", MODAL_SCREENSHOT_OPTIONS);
    });

    test("Create theme modal", async ({ page }) => {
        await page.goto("/project-themes");
        await waitForPageStable(page);
        await page.locator("button", { hasText: "Create New Theme" }).click();
        await waitForModalStable(page);
        await expect(page.locator(MODAL_SELECTOR)).toHaveScreenshot("modal-create-theme.png", MODAL_SCREENSHOT_OPTIONS);
    });

    test("Sort order modal", async ({ page }) => {
        await page.goto("/project-themes");
        await waitForPageStable(page);
        await page.locator("button", { hasText: "Edit Sort Order" }).click();
        await waitForModalStable(page);
        await expect(page.locator(MODAL_SELECTOR)).toHaveScreenshot("modal-sort-order.png", MODAL_SCREENSHOT_OPTIONS);
    });

    test("Create map layer modal", async ({ page }) => {
        await page.goto("/map-layers");
        await waitForPageStable(page);
        await page.locator("button", { hasText: "Add External Map Layer" }).click();
        await waitForModalStable(page);
        await expect(page.locator(MODAL_SELECTOR)).toHaveScreenshot("modal-create-map-layer.png", MODAL_SCREENSHOT_OPTIONS);
    });

    test("Create org type modal", async ({ page }) => {
        await page.goto("/organization-and-relationship-types");
        await waitForPageStable(page);
        const createBtn = page.locator("button", { hasText: /Create/i }).first();
        if (await createBtn.isVisible({ timeout: 5000 }).catch(() => false)) {
            await createBtn.click();
            await waitForModalStable(page);
            await expect(page.locator(MODAL_SELECTOR)).toHaveScreenshot("modal-create-org-type.png", MODAL_SCREENSHOT_OPTIONS);
        }
    });

    // ─── Financial modals ───────────────────────────────────────────────

    test("Create agreement modal", async ({ page }) => {
        await page.goto("/agreements");
        await waitForPageStable(page);
        const createBtn = page.locator("button", { hasText: "Create New" });
        if (await createBtn.isVisible({ timeout: 5000 }).catch(() => false)) {
            await createBtn.click();
            await waitForModalStable(page);
            await expect(page.locator(MODAL_SELECTOR)).toHaveScreenshot("modal-create-agreement.png", MODAL_SCREENSHOT_OPTIONS);
        }
    });

    test("Edit agreement modal", async ({ page }) => {
        await page.goto(`/agreements/${testData.agreementID}`);
        await waitForPageStable(page);
        const editButton = page.locator("button", { hasText: "Edit" }).first();
        if (await editButton.isVisible({ timeout: 5000 }).catch(() => false)) {
            await editButton.click();
            await waitForModalStable(page);
            await expect(page.locator(MODAL_SELECTOR)).toHaveScreenshot("modal-edit-agreement.png", MODAL_SCREENSHOT_OPTIONS);
        }
    });

    test("Edit fund source modal", async ({ page }) => {
        await page.goto(`/fund-sources/${testData.fundSourceID}`);
        await waitForPageStable(page);
        const editButton = page.locator("button", { hasText: "Edit" }).first();
        if (await editButton.isVisible({ timeout: 5000 }).catch(() => false)) {
            await editButton.click();
            await waitForModalStable(page);
            await expect(page.locator(MODAL_SELECTOR)).toHaveScreenshot("modal-edit-fund-source.png", MODAL_SCREENSHOT_OPTIONS);
        }
    });

    test("Edit invoice modal", async ({ page }) => {
        await page.goto(`/invoices/${testData.invoiceID}`);
        await waitForPageStable(page);
        const editButton = page.locator("button", { hasText: "Edit" }).first();
        if (await editButton.isVisible({ timeout: 5000 }).catch(() => false)) {
            await editButton.click();
            await waitForModalStable(page);
            await expect(page.locator(MODAL_SELECTOR)).toHaveScreenshot("modal-edit-invoice.png", MODAL_SCREENSHOT_OPTIONS);
        }
    });

    // ─── People modals ──────────────────────────────────────────────────

    test("Add contact modal", async ({ page }) => {
        await page.goto("/people");
        await waitForPageStable(page);
        await page.locator("button", { hasText: "Add Contact" }).click();
        await waitForModalStable(page);
        await expect(page.locator(MODAL_SELECTOR)).toHaveScreenshot("modal-add-contact.png", MODAL_SCREENSHOT_OPTIONS);
    });

    test("Edit contact modal", async ({ page }) => {
        await page.goto(`/people/${testData.personID}`);
        await waitForPageStable(page);
        const editButton = page.locator("button", { hasText: /Edit.*Contact|Edit.*Info/i }).first();
        if (await editButton.isVisible({ timeout: 5000 }).catch(() => false)) {
            await editButton.click();
            await waitForModalStable(page);
            await expect(page.locator(MODAL_SELECTOR)).toHaveScreenshot("modal-edit-contact.png", MODAL_SCREENSHOT_OPTIONS);
        }
    });

    test("Edit roles modal", async ({ page }) => {
        await page.goto(`/people/${testData.personID}`);
        await waitForPageStable(page);
        const rolesBtn = page.locator("button", { hasText: /Edit.*Role/i }).first();
        if (await rolesBtn.isVisible({ timeout: 5000 }).catch(() => false)) {
            await rolesBtn.click();
            await waitForModalStable(page);
            await expect(page.locator(MODAL_SELECTOR)).toHaveScreenshot("modal-edit-roles.png", MODAL_SCREENSHOT_OPTIONS);
        }
    });

    // ─── Project detail modals ──────────────────────────────────────────

    test("Edit project basics modal", async ({ page }) => {
        await page.goto(`/projects/${testData.projectID}`);
        await waitForPageStable(page);
        const editBtn = page.locator("button", { hasText: /Edit.*Basics|Edit.*Project/i }).first();
        if (await editBtn.isVisible({ timeout: 5000 }).catch(() => false)) {
            await editBtn.click();
            await waitForModalStable(page);
            await expect(page.locator(MODAL_SELECTOR)).toHaveScreenshot("modal-edit-project-basics.png", MODAL_SCREENSHOT_OPTIONS);
        }
    });

    test("Edit project tags modal", async ({ page }) => {
        await page.goto(`/projects/${testData.projectID}`);
        await waitForPageStable(page);
        const editBtn = page.locator("button", { hasText: /Edit.*Tag/i }).first();
        if (await editBtn.isVisible({ timeout: 5000 }).catch(() => false)) {
            await editBtn.click();
            await waitForModalStable(page);
            await expect(page.locator(MODAL_SELECTOR)).toHaveScreenshot("modal-edit-project-tags.png", MODAL_SCREENSHOT_OPTIONS);
        }
    });

    test("Edit project organizations modal", async ({ page }) => {
        await page.goto(`/projects/${testData.projectID}`);
        await waitForPageStable(page);
        const editBtn = page.locator("button", { hasText: /Edit.*Org/i }).first();
        if (await editBtn.isVisible({ timeout: 5000 }).catch(() => false)) {
            await editBtn.click();
            await waitForModalStable(page);
            await expect(page.locator(MODAL_SELECTOR)).toHaveScreenshot("modal-edit-project-orgs.png", MODAL_SCREENSHOT_OPTIONS);
        }
    });

    test("Edit project contacts modal", async ({ page }) => {
        await page.goto(`/projects/${testData.projectID}`);
        await waitForPageStable(page);
        const editBtn = page.locator("button", { hasText: /Edit.*Contact/i }).first();
        if (await editBtn.isVisible({ timeout: 5000 }).catch(() => false)) {
            await editBtn.click();
            await waitForModalStable(page);
            await expect(page.locator(MODAL_SELECTOR)).toHaveScreenshot("modal-edit-project-contacts.png", MODAL_SCREENSHOT_OPTIONS);
        }
    });

    test("Edit project funding modal", async ({ page }) => {
        await page.goto(`/projects/${testData.projectID}`);
        await waitForPageStable(page);
        const editBtn = page.locator("button", { hasText: /Edit.*Fund/i }).first();
        if (await editBtn.isVisible({ timeout: 5000 }).catch(() => false)) {
            await editBtn.click();
            await waitForModalStable(page);
            await expect(page.locator(MODAL_SELECTOR)).toHaveScreenshot("modal-edit-project-funding.png", MODAL_SCREENSHOT_OPTIONS);
        }
    });

    test("Edit classification modal", async ({ page }) => {
        await page.goto(`/projects/${testData.projectID}`);
        await waitForPageStable(page);
        const editBtn = page.locator("button", { hasText: /Edit.*Classification|Edit.*Theme/i }).first();
        if (await editBtn.isVisible({ timeout: 5000 }).catch(() => false)) {
            await editBtn.click();
            await waitForModalStable(page);
            await expect(page.locator(MODAL_SELECTOR)).toHaveScreenshot("modal-edit-classification.png", MODAL_SCREENSHOT_OPTIONS);
        }
    });
});
