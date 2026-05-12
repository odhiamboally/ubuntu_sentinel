# AGENTS.md

## Project

Ubuntu Sentinel is a .NET 10 PeaceTech product for the Andela x OpenAI Codex Accelerator. It is positioned as community intelligence infrastructure for OSF-aligned transformative peace, not merely a reporting dashboard.

## Current Strategy

Build a prize-ready demo that shows the full product spine:

- Low-bandwidth community intake through web, offline queue, and USSD simulator.
- Real-source policy and rights comparison over a curated public clause corpus.
- Four-stage agentic accountability pipeline with deterministic fallback and optional OpenAI runner.
- Explicit community validation gate with consent, location, evidence, safety, and mandatory validator notes.
- Dual conflict/resilience map.
- Status-aware accountability outputs: draft, follow-up, rejection/internal record, and validation-backed brief.
- EN/FR localization across judge-facing flows.

## Non-Negotiables

- AI assists; community validators decide.
- Original community testimony must be preserved.
- Consent and reporter safety checks must be visible.
- Validator notes are mandatory for every validation decision.
- View is read-only. Validate is an explicit action.
- Rejected reports do not become escalation briefs.
- Conflict zones and resilience zones are both first-class.
- External integrations need deterministic demo fallbacks.
- EN/FR localization must cover the judge-facing flow.

## Issue Type and Inference Rules

- Issue type can be inferred from the full description text.
- User selection is a hint, not an unquestioned command.
- The pipeline resolves issue type from the best available signal and surfaces user/system conflicts to validators.
- Reports should remain valid even if the reporter does not understand the taxonomy perfectly.
- Current demo taxonomy: broken promise, resource conflict, governance failure, security concern, service gap, environmental harm, mediation success, recovery need, community resilience.

## Validation and Brief Rules

- Approve requires all checks: consent, location confidence, evidence quality, and reporter safety.
- Follow-up requires validator notes.
- Reject requires validator notes and clears validation checks.
- Approved reports can produce validation-backed accountability briefs.
- Needs follow-up reports produce follow-up briefs focused on missing evidence and next validation steps.
- Rejected reports produce internal rejection records only.
- Sensitive report details remain hidden in shared views until consent and reporter safety are confirmed.

## Fallback Rules

Every external dependency must have a demo-safe fallback:

- OpenAI unavailable -> deterministic agent outputs.
- PostgreSQL/pgvector unavailable -> seeded JSON comparison behind the same policy comparison concept.
- Africa's Talking unavailable -> USSD simulator and webhook-shaped endpoint.
- QuestPDF unavailable -> print-ready HTML/markdown PDF fallback.
- Azure Blob unavailable -> versioned clause-level JSON corpus with public source URLs.
- SMS/email gateways unavailable -> documented notification backlog or logged demo notifications.

## Phase 2 Only

Do not build during the demo sprint unless all demo-critical paths are complete:

- Marten oral histories.
- Full women peacebuilder registry.
- Satellite imagery correlation.
- Production IAM.
- Full consent lifecycle UI.
- Real USSD short code.
- Durable policy document ingestion/storage.

## Engineering Rules

- Keep the solution buildable after each major slice.
- Prefer BaseTemplate structure and discipline.
- Move behavior into `US.Domain`, `US.Application`, `US.Persistence`, and `US.Infrastructure` where practical, but do not destabilize the demo for purity.
- Web and future Mobile must call API boundaries, not server internals.
- Use MudBlazor for app UI controls where practical.
- Keep docs in sync when strategy, architecture, or user-visible behavior changes.

## Verification

- Stop running API/Web before building if file locks occur.
- Run `dotnet build UbuntuSentinel.slnx` after structural changes.
- Browser-verify UI changes when the browser backend is available; otherwise inspect HTTP/logs and ask the user to refresh.
