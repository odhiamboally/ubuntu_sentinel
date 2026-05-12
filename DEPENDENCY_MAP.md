# Dependency Map

## Project Dependencies

```mermaid
flowchart LR
    Shared["US.SharedKernel"]
    Domain["US.Domain"]
    App["US.Application"]
    Persistence["US.Persistence"]
    Infra["US.Infrastructure"]
    Api["US.Api"]
    WebHost["US.Web"]
    WebClient["US.Web.Client"]
    Mobile["US.Mobile optional"]

    Domain --> Shared
    App --> Domain
    App --> Shared
    Persistence --> App
    Persistence --> Domain
    Persistence --> Shared
    Infra --> App
    Infra --> Domain
    Infra --> Shared
    Api --> App
    Api --> Persistence
    Api --> Infra
    Api --> Shared
    WebHost --> WebClient
    WebHost --> Shared
    WebClient --> Shared
    Mobile --> Shared
    WebClient -. HTTP and SignalR .-> Api
    Mobile -. HTTP .-> Api
```

## Product Flow

```mermaid
flowchart TD
    Intake["Web / Offline Queue / USSD"]
    Case["Community intelligence record"]
    Inference["Issue inference and user hint resolution"]
    Pipeline["4-stage accountability pipeline"]
    Policy["Public clause comparison"]
    Safety["Safety and ethics review"]
    Validation["Community validator gate"]
    Draft["Draft accountability brief"]
    FollowUp["Follow-up brief"]
    Rejected["Internal rejection record"]
    Ready["Validation-backed accountability brief"]
    Map["Dual-zone map"]
    Reporter["Reporter status/control future"]
    Partner["Advocate / OSF partner"]

    Intake --> Case
    Case --> Reporter
    Case --> Inference
    Inference --> Pipeline
    Pipeline --> Policy
    Pipeline --> Safety
    Pipeline --> Draft
    Case --> Validation
    Safety --> Validation
    Validation --> Ready
    Validation --> FollowUp
    Validation --> Rejected
    Case --> Map
    Ready --> Partner
```

## Implementation Dependency Order

1. Architecture boundaries.
2. Report/case lifecycle.
3. Intake channels.
4. Seeded public clause comparison.
5. Agent pipeline.
6. Validation gate and status-aware outputs.
7. Dual-zone map.
8. Accountability brief/PDF.
9. Role-shaped UI.
10. Mobile simulator if time remains.

## Current Gaps

- Existing report store still lives in API feature code.
- PostgreSQL/pgvector-ready persistence is not implemented.
- Seeded policy comparison is implemented as a deterministic fallback over versioned API JSON corpus.
- OpenAI-backed runner exists behind configuration; current local demo often runs deterministic fallback unless `OPENAI_API_KEY` is configured.
- Dual-zone map combines seeded zones with submitted report zones; submitted coordinates are region-derived and should later move to precise geocoding/GPS.
- Role-shaped navigation is not implemented.
- PDF generation/export is not implemented yet.
- Validator notification foundation is not implemented yet.
