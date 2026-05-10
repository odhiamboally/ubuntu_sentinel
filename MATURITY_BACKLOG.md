# Maturity Backlog

## Purpose

This backlog captures improvements that matter for a stronger Ubuntu Sentinel, but should not interrupt the 3-day MVP unless they become necessary for the demo or judging criteria.

Use this file when a good idea appears but does not belong in the current slice.

## Levels

### MVP

Required for the capstone proof-of-concept.

### Demo Plus

Helpful for a stronger showcase if time remains.

### Production Path

Important for a serious implementation after the capstone.

## Offline And Sync

| Level | Item | Why It Matters | Status |
| --- | --- | --- | --- |
| MVP | Manual sync button with pending count | Makes offline behavior visible and easy to demo | Done |
| MVP | Automatic sync on browser `online` event | Better matches user expectations after connectivity returns | Done |
| Demo Plus | Retry backoff and last-attempt status | Prevents noisy repeated failures and explains sync state | Pending |
| Demo Plus | Connectivity indicator in the shell | Makes low-bandwidth readiness obvious to judges | Partial |
| Production Path | IndexedDB storage instead of `localStorage` | Supports larger payloads and more durable offline storage | Pending |
| Production Path | Background sync through service worker | Enables sync even when the user is not actively on the form | Pending |
| Production Path | Conflict handling for duplicate/resubmitted reports | Prevents accidental duplicate escalation | Pending |

## Data Protection And Safety

| Level | Item | Why It Matters | Status |
| --- | --- | --- | --- |
| MVP | Sensitive report flag | Allows demo of anonymization-aware flow | In progress |
| MVP | Clear AI/human validation disclaimer | Prevents the app from implying automated institutional truth | Pending |
| Demo Plus | Mask sensitive report details in dashboard cards | Reduces exposure risk during validation | Pending |
| Demo Plus | Consent copy before submission | Makes community data sovereignty visible | Pending |
| Demo Plus | Basic threat model for reporters and validators | Grounds safety decisions in realistic conflict-context risks | Pending |
| Production Path | Client-side encryption for offline queue | Protects reports stored on shared or unsafe devices | Pending |
| Production Path | Role-based access control | Separates community, validator, advocate, and admin access | Pending |
| Production Path | Audit trail for validation decisions | Supports accountability and evidence integrity | Pending |
| Production Path | Data retention and deletion policy | Avoids indefinite storage of sensitive community narratives | Pending |
| Production Path | Secure media handling for photos/audio | Prevents metadata leaks and unsafe attachment exposure | Pending |

## Localization And Regional Adaptability

| Level | Item | Why It Matters | Status |
| --- | --- | --- | --- |
| MVP | Region profiles for OSF areas | Shows adaptability across DRC, Sahel, Sudan, Mozambique | Done |
| MVP | English plus one OSF-relevant language path | Demonstrates multilingual readiness | In progress |
| Demo Plus | Resource-backed UI localization | Avoids hardcoded interface text | Pending |
| Demo Plus | Region-specific issue labels and escalation pathways | Makes each operating area feel grounded | Pending |
| Production Path | Translation workflow with human review | Prevents unsafe or inaccurate automated translation | Pending |
| Production Path | Local dialect/community terminology packs | Supports real community adoption | Pending |

## AI Accountability Pipeline

| Level | Item | Why It Matters | Status |
| --- | --- | --- | --- |
| MVP | Deterministic fallback pipeline output | Keeps demo resilient without an API key | Done |
| MVP | Validation Agent structured report output | Shows Codex/OpenAI agentic workflow | Done |
| MVP | Accountability brief markdown generation | Produces the judge-facing artifact | Done |
| Demo Plus | Step-by-step pipeline timeline in UI | Makes AI assistance visible and explainable | Done |
| Demo Plus | Seeded commitment comparison | Shows OSF accountability alignment | Partial |
| Production Path | RAG over vetted agreements and policy documents | Enables real document intelligence | Pending |
| Production Path | Model output evaluation and human override | Reduces hallucination and protects communities | Pending |
| Production Path | Prompt/version registry for agent outputs | Makes generated briefs auditable and reproducible | Pending |
| Production Path | Bias and harm review checklist | Helps catch outputs that could endanger or misrepresent communities | Pending |

