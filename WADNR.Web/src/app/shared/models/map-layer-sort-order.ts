/**
 * Canonical ordering for the overlay layers in the grouped map layer control, shared across the
 * detail-page maps (Priority Landscape, Fund Source, County, DNR Upland Region).
 *
 * The grouped layer control sorts overlays ascending by `sortOrder`. External map layers keep the
 * control's default of 1, so they always render above these. A page only sets the values for the
 * layers it actually shows; the relative order is preserved regardless of the subset present.
 */
export const MAP_LAYER_SORT_ORDER = {
    ProjectLocations: 10,
    PriorityLandscape: 11,
    /** Second Priority Landscape layer, used where the layer is split into East/West (Priority Landscape detail). */
    PriorityLandscapeWest: 12,
    DNRUplandRegion: 13,
    County: 14,
    ProjectLocationsDetail: 15,
    ProjectTreatmentArea: 16,
} as const;
