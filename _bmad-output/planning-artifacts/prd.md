---

project_name: Demo
user_name: Sander
communication_language: English
document_output_language: English
user_skill_level: intermediate
stepsCompleted: [1]
inputDocuments: [product-brief-Demo-2026-03-01.md]
---

# Product Requirements Document (PRD) - MatchHub

## Executive Summary
MatchHub is a cross-platform SaaS for outdoor sports, designed to provide seamless, real-time match management for clubs and players. The platform ensures accessibility, reliability, and compliance, with a unified Flutter UI and .NET 8 Minimal APIs backend. All development is performed in a VS Code DevContainer, enforcing high test coverage, automated CI/CD, and full observability. MatchHub eliminates fragmented tools and manual errors, enabling every club and player to manage matches, scores, and court logistics with zero friction and high visibility.

## Success Criteria
- All users can enter scores within 1 second on any device.
- Real-time synchronization ensures all users see the correct score within 2 seconds.
- QR pairing and live score sharing work flawlessly, including offline scenarios.
- Club managers can monitor all matches and scores live via dashboard.
- Court reset and side-switch logic are automatic and correct.
- UI meets WCAG 2.1 contrast standards (minimum 7:1).
- Onboarding without account is instant and barrier-free.
- Push notifications are timely and relevant, with no spam.
- Statistics and history are visible per player/team.
- Gamification features (badges, leaderboards) are awarded correctly.
- API clients are compatible with published OpenAPI schemas.
- All functionality is covered by tests (≥85% coverage).
- Offline mode synchronizes correctly when connection is available.

## User Stories

- As a player, I want to enter scores extremely quickly with minimal clicks/taps, so the game is not interrupted.
- As a team member, referee, or spectator, I want instant visual feedback for every score update, so everyone is always up-to-date.
- As a user, I want scores to synchronize in real time across all devices, so everyone sees the same score.
- As a player or team, I want to automatically view my match history and statistics, so I can track my performance.
- As a user, I want to receive push notifications for important events (start, set point, end), so I never miss anything.
- As a user, I want an intuitive UI with large buttons and clear colors, so I can easily use the app on mobile and tablet.
- As a user, I want to enter scores offline and synchronize them later, so I can play even without internet.
- As a club manager, I want to import/export matches, so I can easily manage competitions.
- As a user, I want to share live scores via web link or QR code, so others can follow along.
- As a player, I want to earn badges, appear on leaderboards, and see personal statistics, so using the app is fun and motivating.
- As a new user, I want to start immediately without an account, so onboarding is fast and easy.
- As a user with a disability, I want high contrast and voice-over support, so the app is accessible to everyone.
- As a club manager, I want a dashboard with live refresh, so I can monitor all matches and scores instantly.
- As a user, I want the app to automatically apply court reset and side-switch logic, so the game is fair.
- As a user, I want all data to be stored and synchronized securely and reliably (PostgreSQL, Redis, OIDC/OAuth2).
- As a developer, I want all functionality to be tested and validated (≥85% coverage, CI/CD), so quality is guaranteed.

## Functional Requirements
- Real-time match scoring and court updates
- Ultra-fast score entry (minimal clicks/taps)
- QR code match pairing
- Club dashboard with live refresh
- Court reset and automatic side-switch logic
- Team color coding, dynamic color inversion
- High-contrast UI, sunlight and color-blind friendly
- Accessibility: font/icon scaling, voice-over
- Multi-platform support: mobile, web, wearable (Flutter)
- User authentication via OIDC/OAuth2
- Offline mode: local score entry and later synchronization
- Automatic match history/statistics per player/team
- Push notifications for events
- Integration with club/competition management (import/export)
- Live score sharing via web link/QR code
- Gamification: badges, leaderboards, personal stats
- Simple onboarding (no account required for basic use)

## Technical Requirements
- Flutter UI (single codebase)
- .NET 8 Minimal APIs backend (REST + SignalR)
- VS Code DevContainer for all builds/tests
- Flutter SDK, Dart SDK, Android SDK, Chrome, .NET 8 SDK, PostgreSQL client, Redis CLI
- ≥85% test coverage (unit, integration, acceptance)
- CI/CD via GitHub Actions (DevContainer-only)
- Swagger/OpenAPI docs and contract testing
- PostgreSQL (persistent), Redis (cache/pubsub)
- OIDC/OAuth2 authentication, JWT roles
- Simulators: Android/iOS/Web/WearOS/WatchOS
- OpenTelemetry logging, monitoring via Seq/App Insights
- Static analysis, secrets/dependency/container scans
- No native/platform-specific code without ADR
- Accessibility and sunlight visibility required

## Acceptance Criteria
- Score entry works within 1 second on all devices
- Real-time synchronization: all users see the correct score within 2 seconds
- QR pairing works flawlessly, even offline
- Dashboard displays all active matches and scores live
- Court reset/side-switch logic is automatic and correct
- UI meets WCAG 2.1 contrast standards (minimum 7:1)
- Onboarding without account works instantly, no barriers
- Push notifications arrive for relevant events, no spam
- Statistics and history are visible per player/team
- Gamification works: badges/leaderboards are awarded correctly
- API clients work with published OpenAPI schemas
- All functionality is covered by tests (≥85% coverage)
- Offline mode synchronizes correctly as soon as connection is available

## Edge cases
- Netwerk valt weg tijdens score-invoer: scores worden lokaal opgeslagen en later gesynchroniseerd
- QR-code niet scanbaar: handmatige invoer mogelijk
- Meerdere devices voeren tegelijk scores in: synchronisatieconflicten worden opgelost
- Court reset/side-switch wordt handmatig geforceerd: systeem past automatisch kleuren/logica aan
- Gebruiker met visuele beperking: UI schakelt automatisch naar hoog contrast/voice-over
- Push-notificatie niet afgeleverd: fallback via dashboard
- API schema wijzigt: contract tests blokkeren deployment tot alle clients compliant zijn
- Onboarding zonder account: gebruiker kan alsnog later een account koppelen
- Data-integriteit: scores/statistieken worden gevalideerd bij sync

## Scope en afhankelijkheden
- Afhankelijk van Flutter, .NET 8, PostgreSQL, Redis, OIDC/OAuth2, CI/CD pipeline
- Geen native code tenzij via ADR
- Alle functionaliteit moet builden, testen en simuleren in DevContainer
