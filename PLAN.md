# Ubuntu Sentinel Build Plan

## Goal

Build Ubuntu Sentinel as a serious PeaceTech product demo: community intelligence infrastructure that helps conflict-affected communities submit, validate, compare, and escalate accountability evidence without losing control of their own data.

## Track Positioning

- Primary: Voice & Accountability.
- Secondary: Peace & Community.
- Resource justice: included through promise tracking, policy comparison, mining/service commitments, rights frameworks, and reparative accountability outputs.

## Demo-Day Build List

- [ ] BaseTemplate-inspired solution structure:
  - [x] `US.Domain`
  - [x] `US.Application`
  - [x] `US.Infrastructure`
  - [x] `US.Persistence`
  - [x] `US.Api`
  - [x] `US.Web` / `US.Web.Client`
  - [ ] architecture tests
  - [ ] optional `US.Mobile` simulator
- [x] Multi-channel intake:
  - [x] web submit flow
  - [x] offline browser queue foundation
  - [x] USSD simulator
  - [x] webhook-shaped USSD endpoint
  - [x] optional flags in USSD: women-led, youth-led, sensitive
  - [ ] SMS-shaped intake endpoint or simulator
- [ ] Community data sovereignty:
  - [x] original testimony preserved
  - [x] sensitive handling flag
  - [x] sensitive details hidden until consent and reporter safety clear
  - [x] read-only view vs explicit validation split
  - [ ] reporter-owned status view after auth exists
  - [ ] withdrawal/cancel before validation
- [ ] Policy/RAG promise tracking:
  - [x] seeded public policy/legal corpus in API JSON
  - [x] sourced clause references with URLs
  - [x] category/domain guardrails for matching
  - [x] deterministic semantic comparison fallback
  - [x] promise/rights gap output
  - [ ] pgvector-ready persisted abstraction
  - [ ] admin document upload/ingestion
- [x] Four-agent accountability pipeline:
  - [x] evidence structuring
  - [x] policy RAG comparison
  - [x] safety and ethics review
  - [x] accountability brief generation
  - [x] visible pipeline page
  - [x] fallback mode alert
  - [x] OpenAI-backed runner behind `OPENAI_API_KEY`
  - [x] deterministic fallback clearly labeled in UI
  - [x] status-aware rejected/follow-up pipeline behavior
- [x] Human validation:
  - [x] approve/follow-up/reject workflow
  - [x] consent/location/evidence/safety checks persisted
  - [x] approval blocked until checks complete
  - [x] validator notes required for every decision
  - [x] reject clears validation checks
  - [x] follow-up cannot be repeated from follow-up state
  - [ ] validator notification foundation for new regional reports
  - [ ] role-gated validator view
- [x] Dual-zone map:
  - [x] seeded conflict zones
  - [x] seeded resilience zones
  - [x] Leaflet/OpenStreetMap tile layer
  - [x] submitted reports rendered as live map zones
  - [ ] region filters
  - [ ] precise geocoding/GPS
- [ ] Accountability brief:
  - [x] markdown brief foundation
  - [x] validation-backed vs draft state
  - [x] follow-up brief state
  - [x] rejection/internal record state
  - [x] validator notes included
  - [x] print-ready HTML PDF fallback
  - [x] JSON output for partner systems
- [ ] Role-gated navigation:
  - [ ] Reporter
  - [ ] Community Validator
  - [ ] Advocate/CSO
  - [ ] OSF Partner demo view
- [ ] Localization:
  - [x] global EN/FR language selection
  - [x] EN/FR labels on main demo pages
  - [x] USSD respects selected language independent of region
  - [ ] final full EN/FR audit
  - [ ] Arabic documented as next language
- [ ] Demo/readme:
  - [x] 5-minute judge script foundation
  - [x] Codex usage explanation foundation
  - [x] architecture guardrails
  - [x] maturity backlog
  - [x] final demo readiness checklist
  - [x] PDF export demo path

## Immediate Next Slices

1. Add validator notification foundation if time permits.
2. Final localization/browser demo pass.
3. Add role-gated demo shell if it can be done safely.

## Explicit Phase 2

- Marten oral history registry.
- Women peacebuilder registry as a full module.
- Satellite imagery correlation.
- Full consent lifecycle UI.
- Production IAM/multi-tenant auth.
- Durable policy/CDA document storage.
- Admin document ingestion UI.
- Real USSD short code.

## Success Bar

The demo must make judges feel that Ubuntu Sentinel is not an OSF-branded dashboard. It is community-owned intelligence infrastructure: offline-capable, multi-channel, human-validated, policy-aware, status-aware, and able to show both harm and resilience.
