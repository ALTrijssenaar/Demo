# Demo

MatchHub is a real-time score tracking application built with Flutter (frontend) and ASP.NET Core (backend). This workspace contains the complete implementation including development configurations.

## Project Structure

```
.
├── apps/
│   └── matchhub_flutter/          # Flutter mobile app
├── src/
│   └── backend/
│       └── MatchHub.Api/          # ASP.NET 10.0 Minimal API
├── tests/
│   └── MatchHub.Api.Tests/        # Integration & performance tests
└── _bmad-output/                  # Sprint planning & artifacts
```

## Quick Start

### Prerequisites
- **Flutter SDK**: ARM-native build at `~/flutter-arm` (already installed in DevContainer)
- **.NET SDK**: 10.0 (pre-installed)
- **VS Code Extensions**: 
  - C# (installed)
  - Dart (installed)
  - Flutter (optional but recommended)

### Launch Configuration (F5)

Press **F5** to start the default compound launch: **"MatchHub (API + Flutter)"**

This will:
1. Build the backend (.NET solution)
2. Start the ASP.NET API on `http://localhost:5270`
3. Launch the Flutter app with API endpoint pre-configured

**Available Configurations:**
- `MatchHub (API + Flutter)` — Full stack (default on F5)
- `MatchHub API` — Backend only
- `MatchHub API (Debug)` — Backend with debugger
- `Flutter App` — Frontend only
- `Backend Tests` — Run test suite

### Manual Start

**Backend:**
```bash
cd src/backend/MatchHub.Api
dotnet run
```
API listens on `http://localhost:5270`

**Frontend (Flutter):**
```bash
cd apps/matchhub_flutter
flutter run --dart-define=MATCHHUB_API_URL=http://localhost:5270
```

**Tests:**
```bash
cd tests/MatchHub.Api.Tests
dotnet test
```

## Architecture

- **Backend:** ASP.NET Core 10.0 Minimal APIs with JWT auth, rate limiting, structured logging
- **Database:** PostgreSQL (production) / In-memory (development/tests)
- **Frontend:** Flutter 3.41.2 with high-contrast UI and accessibility features
- **Testing:** xUnit (backend) + Flutter widget tests

## Development Notes

- Rate limiting: 30/10s for score submission, 60/10s for retrieval (production settings in `appsettings.json`)
- Development mode uses in-memory repository (see `appsettings.Development.json`)
- All API endpoints require JWT authentication with `scorer` role
- Input validation: max 100 characters for matchId and playerId
