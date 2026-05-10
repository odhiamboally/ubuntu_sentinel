# Capstone Spec - Ubuntu Sentinel

## One-Line Pitch

Ubuntu Sentinel is community intelligence infrastructure for transformative peace: it helps communities submit evidence through low-bandwidth channels, validate it locally, compare it against public commitments, and generate institution-ready accountability briefs.

## Track

- Primary: Voice & Accountability.
- Secondary: Peace & Community.
- Resource justice thread: promise tracking for CDAs, mining commitments, public services, peace accords, and regional frameworks.

## Problem

Conflict-affected communities often know what institutions, companies, or authorities promised and what actually happened. That knowledge is fragmented across conversations, SMS, WhatsApp, local meetings, and informal reports. Institutions receive late, incomplete, or filtered evidence, while communities lose visibility, control, and narrative power.

Ubuntu Sentinel closes the power gap by making community knowledge structured, validated, localized, policy-aware, and difficult to ignore.

## Product Pillars

1. Multi-channel community intake: web, offline PWA, USSD simulator, SMS-shaped endpoint.
2. Codex agentic intelligence pipeline: evidence structuring, policy comparison, safety review, brief generation.
3. Dual-zone intelligence map: conflict zones and resilience zones.
4. Community data sovereignty: consent, original testimony preservation, validation gate, reporter tracking.
5. Accountability outputs: JSON and PDF-ready briefs for advocates, OSF partners, and regional bodies.

## Demo User Journey

1. A community member submits a report through the USSD simulator or web form.
2. Offline-capable intake queues the report if connectivity fails.
3. The report enters a four-agent pipeline.
4. The policy comparison agent finds a seeded commitment gap.
5. The safety agent flags consent/sensitivity needs.
6. A community validator reviews the record and validation checks.
7. An advocate/partner opens a validation-backed accountability brief.
8. The map shows both conflict signals and resilience zones in the same geography.

## Must-Have Demo Features

- USSD-style intake simulator.
- Web/PWA-style report submission and offline queue.
- Seeded policy document comparison with pgvector-ready abstraction.
- Four-agent pipeline visualization.
- Human validation workflow with consent, location, evidence, and safety checks.
- Dual conflict/resilience map.
- Accountability brief view with PDF generation or PDF-ready fallback.
- Role-shaped navigation for Reporter, Community Validator, Advocate, and OSF Partner.
- EN/FR localization across demo-critical pages.
- Clear Codex usage explanation.

## Acceptance Criteria

- A report can be submitted through web and USSD simulator.
- A report can be queued offline and synced later.
- A seeded policy/CDA comparison produces a visible promise gap.
- The agent pipeline produces structured steps and a final brief.
- A validator can approve, reject, or request follow-up.
- Escalation-ready briefs require human validation.
- Dashboard and map reflect live report/zone state.
- The demo runs without live OpenAI, Africa's Talking, or PostgreSQL by using documented fallbacks.

## Explicitly Out Of Scope

- Marten oral history module.
- Full women peacebuilder registry.
- Satellite imagery correlation.
- Production IAM.
- Full consent lifecycle UI.
- Real WhatsApp integration.
