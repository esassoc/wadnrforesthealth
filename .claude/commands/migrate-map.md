# Migrate Map Skill

When the user invokes `/migrate-map <EntityName>`:

## Overview

This skill guides the migration of Leaflet maps from legacy MVC views to Angular using the `WADNRMapComponent` and associated layer components.

---

## 1. Analyze Legacy Map Implementation

First, examine the legacy MVC implementation:

- **Views**: Look for map containers in `Source/ProjectFirma.Web/Views/{Entity}/`
  - Search for `<div id="*Map*">` or similar map containers
  - Check for `leaflet` or map-related classes
- **JavaScript**: Search `Source/ProjectFirma.Web/Scripts/` for map initialization
  - Look for `L.map()`, `L.geoJSON()`, `L.tileLayer()` calls
  - Identify layer sources (WMS, GeoJSON, markers)
- **Controllers**: Check `Source/ProjectFirma.Web/Controllers/{Entity}Controller.cs`
  - Look for methods returning GeoJSON or map data
  - Identify boundary/location data retrieval patterns

### Questions to Answer:

1. What data is displayed on the map? (boundaries, points, polygons)
2. How is the data retrieved? (inline JSON, API calls, WMS services)
3. Are there multiple layers? What are they?
4. Does the map zoom to fit specific features?
5. Are there popups or tooltips on features?
6. What is the default tile layer? (Terrain, Street, Satellite)

---

## 2. Plan Backend API Endpoints

### GeoJSON Feature Collection Pattern

For entity boundaries or spatial data, create endpoints returning `FeatureCollection`:

```csharp
// In {Entity}Controller.cs
[HttpGet("{entityID}/boundary")]
public async Task<ActionResult<FeatureCollection>> GetBoundary([FromRoute] int entityID)
{
    var features = await Entities.GetBoundaryAsFeatureCollectionAsync(DbContext, entityID);
    return Ok(features);
}

[HttpGet("{entityID}/locations")]
public async Task<ActionResult<FeatureCollection>> GetLocations([FromRoute] int entityID)
{
    var features = await Entities.GetLocationsAsFeatureCollectionAsync(DbContext, entityID);
    return Ok(features);
}
```

### Static Helper Pattern

```csharp
// In {Entity}.StaticHelpers.cs
public static async Task<FeatureCollection> GetBoundaryAsFeatureCollectionAsync(
    WADNRDbContext dbContext, int entityID)
{
    var entity = await dbContext.Entities
        .AsNoTracking()
        .Where(x => x.EntityID == entityID && x.EntityBoundary != null)
        .Select(x => new { x.EntityID, x.EntityBoundary })
        .SingleOrDefaultAsync();

    if (entity?.EntityBoundary == null)
        return new FeatureCollection();

    var feature = new Feature(entity.EntityBoundary, new AttributesTable
    {
        { "EntityID", entity.EntityID }
    });

    return new FeatureCollection(new[] { feature });
}
```

---

## 3. Update Detail DTO for Map Flags

Add boolean flags to the detail DTO to indicate if map data exists:

```csharp
// In {Entity}DetailDto.cs
public bool HasBoundary { get; set; }
public bool HasLocations { get; set; }
public BoundingBoxDto? BoundingBox { get; set; }

// In {Entity}Projections.cs (AsDetail)
HasBoundary = x.EntityBoundary != null,
HasLocations = x.EntityLocations.Any(),
BoundingBox = x.EntityBoundary != null
    ? BoundingBoxDto.FromGeometry(x.EntityBoundary)
    : null,
```

---

## 4. Angular Component Integration

### Available Map Components

| Component | Selector | Purpose |
|-----------|----------|---------|
| `WADNRMapComponent` | `wadnr-map` | Base map container |
| `GenericFeatureCollectionLayerComponent` | `generic-feature-collection-layer` | Custom GeoJSON layers |
| `CountiesLayerComponent` | `counties-layer` | County boundaries (WMS) |
| `PriorityLandscapesLayerComponent` | `priority-landscapes-layer` | Priority landscapes (WMS) |
| `DnrUplandRegionsLayerComponent` | `dnr-upland-regions-layer` | DNR regions (WMS) |
| `ProjectLocationsLayerComponent` | `project-locations-layer` | Project location markers |
| `ProjectLocationsSimpleLayerComponent` | `project-locations-simple-layer` | Simplified project markers |

### Component TypeScript Pattern

```typescript
import { Component } from "@angular/core";
import { Observable } from "rxjs";
import { Map } from "leaflet";
import { WADNRMapComponent, WADNRMapInitEvent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { GenericFeatureCollectionLayerComponent } from "src/app/shared/components/leaflet/layers/generic-feature-collection-layer/generic-feature-collection-layer.component";
import { IFeature } from "src/app/shared/generated/model/i-feature";

@Component({
    // ...
    imports: [
        WADNRMapComponent,
        GenericFeatureCollectionLayerComponent,
        // other layer components as needed
    ],
})
export class EntityDetailComponent {
    public map: Map;
    public layerControl: L.Control.Layers;
    public mapIsReady: boolean = false;

    public boundaryFeatures$: Observable<IFeature[]>;
    public locationFeatures$: Observable<IFeature[]>;

    ngOnInit(): void {
        // Load boundary features
        this.boundaryFeatures$ = this.entityID$.pipe(
            switchMap(id => this.entityService.getBoundary(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        // Load location features
        this.locationFeatures$ = this.entityID$.pipe(
            switchMap(id => this.entityService.getLocations(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }

    handleMapReady(event: WADNRMapInitEvent): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
    }
}
```

### Template Pattern

