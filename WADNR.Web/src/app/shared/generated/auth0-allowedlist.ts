/* AUTO-GENERATED from swagger.json. DO NOT EDIT BY HAND. */

import { HttpInterceptorRouteConfig } from '@auth0/auth0-angular';

export type AllowedHttpMethod = 'GET'|'POST'|'PUT'|'PATCH'|'DELETE'|'OPTIONS'|'HEAD';

type ExactMap = Partial<Record<AllowedHttpMethod, ReadonlyArray<string>>>;
type RegexMap = Partial<Record<AllowedHttpMethod, ReadonlyArray<RegExp>>>;

function stripBase(apiBaseUrl: string, uri: string): string | null {
  const base = apiBaseUrl.endsWith('/') ? apiBaseUrl.slice(0, -1) : apiBaseUrl;

  // Only match our API base
  if (!uri.startsWith(base)) return null;

  // Remove the base. Ensure leading "/" for comparison with OpenAPI paths.
  const rest = uri.substring(base.length);
  if (rest === '') return '/';
  return rest.startsWith('/') ? rest : '/' + rest;
}

const ANON_EXACT: ExactMap = {
  'DELETE': [],
  'GET': ["/","/agreements","/classification-systems","/classification-systems/lookup","/classifications","/counties","/custom-pages/menu-item","/dnr-upland-regions","/external-map-layers/other-maps","/external-map-layers/priority-landscape","/external-map-layers/project-map","/field-definitions","/firma-home-page-images","/fund-source-allocations","/fund-source-allocations/lookup","/fund-sources","/interaction-events","/invoices","/invoices/approval-statuses","/lookups/classification-systems-with-classifications","/lookups/funding-sources","/lookups/organization-relationship-types","/lookups/person-relationship-types","/organization-types","/organizations","/organizations/lookup","/priority-landscapes","/program-indices","/program-indices/lookup","/program-indices/search","/programs","/project-codes","/project-codes/lookup","/project-codes/search","/project-documents/types","/project-images/timings","/project-types","/project-types/lookup","/project-types/taxonomy","/projects","/projects/featured","/projects/mapped-point/feature-collection","/projects/no-simple-location","/tags","/taxonomy-branches","/taxonomy-branches/lookup","/taxonomy-trunks","/taxonomy-trunks/lookup","/with-project-count"],
  'POST': [],
  'PUT': [],
};

const SECURED_EXACT: ExactMap = {
  'DELETE': [],
  'GET': ["/focus-areas","/people","/people/lookup","/roles","/vendors","/vendors/search"],
  'POST': ["/agreements","/classifications","/counties","/dnr-upland-regions","/fund-sources","/interaction-events","/invoice-payment-requests","/invoices","/organizations","/priority-landscapes","/programs","/project-documents","/project-images","/project-internal-notes","/project-notes","/project-types","/projects","/projects/create-workflow/steps/basics","/sitkacapture/generate-pdf","/support-requests","/tags","/taxonomy-branches","/taxonomy-trunks","/treatments","/user-claims"],
  'PUT': [],
};

