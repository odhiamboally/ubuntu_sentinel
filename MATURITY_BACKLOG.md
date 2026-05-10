# Maturity Backlog

## Purpose

This file now separates demo-day commitments from post-hackathon maturity work. Do not use it to hide prize-critical features.

## Demo-Day Commitments

These belong in `PLAN.md`, not here:

- USSD simulator.
- Web/offline intake.
- Seeded policy comparison.
- Four-agent pipeline.
- Dual conflict/resilience map.
- Validation gate.
- Accountability brief/PDF-ready output.
- Role-shaped demo views.
- EN/FR localization.

## Phase 2

| Item | Why It Matters |
| --- | --- |
| Marten oral history module | Supports peace memory and indigenous knowledge later, but is not required for the demo spine. |
| Women peacebuilder registry | Important product pillar later; for demo, represent women-led work through report flags and resilience zones. |
| Satellite imagery correlation | Valuable for environmental harm verification, but too integration-heavy for the 48-hour build. |
| Full consent lifecycle UI | Needed for production, but demo can show consent capture and validation checks first. |
| Production IAM | Demo role selector is enough; production needs real auth, claims, policies, and audit. |
| WhatsApp full conversation flow | Mention as future channel; build USSD/SMS-style flow first. |
| Multi-tenant regional deployment | Important for OSF scale, not needed for local showcase. |
| Advanced evaluator harness for AI outputs | Useful for production safety, not required for judge demo. |
| Full local dialect packs | EN/FR first; Arabic if time permits. |

## Production Hardening

- PostgreSQL deployment scripts.
- pgvector indexes and embedding migrations.
- Redis/MassTransit production configuration.
- Audit trail and immutable case events.
- Data retention and deletion policy.
- Client-side encryption for offline reports.
- Observability with Seq/OpenTelemetry dashboards.
- CI architecture tests.
- Accessibility and low-literacy review.
