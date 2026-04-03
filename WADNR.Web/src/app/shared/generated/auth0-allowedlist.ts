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
  'GET': ["/","/agreement-statuses/lookup","/agreement-types/lookup","/agreements","/classification-systems","/classification-systems/lookup","/classification-systems/with-classifications","/classifications","/cost-share/generate-pdf","/counties","/custom-pages/menu-item","/dnr-upland-regions","/dnr-upland-regions/lookup","/external-map-layers/other-maps","/external-map-layers/priority-landscape","/external-map-layers/project-map","/federal-fund-codes/lookup","/find-your-forester/questions","/find-your-forester/roles","/firma-home-page-images","/fund-source-allocation-priorities/lookup","/fund-source-allocations","/fund-source-allocations/lookup","/fund-source-types/lookup","/fund-sources","/fund-sources/lookup","/funding-sources","/interactions-events","/invoices/approval-statuses","/organization-types","/organization-types/lookup","/organizations","/organizations/lead-implementers","/organizations/lookup","/organizations/lookup-with-short-name","/priority-landscapes","/priority-landscapes/categories","/program-indices","/program-indices/lookup","/program-indices/search","/programs","/project-codes","/project-codes/lookup","/project-codes/search","/project-documents/types","/project-images/timings","/project-person-relationship-types","/project-types","/project-types/lookup","/project-types/taxonomy","/projects","/projects/featured","/projects/mapped-point/feature-collection","/projects/no-simple-location","/relationship-types","/relationship-types/lookup","/relationship-types/summary","/tags","/taxonomy-branches","/taxonomy-branches/lookup","/taxonomy-trunks","/taxonomy-trunks/lookup","/with-project-count"],
  'POST': ["/find-your-forester/by-point","/sitkacapture/generate-pdf"],
  'PUT': [],
};