const ANON_REGEX: RegexMap = {
  'DELETE': [

  ],
  'GET': [
    new RegExp("^/agreements/[^/]+$"),
    new RegExp("^/agreements/[^/]+/contacts$"),
    new RegExp("^/agreements/[^/]+/fund-source-allocations$"),
    new RegExp("^/agreements/[^/]+/projects$"),
    new RegExp("^/classification-systems/[^/]+$"),
    new RegExp("^/classifications/[^/]+$"),
    new RegExp("^/counties/[^/]+$"),
    new RegExp("^/counties/[^/]+/projects/feature-collection$"),
    new RegExp("^/custom-pages/[^/]+$"),
    new RegExp("^/custom-pages/navigation-section/[^/]+$"),
    new RegExp("^/custom-rich-texts/[^/]+$"),
    new RegExp("^/dnr-upland-regions/[^/]+$"),
    new RegExp("^/dnr-upland-regions/[^/]+/fund-source-allocations$"),
    new RegExp("^/dnr-upland-regions/[^/]+/projects/feature-collection$"),
    new RegExp("^/field-definitions/[^/]+$"),
    new RegExp("^/file-resources/GetWithApiKey/[^/]+$"),
    new RegExp("^/file-resources/[^/]+$"),
    new RegExp("^/fund-source-allocations/[^/]+$"),
    new RegExp("^/fund-sources/[^/]+$"),
    new RegExp("^/fund-sources/[^/]+/agreements$"),
    new RegExp("^/fund-sources/[^/]+/allocations$"),
    new RegExp("^/fund-sources/[^/]+/budget-line-items$"),
    new RegExp("^/fund-sources/[^/]+/files$"),
    new RegExp("^/fund-sources/[^/]+/notes$"),
    new RegExp("^/fund-sources/[^/]+/projects$"),
    new RegExp("^/interaction-events/[^/]+$"),
    new RegExp("^/interaction-events/[^/]+/contacts$"),
    new RegExp("^/interaction-events/[^/]+/file-resources$"),
    new RegExp("^/interaction-events/[^/]+/projects$"),
    new RegExp("^/interaction-events/[^/]+/simple-location/feature-collection$"),
    new RegExp("^/invoice-payment-requests/[^/]+/invoices$"),
    new RegExp("^/invoices/[^/]+$"),
    new RegExp("^/organizations/[^/]+$"),
    new RegExp("^/organizations/[^/]+/agreements$"),
    new RegExp("^/organizations/[^/]+/boundary$"),
    new RegExp("^/organizations/[^/]+/programs$"),
    new RegExp("^/organizations/[^/]+/project-locations$"),
    new RegExp("^/priority-landscapes/[^/]+$"),
    new RegExp("^/priority-landscapes/[^/]+/file-resources$"),
    new RegExp("^/priority-landscapes/[^/]+/projects$"),
    new RegExp("^/priority-landscapes/[^/]+/projects/feature-collection$"),
    new RegExp("^/program-indices/[^/]+$"),
    new RegExp("^/programs/[^/]+$"),
    new RegExp("^/programs/[^/]+/notifications$"),
    new RegExp("^/programs/[^/]+/projects$"),
    new RegExp("^/project-codes/[^/]+$"),
    new RegExp("^/project-documents/[^/]+$"),
    new RegExp("^/project-images/[^/]+$"),
    new RegExp("^/project-notes/[^/]+$"),
    new RegExp("^/project-types/[^/]+$"),
    new RegExp("^/projects/[^/]+$"),
    new RegExp("^/projects/[^/]+/classifications$"),
    new RegExp("^/projects/[^/]+/documents$"),
    new RegExp("^/projects/[^/]+/external-links$"),
    new RegExp("^/projects/[^/]+/fact-sheet$"),
    new RegExp("^/projects/[^/]+/images$"),
    new RegExp("^/projects/[^/]+/interaction-events$"),
    new RegExp("^/projects/[^/]+/locations/generic-layers$"),
    new RegExp("^/projects/[^/]+/map-popup$"),
    new RegExp("^/projects/[^/]+/notes$"),
    new RegExp("^/projects/[^/]+/notifications$"),
    new RegExp("^/projects/[^/]+/treatment-areas$"),
    new RegExp("^/projects/[^/]+/treatments$"),
    new RegExp("^/projects/[^/]+/update-history$"),
    new RegExp("^/search/projects/[^/]+$"),
    new RegExp("^/tags/[^/]+$"),
    new RegExp("^/taxonomy-branches/[^/]+$"),
    new RegExp("^/taxonomy-trunks/[^/]+$"),
    new RegExp("^/treatments/[^/]+$"),
  ],
  'POST': [

  ],
  'PUT': [

  ],
};

