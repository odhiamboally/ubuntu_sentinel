# Codex Usage Notes

## Human Input

The human builder defined:

- the OSF interpretation,
- the revised cross-track positioning,
- the ethical guardrails,
- the requirement for BaseTemplate-style architecture,
- the decision to build the real product spine rather than a thin MVP,
- the final demo priorities.

## Codex Contributions

Codex is being used as a senior engineering collaborator to:

- inspect OSF/project documents,
- translate strategy into architecture and implementation slices,
- scaffold clean architecture projects,
- migrate existing code into stronger boundaries,
- implement USSD, pipeline, map, validation, and brief slices,
- maintain documentation as the strategy changes,
- debug build/runtime issues,
- keep demo reliability in mind.

## Product AI Pipeline

The app demonstrates a four-agent workflow:

1. Evidence Structuring Agent.
2. Policy RAG Comparison Agent.
3. Safety and Ethics Review Agent.
4. Accountability Brief Generation Agent.

## Human-In-The-Loop Boundary

Codex/OpenAI outputs are draft intelligence. They do not become escalation-ready until community validation confirms consent, location confidence, evidence quality, and reporter safety.

## Demo Fallback Rule

Every AI or external-service integration must have a deterministic fallback so the demo remains reliable without:

- live OpenAI access,
- Africa's Talking connectivity,
- PostgreSQL/pgvector availability,
- PDF package/runtime surprises.

## What Codex Did Not Decide

Codex did not decide:

- the mission,
- the moral stance,
- the final track framing,
- the OSF interpretation,
- what tradeoffs are acceptable.

Those remain human decisions.
