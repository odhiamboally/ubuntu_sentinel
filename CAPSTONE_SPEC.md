# Capstone Spec - Ubuntu Sentinel

## One-Line Pitch

Ubuntu Sentinel is community intelligence infrastructure for transformative peace: it helps communities submit evidence through low-bandwidth channels, validate it locally, compare it against public commitments, and generate institution-ready accountability outputs only when the community validation gate clears.

## Track

- Primary: Voice & Accountability.
- Secondary: Peace & Community.
- Resource justice thread: promise tracking for CDAs, mining commitments, public services, peace accords, rights frameworks, and regional institutions.

## Problem

Conflict-affected communities often know what institutions, companies, or authorities promised and what actually happened. That knowledge is fragmented across conversations, SMS, WhatsApp, local meetings, and informal reports. Institutions receive late, incomplete, or filtered evidence, while communities lose visibility, control, and narrative power.

Ubuntu Sentinel closes the power gap by making community knowledge structured, validated, localized, policy-aware, and difficult to ignore.

## Product Pillars

1. Multi-channel community intake: web, offline queue, USSD simulator, and webhook-shaped low-bandwidth API.
2. Codex agentic intelligence pipeline: evidence structuring, policy comparison, safety review, brief generation.
3. Dual-zone intelligence map: conflict zones and resilience zones.
4. Community data sovereignty: original testimony preservation, sensitivity handling, validation gate, reporter-oriented future tracking.
5. Accountability outputs: status-aware briefs and records for advocates, OSF partners, and regional bodies.

## Current Issue Taxonomy

- Broken promise.
- Resource conflict.
- Governance failure.
- Security concern.
- Service gap.
- Environmental harm.
- Mediation success.
- Recovery need.
- Community resilience.

Issue type may be inferred from testimony. Reporter selection is treated as a hint; pipeline and validator review preserve the raw description and surface conflicts.

## Demo Policy Corpus

The demo uses a versioned public-source corpus in `src/Api/US.Api/data/policy-documents.json`.

Minimum active corpus:

- African Charter on Human and Peoples' Rights: Articles 4, 6, 14, 16, 24.
- Constitution of the Democratic Republic of Congo: Articles 16 and 58.
- Democratic Republic of Congo Mining Code reform materials: community development and local benefit obligations.
- Mali Agreement for Peace and Reconciliation: civilian protection and reconciliation commitments.
- African Union Convention on Preventing and Combating Corruption: Article 4.
- Sudan Constitutional Declaration: rights and freedoms provisions.
- Guiding Principles on Internal Displacement: protection and assistance principles.
- Mozambique peace and recovery commitments.

The target architecture is pgvector/embedding retrieval over persisted policy documents. The demo path is deterministic and source-traceable.

## Demo User Journey

1. A community member chooses a language and submits a report through the USSD simulator or web form.
2. Offline-capable web intake queues the report if connectivity fails.
3. The report enters a four-stage pipeline.
4. The policy comparison stage matches the report to public source clauses.
5. The safety stage flags consent/sensitivity needs.
6. A community validator reviews checks and enters mandatory notes.
7. The system produces a status-aware output:
   - draft brief while pending,
   - follow-up brief when more information is needed,
   - internal rejection record when rejected,
   - validation-backed brief when approved.
8. The map shows both conflict signals and resilience zones.

## Must-Have Demo Features

- USSD-style intake simulator.
- Web/PWA-style report submission and offline queue.
- Public clause-level policy/legal corpus with sourced article references.
- Four-agent pipeline visualization.
- OpenAI-backed agent runner when configured, deterministic fallback when not configured.
- Human validation workflow with consent, location, evidence, safety, and mandatory notes.
- Explicit view vs validate separation.
- Dual conflict/resilience map.
- Accountability brief view with PDF generation or PDF-ready fallback.
- EN/FR localization across demo-critical pages.
- Clear Codex usage explanation.

## Acceptance Criteria

- A report can be submitted through web and USSD simulator.
- A report can be queued offline and synced later.
- A sourced clause comparison produces a visible rights, promise, or accountability gap.
- The agent pipeline produces structured steps and status-aware outputs.
- A validator can approve, reject, or request follow-up with mandatory notes.
- Approval is blocked until all validation checks pass.
- Rejected reports do not produce escalation-ready briefs.
- Dashboard and map reflect live report/zone state.
- The demo can run without live OpenAI, Africa's Talking, PostgreSQL, or PDF package surprises by using documented fallbacks.

## Explicitly Out Of Scope

- Marten oral history module.
- Full women peacebuilder registry.
- Satellite imagery correlation.
- Production IAM.
- Full consent lifecycle UI.
- Real WhatsApp integration.