const SECURED_REGEX: RegexMap = {
  'DELETE': [
    new RegExp("^/agreements/[^/]+$"),
    new RegExp("^/classifications/[^/]+$"),
    new RegExp("^/counties/[^/]+$"),
    new RegExp("^/dnr-upland-regions/[^/]+$"),
    new RegExp("^/fund-sources/[^/]+$"),
    new RegExp("^/interaction-events/[^/]+$"),
    new RegExp("^/invoices/[^/]+/voucher$"),
    new RegExp("^/organizations/[^/]+$"),
    new RegExp("^/organizations/[^/]+/boundary$"),
    new RegExp("^/priority-landscapes/[^/]+$"),
    new RegExp("^/programs/[^/]+$"),
    new RegExp("^/project-documents/[^/]+$"),
    new RegExp("^/project-images/[^/]+$"),
    new RegExp("^/project-internal-notes/[^/]+$"),
    new RegExp("^/project-notes/[^/]+$"),
    new RegExp("^/project-types/[^/]+$"),
    new RegExp("^/projects/[^/]+$"),
    new RegExp("^/projects/[^/]+/block-list$"),
    new RegExp("^/projects/[^/]+/update-workflow/current$"),
    new RegExp("^/tags/[^/]+$"),
    new RegExp("^/taxonomy-branches/[^/]+$"),
    new RegExp("^/taxonomy-trunks/[^/]+$"),
    new RegExp("^/treatments/[^/]+$"),
  ],
  'GET': [
    new RegExp("^/classifications/[^/]+/projects$"),
    new RegExp("^/counties/[^/]+/projects$"),
    new RegExp("^/dnr-upland-regions/[^/]+/focus-areas$"),
    new RegExp("^/dnr-upland-regions/[^/]+/projects$"),
    new RegExp("^/focus-areas/[^/]+$"),
    new RegExp("^/fund-sources/[^/]+/notes-internal$"),
    new RegExp("^/organizations/[^/]+/projects$"),
    new RegExp("^/organizations/[^/]+/projects/pending$"),
    new RegExp("^/people/[^/]+$"),
    new RegExp("^/people/[^/]+/agreements$"),
    new RegExp("^/people/[^/]+/interaction-events$"),
    new RegExp("^/people/[^/]+/projects$"),
    new RegExp("^/project-internal-notes/[^/]+$"),
    new RegExp("^/project-types/[^/]+/projects$"),
    new RegExp("^/project-types/[^/]+/projects/mapped-point/feature-collection$"),
    new RegExp("^/projects/[^/]+/audit-logs$"),
    new RegExp("^/projects/[^/]+/basics/edit$"),
    new RegExp("^/projects/[^/]+/classifications/edit$"),
    new RegExp("^/projects/[^/]+/counties$"),
    new RegExp("^/projects/[^/]+/create-workflow/progress$"),
    new RegExp("^/projects/[^/]+/create-workflow/steps/basics$"),
    new RegExp("^/projects/[^/]+/create-workflow/steps/classifications$"),
    new RegExp("^/projects/[^/]+/create-workflow/steps/contacts$"),
    new RegExp("^/projects/[^/]+/create-workflow/steps/counties$"),
    new RegExp("^/projects/[^/]+/create-workflow/steps/dnr-upland-regions$"),
    new RegExp("^/projects/[^/]+/create-workflow/steps/expected-funding$"),
    new RegExp("^/projects/[^/]+/create-workflow/steps/location-detailed$"),
    new RegExp("^/projects/[^/]+/create-workflow/steps/location-simple$"),
    new RegExp("^/projects/[^/]+/create-workflow/steps/organizations$"),
    new RegExp("^/projects/[^/]+/create-workflow/steps/priority-landscapes$"),
    new RegExp("^/projects/[^/]+/dnr-upland-regions$"),
    new RegExp("^/projects/[^/]+/funding$"),
    new RegExp("^/projects/[^/]+/internal-notes$"),
    new RegExp("^/projects/[^/]+/invoice-payment-requests$"),
    new RegExp("^/projects/[^/]+/invoices$"),
    new RegExp("^/projects/[^/]+/location-detailed$"),
    new RegExp("^/projects/[^/]+/location-simple$"),
    new RegExp("^/projects/[^/]+/map-extent$"),
    new RegExp("^/projects/[^/]+/priority-landscapes$"),
    new RegExp("^/projects/[^/]+/update-workflow/current$"),
    new RegExp("^/projects/[^/]+/update-workflow/diff$"),
    new RegExp("^/projects/[^/]+/update-workflow/progress$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/[^/]+/diff$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/basics$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/contacts$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/counties$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/dnr-upland-regions$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/documents-notes$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/expected-funding$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/external-links$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/location-detailed$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/location-simple$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/organizations$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/photos$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/priority-landscapes$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/treatments$"),
    new RegExp("^/projects/[^/]+/update-workflow/treatment-areas$"),
    new RegExp("^/projects/[^/]+/update-workflow/treatments/[^/]+$"),
    new RegExp("^/roles/[^/]+$"),
    new RegExp("^/roles/[^/]+/people$"),
    new RegExp("^/tags/[^/]+/projects$"),
    new RegExp("^/taxonomy-branches/[^/]+/projects$"),
    new RegExp("^/taxonomy-branches/[^/]+/projects/mapped-point/feature-collection$"),
    new RegExp("^/taxonomy-trunks/[^/]+/projects$"),
    new RegExp("^/user-claims/[^/]+$"),
    new RegExp("^/vendors/[^/]+$"),
    new RegExp("^/vendors/[^/]+/organizations$"),
    new RegExp("^/vendors/[^/]+/people$"),
  ],
  'POST': [
    new RegExp("^/invoices/[^/]+/voucher$"),
    new RegExp("^/project-images/[^/]+/set-key-photo$"),
    new RegExp("^/projects/[^/]+/block-list$"),
    new RegExp("^/projects/[^/]+/create-workflow/approve$"),
    new RegExp("^/projects/[^/]+/create-workflow/reject$"),
    new RegExp("^/projects/[^/]+/create-workflow/return$"),
    new RegExp("^/projects/[^/]+/create-workflow/steps/location-detailed/approve-gdb$"),
    new RegExp("^/projects/[^/]+/create-workflow/steps/location-detailed/upload-gdb$"),
    new RegExp("^/projects/[^/]+/create-workflow/submit$"),
    new RegExp("^/projects/[^/]+/create-workflow/withdraw$"),
    new RegExp("^/projects/[^/]+/location-detailed/approve-gdb$"),
    new RegExp("^/projects/[^/]+/location-detailed/upload-gdb$"),
    new RegExp("^/projects/[^/]+/update-workflow/approve$"),
    new RegExp("^/projects/[^/]+/update-workflow/return$"),
    new RegExp("^/projects/[^/]+/update-workflow/start$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/[^/]+/revert$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/location-detailed/approve-gdb$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/location-detailed/upload-gdb$"),
    new RegExp("^/projects/[^/]+/update-workflow/submit$"),
    new RegExp("^/projects/[^/]+/update-workflow/treatments$"),
  ],
  'PUT': [
    new RegExp("^/agreements/[^/]+$"),
    new RegExp("^/classifications/[^/]+$"),
    new RegExp("^/counties/[^/]+$"),
    new RegExp("^/custom-rich-texts/[^/]+$"),
    new RegExp("^/dnr-upland-regions/[^/]+$"),
    new RegExp("^/field-definitions/[^/]+$"),
    new RegExp("^/fund-sources/[^/]+$"),
    new RegExp("^/interaction-events/[^/]+$"),
    new RegExp("^/invoices/[^/]+$"),
    new RegExp("^/organizations/[^/]+$"),
    new RegExp("^/priority-landscapes/[^/]+$"),
    new RegExp("^/programs/[^/]+$"),
    new RegExp("^/project-documents/[^/]+$"),
    new RegExp("^/project-images/[^/]+$"),
    new RegExp("^/project-internal-notes/[^/]+$"),
    new RegExp("^/project-notes/[^/]+$"),
    new RegExp("^/project-types/[^/]+$"),
    new RegExp("^/projects/[^/]+$"),
    new RegExp("^/projects/[^/]+/basics$"),
    new RegExp("^/projects/[^/]+/classifications$"),
    new RegExp("^/projects/[^/]+/contacts$"),
    new RegExp("^/projects/[^/]+/counties$"),
    new RegExp("^/projects/[^/]+/create-workflow/steps/basics$"),
    new RegExp("^/projects/[^/]+/create-workflow/steps/classifications$"),
    new RegExp("^/projects/[^/]+/create-workflow/steps/contacts$"),
    new RegExp("^/projects/[^/]+/create-workflow/steps/counties$"),
    new RegExp("^/projects/[^/]+/create-workflow/steps/dnr-upland-regions$"),
    new RegExp("^/projects/[^/]+/create-workflow/steps/expected-funding$"),
    new RegExp("^/projects/[^/]+/create-workflow/steps/location-detailed$"),
    new RegExp("^/projects/[^/]+/create-workflow/steps/location-simple$"),
    new RegExp("^/projects/[^/]+/create-workflow/steps/organizations$"),
    new RegExp("^/projects/[^/]+/create-workflow/steps/priority-landscapes$"),
    new RegExp("^/projects/[^/]+/dnr-upland-regions$"),
    new RegExp("^/projects/[^/]+/external-links$"),
    new RegExp("^/projects/[^/]+/funding$"),
    new RegExp("^/projects/[^/]+/location-detailed$"),
    new RegExp("^/projects/[^/]+/location-simple$"),
    new RegExp("^/projects/[^/]+/map-extent$"),
    new RegExp("^/projects/[^/]+/organizations$"),
    new RegExp("^/projects/[^/]+/priority-landscapes$"),
    new RegExp("^/projects/[^/]+/tags$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/basics$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/contacts$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/counties$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/dnr-upland-regions$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/documents-notes$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/expected-funding$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/external-links$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/location-detailed$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/location-simple$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/organizations$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/photos$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/priority-landscapes$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/treatments$"),
    new RegExp("^/projects/[^/]+/update-workflow/treatments/[^/]+$"),
    new RegExp("^/tags/[^/]+$"),
    new RegExp("^/taxonomy-branches/[^/]+$"),
    new RegExp("^/taxonomy-trunks/[^/]+$"),
    new RegExp("^/treatments/[^/]+$"),
  ],
};

