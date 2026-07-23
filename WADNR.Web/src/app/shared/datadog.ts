import { datadogLogs } from "@datadog/browser-logs";
import { environment } from "src/environments/environment";

/**
 * Initializes Datadog Browser Logs. Called once from main.ts before the app bootstraps.
 *
 * When no clientToken is configured (e.g. local dev, where environment.datadog.clientToken
 * is empty) initialization is skipped so local noise is not forwarded to Datadog.
 */
export function initializeDatadogLogs(): void {
    const config = environment.datadog;

    if (!config?.clientToken) {
        return;
    }

    datadogLogs.init({
        clientToken: config.clientToken,
        site: config.site,
        service: config.service,
        env: config.env,
        // Forward uncaught exceptions, unhandled promise rejections, and failed
        // network requests to Datadog Logs automatically.
        forwardErrorsToLogs: true,
        // Also mirror browser console.error output into Datadog.
        forwardConsoleLogs: ["error"],
        sessionSampleRate: 100,
    });
}

/**
 * True when Datadog Browser Logs has been initialized for this session.
 * Used to guard explicit logger calls (e.g. in the global error handler).
 */
export function isDatadogLogsEnabled(): boolean {
    return !!datadogLogs.getInitConfiguration();
}
