---
project_name: Demo
document_type: Architecture Decision Record
created: 2026-03-01
output_language: English
---

# ADR-0001: Initial Project Structure for Story 1.1

## Status
Accepted

## Context
Story 1.1 requires implementing a Flutter score-entry UI, a .NET Minimal API backend, and automated tests in a repository that previously contained only planning artifacts.

## Decision
Adopt a monorepo structure with clear platform boundaries:

- `apps/matchhub_flutter` for Flutter client UI and widget tests
- `src/backend/MatchHub.Api` for ASP.NET Core Minimal API endpoints
- `tests/MatchHub.Api.Tests` for backend integration and performance-focused tests

Use an ARM-native Flutter SDK installed from official Flutter Git stable branch in this ARM64 DevContainer.

## Consequences
- Clear separation of frontend, backend, and tests improves maintainability.
- Story implementation can proceed in DevContainer without platform-specific native code.
- Additional tooling setup is needed in CI to run both `dotnet test` and `flutter test`.

## Alternatives Considered
- Keep all code in root: rejected due poor separation and scaling issues.
- Backend-only implementation: rejected because story 1.1 includes UI requirements.
- Use old third-party ARM Dart SDK (2021): rejected due incompatibility with current Flutter (Dart 3.11 required).

## References
- _bmad-output/implementation-artifacts/1-1-fast-accurate-score-entry.md
- _bmad-output/implementation-artifacts/high-level-architecture.md
- _bmad-output/planning-artifacts/prd.md
