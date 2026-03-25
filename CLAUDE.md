# God Games — Project Context for Claude Code

## What this project is
A hybrid idle RPG / god game where players are patron deities sponsoring mortal champions.
Gods (players) check in occasionally to submit interventions (free-text divine commands). 
Champions auto-progress through a shared world on a scheduled tick (every 6 hours).
An AI narrative layer (Anthropic API) generates story entries after each tick resolves.

## Architecture: Hybrid Option C
- **Deterministic game engine** (pure C#) handles all stat math, combat, XP, progression
- **AI narrative layer** fires AFTER tick resolves — cosmetic only, never affects game state
- **Clean Architecture**: Domain → Application → Infrastructure + AI (outer rings depend inward)

## Solution structure
```
GodGames.sln
├── src/
│   ├── GodGames.Domain          # Entities, value objects, enums — zero dependencies
│   ├── GodGames.Application     # Use cases, interfaces, MediatR handlers
│   ├── GodGames.Infrastructure  # EF Core, repositories, PostgreSQL, Redis
│   ├── GodGames.AI              # Anthropic client, prompt builder, narrative parser
│   ├── GodGames.API             # ASP.NET Core Web API, controllers, SignalR hubs
│   ├── GodGames.Worker          # Hangfire world tick host (separate process)
│   └── GodGames.Web             # Blazor WASM — god dashboard
└── tests/
    ├── GodGames.Tests.Unit      # xUnit, pure domain + game logic, no infra deps
    └── GodGames.Tests.Integration # Testcontainers, API + DB end-to-end
```

## Tech stack
- **Runtime**: .NET 9
- **API**: ASP.NET Core Web API
- **Auth**: ASP.NET Identity + JWT (7-day expiry)
- **ORM**: EF Core 9 with PostgreSQL (Npgsql)
- **Cache**: Redis (StackExchange.Redis)
- **Background jobs**: Hangfire (PostgreSQL storage)
- **Messaging**: MediatR (domain events, no external broker at MVP)
- **Frontend**: Blazor WebAssembly
- **Real-time**: SignalR
- **AI**: Anthropic API (claude-sonnet-4-20250514), called from GodGames.AI project
- **Logging**: Serilog (console + file sinks, App Insights on prod)
- **Testing**: xUnit, Testcontainers, Moq
- **Local dev**: Docker Compose (PostgreSQL 16 + Redis 7 + pgAdmin)

## Domain model (MVP)

### Champion
- Id, GodId (FK), Name, Class (enum: Warrior / Mage / Rogue)
- Stats: STR, DEX, INT, WIS, VIT (value object)
- HP, MaxHP, Level, XP
- PowerUpSlot (active power-up, nullable)
- CreatedAt, LastTickAt

### God (player account)
- Id, Email (ASP.NET Identity user)
- One champion per god at MVP

### World
- One shared world instance
- 3 biomes: Safe / Normal / Dangerous
- WorldEvents: 20 seeded events with stat requirements and outcome modifiers

### Intervention
- Id, GodId, ChampionId
- RawCommand (string), ParsedEffect (JSON), IsApplied (bool)
- One pending intervention per god per tick window

### NarrativeEntry
- Id, ChampionId, TickNumber, StoryText, CreatedAt
- Append-only, last 10 shown on dashboard

## Core game loop
1. God submits free-text intervention via POST /interventions (one per tick window)
2. Hangfire fires WorldTickJob every 6 hours
3. For each active champion:
   a. Fetch pending intervention → run InterventionParser → apply StatEffect
   b. Select world event based on champion biome + stats
   c. Run GameEngineService.ResolveEvent() → combat/loot/XP outcome
   d. Persist champion state changes
   e. Publish TickResolved domain event via MediatR
4. NarrativeService listens to TickResolved:
   a. Build prompt context (champion stats + event + intervention)
   b. Call Anthropic API (max 500 tokens in, 200 out)
   c. Persist NarrativeEntry — fallback to canned entry on any failure
5. SignalR pushes ChampionUpdated to connected god's dashboard

## Critical rules — do not break these
- **AI must never affect game state** — NarrativeService only writes to NarrativeEntry table
- **Tick must always complete** even if Anthropic API is down — NarrativeService catches all exceptions
- **One API call per champion per tick** — token budget: 500 in / 200 out
- **GameEngineService has zero infrastructure dependencies** — must be unit testable with no DB or mocks
- **Gods can only read/write their own champion** — enforced at API controller layer via GodId claim

## Intervention system
12 predefined command patterns mapped to StatEffect objects. Examples:
- "grant fire resistance" → +10 VIT for 3 ticks
- "blessed blade" → +15 STR for 3 ticks  
- "divine shield" → +20 HP flat
- Unknown commands → minor default blessing

## Power-up system
- 12 predefined power-up types at MVP
- Granted via god intervention
- Lasts 3 ticks then expires automatically
- One active power-up per champion at a time

## NFRs to keep in mind
- Dashboard loads < 2s
- Tick job completes < 30s for up to 50 champions
- AI failures degrade gracefully (fallback narrative, no tick failure)
- Redis loss is tolerable — cache only, never source of truth
- All secrets via dotnet user-secrets locally, Azure Key Vault on prod

## Current sprint
**Sprint 1 — Foundation: Solution scaffold + auth**
- [ ] Create ADO repo and branch policy
- [ ] Scaffold solution with all projects and references
- [ ] Docker Compose: PostgreSQL + Redis + pgAdmin
- [ ] EF Core setup and initial migration
- [ ] God can register with email + password
- [ ] God can log in and receive JWT
- [ ] Serilog structured logging

## ADO project
- Organisation: barbieezomo
- Project: God Games
- Process template: Basic (work item types: Epic, Issue, Task)
- Repo: God Games
- Sprints: Sprint 1 (24 Mar) → Sprint 5 (01 Jun 2026)

## Out of scope for MVP
- Multiplayer champion encounters
- Ascendance / godhood endgame
- Economy (gold, trading, crafting)
- Factions and god alliances
- Mobile app
- Push notifications
- Leaderboard
- Terraform / IaC (planned post-MVP)
