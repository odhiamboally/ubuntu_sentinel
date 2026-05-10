# Plan Execution Strategy

## Strategy

Build the real product spine, but keep every slice runnable. We are borrowing BaseTemplate's structure and discipline while avoiding a rewrite that leaves the demo broken.

## Guiding Principle

Every implemented slice must improve the judge-facing story:

Community intake -> agentic intelligence -> community validation -> policy gap -> accountable brief -> conflict/resilience visibility.

## Architecture First, Then Feature Migration

1. Establish clean project boundaries.
2. Keep current behavior working while moving logic into the right layer.
3. Add adapters for real integrations with deterministic demo fallbacks.
4. Promote only the pieces that serve the demo-day build list.

## Layer Rules

- `US.Domain`: entities, value objects, enums, core rules.
- `US.Application`: use cases, interfaces, DTO mapping, pipeline contracts.
- `US.Persistence`: EF Core/PostgreSQL/pgvector-ready data access and seeded demo stores.
- `US.Infrastructure`: external services such as OpenAI, Africa's Talking, PDF, cache, messaging.
- `US.Api`: endpoints, SignalR hubs, composition root.
- `US.Web` / `US.Web.Client`: Blazor host and interactive UI.
- `US.Mobile`: optional MAUI simulator that consumes the same API.
- `US.SharedKernel`: stable contracts and primitives shared across boundaries.

## Build Order

1. Reports and case lifecycle.
2. Intake channels: web, offline, USSD simulator.
3. Policy/RAG comparison.
4. Agent pipeline.
5. Validation and roles.
6. Map and resilience zones.
7. Accountability brief and PDF.
8. Demo polish.

## External Dependency Rule

Use real package boundaries where useful, but every external dependency must have a demo-safe fallback:

- OpenAI unavailable -> deterministic agent outputs.
- PostgreSQL/pgvector unavailable -> seeded in-memory comparison behind the same interface.
- Africa's Talking unavailable -> USSD simulator and webhook-compatible endpoint.
- QuestPDF blocked -> PDF-ready markdown/HTML export until package is stable.

## Human Input

The human defines OSF framing, ethical constraints, demo priorities, and final acceptance.

## Codex Contribution

Codex implements structure, features, refactors, docs, bug fixes, and demo verification while keeping human validation and community data sovereignty explicit.

## Verification Loop

After each meaningful slice:

1. Stop running API/Web processes.
2. Build the solution.
3. Run targeted tests if present.
4. Start API/Web.
5. Verify the browser demo path when UI changed.
6. Update docs only when behavior or architecture changed.
