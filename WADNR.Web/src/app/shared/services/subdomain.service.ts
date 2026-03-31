import { Injectable } from "@angular/core";

@Injectable({ providedIn: "root" })
export class SubdomainService {
    getSubdomain(): string {
        const host = window.location.hostname;
        const parts = host.split(".");
        let subdomain = "main";
        // Local dev: treat both 'localhost.esassoc.com' and 'wadnr.localhost.esassoc.com' as main
        if (host === "localhost.esassoc.com" || host === "wadnr.localhost.esassoc.com") {
            subdomain = "main";
        } else if (host.endsWith(".esa-qa.sitkatech.com")) {
            // QA: area-wadnr.esa-qa.sitkatech.com or wadnr.esa-qa.sitkatech.com
            const first = parts[0];
            if (first === "wadnr") {
                subdomain = "main";
            } else if (first.endsWith("-wadnr")) {
                subdomain = first.replace("-wadnr", "");
            } else {
                subdomain = first;
            }
        } else if (parts.length > 2) {
            subdomain = parts[0];
        }
        console.debug(`[SubdomainService] Host: ${host}, Detected subdomain: ${subdomain}`);
        return subdomain;
    }
}
