# Codex Usage Notes

## Summary

Codex was used as a senior engineering collaborator, not as an autopilot. The human builder supplied the mission interpretation, OSF framing, ethical boundaries, judging priorities, and final acceptance. Codex helped turn those decisions into architecture, workflows, code, UI, debugging, and documentation.

## Human Input

The human builder defined:

- the OSF/PeaceTech interpretation,
- the primary and secondary track positioning,
- the decision to treat Ubuntu Sentinel as community intelligence infrastructure rather than a thin reporting app,
- the requirement for BaseTemplate-style discipline,
- the priority of a working, demoable solution,
- the insistence that AI must assist while community validators decide,
- the UI preference for compact, MudBlazor-forward, corporate screens,
- the final judgment on product viability and demo quality.

## Codex Contributions

Codex helped with:

- reading and synthesizing project documents,
- comparing workshop/scaffold guidance against the capstone strategy,
- revising the solution from a reporting MVP into a cross-track PeaceTech platform,
- maintaining architecture and planning docs,
- creating clean-architecture-inspired project structure,
- implementing web intake, offline queue, USSD simulator, report list, validation, pipeline, map, and brief slices,
- debugging Blazor render-mode/provider issues,
- debugging CORS, SignalR, service registration, and file-lock build issues,
- refining validation workflow state rules,
- making rejected/follow-up/approved outputs status-aware,
- keeping deterministic fallbacks visible and honest.

## Documentation Leverage

Codex was used to create and maintain:

- `AGENTS.md` for agent instructions and engineering rules,
- `ARCHITECTURE_CHECK.md` for architecture boundaries,
- `PLAN.md` for build status,
- `CAPSTONE_SPEC.md` for product acceptance criteria,
- `DEPENDENCY_MAP.md` for dependency and product flows,
- `MATURITY_BACKLOG.md` for phase 2 scope,
- `UX_GUIDELINES.md` for UI consistency,
- demo and strategy documents under `docs/`.

The human builder used these docs as a control system: when the implementation drifted, the docs were updated or used to pull the work back into alignment.

## Architecture Leverage

Codex helped translate the BaseTemplate discipline into Ubuntu Sentinel:

- separate domain/application/infrastructure/persistence/API/web projects,
- API boundaries for web and future mobile clients,
- deterministic fallbacks behind real integration concepts,
- status-aware workflow rules,
- source-traceable policy comparison,
- build verification after meaningful slices.

The current implementation still has demo bridges in API feature code and in-memory storage. Those are intentional for demo speed and are documented for post-demo migration.

## Workflow Leverage

Codex implemented and refined these workflows:

- report submission through web and USSD,
- optional offline queue and sync,
- issue type inference and user hints,
- sensitive report handling,
- read-only report view vs explicit validation action,
- validation checks with mandatory notes,
- approve/follow-up/reject outcomes,
- follow-up and rejection state rules,
- live report updates through SignalR.

## Agentic Pipeline Leverage

Ubuntu Sentinel demonstrates a four-stage agentic workflow:

1. Evidence Structuring Agent.
2. Policy RAG Comparison Agent.
3. Safety and Ethics Review Agent.
4. Accountability Brief Generation Agent.

When `OPENAI_API_KEY` is configured, the pipeline can use the OpenAI-backed runner. When it is not configured, the app uses deterministic fallback outputs and labels that mode clearly. This keeps the demo honest and reliable.

## Product AI Decisions

Codex and the human builder refined several AI/product rules:

- issue type can be inferred from full testimony,
- user-selected issue type is a hint, not an unquestioned command,
- conflicts are visible to validators,
- validation notes are mandatory,
- rejected reports produce internal rejection records rather than advocacy briefs,
- follow-up reports produce follow-up briefs,
- approved reports produce validation-backed briefs.

## OSF and Initiative Alignment

Codex helped connect the implementation to OSF's Transformative Peace in Africa initiative:

- community validation over institutional extraction,
- low-bandwidth intake through USSD,
- source-traceable accountability against public commitments,
- dual-zone mapping of conflict and resilience,
- explicit women/youth/sensitive flags,
- brief outputs that protect consent and reporter safety.

## Debugging Leverage

Codex helped diagnose and fix:

- missing dependency injection registrations,
- MudBlazor provider placement and render-mode issues,
- SignalR/CORS behavior,
- stale UI state after validation,
- overly permissive validation re-entry,
- report list sensitive-preview logic,
- pipeline/brief mismatch after rejection,
- file locks during `dotnet build`.

## What Codex Did Not Decide

Codex did not decide:

- the mission,
- the moral stance,
- the final track framing,
- the OSF interpretation,
- what tradeoffs were acceptable,
- whether a feature felt prize-worthy.

Those remained human decisions.
