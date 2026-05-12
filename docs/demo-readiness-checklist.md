# Demo Readiness Checklist

## Runtime

- [ ] API starts on `http://localhost:5138`.
- [ ] Web starts on `http://localhost:5009`.
- [ ] Browser hard refresh clears stale assets.
- [ ] App runs without `OPENAI_API_KEY` using labeled deterministic fallback.
- [ ] Optional: app runs with `OPENAI_API_KEY` using OpenAI-backed pipeline.

## Demo Path

- [ ] Landing page language selection works.
- [ ] Staff login works: `validator` / `Validator123!`.
- [ ] Admin login works: `admin` / `Admin123!`.
- [ ] Submit page uses selected language.
- [ ] USSD simulator uses selected language and does not force language by region.
- [ ] Web report submission succeeds.
- [ ] Offline queue/sync can be demonstrated or explained.
- [ ] Reports list updates after submission.
- [ ] View opens read-only details.
- [ ] Validate opens explicit validation workflow.
- [ ] Validator notes are required.
- [ ] Approve requires all checks.
- [ ] Follow-up generates follow-up brief.
- [ ] Reject clears checks and generates internal rejection record.
- [ ] Pipeline shows fallback/OpenAI mode.
- [ ] Pipeline shows sourced clause comparison.
- [ ] Brief is status-aware.
- [ ] Validator role sees brief content without admin diagnostics.
- [ ] Admin role can inspect pipeline/brief diagnostics.
- [ ] Brief Print / Save PDF action opens the browser print flow.
- [ ] Brief Export JSON downloads partner-system payload.
- [ ] Map shows conflict and resilience layers.

## Suggested Demo Data

Primary scenario:

```text
Market demolition without notice by government
```

Use Eastern DRC / Goma / Governance failure. This maps to African Charter Article 14.

Alternative scenario:

```text
Our village was promised a clean water point under the mining agreement, but nothing has been built. Families still pay for water and youth leaders have reported this several times.
```

Use Eastern DRC / mining or resource conflict. This maps to mining/community benefit obligations.

## Judge Talking Points

- This is not just intake; it structures, validates, compares, and routes evidence.
- AI assists but does not authorize escalation.
- Validators must leave notes for every decision.
- Rejected and follow-up reports do not produce final advocacy briefs.
- The map shows resilience as well as conflict.
- Codex was used for architecture, implementation, debugging, docs, and workflow refinement.