/**
 * Auth0 httpInterceptor.allowedList generator.
 *
 * This generates route configs that:
 * - For anonymous routes: attach token if user is logged in, allow request if not (allowAnonymous: true)
 * - For secured routes: require token (no allowAnonymous flag)
 *
 * This allows endpoints like /projects to work for both anonymous users (showing public data)
 * and authenticated users (showing additional data based on their identity).
 */
export function buildAuth0AllowedList(apiBaseUrl: string): HttpInterceptorRouteConfig[] {
  const methods: AllowedHttpMethod[] = ['GET','POST','PUT','PATCH','DELETE','OPTIONS','HEAD'];
  const entries: HttpInterceptorRouteConfig[] = [];
  const base = apiBaseUrl.endsWith('/') ? apiBaseUrl.slice(0, -1) : apiBaseUrl;

  // Anonymous routes: attach token if available, allow anonymous access
  for (const method of methods) {
    const exactPaths = ANON_EXACT[method];
    if (exactPaths) {
      for (const path of exactPaths) {
        entries.push({
          uri: base + path,
          httpMethod: method,
          allowAnonymous: true
        });
      }
    }
    const regexPatterns = ANON_REGEX[method];
    if (regexPatterns) {
      for (const rx of regexPatterns) {
        entries.push({
          uriMatcher: (uri: string) => {
            const p = stripBase(apiBaseUrl, uri);
            return p !== null && rx.test(p);
          },
          httpMethod: method,
          allowAnonymous: true
        });
      }
    }
  }

  // Secured routes: require token
  for (const method of methods) {
    const exactPaths = SECURED_EXACT[method];
    if (exactPaths) {
      for (const path of exactPaths) {
        entries.push({
          uri: base + path,
          httpMethod: method
        });
      }
    }
    const regexPatterns = SECURED_REGEX[method];
    if (regexPatterns) {
      for (const rx of regexPatterns) {
        entries.push({
          uriMatcher: (uri: string) => {
            const p = stripBase(apiBaseUrl, uri);
            return p !== null && rx.test(p);
          },
          httpMethod: method
        });
      }
    }
  }

  return entries;
}
