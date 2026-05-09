# Ubuntu Sentinel MVP Plan

## Goal

Build a working 3-day proof-of-concept that demonstrates community reporting, localization, offline sync, realtime dashboard updates, AI-assisted accountability analysis, and human validation.

## Day 1 - Foundation

- Create solution structure.
- Add API, Blazor Web, and Shared projects.
- Define shared contracts and enums.
- Implement report submission endpoint.
- Implement frontend report form.
- Add region profile seed data.
- Add localization foundation and initial language resources.
- Confirm the solution builds.

## Day 2 - Offline, Sync, Validation

- Add browser-side offline queue.
- Add sync service and visible pending count.
- Add SignalR hub for report updates.
- Add dashboard report list/map-ready view.
- Add validator workflow: approve, reject, needs follow-up.
- Add seeded/demo persistence if full database setup slows the build.
- Manually verify the offline-to-online demo path.

## Day 3 - AI Pipeline, Demo Polish, Docs

- Implement AI pipeline contracts:
  - Validation Agent
  - Document Intelligence Agent
  - Reparative Justice Calculator
  - Advocacy Drafter
- Add OpenAI-backed implementation with deterministic fallback.
- Add accountability brief view/export-ready markdown.
- Polish UI for judge-facing flow.
- Add README usage, demo script, and Codex usage explanation.
- Run final build and browser demo pass.

## Demo Scenario

Aisha, a youth leader in Eastern DRC, reports that a mining company promised a clean water well by March 2024 under a community agreement, but the site remains dry. Ubuntu Sentinel structures the report, detects the accountability gap, estimates reparative action, and generates a brief for validation before escalation.

## Success Bar

The app does not need to be production-ready. It must be coherent, working, and believable as a foundation worth pursuing.
