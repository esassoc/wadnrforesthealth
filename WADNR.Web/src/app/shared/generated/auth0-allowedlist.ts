/* AUTO-GENERATED from swagger.json. DO NOT EDIT BY HAND. */

export type AllowedHttpMethod = 'GET'|'POST'|'PUT'|'PATCH'|'DELETE'|'OPTIONS'|'HEAD';

type ExactMap = Partial<Record<AllowedHttpMethod, ReadonlySet<string>>>;
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
  'DELETE': new Set([]),
  'GET': new Set(["/","/agreements","/custom-pages/menu-item","/field-definitions","/organizations","/programs","/projects","/projects/mapped-point/feature-collection","/projects/no-simple-location"]),
  'POST': new Set([]),
  'PUT': new Set([]),
};

const SECURED_EXACT: ExactMap = {
  'DELETE': new Set([]),
  'GET': new Set(["/classification-systems","/classification-systems/lookup","/classifications","/counties","/dnr-upland-regions","/focus-areas","/fund-source-allocations","/fund-source-allocations/lookup","/fund-sources","/interaction-events","/invoices","/organization-types","/people","/people/lookup","/priority-landscapes","/program-indices","/program-indices/lookup","/program-indices/search","/project-codes","/project-codes/lookup","/project-codes/search","/project-documents/types","/project-types","/project-types/taxonomy","/roles","/tags","/taxonomy-branches","/taxonomy-branches/lookup","/taxonomy-trunks","/taxonomy-trunks/lookup","/vendors","/vendors/search","/with-project-count"]),
  'POST': new Set(["/agreements","/classifications","/counties","/dnr-upland-regions","/fund-sources","/interaction-events","/organizations","/priority-landscapes","/programs","/project-documents","/project-types","/projects","/sitkacapture/generate-pdf","/tags","/taxonomy-branches","/taxonomy-trunks","/treatments","/user-claims"]),
  'PUT': new Set([]),
};

