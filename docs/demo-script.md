# Ubuntu Sentinel Demo Script

## Opening

Ubuntu Sentinel is community intelligence infrastructure for transformative peace. It helps communities submit evidence through low-bandwidth channels, validate it locally, compare it against public commitments, and generate accountability outputs institutions can act on only after community validation.

## Canonical Demo Scenario

Use a scenario that maps clearly to the seeded public corpus:

```text
Market demolition without notice by government
```

Recommended fields:

- Region: Eastern DRC.
- Location: Goma.
- Issue type: allow inference or select Governance failure.
- Urgency: High or Medium.
- Sensitive: optional, depending on whether you want to demonstrate hidden details and safety validation.

This maps cleanly to African Charter Article 14 on property rights and shows the real-source comparison without forcing the match.

Alternative resource justice scenario:

```text
Our village was promised a clean water point under the mining agreement, but nothing has been built. Families still pay for water and youth leaders have reported this several times.
```

Use this if demonstrating mining/community benefit commitments.

## Five-Minute Flow

### 1. Language and Field Intake

Start at the landing page, choose English or French, then show the USSD simulator or web form.

Script:

> A community member does not need a smartphone or stable internet. They can submit through a USSD-style flow, and language is treated as a communication preference, not forced by region.

Submit the scenario. Show the success notification and final USSD `END` screen if using USSD.

### 2. Reports List

Open Reports and show the submitted report appearing in the operational list.

Show:

- status,
- urgency,
- sensitive hiding if applicable,
- View as read-only,
- Validate as an explicit action.

Script:

> View is not validation. Community validation is a deliberate action with checks and notes.

### 3. Agent Pipeline

Open the pipeline from report actions.

Show:

- fallback/OpenAI mode alert,
- evidence structuring,
- policy comparison,
- safety review,
- brief generation,
- real-source clause comparison.

Script:

> Codex is not decorative here. It converts informal community testimony into structured, reviewable accountability intelligence, while preserving human validation as the gate.

### 4. Community Validation

Open Validate from the report actions.

Show:

- consent,
- location confidence,
- evidence quality,
- reporter safety,
- mandatory validator notes.

Try approving before notes/checks if useful, then complete the checks and approve or mark follow-up.

Script:

> AI does not decide what gets escalated. Community validators do, and every decision needs notes for accountability.

### 5. Accountability Output

Open the brief.

Show status-aware behavior:

- draft while pending,
- follow-up brief when follow-up is required,
- internal rejection record if rejected,
- validation-backed brief if approved.

Script:

> The same report data does not always produce the same output. Ubuntu Sentinel respects the validation decision.

### 6. Dual-Zone Map

Open the map.

Show:

- red conflict zones,
- green resilience zones,
- OpenStreetMap geography,
- submitted report zones.

Script:

> Most systems map where peace is failing. Ubuntu Sentinel also maps where peace is already working.

## Closing

Ubuntu Sentinel is built around one hypothesis:

> Community voice + structured AI + community validation = accountable peace.
