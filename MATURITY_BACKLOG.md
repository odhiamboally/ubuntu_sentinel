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
| Reporter-owned report portal | Once login exists, reporters should only see their own submitted reports, with status tracking, follow-up messaging, withdrawal/cancel requests, and safe edit rules before validation. |
| Durable policy/CDA document storage | Demo uses an API JSON corpus; production should store uploaded policies/CDAs in Blob/S3/Azure Storage with metadata, versioning, and embeddings. |
| Admin document upload and ingestion UI | Demo uses curated sourced clauses; production should allow trusted admins to upload PDFs/DOCX, extract clauses, classify them, and approve them into the corpus. |
| Region context CRUD administration | Demo uses versioned JSON region profiles; production should persist regions, bodies, pathways, languages, priority issue types, and related policy sources in EF/PostgreSQL with admin screens and audit history. |
| Real USSD short code | Demo uses a USSD simulator and webhook-shaped endpoint; production short codes require carrier/Africa's Talking approval and country-specific setup. |
| Production notification gateways | Demo can use an outbox/logged notification foundation; production should wire SMS/email/WhatsApp delivery with templates, retries, opt-in rules, and audit trails. |
| Server-side PDF generation with QuestPDF | Demo can use print-ready HTML/PDF fallback; production should generate durable PDFs server-side with templates, signatures, and audit metadata. |
| Branch protection and PR governance | During the demo sprint, direct main work is acceptable for speed. Production should require protected branches, PR review, build checks, and architecture/test gates. |
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
