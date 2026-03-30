# God Games

> *Sponsor a mortal champion. Issue divine commands. Watch them survive — or perish.*

A hybrid idle RPG / god game where players act as patron deities guiding mortal champions through a shared world. Gods check in occasionally to submit interventions. Champions auto-progress on a scheduled tick every 6 hours. An AI narrative layer generates story entries after each tick resolves.

---

## Stack

| Layer | Technology |
|---|---|
| API | ASP.NET Core 9 — Web API + SignalR |
| Auth | ASP.NET Identity + JWT (7-day expiry) |
| Database | PostgreSQL 16 via EF Core 9 (Npgsql) |
| Cache | Redis 7 |
| Background jobs | Hangfire (PostgreSQL storage) |
| AI narrative | Anthropic API — `claude-sonnet-4-20250514` |
| Web client | Blazor WebAssembly |
| Mobile client | .NET MAUI 9 (Android + Windows) |
| Messaging | MediatR (domain events) |
| Logging | Serilog |
| Testing | xUnit + Testcontainers |

---

## Project Structure

```
GodGames.sln
├── src/
│   ├── GodGames.Domain          # Entities, value objects, enums
│   ├── GodGames.Application     # Use cases, MediatR handlers, interfaces
│   ├── GodGames.Infrastructure  # EF Core, repositories, PostgreSQL, Redis
│   ├── GodGames.AI              # Anthropic client, prompt builder, narrative parser
│   ├── GodGames.API             # ASP.NET Core Web API + SignalR hub
│   ├── GodGames.Worker          # Hangfire world tick host
│   ├── GodGames.Web             # Blazor WASM god dashboard
│   └── GodGames.Mobile          # .NET MAUI mobile app
└── tests/
    ├── GodGames.Tests.Unit      # xUnit, pure domain + game logic
    └── GodGames.Tests.Integration # Testcontainers, API + DB end-to-end
```

---

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (for PostgreSQL + Redis)
- [.NET MAUI workload](https://learn.microsoft.com/en-us/dotnet/maui/get-started/installation) (mobile only)

### 1 — Start infrastructure

```bash
docker-compose up -d
```

Starts PostgreSQL on `5432`, Redis on `6379`, and pgAdmin on `http://localhost:5050`.

### 2 — Apply database migrations

```bash
dotnet ef database update --project src/GodGames.Infrastructure --startup-project src/GodGames.API
```

### 3 — Run the API

```bash
dotnet run --project src/GodGames.API
# Listening on http://localhost:5134
```

### 4 — Run the web dashboard

```bash
dotnet run --project src/GodGames.Web
# Open http://localhost:5000
```

### 5 — Run the background worker

```bash
dotnet run --project src/GodGames.Worker
```

### 6 — Run the mobile app

**Windows:**
```bash
dotnet run --project src/GodGames.Mobile -f net9.0-windows10.0.19041.0
```

**Android emulator** (API running, emulator open):
```bash
dotnet run --project src/GodGames.Mobile -f net9.0-android
```

> The Android build points to `http://10.0.2.2:5134` — the emulator's alias for host localhost.

---

## Game Loop

```
God submits intervention (free-text divine command)
        ↓
Hangfire fires WorldTickJob every 6 hours
        ↓
For each active champion:
  1. Apply pending intervention → StatEffect
  2. Select world event based on biome + stats
  3. GameEngineService resolves event → XP / HP / loot outcome
  4. Persist champion state
  5. Publish TickResolved domain event
        ↓
NarrativeService (AI layer):
  - Build prompt from champion context + event + intervention
  - Call Anthropic API (500 tokens in / 200 out)
  - Persist NarrativeEntry — fallback to canned entry on failure
        ↓
SignalR pushes ChampionUpdated to god's dashboard
```

---

## Champion Classes

| Class | Flavour |
|---|---|
| **Warrior** | High STR/VIT, frontline fighter |
| **Mage** | High INT/WIS, volatile power |
| **Rogue** | High DEX, evasive and cunning |

## Spirit Alignments

| Trait | Effect |
|---|---|
| **Brave** | Unwavering resolve |
| **Cautious** | Tactical calculation |
| **Reckless** | Chaos-fuelled power |
| **Cunning** | Master of shadows |

---

## Environment Variables

Copy `.env.example` to `.env` (or use `dotnet user-secrets` locally):

| Key | Description |
|---|---|
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string |
| `ConnectionStrings__Redis` | Redis connection string |
| `Jwt__Secret` | JWT signing secret (min 32 chars) |
| `Anthropic__ApiKey` | Anthropic API key |
| `Hangfire__DashboardPath` | Hangfire dashboard path (default `/hangfire`) |

---

## Running Tests

```bash
# Unit tests (no infrastructure required)
dotnet test tests/GodGames.Tests.Unit

# Integration tests (requires Docker)
dotnet test tests/GodGames.Tests.Integration
```

---

## Design System

The web and mobile clients use the **Celestial Brilliance** design system — an obsidian void dark theme with solar gold accents.

| Token | Value |
|---|---|
| Background | `#050508` |
| Surface | `#131317` |
| Gold | `#ffd700` |
| Cream | `#fff6df` |
| Silver | `#bfc8ce` |

Fonts: **Newsreader** (headlines) · **Work Sans** (body) · **Material Symbols Outlined** (icons)

---

*"The stars are but witnesses to your becoming."*
