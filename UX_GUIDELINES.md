# UX Guidelines

## Purpose

Keep Ubuntu Sentinel compact, corporate, OSF-credible, and field-aware.

## Visual Direction

- Use a restrained MudBlazor-forward interface.
- Keep typography compact.
- Prefer tables for operational lists.
- Use badges/chips for status, urgency, role, validation state, and zone type.
- Avoid marketing-page composition in the product shell.
- Keep the dashboard metrics-first.

## Navigation

Primary demo navigation should support:

- Dashboard.
- Reports.
- Map.
- Validate as an explicit report action, not a hidden side effect of View.
- Briefs/Pipeline when opened from a report.
- Role selector or role-shaped view control.

## Feedback Rules

- Successful user actions should produce visible feedback.
- Failed user actions should explain whether data was preserved.
- Offline state must be explicit.
- Destructive actions require confirmation.
- Submit, validation, sync, and PDF generation feedback should use MudBlazor dialogs/snackbars where possible.
- Disabled decision buttons must have visible nearby explanation.
- Validator notes are required before approve, follow-up, or reject.

## Community Safety UX

- Consent must be visible at intake.
- Sensitive reports must not expose full details in shared/advocate views before validation.
- AI output must be labeled draft until validation.
- Original testimony must be preserved when translation is introduced.
- Rejected reports must render as internal records, not escalation briefs.
- Follow-up reports must render as follow-up briefs, not final advocacy outputs.

## Demo UX Priorities

1. USSD simulator must feel immediately understandable.
2. Agent pipeline must make Codex contribution visible.
3. Conflict/resilience map must communicate OSF alignment fast.
4. Accountability brief must look institution-ready.
