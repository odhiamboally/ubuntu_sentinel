# Ubuntu Sentinel

Ubuntu Sentinel is community intelligence infrastructure for transformative peace. It helps conflict-affected communities submit evidence through low-bandwidth channels, validate it locally, compare it against public commitments, and generate institution-ready accountability briefs.

## Positioning

- Primary track: Voice & Accountability.
- Secondary track: Peace & Community.
- Resource justice thread: CDA, mining, public service, and peace accord promise tracking.

Ubuntu Sentinel is not a dashboard for OSF staff to read community reports. It is a system where communities retain voice, validation power, and narrative control while producing evidence that institutions can act on.

## Demo-Day Target

- Web/offline report intake.
- USSD-style intake simulator.
- Seeded policy/RAG promise comparison.
- Four-agent Codex pipeline.
- Human/community validation gate.
- Dual conflict/resilience map.
- Role-shaped views: Reporter, Community Validator, Advocate, OSF Partner.
- Accountability brief with PDF-ready output.
- EN/FR localization.

## Current State

Implemented foundations:

- .NET 10 solution.
- ASP.NET Core API.
- Blazor Web App with Interactive WebAssembly.
- MudBlazor UI foundation.
- Web report submission.
- Browser offline queue foundation.
- SignalR updates.
- Human validation workflow foundation.
- Deterministic accountability brief foundation.
- Clean architecture projects started: `US.Domain`, `US.Application`, `US.Persistence`.

In progress:

- Moving behavior out of API feature code into clean architecture layers.
- USSD simulator.
- Policy comparison and four-agent pipeline UI.
- Dual-zone map.
- PDF-ready accountability brief.

## Run Locally

Requirements:

- .NET 10 SDK.

Build:

```powershell
dotnet build UbuntuSentinel.slnx
```

Run API:

```powershell
dotnet run --project src\Api\US.Api\US.Api.csproj --urls http://localhost:5138
```

Run Web:

```powershell
dotnet run --project src\Web\US.Web\US.Web\US.Web.csproj --urls http://localhost:5009
```

Open:

```text
http://localhost:5009
```

## Build Documents

- [CAPSTONE_SPEC.md](CAPSTONE_SPEC.md) - revised product spec.
- [PLAN.md](PLAN.md) - demo-day build plan.
- [PLAN_EXECUTION_STRATEGY.md](PLAN_EXECUTION_STRATEGY.md) - implementation order.
- [ARCHITECTURE_CHECK.md](ARCHITECTURE_CHECK.md) - BaseTemplate-inspired architecture guardrails.
- [DEPENDENCY_MAP.md](DEPENDENCY_MAP.md) - dependency and product flow maps.
- [MATURITY_BACKLOG.md](MATURITY_BACKLOG.md) - phase 2 and production hardening.
- [UX_GUIDELINES.md](UX_GUIDELINES.md) - UI/UX rules.
- [docs/product-strategy.md](docs/product-strategy.md) - revised OSF-aligned product strategy.
- [docs/demo-script.md](docs/demo-script.md) - judge-facing demo script.
- [docs/codex-usage.md](docs/codex-usage.md) - Codex usage narrative.
