---
project_name: Demo
user_name: Sander
communication_language: English
document_output_language: English
user_skill_level: intermediate
stepsCompleted: [1, 2]
inputDocuments: []
---


# Product Brief for MatchHub

## Executive Summary
MatchHub is a cross-platform SaaS for outdoor sports, governed by a strict constitution to ensure platform independence, testability, scalability, and maintainability. All UI is built in Flutter, all APIs in .NET 8 Minimal APIs, and all development runs in a VS Code DevContainer. The platform enforces high test coverage, automated CI/CD, accessibility for sunlight and color-blindness, and full observability. No code is accepted unless it can be built, tested, and simulated in the DevContainer.

## Vision
Enable seamless, real-time match management for clubs and players, regardless of device or environment. Solve the fragmentation and inconsistency of current sports management tools by providing a unified, accessible, and reliable platform. Success means every club and player can manage matches, scores, and court logistics with zero friction, high visibility, and total confidence.


## Features
- Flutter UI for all platforms (mobile, web, wearable)
- .NET 8 Minimal APIs backend (REST + SignalR)
- Real-time match scoring and court updates
- QR code match pairing
- Club dashboard with live refresh
- Court reset and side-switch logic
- High-contrast, sunlight-visible UI
- Color-coded teams and dynamic color inversion
- Accessibility: font/icon scaling, contrast testing
- Simulators for Android, iOS, Web, WearOS/WatchOS
- PostgreSQL for persistent data, Redis for caching/pubsub
- OIDC/OAuth2 authentication (Auth0/Azure AD B2C)
- Automated unit, integration, and acceptance tests (≥85% coverage)
- Swagger/OpenAPI contract validation
- CI/CD via GitHub Actions (DevContainer-only)
- Telemetry and logging (OpenTelemetry, Seq/App Insights)
- Static analysis, security scans, and governance enforcement
- Supersnelle en eenvoudige score-invoer (minimale clicks/taps)
- Directe visuele feedback bij elke score-update
- Real-time synchronisatie tussen meerdere devices (teamleden, scheidsrechter, publiek)
- Automatische matchhistorie en statistieken per speler/team
- Push-notificaties bij belangrijke events (match start, set point, einde)
- Intuïtieve UI voor mobiel en tablet (grote knoppen, duidelijke kleuren)
- Offline modus: scores lokaal invoeren en later synchroniseren
- Integratie met club- of competitiebeheer (import/export van wedstrijden)
- Live score sharing via web-link of QR-code
- Gamification: badges, leaderboards, persoonlijke statistieken
- Eenvoudige onboarding (geen account nodig voor basisgebruik)
- Toegankelijkheid: grote contrasten, voice-over ondersteuning

## Target Users
Sports clubs, match organizers, and players needing reliable, real-time match management across devices and environments.

## Value Proposition
One platform for all match management needs, with guaranteed reliability, accessibility, and compliance. No more fragmented tools, manual errors, or platform lock-in.

## Constraints
- No native/platform-specific code without ADR approval
- All code must build and test in DevContainer
- 85%+ test coverage required
- All APIs must expose OpenAPI docs
- Accessibility and sunlight visibility are mandatory

## Next Steps
Proceed to user definition and stakeholder analysis.
