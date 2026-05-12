# Architecture Check

## Purpose

This file prevents drift while Ubuntu Sentinel evolves from a fast capstone build into a production-shaped PeaceTech platform. It is current to the demo implementation and forward-looking toward the architecture we want after the showcase.

## Current Structure

```text
UbuntuSentinel.slnx
src/
  Api/US.Api
  Application/US.Application
  Domain/US.Domain
  Infrastructure/US.Infrastructure
  Persistence/US.Persistence
  SharedKernel/US.SharedKernel
  Web/US.Web
    US.Web
    US.Web.Client
docs/
```

The current structure is clean-architecture inspired but still contains demo bridges, especially in API feature code and in-memory stores. Do not deepen those bridges unless the demo depends on it.

## Target Direction

```text
src/
  Backend/
    Api/US.Api
    Application/US.Application
    Domain/US.Domain
    Infrastructure/US.Infrastructure
    Persistence/US.Persistence
  Frontend/
    Web/US.Web
    Web/US.Web.Client
    Mobile/US.Mobile        # optional simulator
  Shared/
    US.SharedKernel
tests/
  US.Tests.Architecture
  US.Tests.Unit
  US.Tests.Integration
docs/
```

## Dependency Rules

```text
Domain -> SharedKernel only
Application -> Domain, SharedKernel
Persistence -> Application, Domain, SharedKernel
Infrastructure -> Application, Domain, SharedKernel
Api -> Application, Infrastructure, Persistence, SharedKernel
Web.Client -> SharedKernel only, plus HTTP/SignalR calls to Api
Web Host -> Web.Client, SharedKernel
Mobile -> SharedKernel only, plus HTTP calls to Api
```

No UI project may reference API internals.

## Current Demo Bridges

- `US.Api` still owns some feature endpoints, in-memory report storage, USSD flow, policy comparison, and deterministic pipeline code.
- This is acceptable for the demo while the behavior remains visible, testable, and buildable.
- After demo, report lifecycle, validation workflow, policy comparison, notification dispatch, and brief generation should move behind application/infrastructure contracts.

## Domain Ownership

`US.Domain` should own:

- community report lifecycle rules,
- intake channel vocabulary,
- validation decision vocabulary,
- conflict/resilience zone model,
- policy document vocabulary,
- agent pipeline stage vocabulary,
- role vocabulary.

## Application Ownership

`US.Application` should own:

- report use cases,
- validation use cases,
- issue inference contracts,
- agent pipeline contracts,
- policy comparison contracts,
- PDF/brief generation contracts,
- role-shaped query contracts,
- validator notification use cases.

## Persistence Ownership

`US.Persistence` should own:

- EF Core contexts and entities,
- PostgreSQL and pgvector-ready mappings,
- report/event persistence,
- region context persistence,
- policy corpus metadata persistence,
- demo persistence fallbacks.

## Infrastructure Ownership

`US.Infrastructure` should own:

- OpenAI/Microsoft.Extensions.AI adapters,
- Africa's Talking adapters,
- QuestPDF/HTML PDF generation,
- Redis/MassTransit adapters,
- email/SMS notification gateways,
- deterministic fallback implementations for demo reliability.

## API Ownership

`US.Api` owns:

- endpoint mapping,
- SignalR hubs,
- dependency injection composition,
- authentication/authorization composition for demo roles,
- HTTP boundary contracts.

## Frontend Ownership

Frontend owns:

- MudBlazor UI,
- global language selection,
- offline browser queue,
- USSD simulator page/modal,
- report submission and listing,
- read-only view vs explicit validation actions,
- pipeline visualization,
- map visualization,
- localized labels.

## Demo-Day Architecture Decisions

- PostgreSQL/pgvector is the target persistence/RAG design.
- The demo uses a versioned JSON public clause corpus with deterministic matching and source URLs.
- OpenAI-backed pipeline exists behind configuration; deterministic fallback is required and clearly labeled.
- USSD must be demoable even if Africa's Talking is not connected.
- AI output must always be marked draft/internal until validation.
- Rejected and follow-up reports must generate status-aware outputs.
- Conflict zones and resilience zones are both first-class.
- Role-gated views can be demo-role selectors before production IAM.

## Phase 2 Only

- Marten oral histories.
- Full women peacebuilder registry.
- Satellite imagery correlation.
- Production IAM.
- Full consent lifecycle UI.
- Real short-code USSD deployment.

## Pre-Commit Check

Before adding code, ask:

1. Does it serve the revised demo spine?
2. Does it belong in the correct layer or is it an intentional demo bridge?
3. Does it preserve community validation and data sovereignty?
4. Does it have a demo fallback if an external service is unavailable?
5. Does it keep the solution buildable?
