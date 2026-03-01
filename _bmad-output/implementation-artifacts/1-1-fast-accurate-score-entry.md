# Story 1.1: Fast Accurate Score Entry

Status: review

<!-- Note: Validation is optional. Run validate-create-story for quality check before dev-story. -->

## Story

As a player,
I want to enter scores quickly and accurately,
so that the game flow is uninterrupted.

## Acceptance Criteria

1. Score entry via API completes in <1 second for 95% of requests.
2. Score entry UI supports minimal taps and immediate local confirmation of the entered score.
3. Score data is persisted in PostgreSQL and available for retrieval immediately after submission.
4. API returns appropriate error codes for invalid input, unauthorized access, and server errors.
5. Data integrity is maintained during concurrent score submissions (no lost or duplicated scores).
6. Accessibility requirements (WCAG 2.1) are met for all score entry UIs (high contrast, scalable text, voice-over).
7. All score entry endpoints and UI flows are covered by automated unit and integration tests (>=85% coverage).

## Tasks / Subtasks

- [x] Define score entry UX flow for mobile, web, and wearable (large buttons, minimal taps, high-contrast mode)
  - [x] Capture UX constraints from PRD and accessibility requirements
  - [x] Validate interaction time target (<1s) and define tap count limit
- [x] Implement Flutter score entry UI component(s)
  - [x] Provide input validation (no negative scores, bounds checks, timestamp presence)
  - [x] Provide immediate local feedback on successful tap (no network wait)
- [x] Implement backend score submission API
  - [x] POST /api/matches/{matchId}/scores with request/response as per API spec
  - [x] Validate input and authorization (OIDC/OAuth2, JWT roles)
  - [x] Persist Score and Match updates to PostgreSQL
- [x] Implement backend score retrieval API (for verification)
  - [x] GET /api/matches/{matchId}/scores
- [x] Add tests
  - [x] Unit tests for validation, error codes, and data model serialization
  - [x] Integration tests for submit + retrieve
  - [x] Performance test for <1s submission latency
- [x] Document any new architecture or data model decisions with ADR if new structure or patterns are introduced

## Dev Notes

- Architecture: Flutter client, ASP.NET 8 Minimal APIs backend, SignalR for real-time updates, PostgreSQL + Redis. This story focuses on fast score entry and persistence; real-time broadcast can be stubbed if not yet required by story 1.2/1.3.
- API contract and data models are defined; adhere to request/response payloads and timestamps.
- Performance target is strict: end-to-end API response <1s for 95% of requests.
- Accessibility: UI must meet WCAG 2.1 contrast and support voice-over and scaling.
- Test coverage goal is >=85% for this feature set.

### Project Structure Notes

- If Flutter/.NET project structure does not exist yet, create a minimal scaffold using standard templates inside the repo and record an ADR explaining the chosen layout.
- Do not introduce native platform code without an ADR.

### References

- Source: _bmad-output/planning-artifacts/prd.md (Executive Summary, Technical Requirements, Acceptance Criteria)
- Source: _bmad-output/implementation-artifacts/real-time-match-scoring-acceptance-criteria.md
- Source: _bmad-output/implementation-artifacts/real-time-match-scoring-api-spec.md
- Source: _bmad-output/implementation-artifacts/real-time-match-scoring-test-plan.md
- Source: _bmad-output/implementation-artifacts/high-level-architecture.md
- Source: _bmad-output/implementation-artifacts/epic-breakdown-implementation-tasks.md
- Source: _bmad-output/implementation-artifacts/team-onboarding-guide.md

## Dev Agent Record

### Agent Model Used

GPT-5.3-Codex

### Debug Log References

- Flutter SDK not found in DevContainer; UI tasks cannot start.
- Installed Flutter x64 from official Linux archive; Dart binary fails with Exec format error on ARM container.
- Installed ARM-native Flutter (`~/flutter-arm`) from official stable git branch (with LF checkout) and validated Flutter 3.41.2 / Dart 3.11.0.
- Backend tests: `dotnet test` in `tests/MatchHub.Api.Tests` passed (5 tests).
- Flutter tests: `flutter test` in `apps/matchhub_flutter` passed (2 tests).

### Completion Notes List

- Blocked: Install Flutter SDK (or update DevContainer) before implementing UI tasks.
- Blocked: Need ARM64-compatible Flutter SDK or x64 DevContainer to proceed with Flutter UI work.
- Implemented accessible score-entry UI with large controls and immediate local feedback via snackbars.
- Implemented `POST /api/matches/{matchId}/scores` with auth and request validation.
- Implemented `GET /api/matches/{matchId}/scores` with in-memory persistence and concurrent-safe updates.
- Added integration and performance-focused backend tests plus Flutter widget tests.
- Added ADR documenting initial monorepo structure required for implementation.

### File List

- apps/matchhub_flutter/lib/main.dart
- apps/matchhub_flutter/lib/score_service.dart
- apps/matchhub_flutter/test/widget_test.dart
- src/backend/MatchHub.Api/Program.cs
- src/backend/MatchHub.Api/Domain/IScoreRepository.cs
- src/backend/MatchHub.Api/Domain/ScoreModels.cs
- src/backend/MatchHub.Api/Domain/ScoreStore.cs
- src/backend/MatchHub.Api/Infrastructure/PostgresScoreRepository.cs
- src/backend/MatchHub.Api/appsettings.json
- src/backend/MatchHub.Api/appsettings.Development.json
- tests/MatchHub.Api.Tests/MatchHub.Api.Tests.csproj
- tests/MatchHub.Api.Tests/CustomWebApplicationFactory.cs
- tests/MatchHub.Api.Tests/ScoreApiTests.cs
- tests/MatchHub.Api.Tests/UnitTest1.cs (deleted)
- _bmad-output/implementation-artifacts/adr-0001-initial-project-structure.md

### Change Log

- 2026-03-01: Completed Story 1.1 implementation for fast score entry across Flutter UI and .NET backend; added automated tests and ADR.
- 2026-03-01: Applied high-severity security fixes: JWT authentication, protected GET endpoint, PostgreSQL persistence path, Flutter API integration.
- 2026-03-01: Applied medium-severity improvements: input length validation (100 char max), rate limiting (30/10s submission, 60/10s retrieval), structured logging for audit trail. Added 3 validation tests. All 9 backend tests + 2 Flutter tests pass.
