# Architecture Check

## Purpose

This file keeps Ubuntu Sentinel from drifting into either under-structured demo code or over-engineered enterprise code. Use it before adding new folders, dependencies, services, or cross-cutting patterns.

## Intended MVP Structure

```text
UbuntuSentinel.sln
src/
  UbuntuSentinel.Api/
    Features/
    Infrastructure/
    Program.cs
  UbuntuSentinel.Web/
    Pages/
    Components/
    Services/
    wwwroot/
  UbuntuSentinel.Shared/
    Contracts/
    Enums/
    Localization/
docs/
```

## Project Responsibilities

### UbuntuSentinel.Shared

Owns contracts that both API and Web need:

- Request/response DTOs.
- Public enums.
- Region/localization models.
- Validation status and report status types.

Shared must not depend on API, Web, persistence, OpenAI, SignalR hubs, or browser APIs.

### UbuntuSentinel.Api

Owns server behavior:

- Report submission and retrieval.
- AI-assisted accountability pipeline.
- Validation workflow.
- Region profile loading.
- Realtime notifications through SignalR.
- Persistence abstraction and implementation.

API may depend on Shared. API must not depend on Web.

### UbuntuSentinel.Web

Owns user experience:

- Report submission UI.
- Offline queue and sync UI.
- Region/language selection.
- Dashboard.
- Validation screen.
- Accountability brief display.

Web may depend on Shared. Web should call API through typed services, not duplicate server logic.

## Feature Folder Rule

Prefer feature folders for MVP velocity:

```text
Features/
  Reports/
  Validation/
  Accountability/
  Dashboard/
  Regions/
```

Each feature may contain its endpoint handlers/controllers, services, and small internal models. Move code to shared infrastructure only when at least two features genuinely need it.

## Dependency Rules

- `Shared` has no project dependencies.
- `Api` references `Shared`.
- `Web` references `Shared`.
- `Web` talks to `Api` over HTTP/SignalR, not by referencing API internals.
- AI provider code stays behind an interface.
- Persistence stays behind a repository/service boundary.
- Browser storage code stays in Web.

## MVP Architecture Decisions

- Use a modular monolith.
- Use Blazor for the judge-facing app.
- Use SignalR for realtime dashboard updates.
- Use browser storage for offline reports.
- Use seeded/demo fallback for AI output when no OpenAI key is present.
- Use simple persistence first; PostgreSQL can be added behind the same boundary if time allows.
- Keep localization as resource/data-driven UI text and region profiles.

## Architecture Drift Checks

Before adding a new file or dependency, ask:

1. Does this support the demo path in `CAPSTONE_SPEC.md`?
2. Does this belong in API, Web, or Shared?
3. Is this duplicating logic already owned elsewhere?
4. Is this adding production complexity that the 3-day MVP does not need?
5. Can this be represented as seeded data, a small service, or a feature-local helper?

## Avoid For MVP

- Separate Domain/Application/Infrastructure projects unless the codebase grows enough to justify them.
- Full CQRS/MediatR pipeline.
- Full identity provider integration.
- Real WhatsApp/USSD integrations.
- Full vector database/RAG stack.
- Premature plugin/MCP server implementation.
- Deep generic repository layers.

## Required Before Final Demo

- App builds.
- Submit report flow works.
- Offline queue behavior is demonstrable.
- Region/language selection is visible.
- SignalR dashboard update is demonstrable.
- AI pipeline produces visible structured output or deterministic fallback.
- Human validation state is visible.
- README explains architecture and Codex usage.