const ANON_REGEX: RegexMap = {
  'DELETE': [

  ],
  'GET': [
    new RegExp("^/agreements/[^/]+$"),
    new RegExp("^/agreements/[^/]+/contacts$"),
    new RegExp("^/agreements/[^/]+/fund-source-allocations$"),
    new RegExp("^/agreements/[^/]+/projects$"),
    new RegExp("^/custom-pages/[^/]+$"),
    new RegExp("^/custom-pages/navigation-section/[^/]+$"),
    new RegExp("^/custom-rich-texts/[^/]+$"),
    new RegExp("^/field-definitions/[^/]+$"),
    new RegExp("^/organizations/[^/]+$"),
    new RegExp("^/organizations/[^/]+/agreements$"),
    new RegExp("^/organizations/[^/]+/boundary$"),
    new RegExp("^/organizations/[^/]+/programs$"),
    new RegExp("^/organizations/[^/]+/project-locations$"),
    new RegExp("^/organizations/[^/]+/projects$"),
    new RegExp("^/organizations/[^/]+/projects/pending$"),
    new RegExp("^/programs/[^/]+$"),
    new RegExp("^/programs/[^/]+/notifications$"),
    new RegExp("^/programs/[^/]+/projects$"),
    new RegExp("^/projects/[^/]+$"),
    new RegExp("^/projects/[^/]+/audit-logs$"),
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
    new RegExp("^/projects/[^/]+/treatments$"),
    new RegExp("^/projects/[^/]+/update-history$"),
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
    new RegExp("^/organizations/[^/]+$"),
    new RegExp("^/organizations/[^/]+/boundary$"),
    new RegExp("^/priority-landscapes/[^/]+$"),
    new RegExp("^/programs/[^/]+$"),
    new RegExp("^/project-documents/[^/]+$"),
    new RegExp("^/project-types/[^/]+$"),
    new RegExp("^/projects/[^/]+$"),
    new RegExp("^/tags/[^/]+$"),
    new RegExp("^/taxonomy-branches/[^/]+$"),
    new RegExp("^/taxonomy-trunks/[^/]+$"),
    new RegExp("^/treatments/[^/]+$"),
  ],
  'GET': [
    new RegExp("^/classification-systems/[^/]+$"),
    new RegExp("^/classifications/[^/]+$"),
    new RegExp("^/classifications/[^/]+/projects$"),
    new RegExp("^/counties/[^/]+$"),
    new RegExp("^/counties/[^/]+/projects$"),
    new RegExp("^/dnr-upland-regions/[^/]+$"),
    new RegExp("^/dnr-upland-regions/[^/]+/fund-source-allocations$"),
    new RegExp("^/dnr-upland-regions/[^/]+/projects$"),
    new RegExp("^/file-resources/GetWithApiKey/[^/]+$"),
    new RegExp("^/file-resources/[^/]+$"),
    new RegExp("^/focus-areas/[^/]+$"),
    new RegExp("^/focus-areas/for-region/[^/]+$"),
    new RegExp("^/fund-source-allocations/[^/]+$"),
    new RegExp("^/fund-source-allocations/for-fund-source/[^/]+$"),
    new RegExp("^/fund-sources/[^/]+$"),
    new RegExp("^/fund-sources/[^/]+/agreements$"),
    new RegExp("^/fund-sources/[^/]+/allocations$"),
    new RegExp("^/fund-sources/[^/]+/budget-line-items$"),
    new RegExp("^/fund-sources/[^/]+/files$"),
    new RegExp("^/fund-sources/[^/]+/notes$"),
    new RegExp("^/fund-sources/[^/]+/notes-internal$"),
    new RegExp("^/fund-sources/[^/]+/projects$"),
    new RegExp("^/interaction-events/[^/]+$"),
    new RegExp("^/interaction-events/[^/]+/contacts$"),
    new RegExp("^/interaction-events/[^/]+/file-resources$"),
    new RegExp("^/interaction-events/[^/]+/projects$"),
    new RegExp("^/interaction-events/[^/]+/simple-location/feature-collection$"),
    new RegExp("^/invoices/[^/]+$"),
    new RegExp("^/invoices/for-payment-request/[^/]+$"),
    new RegExp("^/invoices/for-project/[^/]+$"),
    new RegExp("^/people/[^/]+$"),
    new RegExp("^/people/[^/]+/agreements$"),
    new RegExp("^/people/[^/]+/interaction-events$"),
    new RegExp("^/people/[^/]+/projects$"),
    new RegExp("^/priority-landscapes/[^/]+$"),
    new RegExp("^/priority-landscapes/[^/]+/file-resources$"),
    new RegExp("^/priority-landscapes/[^/]+/projects$"),
    new RegExp("^/program-indices/[^/]+$"),
    new RegExp("^/program-indices/for-biennium/[^/]+$"),
    new RegExp("^/program-indices/lookup/for-biennium/[^/]+$"),
    new RegExp("^/project-codes/[^/]+$"),
    new RegExp("^/project-documents/[^/]+$"),
    new RegExp("^/project-types/[^/]+$"),
    new RegExp("^/project-types/[^/]+/projects$"),
    new RegExp("^/project-types/[^/]+/projects/mapped-point/feature-collection$"),
    new RegExp("^/roles/[^/]+$"),
    new RegExp("^/roles/[^/]+/people$"),
    new RegExp("^/tags/[^/]+$"),
    new RegExp("^/tags/[^/]+/projects$"),
    new RegExp("^/taxonomy-branches/[^/]+$"),
    new RegExp("^/taxonomy-branches/[^/]+/projects$"),
    new RegExp("^/taxonomy-branches/[^/]+/projects/mapped-point/feature-collection$"),
    new RegExp("^/taxonomy-trunks/[^/]+$"),
    new RegExp("^/taxonomy-trunks/[^/]+/projects$"),
    new RegExp("^/treatments/[^/]+$"),
    new RegExp("^/user-claims/[^/]+$"),
    new RegExp("^/vendors/[^/]+$"),
    new RegExp("^/vendors/[^/]+/organizations$"),
    new RegExp("^/vendors/[^/]+/people$"),
  ],
  'POST': [

  ],
  'PUT': [
    new RegExp("^/agreements/[^/]+$"),
    new RegExp("^/classifications/[^/]+$"),
    new RegExp("^/counties/[^/]+$"),
    new RegExp("^/dnr-upland-regions/[^/]+$"),
    new RegExp("^/fund-sources/[^/]+$"),
    new RegExp("^/interaction-events/[^/]+$"),
    new RegExp("^/organizations/[^/]+$"),
    new RegExp("^/priority-landscapes/[^/]+$"),
    new RegExp("^/programs/[^/]+$"),
    new RegExp("^/project-documents/[^/]+$"),
    new RegExp("^/project-types/[^/]+$"),
    new RegExp("^/projects/[^/]+$"),
    new RegExp("^/tags/[^/]+$"),
    new RegExp("^/taxonomy-branches/[^/]+$"),
    new RegExp("^/taxonomy-trunks/[^/]+$"),
    new RegExp("^/treatments/[^/]+$"),
  ],
};

function matchesAnon(method: AllowedHttpMethod, p: string): boolean {
  const exact = ANON_EXACT[method];
  if (exact?.has(p)) return true;

  const regexes = ANON_REGEX[method] ?? [];
  return regexes.some(rx => rx.test(p));
}

function matchesSecured(method: AllowedHttpMethod, p: string): boolean {
  const exact = SECURED_EXACT[method];
  if (exact?.has(p)) return true;

  const regexes = SECURED_REGEX[method] ?? [];
  return regexes.some(rx => rx.test(p));
}

/**
 * Auth0 httpInterceptor.allowedList generator.
 *
 * Rule:
 * - If request matches an anonymous route for that method => DO NOT attach token.
 * - Else if it matches a secured route for that method => attach token.
 * - Else => do nothing.
 *
 * This prevents overlap issues like:
 *   secured template:  /jurisdictions/{id}  (regex ^/jurisdictions/[^/]+$)
 *   anonymous literal: /jurisdictions/user-viewable
 * by always checking anonymous first.
 */
export function buildAuth0AllowedList(apiBaseUrl: string) {
  const methods: AllowedHttpMethod[] = ['GET','POST','PUT','PATCH','DELETE','OPTIONS','HEAD'];

  return methods.map(httpMethod => ({
    httpMethod,
    uriMatcher: (uri: string) => {
      const p = stripBase(apiBaseUrl, uri);
      if (p === null) return false;

      if (matchesAnon(httpMethod, p)) return false;
      return matchesSecured(httpMethod, p);
    }
  }));
}
