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
    Intake["Web / Offline PWA / USSD / SMS"]
    Case["Community intelligence record"]
    Pipeline["4-agent Codex pipeline"]
    Policy["Seeded policy / CDA comparison"]
    Safety["Safety and ethics review"]
    Validation["Community validator gate"]
    Draft["Draft accountability brief"]
    Ready["Validation-backed accountability brief"]
    Map["Dual-zone map"]
    Reporter["Reporter status and control"]
    Partner["Advocate / OSF partner"]

    Intake --> Case
    Case --> Reporter
    Case --> Pipeline
    Pipeline --> Policy
    Pipeline --> Safety
    Pipeline --> Draft
    Case --> Validation
    Safety --> Validation
    Validation --> Ready
    Case --> Map
    Ready --> Partner
```

## Implementation Dependency Order

1. Architecture boundaries.
2. Report/case lifecycle migration.
3. Intake channel abstraction.
4. Seeded policy comparison.
5. Agent pipeline.
6. Validation gate.
7. Dual-zone map.
8. Accountability brief/PDF.
9. Role-shaped UI.
10. Mobile simulator if time remains.

## Current Gaps

- Existing report store still lives in API feature code.
- PostgreSQL/pgvector-ready persistence is not implemented.
- Seeded policy comparison is implemented as a deterministic RAG fallback.
- USSD simulator is implemented as a foundation and needs polish/status tracking.
- Dual-zone map is implemented as a seeded foundation and needs Leaflet/filters.
- Role-shaped navigation is not implemented.
- PDF generation is not implemented.
