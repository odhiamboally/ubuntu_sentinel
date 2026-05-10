# Architecture Check

## Purpose

This file prevents drift while Ubuntu Sentinel pivots from a reporting MVP into a BaseTemplate-inspired, prize-ready product demo.

## Target Structure

```text
UbuntuSentinel.slnx
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
    Mobile/US.Mobile        # optional MAUI simulator
  Shared/
    US.SharedKernel
tests/
  US.Tests.Architecture
  US.Tests.Unit
  US.Tests.Integration
docs/
```

The current repository is being migrated toward this structure. Do not add new feature work that deepens the old flat structure unless it is a temporary bridge.

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

## Domain Ownership

`US.Domain` owns:

- community report lifecycle rules,
- intake channel vocabulary,
- conflict/resilience zone model,
- policy document vocabulary,
- agent pipeline stage vocabulary,
- role vocabulary.

## Application Ownership

`US.Application` owns:

- report use cases,
- validation use cases,
- agent pipeline contracts,
- policy comparison contracts,
- PDF/brief generation contracts,
- role-shaped query contracts.

## Persistence Ownership

`US.Persistence` owns:

- EF Core contexts and entities,
- PostgreSQL and pgvector-ready mappings,
- seed data repositories,
- demo persistence fallbacks.

## Infrastructure Ownership

`US.Infrastructure` owns:

- OpenAI/Microsoft.Extensions.AI adapters,
- Africa's Talking adapters,
- QuestPDF generation,
- Redis/MassTransit adapters,
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
- offline browser queue,
- USSD simulator page,
- pipeline visualization,
- map visualization,
- role-shaped navigation,
- localized labels.

## Demo-Day Architecture Decisions

- PostgreSQL/pgvector is the target persistence/RAG design.
- A deterministic seeded comparison fallback is allowed for demo reliability.
- USSD must be demoable even if Africa's Talking is not connected.
- AI output must always be marked draft until community validation.
- Conflict zones and resilience zones are both first-class.
- Role-gated views can be demo-role selectors before production IAM.

## Phase 2 Only

Do not build these in the 48-hour demo window:

- Marten oral histories.
- Full women peacebuilder registry.
- Satellite imagery correlation.
- Production IAM.
- Full consent lifecycle UI.

## Pre-Commit Check

Before adding code, ask:

1. Does it serve the revised demo spine?
2. Does it belong in the correct layer?
3. Does it preserve community validation and data sovereignty?
4. Does it have a demo fallback if an external service is unavailable?
5. Does it keep the solution buildable?