```html
<!-- Conditional map display -->
@if (entity.HasBoundary || entity.HasLocations) {
<div class="card">
    <div class="card-header"><span class="card-title">Map</span></div>
    <div class="card-body">
        <wadnr-map
            [mapHeight]="'400px'"
            [selectedTileLayer]="'Terrain'"
            [boundingBox]="entity.BoundingBox"
            [showLegend]="true"
            (onMapLoad)="handleMapReady($event)">

            <!-- Custom boundary layer -->
            @if (mapIsReady && (boundaryFeatures$ | async)) {
            <generic-feature-collection-layer
                [map]="map"
                [layerControl]="layerControl"
                [featureCollection]="boundaryFeatures$ | async"
                [layerName]="'Boundary'"
                [layerColor]="'#FF6600'"
                [displayOnLoad]="true">
            </generic-feature-collection-layer>
            }

            <!-- Custom location layer -->
            @if (mapIsReady && (locationFeatures$ | async)) {
            <generic-feature-collection-layer
                [map]="map"
                [layerControl]="layerControl"
                [featureCollection]="locationFeatures$ | async"
                [layerName]="'Locations'"
                [layerColor]="'#3388ff'"
                [displayOnLoad]="true">
            </generic-feature-collection-layer>
            }

        </wadnr-map>
    </div>
</div>
}
```

---

## 5. Using Pre-built WMS/WFS Layers

For existing reference layers, use the specialized components:

```html
<!-- Counties layer with highlight -->
<counties-layer
    [map]="map"
    [layerControl]="layerControl"
    [selectedIDs]="[entity.CountyID]"
    [mode]="highlightedLayerMode">
</counties-layer>

<!-- Reference counties layer (all counties) -->
<counties-layer
    [map]="map"
    [layerControl]="layerControl"
    [mode]="referenceLayerMode"
    [fitBoundsOnWmsAddToControl]="false">
</counties-layer>
```

### OverlayMode Enum Values

```typescript
import { OverlayMode } from "src/app/shared/components/leaflet/layers/generic-wms-wfs-layer/overlay-mode.enum";

public highlightedLayerMode = OverlayMode.Single;      // Single highlighted feature
public referenceLayerMode = OverlayMode.ReferenceOnly; // Background reference
public allFeaturesMode = OverlayMode.All;              // All features displayed
```

---

## 6. WADNRMapComponent Configuration Reference

### Input Properties

| Input | Type | Default | Description |
|-------|------|---------|-------------|
| `mapHeight` | string | `'500px'` | CSS height of map container |
| `selectedTileLayer` | string | `'Terrain'` | Default base layer (`'Terrain'`, `'Street'`, `'Satellite'`) |
| `boundingBox` | `BoundingBoxDto` | WA state | Initial bounds |
| `showLegend` | boolean | `false` | Show legend control |
| `legendPosition` | `ControlPosition` | `'topleft'` | Legend position |
| `disableMapInteraction` | boolean | `false` | Lock map (no pan/zoom) |
| `collapseLayerControlOnLoad` | boolean | `false` | Start with layers control collapsed |

### Output Events

| Output | Type | Description |
|--------|------|-------------|
| `onMapLoad` | `WADNRMapInitEvent` | Fired when map is ready; provides `map` and `layerControl` |
| `onOverlayToggle` | `LayersControlEvent` | Fired when overlay is toggled |
| `onLegendControlReady` | `Control` | Fired when legend control is ready |

---

## 7. Map Layout Patterns

### Side-by-side with details (6-6 grid)

```html
<div class="grid-12">
    <div class="g-col-6">
        <!-- Map card -->
    </div>
    <div class="g-col-6">
        <!-- Details card -->
    </div>
</div>
```

### Full-width map above grid

```html
<div class="grid-12">
    <div class="g-col-12">
        <!-- Map card -->
    </div>
    <div class="g-col-12">
        <!-- Grid card -->
    </div>
</div>
```

### Map with "No Location" fallback

```html
@if (entity.HasBoundary || entity.HasLocations) {
<div class="card">
    <div class="card-header"><span class="card-title">Location</span></div>
    <div class="card-body">
        <wadnr-map ...>
            <!-- layers -->
        </wadnr-map>
    </div>
</div>
} @else {
<div class="card">
    <div class="card-header"><span class="card-title">Location</span></div>
    <div class="card-body">
        <p class="text-muted">No location data available for this entity.</p>
    </div>
</div>
}
```

---

## 8. Migration Checklist

- [ ] Identified all map layers from legacy implementation
- [ ] Created API endpoints for custom GeoJSON data (if needed)
- [ ] Added `HasBoundary`/`HasLocations` flags to detail DTO
- [ ] Added `BoundingBox` to detail DTO (if applicable)
- [ ] Added `WADNRMapComponent` to component imports
- [ ] Added appropriate layer components to imports
- [ ] Implemented `handleMapReady` method
- [ ] Set up feature observables for custom layers
- [ ] Added map template with conditional rendering
- [ ] Verified map displays correctly
- [ ] Verified layers toggle correctly
- [ ] Verified zoom/bounds behavior
- [ ] Ran `npm run gen-model` after API changes

---

## Common Issues and Solutions

### Map doesn't display
- Ensure `handleMapReady` is called and sets `mapIsReady = true`
- Layer components need `@if (mapIsReady)` guard
- Check that `map` and `layerControl` are passed to layer components

### Features don't appear
- Check browser console for GeoJSON parsing errors
- Verify API endpoint returns valid GeoJSON structure
- Ensure `featureCollection` observable is subscribed (use `| async`)

### Map is wrong size
- Set explicit `mapHeight` input
- Ensure parent container has defined height

### Wrong initial bounds
- Pass `boundingBox` input to WADNRMapComponent
- Or let layer component handle bounds with `fitBoundsOnWmsAddToControl`
