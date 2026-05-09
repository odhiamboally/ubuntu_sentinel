# Capstone Spec - Ubuntu Sentinel

## One-Line Pitch

Ubuntu Sentinel helps conflict-affected communities transform raw local reports into validated, localized, institution-ready accountability briefs.

## Track

Voice & Accountability.

## Problem

Communities affected by conflict often know where promises have been broken, where harms are recurring, and where women/youth peacebuilders are already acting, but that knowledge rarely reaches institutions in a structured, safe, and actionable form.

## MVP User Journey

1. A community member submits a report in a localized, low-bandwidth form.
2. If offline, the report is saved locally and placed in a sync queue.
3. When connectivity returns, the report syncs to the API.
4. The AI-assisted pipeline structures the report, compares it to seeded commitments, proposes reparative action, and drafts an accountability brief.
5. A validator reviews the report and AI output before escalation.
6. The dashboard updates in realtime and shows reports by region, urgency, status, and issue type.

## Must-Have Features

- Report submission form.
- Offline queue with visible pending state and sync action.
- Localization foundation with at least English plus one OSF-relevant language.
- Region profiles for OSF operational areas: Sahel, DRC, Sudan, Mozambique.
- AI-assisted accountability pipeline with demo-safe fallback.
- Human validation workflow.
- Dashboard with realtime updates through SignalR.
- Generated accountability brief view/export-ready markdown.
- README/demo notes explaining how Codex was used and where human judgment shaped the work.

## Acceptance Criteria

- A user can submit a report from the frontend.
- A report submitted while offline is retained locally and can sync later.
- The UI can switch language or region context.
- The API can process a report through a visible agent pipeline.
- A validator can approve, reject, or mark a report for follow-up.
- A dashboard viewer can see synced/validated reports update without page refresh.
- The demo can run without a live OpenAI key by using seeded pipeline output.
- Documentation clearly separates human decisions from Codex assistance.

## Out of Scope For 3-Day MVP

- Real WhatsApp/USSD integration.
- Production identity and role management.
- Full RAG/vector search.
- Multi-tenant deployment.
- Legal-grade evidence chain of custody.
- Real institutional submission workflow.
