---
name: plan-story
description: Fetch a Jira card by key (e.g. WADNR-100) and produce a detailed implementation plan. Use this when starting work on a new story.
allowed-tools: [mcp__atlassian__getAccessibleAtlassianResources, mcp__atlassian__getJiraIssue, Read, Glob, Grep, Task, EnterPlanMode]
---

Given a Jira issue key passed as an argument (e.g. `WADNR-100`), fetch the story details and produce a full implementation plan.

## Steps

1. **Fetch the Jira issue.** Call `getAccessibleAtlassianResources` to obtain the cloud ID, then call `getJiraIssue` with the issue key provided as the argument. Extract the summary, description, status, and any linked issues.

2. **Summarize the requirements.** Parse the Jira description into a clear list of requirements, acceptance criteria, assumptions, and out-of-scope items. Present this summary to the user so they can confirm understanding before planning.

3. **Explore the codebase.** Based on the requirements, use the Explore agent (Task tool with subagent_type=Explore) to investigate:
   - Existing entities, DTOs, controllers, and EF business-logic classes related to the feature
   - Similar features already implemented (e.g. if the story is about a new entity, look at how Projects or Agreements were built end-to-end)
   - Database tables, lookup tables, and seed data relevant to the domain
   - Frontend components, routes, and services for the related area
   - Any generated files that will need regeneration after backend changes

4. **Enter plan mode.** Call `EnterPlanMode` and write a detailed, step-by-step implementation plan covering:

   ### Plan structure
   - **Database layer** — new tables, columns, lookup tables, seed data, foreign keys, views (including `vGeoServer*` views if spatial) in `WADNR.Database/`
   - **EF Models layer** — new entities (will be scaffolded), static helpers, projections in `WADNR.EFModels/Entities/`
   - **DTO layer** — new DTOs in `WADNR.Models/DataTransferObjects/`
   - **API layer** — new controllers extending `SitkaController<T>`, routes, authorization attributes (`[NormalUserFeature]`, `[AdminFeature]`, `[ProjectEditFeature]`, etc.)
   - **Tests** — MSTest for API in `WADNR.API.Tests`, Jasmine for Angular components
   - **Code generation** — build API to regenerate `swagger.json`, run `npm run gen-model` in WADNR.Web
   - **Frontend layer** — new standalone components in `WADNR.Web/src/app/pages/`, lazy-loaded routes in `app.routes.ts`, `<wadnr-grid>` grids, reactive forms, modals with `@ngneat/dialog`

   For each step, reference specific existing files as templates to follow (e.g. "Model after `ProjectController.cs`") and call out the exact file paths where new code should go.

5. **Present the plan** for user approval via `ExitPlanMode`.

## Notes

- The cloud ID for the Jira instance is obtained dynamically via `getAccessibleAtlassianResources`.
- Always follow existing patterns in the codebase — find the closest analogous feature and replicate its structure.
- The plan should be detailed enough that each step can be executed independently.
- Reference the CLAUDE.md architecture patterns (authorization attributes, controller/static helper/projection pattern, route guards, Observable + async pipe, standalone components, BEM SCSS, etc.).
- Do NOT start writing code — this skill only produces a plan.