## Realtime And Collaboration

| Level | Item | Why It Matters | Status |
| --- | --- | --- | --- |
| MVP | SignalR dashboard update after report submission | Demonstrates live sync | Done |
| Demo Plus | Validator receives new report notification | Makes workflow feel operational | Partial |
| Production Path | Multi-validator assignment and review state | Supports real moderation teams | Pending |

## Persistence

| Level | Item | Why It Matters | Status |
| --- | --- | --- | --- |
| MVP | In-memory repository | Fastest way to prove the flow | Done |
| Demo Plus | SQLite persistence | Keeps data across restarts without infrastructure burden | Pending |
| Production Path | PostgreSQL with JSONB and future pgvector | Supports scalable structured reports and future RAG | Pending |
| Production Path | Backup and restore strategy | Protects community evidence from accidental loss | Pending |
| Production Path | Data migration strategy | Allows schema evolution without losing validated reports | Pending |

## Quality, Testing, And Reliability

| Level | Item | Why It Matters | Status |
| --- | --- | --- | --- |
| MVP | Build verification before each commit | Keeps the demo from breaking while moving fast | In progress |
| Demo Plus | API contract tests for report and region endpoints | Catches regressions in the demo path | Pending |
| Demo Plus | Offline queue unit tests with JS interop abstraction | Protects a core capstone requirement | Pending |
| Demo Plus | Browser smoke test for submit and sync flow | Verifies the judge-facing path | Pending |
| Production Path | Load and resilience testing | Proves the system can handle field use beyond a demo | Pending |
| Production Path | Observability with structured logs and traces | Makes failures diagnosable in real deployments | Pending |

## Accessibility And Usability

| Level | Item | Why It Matters | Status |
| --- | --- | --- | --- |
| MVP | Mobile-responsive report form | Supports low-resource/mobile-first use | In progress |
| Demo Plus | Keyboard and screen-reader pass | Keeps the civic workflow accessible | Pending |
| Demo Plus | Low-literacy/plain-language mode | Makes reporting easier across education levels | Pending |
| Demo Plus | Voice-note placeholder workflow | Signals future accessibility without building audio ingestion now | Pending |
| Production Path | Full voice/audio transcription workflow | Supports oral reporting and low-literacy participation | Pending |
| Production Path | Progressive Web App install/offline shell | Improves field usability in unstable connectivity | Pending |

## Deployment And Operations

| Level | Item | Why It Matters | Status |
| --- | --- | --- | --- |
| MVP | Local run instructions for API and Web | Makes the project reviewable by judges | Pending |
| Demo Plus | One-command local demo script | Reduces showcase setup risk | Pending |
| Demo Plus | Seed data reset endpoint or command | Makes repeated demos predictable | Pending |
| Production Path | Containerized deployment | Makes hosting portable across environments | Pending |
| Production Path | Environment-based configuration and secret management | Keeps API keys and sensitive settings out of source control | Pending |
| Production Path | CI pipeline for build/test | Protects quality as the project grows | Pending |

## Documentation And Showcase

| Level | Item | Why It Matters | Status |
| --- | --- | --- | --- |
| MVP | README run instructions | Makes the project reviewable | Pending |
| MVP | Codex usage explanation | Required to show how Codex was leveraged | Pending |
| MVP | Demo script | Keeps the video/live demo tight | Pending |
| Demo Plus | Architecture diagram | Helps judges understand the workflow quickly | Pending |
| Demo Plus | Screenshots or short GIF | Improves repo presentation | Pending |

## Promotion Rule

Move an item from this backlog into `PLAN.md` only when it becomes necessary for:

- a must-have capstone requirement,
- the next vertical slice,
- demo reliability,
- or a judging criterion.
