# Ubuntu Sentinel Build Plan

## Goal

Build Ubuntu Sentinel as a serious PeaceTech product demo: community intelligence infrastructure that helps conflict-affected communities submit, validate, compare, and escalate accountability evidence without losing control of their own data.

This is no longer a thin reporting MVP. The target is a prize-ready Andela x OpenAI Codex Accelerator showcase aligned with OSF's "shifting power to communities" mission.

## Track Positioning

- Primary: Voice & Accountability.
- Secondary: Peace & Community.
- Resource justice: included through promise tracking, policy comparison, CDA/mining/service commitments, and reparative accountability briefs.

## Demo-Day Build List

- [ ] BaseTemplate-inspired solution structure:
  - [x] `US.Domain`
  - [x] `US.Application`
  - [x] `US.Infrastructure`
  - [x] `US.Persistence`
  - [x] `US.Api`
  - [x] `US.Web` / `US.Web.Client`
  - [ ] `US.Mobile` MAUI simulator if web/API spine is stable
  - [ ] architecture tests
- [ ] Multi-channel intake:
  - [x] web submit flow
  - [x] offline browser queue foundation
  - [x] USSD simulator foundation
  - [x] Africa's Talking webhook-shaped endpoint foundation
  - [ ] SMS-shaped intake endpoint or simulator
- [ ] Community data sovereignty:
  - [ ] consent captured at intake
  - [ ] original testimony preserved
  - [ ] reporter status view
  - [ ] withdrawal/cancel before validation
- [ ] Policy/RAG promise tracking:
  - [ ] seeded policy/CDA documents
  - [ ] pgvector-ready abstraction
  - [ ] deterministic semantic comparison fallback
  - [ ] promise-delivery gap output
- [ ] Four-agent Codex pipeline:
  - [ ] evidence structuring
  - [ ] policy RAG comparison
  - [ ] safety and ethics review
  - [ ] accountability brief generation
  - [ ] visible pipeline page
- [ ] Human validation:
  - [x] approve/follow-up/reject workflow foundation
  - [ ] consent/location/evidence/safety checks persisted
  - [ ] community validator role view
  - [ ] brief escalation blocked until validation
- [ ] Dual-zone map:
  - [x] seeded conflict zones
  - [x] seeded resilience zones
  - [ ] women-led/youth-led resilience signals
  - [ ] region filters
- [ ] Accountability brief:
  - [x] markdown brief foundation
  - [ ] QuestPDF generation
  - [ ] JSON output for partner systems
  - [ ] validation-backed vs draft state
  - [ ] EN/FR output
- [ ] Role-gated navigation:
  - [ ] Reporter
  - [ ] Community Validator
  - [ ] Advocate/CSO
  - [ ] OSF Partner demo view
- [ ] Localization:
  - [x] EN/FR UI foundation
  - [ ] full EN/FR coverage on demo pages
  - [ ] AR documented as next language unless time permits
- [ ] Demo/readme:
  - [ ] 5-minute judge script
  - [ ] Codex usage explanation
  - [ ] architecture diagram
  - [ ] Phase 2 roadmap

## Explicit Phase 2

Do not build these for the demo:

- Marten oral history registry.
- Women peacebuilder registry as a full module.
- Satellite imagery correlation.
- Full consent lifecycle UI.
- Production IAM/multi-tenant auth.

Represent women-led peacebuilding through report flags, resilience zones, seeded examples, and demo narrative.

## 48-Hour Execution Order

1. Align solution structure and docs to the revised product.
2. Move current report behavior behind application/domain/persistence boundaries.
3. Add seeded policy documents and promise comparison service.
4. Add USSD simulator and webhook-shaped intake.
5. Add four-agent pipeline result and pipeline UI.
6. Add dual-zone map.
7. Upgrade validation checks and role-shaped navigation.
8. Add QuestPDF or PDF-ready export fallback.
9. Polish demo, docs, and browser run.

## Success Bar

The demo must make judges feel that Ubuntu Sentinel is not an OSF-branded dashboard. It is community-owned intelligence infrastructure: offline-capable, multi-channel, human-validated, policy-aware, and able to show both harm and resilience.