const SECURED_EXACT: ExactMap = {
  'DELETE': [],
  'GET': ["/agreements/excel-download","/api/Job/import-history","/custom-pages","/custom-rich-texts","/external-map-layers","/field-definitions","/find-your-forester/assignable-people","/focus-areas","/focus-areas/locations","/fund-sources/excel-download","/gis-bulk-import/source-organizations","/invoices","/loa-upload/dashboard","/people","/people/lookup","/people/lookup/wadnr","/people/stewardship-areas/regions","/programs/eligible-editors","/project-update-configurations","/projects/excel-download","/projects/lookup","/projects/no-contact-count","/projects/pending","/projects/pending/excel-download","/projects/people-receiving-reminders","/projects/update-status","/report-templates","/report-templates/models","/roles","/vendors","/vendors/excel-download","/vendors/search"],
  'POST': ["/agreements","/agreements/upload-file","/api/Job/clear-outdated-imports","/classifications","/classifications/upload-key-image","/custom-pages","/dnr-upland-regions","/external-map-layers","/find-your-forester/work-units/bulk-assign","/firma-home-page-images","/focus-areas","/fund-source-allocation-notes","/fund-source-allocation-notes-internal","/fund-source-allocations","/fund-sources","/gis-bulk-import/attempts","/impersonation/stop","/interactions-events","/invoice-payment-requests","/invoices","/loa-upload/publish","/organization-types","/organizations","/people","/priority-landscapes","/programs","/programs/upload-example-geospatial-file","/programs/upload-program-file","/project-documents","/project-images","/project-internal-notes","/project-notes","/project-types","/projects","/projects/create-workflow/steps/basics","/projects/send-custom-notification","/projects/send-preview-notification","/relationship-types","/report-templates","/report-templates/generate-reports","/support-requests","/tags","/tags/bulk-tag-projects","/taxonomy-branches","/taxonomy-trunks","/treatments","/user-claims"],
  'PUT': ["/classifications/sort-order","/firma-home-page-images/sort-order","/project-types/sort-order","/project-update-configurations","/projects/featured"],
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
    new RegExp("^/classifications/[^/]+/projects$"),
    new RegExp("^/counties/[^/]+$"),
    new RegExp("^/counties/[^/]+/projects$"),
    new RegExp("^/counties/[^/]+/projects/feature-collection$"),
    new RegExp("^/custom-pages/[^/]+$"),
    new RegExp("^/custom-pages/navigation-section/[^/]+$"),
    new RegExp("^/custom-rich-texts/[^/]+$"),
    new RegExp("^/dnr-upland-regions/[^/]+$"),
    new RegExp("^/dnr-upland-regions/[^/]+/fund-source-allocations$"),
    new RegExp("^/dnr-upland-regions/[^/]+/projects$"),
    new RegExp("^/dnr-upland-regions/[^/]+/projects/feature-collection$"),
    new RegExp("^/field-definitions/[^/]+$"),
    new RegExp("^/file-resources/GetWithApiKey/[^/]+$"),
    new RegExp("^/file-resources/[^/]+$"),
    new RegExp("^/fund-source-allocation-notes/[^/]+$"),
    new RegExp("^/fund-source-allocations/[^/]+$"),
    new RegExp("^/fund-source-allocations/[^/]+/agreements$"),
    new RegExp("^/fund-source-allocations/[^/]+/budget-line-items$"),
    new RegExp("^/fund-source-allocations/[^/]+/expenditure-summary$"),
    new RegExp("^/fund-source-allocations/[^/]+/expenditures$"),
    new RegExp("^/fund-source-allocations/[^/]+/files$"),
    new RegExp("^/fund-source-allocations/[^/]+/notes$"),
    new RegExp("^/fund-source-allocations/[^/]+/program-index-project-codes$"),
    new RegExp("^/fund-source-allocations/[^/]+/projects$"),
    new RegExp("^/fund-sources/[^/]+$"),
    new RegExp("^/fund-sources/[^/]+/agreements$"),
    new RegExp("^/fund-sources/[^/]+/allocations$"),
    new RegExp("^/fund-sources/[^/]+/budget-line-items$"),
    new RegExp("^/fund-sources/[^/]+/files$"),
    new RegExp("^/fund-sources/[^/]+/notes$"),
    new RegExp("^/fund-sources/[^/]+/projects$"),
    new RegExp("^/interactions-events/[^/]+$"),
    new RegExp("^/interactions-events/[^/]+/contacts$"),
    new RegExp("^/interactions-events/[^/]+/file-resources$"),
    new RegExp("^/interactions-events/[^/]+/projects$"),
    new RegExp("^/interactions-events/[^/]+/simple-location/feature-collection$"),
    new RegExp("^/invoices/[^/]+$"),
    new RegExp("^/organizations/[^/]+$"),
    new RegExp("^/organizations/[^/]+/agreements$"),
    new RegExp("^/organizations/[^/]+/boundary$"),
    new RegExp("^/organizations/[^/]+/programs$"),
    new RegExp("^/organizations/[^/]+/project-locations$"),
    new RegExp("^/organizations/[^/]+/projects$"),
    new RegExp("^/priority-landscapes/[^/]+$"),
    new RegExp("^/priority-landscapes/[^/]+/file-resources$"),
    new RegExp("^/priority-landscapes/[^/]+/projects$"),
    new RegExp("^/priority-landscapes/[^/]+/projects/feature-collection$"),
    new RegExp("^/program-indices/[^/]+$"),
    new RegExp("^/programs/[^/]+$"),
    new RegExp("^/programs/[^/]+/block-list$"),
    new RegExp("^/programs/[^/]+/notifications$"),
    new RegExp("^/programs/[^/]+/projects$"),
    new RegExp("^/project-codes/[^/]+$"),
    new RegExp("^/project-documents/[^/]+$"),
    new RegExp("^/project-images/[^/]+$"),
    new RegExp("^/project-notes/[^/]+$"),
    new RegExp("^/project-types/[^/]+$"),
    new RegExp("^/project-types/[^/]+/projects$"),
    new RegExp("^/project-types/[^/]+/projects/mapped-point/feature-collection$"),
    new RegExp("^/projects/[^/]+$"),
    new RegExp("^/projects/[^/]+/classifications$"),
    new RegExp("^/projects/[^/]+/documents$"),
    new RegExp("^/projects/[^/]+/external-links$"),
    new RegExp("^/projects/[^/]+/fact-sheet$"),
    new RegExp("^/projects/[^/]+/images$"),
    new RegExp("^/projects/[^/]+/interaction-events$"),
    new RegExp("^/projects/[^/]+/invoice-payment-requests$"),
    new RegExp("^/projects/[^/]+/invoices$"),
    new RegExp("^/projects/[^/]+/locations/generic-layers$"),
    new RegExp("^/projects/[^/]+/map-popup$"),
    new RegExp("^/projects/[^/]+/map-popup-html$"),
    new RegExp("^/projects/[^/]+/notes$"),
    new RegExp("^/projects/[^/]+/notifications$"),
    new RegExp("^/projects/[^/]+/treatment-areas$"),
    new RegExp("^/projects/[^/]+/treatments$"),
    new RegExp("^/projects/[^/]+/update-history$"),
    new RegExp("^/projects/[^/]+/update-history/[^/]+/diff$"),
    new RegExp("^/search/projects/[^/]+$"),
    new RegExp("^/tags/[^/]+$"),
    new RegExp("^/tags/[^/]+/projects$"),
    new RegExp("^/taxonomy-branches/[^/]+$"),
    new RegExp("^/taxonomy-branches/[^/]+/projects$"),
    new RegExp("^/taxonomy-branches/[^/]+/projects/mapped-point/feature-collection$"),
    new RegExp("^/taxonomy-trunks/[^/]+$"),
    new RegExp("^/taxonomy-trunks/[^/]+/projects$"),
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
    new RegExp("^/agreements/[^/]+/contacts/[^/]+$"),
    new RegExp("^/classifications/[^/]+$"),
    new RegExp("^/custom-pages/[^/]+$"),
    new RegExp("^/dnr-upland-regions/[^/]+$"),
    new RegExp("^/external-map-layers/[^/]+$"),
    new RegExp("^/firma-home-page-images/[^/]+$"),
    new RegExp("^/focus-areas/[^/]+$"),
    new RegExp("^/focus-areas/[^/]+/location$"),
    new RegExp("^/fund-source-allocation-notes-internal/[^/]+$"),
    new RegExp("^/fund-source-allocation-notes/[^/]+$"),
    new RegExp("^/fund-source-allocations/[^/]+$"),
    new RegExp("^/fund-source-allocations/[^/]+/files/[^/]+$"),
    new RegExp("^/fund-sources/[^/]+$"),
    new RegExp("^/fund-sources/[^/]+/files/[^/]+$"),
    new RegExp("^/fund-sources/[^/]+/notes-internal/[^/]+$"),
    new RegExp("^/fund-sources/[^/]+/notes/[^/]+$"),
    new RegExp("^/interactions-events/[^/]+$"),
    new RegExp("^/interactions-events/[^/]+/file-resources/[^/]+$"),
    new RegExp("^/invoices/[^/]+/voucher$"),
    new RegExp("^/organization-types/[^/]+$"),
    new RegExp("^/organizations/[^/]+$"),
    new RegExp("^/organizations/[^/]+/boundary$"),
    new RegExp("^/organizations/[^/]+/logo$"),
    new RegExp("^/people/[^/]+$"),
    new RegExp("^/priority-landscapes/[^/]+$"),
    new RegExp("^/priority-landscapes/[^/]+/file-resources/[^/]+$"),
    new RegExp("^/programs/[^/]+$"),
    new RegExp("^/programs/[^/]+/block-list/[^/]+$"),
    new RegExp("^/programs/[^/]+/notifications/[^/]+$"),
    new RegExp("^/project-documents/[^/]+$"),
    new RegExp("^/project-images/[^/]+$"),
    new RegExp("^/project-internal-notes/[^/]+$"),
    new RegExp("^/project-notes/[^/]+$"),
    new RegExp("^/project-types/[^/]+$"),
    new RegExp("^/projects/[^/]+$"),
    new RegExp("^/projects/[^/]+/block-list$"),
    new RegExp("^/projects/[^/]+/update-workflow/current$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/documents-notes/documents/[^/]+$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/documents-notes/notes/[^/]+$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/photos/images/[^/]+$"),
    new RegExp("^/relationship-types/[^/]+$"),
    new RegExp("^/report-templates/[^/]+$"),
    new RegExp("^/tags/[^/]+$"),
    new RegExp("^/taxonomy-branches/[^/]+$"),
    new RegExp("^/taxonomy-trunks/[^/]+$"),
    new RegExp("^/treatments/[^/]+$"),
  ],
  'GET': [
    new RegExp("^/api/Job/[^/]+/freshness$"),
    new RegExp("^/cost-share/generate-pdf/[^/]+$"),
    new RegExp("^/custom-pages/[^/]+$"),
    new RegExp("^/dnr-upland-regions/[^/]+/expenditures-by-cost-type$"),
    new RegExp("^/dnr-upland-regions/[^/]+/focus-areas$"),
    new RegExp("^/external-map-layers/[^/]+$"),
    new RegExp("^/find-your-forester/work-units/[^/]+$"),
    new RegExp("^/focus-areas/[^/]+$"),
    new RegExp("^/focus-areas/[^/]+/location$"),
    new RegExp("^/focus-areas/[^/]+/location/staged-features$"),
    new RegExp("^/focus-areas/[^/]+/projects$"),
    new RegExp("^/focus-areas/[^/]+/projects/feature-collection$"),
    new RegExp("^/fund-source-allocation-notes-internal/[^/]+$"),
    new RegExp("^/fund-source-allocations/[^/]+/change-logs$"),
    new RegExp("^/fund-source-allocations/[^/]+/notes-internal$"),
    new RegExp("^/fund-sources/[^/]+/notes-internal$"),
    new RegExp("^/gis-bulk-import/attempts/[^/]+$"),
    new RegExp("^/gis-bulk-import/attempts/[^/]+/default-mappings$"),
    new RegExp("^/gis-bulk-import/attempts/[^/]+/features$"),
    new RegExp("^/gis-bulk-import/attempts/[^/]+/features-geojson$"),
    new RegExp("^/gis-bulk-import/attempts/[^/]+/metadata-attributes$"),
    new RegExp("^/invoice-payment-requests/[^/]+/invoices$"),
    new RegExp("^/organization-types/[^/]+$"),
    new RegExp("^/organizations/[^/]+/agreements/excel-download$"),
    new RegExp("^/organizations/[^/]+/boundary/staged-features$"),
    new RegExp("^/organizations/[^/]+/projects/pending$"),
    new RegExp("^/people/[^/]+$"),
    new RegExp("^/people/[^/]+/agreements$"),
    new RegExp("^/people/[^/]+/agreements/excel-download$"),
    new RegExp("^/people/[^/]+/api-key$"),
    new RegExp("^/people/[^/]+/interaction-events$"),
    new RegExp("^/people/[^/]+/notifications$"),
    new RegExp("^/people/[^/]+/projects$"),
    new RegExp("^/programs/[^/]+/delete-info$"),
    new RegExp("^/programs/[^/]+/projects/download-gdb$"),
    new RegExp("^/project-internal-notes/[^/]+$"),
    new RegExp("^/project-update-configurations/email-preview/[^/]+$"),
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
    new RegExp("^/projects/[^/]+/location-detailed$"),
    new RegExp("^/projects/[^/]+/location-simple$"),
    new RegExp("^/projects/[^/]+/map-extent$"),
    new RegExp("^/projects/[^/]+/priority-landscapes$"),
    new RegExp("^/projects/[^/]+/update-workflow/current$"),
    new RegExp("^/projects/[^/]+/update-workflow/diff$"),
    new RegExp("^/projects/[^/]+/update-workflow/history$"),
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
    new RegExp("^/relationship-types/[^/]+$"),
    new RegExp("^/report-templates/[^/]+$"),
    new RegExp("^/report-templates/by-model/[^/]+$"),
    new RegExp("^/roles/[^/]+$"),
    new RegExp("^/roles/[^/]+/people$"),
    new RegExp("^/user-claims/[^/]+$"),
    new RegExp("^/vendors/[^/]+$"),
    new RegExp("^/vendors/[^/]+/organizations$"),
    new RegExp("^/vendors/[^/]+/people$"),
  ],
  'POST': [
    new RegExp("^/agreements/[^/]+/contacts$"),
    new RegExp("^/api/Job/[^/]+/trigger$"),
    new RegExp("^/focus-areas/[^/]+/location/approve-gdb$"),
    new RegExp("^/focus-areas/[^/]+/location/upload-gdb$"),
    new RegExp("^/fund-source-allocations/[^/]+/duplicate$"),
    new RegExp("^/fund-source-allocations/[^/]+/files$"),
    new RegExp("^/fund-sources/[^/]+/files$"),
    new RegExp("^/fund-sources/[^/]+/notes$"),
    new RegExp("^/fund-sources/[^/]+/notes-internal$"),
    new RegExp("^/gis-bulk-import/attempts/[^/]+/import$"),
    new RegExp("^/gis-bulk-import/attempts/[^/]+/upload$"),
    new RegExp("^/impersonation/[^/]+$"),
    new RegExp("^/interactions-events/[^/]+/file-resources$"),
    new RegExp("^/invoices/[^/]+/voucher$"),
    new RegExp("^/loa-upload/import/[^/]+$"),
    new RegExp("^/organizations/[^/]+/boundary/approve-gdb$"),
    new RegExp("^/organizations/[^/]+/boundary/upload-gdb$"),
    new RegExp("^/organizations/[^/]+/logo$"),
    new RegExp("^/people/[^/]+/api-key$"),
    new RegExp("^/priority-landscapes/[^/]+/file-resources$"),
    new RegExp("^/programs/[^/]+/block-list$"),
    new RegExp("^/programs/[^/]+/notifications$"),
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
    new RegExp("^/projects/[^/]+/update-workflow/steps/documents-notes/documents$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/documents-notes/notes$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/location-detailed/approve-gdb$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/location-detailed/upload-gdb$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/photos/images$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/photos/images/[^/]+/set-key-photo$"),
    new RegExp("^/projects/[^/]+/update-workflow/submit$"),
    new RegExp("^/projects/[^/]+/update-workflow/treatments$"),
    new RegExp("^/report-templates/system/approval-letter/[^/]+$"),
    new RegExp("^/report-templates/system/invoice-payment-request/[^/]+$"),
  ],
  'PUT': [
    new RegExp("^/agreements/[^/]+$"),
    new RegExp("^/agreements/[^/]+/contacts/[^/]+$"),
    new RegExp("^/agreements/[^/]+/fund-source-allocations$"),
    new RegExp("^/agreements/[^/]+/projects$"),
    new RegExp("^/classifications/[^/]+$"),
    new RegExp("^/custom-pages/[^/]+$"),
    new RegExp("^/custom-pages/[^/]+/content$"),
    new RegExp("^/custom-rich-texts/[^/]+$"),
    new RegExp("^/dnr-upland-regions/[^/]+$"),
    new RegExp("^/external-map-layers/[^/]+$"),
    new RegExp("^/field-definitions/[^/]+$"),
    new RegExp("^/firma-home-page-images/[^/]+$"),
    new RegExp("^/focus-areas/[^/]+$"),
    new RegExp("^/fund-source-allocation-notes-internal/[^/]+$"),
    new RegExp("^/fund-source-allocation-notes/[^/]+$"),
    new RegExp("^/fund-source-allocations/[^/]+$"),
    new RegExp("^/fund-source-allocations/[^/]+/budget-line-items$"),
    new RegExp("^/fund-source-allocations/[^/]+/files/[^/]+$"),
    new RegExp("^/fund-source-allocations/[^/]+/program-index-project-codes$"),
    new RegExp("^/fund-sources/[^/]+$"),
    new RegExp("^/fund-sources/[^/]+/files/[^/]+$"),
    new RegExp("^/fund-sources/[^/]+/notes-internal/[^/]+$"),
    new RegExp("^/fund-sources/[^/]+/notes/[^/]+$"),
    new RegExp("^/interactions-events/[^/]+$"),
    new RegExp("^/interactions-events/[^/]+/file-resources/[^/]+$"),
    new RegExp("^/interactions-events/[^/]+/simple-location$"),
    new RegExp("^/invoices/[^/]+$"),
    new RegExp("^/organization-types/[^/]+$"),
    new RegExp("^/organizations/[^/]+$"),
    new RegExp("^/people/[^/]+$"),
    new RegExp("^/people/[^/]+/primary-contact-organizations$"),
    new RegExp("^/people/[^/]+/roles$"),
    new RegExp("^/people/[^/]+/stewardship-areas$"),
    new RegExp("^/people/[^/]+/toggle-active$"),
    new RegExp("^/priority-landscapes/[^/]+$"),
    new RegExp("^/priority-landscapes/[^/]+/file-resources/[^/]+$"),
    new RegExp("^/programs/[^/]+$"),
    new RegExp("^/programs/[^/]+/editors$"),
    new RegExp("^/programs/[^/]+/gis-import-config/basics$"),
    new RegExp("^/programs/[^/]+/gis-import-config/crosswalk-values$"),
    new RegExp("^/programs/[^/]+/gis-import-config/default-mappings$"),
    new RegExp("^/programs/[^/]+/notifications/[^/]+$"),
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
    new RegExp("^/projects/[^/]+/update-workflow/steps/documents-notes/documents/[^/]+$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/documents-notes/notes/[^/]+$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/expected-funding$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/external-links$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/location-detailed$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/location-simple$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/organizations$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/photos$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/photos/images/[^/]+$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/priority-landscapes$"),
    new RegExp("^/projects/[^/]+/update-workflow/steps/treatments$"),
    new RegExp("^/projects/[^/]+/update-workflow/treatments/[^/]+$"),
    new RegExp("^/relationship-types/[^/]+$"),
    new RegExp("^/report-templates/[^/]+$"),
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
