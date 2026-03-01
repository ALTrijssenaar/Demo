---
project_name: Demo
document_type: Real-Time Match Scoring API & Data Model Spec
created: 2026-03-01
output_language: English
source: epic-breakdown-implementation-tasks.md
---

# Real-Time Match Scoring: API Endpoints & Data Models

## API Endpoints

### 1. Submit Score
- **POST /api/matches/{matchId}/scores**
- Request Body:
```json
{
  "playerId": "string",
  "score": "integer",
  "timestamp": "ISO8601 string"
}
```
- Response:
```json
{
  "success": true,
  "updatedScore": "integer",
  "matchId": "string"
}
```

### 2. Get Match Scores
- **GET /api/matches/{matchId}/scores**
- Response:
```json
{
  "matchId": "string",
  "scores": [
    { "playerId": "string", "score": "integer", "timestamp": "ISO8601 string" }
  ]
}
```

### 3. Real-Time Score Updates (SignalR)
- **Event: ScoreUpdated**
- Payload:
```json
{
  "matchId": "string",
  "playerId": "string",
  "score": "integer",
  "timestamp": "ISO8601 string"
}
```

### 4. Get Match Details
- **GET /api/matches/{matchId}**
- Response:
```json
{
  "matchId": "string",
  "players": [ { "playerId": "string", "name": "string" } ],
  "status": "string",
  "startTime": "ISO8601 string",
  "endTime": "ISO8601 string"
}
```

---

## Data Models

### Match
```json
{
  "matchId": "string",
  "players": [ { "playerId": "string", "name": "string" } ],
  "scores": [ { "playerId": "string", "score": "integer", "timestamp": "ISO8601 string" } ],
  "status": "string",
  "startTime": "ISO8601 string",
  "endTime": "ISO8601 string"
}
```

### Score
```json
{
  "playerId": "string",
  "score": "integer",
  "timestamp": "ISO8601 string"
}
```

### Player
```json
{
  "playerId": "string",
  "name": "string"
}
```

### Club
```json
{
  "clubId": "string",
  "name": "string",
  "members": [ "playerId" ]
}
```

---

## Next Steps
- Implement backend endpoints and SignalR events
- Integrate frontend score entry and real-time updates
- Write automated tests for API and event flows
- Document error handling and edge cases
