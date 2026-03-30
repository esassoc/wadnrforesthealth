import { DomSanitizer, SafeResourceUrl } from "@angular/platform-browser";
import { FileResourceService } from "src/app/shared/generated/api/file-resource.service";

/**
 * Build a SafeResourceUrl for a file resource GUID using the FileResourceService basePath.
 * Returns null if guid is falsy or building the URL fails.
 */
export function getFileResourceUrl(fileResourceService: FileResourceService | undefined | null, sanitizer: DomSanitizer, guid?: string | null): SafeResourceUrl | null {
    if (!guid) return null;
    try {
        const base = (fileResourceService && fileResourceService.configuration && fileResourceService.configuration.basePath) || "";
        const path = `/file-resources/${encodeURIComponent(guid)}`;
        const full = `${base}${path}`;
        return sanitizer.bypassSecurityTrustResourceUrl(full);
    } catch (e) {
        // keep parity with previous behavior: log and return null
        // eslint-disable-next-line no-console
        console.error("Failed to build file resource url", e);
        return null;
    }
}

/**
 * Build a SafeResourceUrl for a file resource GUID using an explicit basePath string.
 */
export function getFileResourceUrlFromBase(basePath: string | undefined | null, sanitizer: DomSanitizer, guid?: string | null): SafeResourceUrl | null {
    if (!guid) return null;
    try {
        const base = basePath || "";
        const path = `/file-resources/${encodeURIComponent(guid)}`;
        const full = `${base}${path}`;
        return sanitizer.bypassSecurityTrustResourceUrl(full);
    } catch (e) {
        // eslint-disable-next-line no-console
        console.error("Failed to build file resource url", e);
        return null;
    }
}
