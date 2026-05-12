# Ubuntu Sentinel

Ubuntu Sentinel is community intelligence infrastructure for transformative peace. It helps conflict-affected communities submit evidence through low-bandwidth channels, validate it locally, compare it against public commitments, and generate accountability outputs that keep communities in control.

## Positioning

- Primary track: Voice & Accountability.
- Secondary track: Peace & Community.
- Resource justice thread: mining, CDA, public service, land, rights, and peace accord promise tracking.

Ubuntu Sentinel is not a dashboard for OSF staff to read community reports. It is a system where communities retain voice, validation power, and narrative control while producing evidence that institutions can act on.

## Demo-Day Target

- Web/offline report intake.
- USSD-style intake simulator.
- Seeded public policy/rights comparison.
- Four-stage Codex/accountability pipeline.
- Human/community validation gate.
- Status-aware brief outputs.
- Dual conflict/resilience map.
- Role-shaped views: Reporter, Community Validator, Advocate, OSF Partner.
- EN/FR localization.

## Current State

Implemented:

- .NET 10 solution.
- ASP.NET Core API.
- Blazor Web App with Interactive WebAssembly.
- MudBlazor UI foundation.
- Global EN/FR language selection.
- Web report submission.
- Browser offline queue foundation.
- USSD simulator and webhook-shaped endpoint.
- SignalR report updates.
- Issue type inference with user hint support.
- Public-source policy/legal JSON corpus.
- Deterministic policy comparison with domain guardrails.
- Four-stage pipeline UI.
- OpenAI-backed runner behind `OPENAI_API_KEY` with deterministic fallback.
- Human validation workflow with mandatory notes.
- Demo role shell for Reporter, Validator, and Admin.
- Seeded staff login for Validator/Admin; reporters remain anonymous.
- Local JSON report persistence for demo continuity.
- Read-only report view and explicit validation action.
- Status-aware briefs: draft, follow-up, rejection/internal record, validation-backed.
- Admin-only pipeline and brief diagnostics.
- Print-ready brief export for browser Save as PDF.
- Partner-system JSON brief export.
- Dual conflict/resilience map with OpenStreetMap.

Still open:

- Server-side PDF generation.
- Production ASP.NET Identity/IAM.
- PostgreSQL-backed persistence.
- Validator notifications.
- PostgreSQL/pgvector persistence.
- Production auth/IAM.

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

Optional OpenAI-backed pipeline:

```powershell
$env:OPENAI_API_KEY="your-key"
```

Without the key, the app runs a deterministic fallback and labels that clearly in the UI.

## Demo Corpus

The demo policy corpus lives at:

```text
src/Api/US.Api/data/policy-documents.json
```

It includes African Charter provisions, DRC constitution/mining materials, Mali peace commitments, AU anti-corruption commitments, Sudan constitutional provisions, displacement principles, and Mozambique recovery commitments.

## Build Documents

- [CAPSTONE_SPEC.md](CAPSTONE_SPEC.md) - product spec.
- [PLAN.md](PLAN.md) - demo-day build tracker.
- [ARCHITECTURE_CHECK.md](ARCHITECTURE_CHECK.md) - architecture guardrails.
- [DEPENDENCY_MAP.md](DEPENDENCY_MAP.md) - dependency and product flow maps.
- [MATURITY_BACKLOG.md](MATURITY_BACKLOG.md) - phase 2 and production hardening.
- [UX_GUIDELINES.md](UX_GUIDELINES.md) - UI/UX rules.
- [CONTRIBUTING.md](CONTRIBUTING.md) - branch, commit, PR, and review rules.
- [docs/product-strategy.md](docs/product-strategy.md) - OSF-aligned product strategy.
- [docs/demo-script.md](docs/demo-script.md) - judge-facing demo script.
- [docs/codex-usage.md](docs/codex-usage.md) - Codex usage narrative.
- [docs/demo-readiness-checklist.md](docs/demo-readiness-checklist.md) - final demo checklist.
- [docs/codex-collaboration-log.md](docs/codex-collaboration-log.md) - human/Codex collaboration summary.
- [docs/repository-governance.md](docs/repository-governance.md) - PR and branch protection target.
