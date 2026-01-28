# Migration Status Report

Generated: 2026-01-28

## Summary
- **Total Legacy Domain Controllers**: 52 (excluding infrastructure: Account, Home, Help, etc.)
- **Migrated**: 26 (API + Angular with routes)
- **Partial**: 5 (API exists but Angular incomplete or vice versa)
- **Not Started**: 21

## Detailed Status

| Entity | Legacy Controller | Views | API Controller | Angular Page | Status | Priority |
|--------|------------------|-------|----------------|--------------|--------|----------|
| Agreement | Yes | 6 | Yes | Yes | **Migrated** | - |
| Classification | Yes | 3 | Yes | Yes | **Migrated** | - |
| ClassificationSystem | Yes | 1 | Yes | Yes | **Migrated** | - |
| County | Yes | 3 | Yes | Yes | **Migrated** | - |
| CustomPage | Yes | 4 | Yes | No* | Partial | Medium |
| DNRUplandRegion | Yes | 5 | Yes | Yes | **Migrated** | - |
| ExcelUpload | Yes | 2 | No | No | Not Started | Low |
| FieldDefinition | Yes | 2 | Yes | Yes | **Migrated** | - |
| FileResource | Yes | 0 | Yes | N/A | **Migrated** | - |
| FindYourForester | Yes | 3 | No | No | Not Started | Medium |
| FirmaHomePageImage | Yes | 2 | No | No | Not Started | Low |
| FirmaPage | Yes | 3 | No | No | Not Started | Low |
| FocusArea | Yes | 7 | Yes | Yes | **Migrated** | - |
| FundSource | Yes | 7 | Yes | Yes | **Migrated** | - |
| FundSourceAllocation | Yes | 6 | Yes | Yes | **Migrated** | - |
| GisFeature | Yes | 1 | No | No | Not Started | Low |
| GisProjectBulkUpdate | Yes | 6 | No | No | Not Started | Medium |
| InteractionEvent | Yes | 5 | Yes | Yes | **Migrated** | - |
| Invoice | Yes | 3 | Yes | Yes | **Migrated** | - |
| Job | Yes | 1 | No | No | Not Started | Low |
| MapLayer | Yes | 2 | No | No | Not Started | Medium |
| Organization | Yes | 7 | Yes | Yes | **Migrated** | - |
| OrganizationType | No | 3 | Yes | No | Partial | Low |
| PersonOrganization | Yes | 1 | No | No | Not Started | Low |
| PriorityLandscape | Yes | 5 | Yes | Yes | **Migrated** | - |
| Program | Yes | 8 | Yes | Yes | **Migrated** | - |
| ProgramIndex | Yes | 1 | Yes | Yes | **Migrated** | - |
| Project | Yes | 10 | Yes | Yes | **Migrated** | - |
| ProjectClassification | Yes | 0 | No | No | Not Started | Low |
| ProjectCode | Yes | 1 | Yes | Yes | **Migrated** | - |
| ProjectCounty | Yes | 0 | No | No | Not Started | Low |
| ProjectCreate | Yes | 15 | No | No | Not Started | **High** |
| ProjectDocument | Yes | 0 | Yes | No | Partial | Medium |
| ProjectExternalLink | Yes | 0 | No | No | Not Started | Low |
| ProjectFunding | Yes | 0 | No | No | Not Started | Medium |
| ProjectImage | Yes | 2 | No | No | Not Started | Medium |
| ProjectInternalNote | Yes | 0 | No | No | Not Started | Low |
| ProjectLocation | Yes | 0 | No | No | Not Started | Medium |
| ProjectNote | Yes | 0 | No | No | Not Started | Low |
| ProjectOrganization | Yes | 0 | No | No | Not Started | Low |
| ProjectPerson | Yes | 0 | No | No | Not Started | Low |
| ProjectPriorityLandscape | Yes | 1 | No | No | Not Started | Low |
| ProjectRegion | Yes | 1 | No | No | Not Started | Low |
| ProjectType | Yes | 3 | Yes | Yes | **Migrated** | - |
| ProjectUpdate | Yes | 23 | No | No | Not Started | **High** |
| Reports | Yes | 4 | No | No | Not Started | Medium |
| Role | Yes | 2 | Yes | Yes | **Migrated** | - |
| Tag | Yes | 4 | Yes | Yes | **Migrated** | - |
| TaxonomyBranch | Yes | 3 | Yes | Yes | **Migrated** | - |
| TaxonomyTrunk | Yes | 3 | Yes | Yes | **Migrated** | - |
| Treatment | Yes | 2 | Yes | Yes | **Migrated** | - |
| TreatmentUpdate | Yes | 1 | No | No | Not Started | Medium |
| User (Person) | Yes | 4 | Yes | Yes | **Migrated** | - |
| Vendor | Yes | 3 | Yes | Yes | **Migrated** | - |
| WebServices | Yes | 3 | No | No | Not Started | Low |

*CustomPage has API but uses catch-all route in Angular

## Migration Progress

```
Migrated:       26 entities (50%)
Partial:         5 entities (10%)
Not Started:    21 entities (40%)
███████████████████████░░░░░░░░░░░░░░░░░░ 60%
```

## Recommendations

### Quick Wins (Low Complexity, High Value)
1. **MapLayer** - 2 views, admin feature for map configuration
2. **TreatmentUpdate** - 1 view, extends existing Treatment migration
3. **FirmaPage** - 3 views, CMS content pages (similar to CustomPage)

### Dependencies to Address First
1. **ProjectCreate** (15 views, 24 actions) should be migrated before ProjectUpdate
2. **ProjectLocation** is needed for full Project edit workflow
3. **PersonOrganization** is needed for complete Person/Organization management
4. **GisProjectBulkUpdate** depends on GIS feature infrastructure

### Complex Migrations (Plan Carefully)
1. **ProjectUpdate** - 23 views, 50 action methods - most complex entity, handles annual updates
2. **ProjectCreate** - 15 views, 24 actions - wizard-based project creation workflow
3. **Program** - 8 views, 15 actions - has import/crosswalk features beyond basic CRUD
4. **GisProjectBulkUpdate** - 6 views - requires GIS file upload and processing infrastructure
5. **FindYourForester** - public-facing forester lookup with map integration

### New Only (No Legacy Equivalent)
- **SystemInfo** - API health/info endpoint (new infrastructure)
- **UserClaims** - Authentication claims endpoint (new auth system)
- **SitkaCaptureController** - Screenshot service integration
