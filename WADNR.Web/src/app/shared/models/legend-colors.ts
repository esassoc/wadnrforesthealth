export type Palette = { [id: string]: string };
export type LegendColors = { [propertyName: string]: Palette } | Palette;

export const PROJECT_STAGE_PALETTE: Palette = Object.freeze({
    "2": "#80B2FF",
    "3": "#1975FF",
    "4": "#000066",
    "5": "#D6D6D6",
});

export const PROJECT_STAGE_LEGEND_COLORS: Record<string, Palette> = Object.freeze({
    ProjectStageID: PROJECT_STAGE_PALETTE,
});
