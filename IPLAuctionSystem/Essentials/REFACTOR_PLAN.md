# IPL Auction System — Enterprise Refactor Plan

**Project:** IPLAuctionSystem  
**Target:** Professional enterprise-grade cricket auction platform  
**Approach:** Phased refactor, ascending complexity (quick wins → full rebuild)  
**Deployment phase:** Excluded from this plan

---

## Current State (Baseline)

- **Single project** ASP.NET Core MVC (.NET 10)
- **EF Core Code-First** + SQL Server, 2 entities (`Team`, `Player`)
- **SignalR hub** defined but never invoked
- **No authentication/authorization** (`UseAuthorization()` is a no-op)
- **No service/repository layer**, no DTOs, no validation framework
- **Fat controllers** with inline business logic and file I/O
- **No automated tests** (zero test project)
- **No REST API**, no Swagger/OpenAPI
- **Razor Views** with inline CSS/JS, duplicated `FormatCurrency()` helpers
- **Missing fallback images**, decimal precision bug (`Team.Budget` default mismatch)

---

## Goal State (Enterprise)

- **Multi-project layered solution:** Domain, Application, Infrastructure, Web, Tests
- **ASP.NET Core Identity** + JWT authentication, role-based authorization (`Admin`, `Auctioneer`, `FranchiseOwner`, `Viewer`)
- **Repository pattern** + Unit of Work, service layer, DTOs, FluentValidation, AutoMapper/Mapster
- **REST API** (versioned) + Swagger/OpenAPI documentation
- **Real-time auction engine** via SignalR (server-side bidding, countdown, persistence)
- **Modern frontend:** React SPA or enhanced Razor Views with Tag Helpers/View Components
- **Structured logging** (Serilog), health checks, security headers, global exception handling
- **New domain entities:** `AuctionSession`, `Bid`, `PlayerCategory`, `TeamBudget`, `Money` value object
- **Cricket-specific features:** Player categories (Batsman/Bowler/All-rounder/Wicketkeeper), franchise purse management, retained players, right-to-match cards, unsold player re-auction pool, auction rounds, sold/unsold/pending state machine

---

## Phase 1 — Stabilization & Quick Wins (Week 1)

*Low effort, high immediate value. Zero breaking changes to existing MVC routes.*

| # | Task | Details |
|---|------|---------|
| 1.1 | Fix remaining CS86xx nullable warnings | Null-forgiving operators `!`, null-coalescing `??`, explicit null checks in all Razor views (`Home/Index.cshtml`, `Players/Details.cshtml`, `Teams/Details.cshtml`, `Dashboard/Index.cshtml`, etc.) |
| 1.2 | Add structured logging | Install `Serilog.AspNetCore`, configure in `Program.cs`, sink to console + rolling file, add correlation IDs |
| 1.3 | Global exception handling middleware | Replace `/Home/Error` with middleware returning `ProblemDetails` for API and friendly views for MVC |
| 1.4 | Add health checks | `AddHealthChecks()` + `MapHealthChecks("/health")` for DB + app liveness |
| 1.5 | Add response compression | `AddResponseCompression()` middleware |
| 1.6 | Fix decimal precision bug | Align `Team.Budget` default with actual ₹100 Cr (`1000000000`) across model, code, and README |
| 1.7 | Add missing fallback images | Create `wwwroot/images/default-player.png` and `wwwroot/images/default-team.png` |
| 1.8 | Add `EnableRetryOnFailure` to SQL Server provider | Connection resiliency for transient SQL faults |
| 1.9 | Add security headers | `NWebsec` / CSP headers (X-Frame-Options, X-Content-Type-Options, Content-Security-Policy) |

**Exit criteria:** Warning-free build, structured logs, health endpoint responding, security headers present, budget math correct.

---

## Phase 2 — Layered Architecture Foundation (Weeks 2–3)

*Split the monolith into separate projects. Introduce repository + service layers, DTOs, validation, mapping.*

| # | Task | Details |
|---|------|---------|
| 2.1 | Create solution structure | 5 projects: `IPLAuctionSystem.Domain`, `IPLAuctionSystem.Application`, `IPLAuctionSystem.Infrastructure`, `IPLAuctionSystem.Web`, `IPLAuctionSystem.Tests` |
| 2.2 | Domain project | Entities (`Team`, `Player`, `AuctionSession`, `Bid`), enums (`AuctionStatus`, `PlayerCategory`), value objects (`Money`, `TeamBudget`), domain events (`PlayerSold`, `BidPlaced`, `AuctionStarted`, `AuctionEnded`). Zero EF/ASP.NET dependencies. |
| 2.3 | Application project | DTOs (`TeamDto`, `PlayerDto`, `BidDto`, `AuctionSessionDto`, `CreateTeamDto`, `CreatePlayerDto`), service interfaces (`ITeamService`, `IPlayerService`, `IAuctionService`, `IBidService`), command/query handlers, FluentValidation rules |
| 2.4 | Infrastructure project | EF Core `DbContext` (`ApplicationDbContext`), repositories (`EFTeamRepository`, `EFPlayerRepository`, `EFBidRepository`, `EFAuctionSessionRepository`), Unit of Work, file storage service (local + `IFileStorage` interface for cloud swap), migration configuration |
| 2.5 | Web project refactor | Controllers call services only (no direct `DbContext`), Razor Views use View Models/DTOs, AutoMapper/Mapster profiles, Tag Helpers for `FormatCurrency`, View Components for shared UI |
| 2.6 | Application project continued | Additional DTOs, service implementations, validation rules |
| 2.7 | Add AutoMapper/Mapster | Entity ↔ DTO mapping profiles for all entities |
| 2.8 | Add FluentValidation | Replace `[Bind]` + data annotations with explicit validators for all create/edit forms |
| 2.9 | Repository pattern | Generic `IRepository<T>` + specific `ITeamRepository`, `IPlayerRepository`, `IBidRepository`, `IAuctionSessionRepository` with Unit of Work |

**Exit criteria:** Clean separation of concerns, controllers are thin, business logic testable without web host, all existing MVC routes still functional.

---

## Phase 3 — Authentication & Authorization (Weeks 4–5)

*Enterprise security: identity, JWT, roles, policies.*

| # | Task | Details |
|---|------|---------|
| 3.1 | ASP.NET Core Identity | `ApplicationUser : IdentityUser`, `ApplicationDbContext : IdentityDbContext<ApplicationUser>`, registration, login, logout, password reset, email confirmation |
| 3.2 | JWT Bearer authentication | `AddAuthentication(JwtBearerDefaults...)`, token issuance endpoint (`/api/auth/login`, `/api/auth/register`), refresh token rotation |
| 3.3 | Role model | `Admin`, `Auctioneer`, `FranchiseOwner`, `Viewer`. Seed default roles + super-admin on startup migration |
| 3.4 | Policy-based authorization | `RequireRole`, `RequireClaim`, resource-based policies (`CanEditOwnTeam`, `CanRunAuction`, `CanViewFinancials`) |
| 3.5 | `[Authorize]` on all mutating controllers | Read-only actions open to `Viewer`; write actions to `Admin`/`Auctioneer`; auction control to `Auctioneer` only |
| 3.6 | Cookie policy | `SameSiteMode.Strict`, secure cookies in production, sliding expiration |
| 3.7 | Account management views | Login, Register, Forgot Password, Change Password, Profile, Manage Team (for FranchiseOwner) |
| 3.8 | User context in services | `ICurrentUserService` to inject authenticated user ID into audit fields and bid ownership |

**Exit criteria:** Fully authenticated app, role-based access enforced, no anonymous mutating operations, JWT tokens issued and validated.

---

## Phase 4 — REST API Layer (Weeks 6–7)

*Expose the domain via versioned, documented API for SPA/mobile/third-party clients.*

| # | Task | Details |
|---|------|---------|
| 4.1 | API controllers | `[ApiController]`, `[Route("api/v{version:apiVersion}/[controller]")]`, return `IActionResult` / `ProblemDetails` |
| 4.2 | API versioning | `Microsoft.AspNetCore.Mvc.Versioning`, support v1 with path-based versioning |
| 4.3 | Swagger/OpenAPI | `Swashbuckle.AspNetCore`, XML comments, bearer token auth in Swagger UI, `/swagger` endpoint |
| 4.4 | Global API exception handling | Middleware returns RFC 7807 `ProblemDetails` for all API errors |
| 4.5 | Rate limiting | `AspNetCoreRateLimit` per IP / per API key, auction endpoints rate-limited separately |
| 4.6 | Input sanitization | Anti-forgery tokens for MVC, model binding validation for API, no over-posting |
| 4.7 | API endpoints | `GET/POST/PUT/DELETE /api/v1/teams`, `/api/v1/players`, `/api/v1/auction/sessions`, `/api/v1/bids`, `/api/v1/auth/*` |

**Exit criteria:** `/api/v1/` endpoints functional, Swagger UI accessible, API versioning working, rate limiting active.

---

## Phase 5 — Real-Time Auction Engine (Weeks 8–10)

*Make SignalR the core of live bidding — the defining feature of a cricket auction app.*

| # | Task | Details |
|---|------|---------|
| 5.1 | Auction session state machine | `AuctionSession` entity: `Scheduled`, `Active`, `Paused`, `Completed`. Track current player, timer, winning bid, round number |
| 5.2 | Bid domain logic | `BidService.PlaceBid()` validates: player is pending/active, team has budget, bid > base price + minimum increment, optimistic concurrency (`RowVersion`) |
| 5.3 | Wire up SignalR hub | `AuctionHub`: `JoinAuction`, `PlaceBid`, `GetCurrentState`. Broadcasts: `ReceiveNewBid`, `ReceiveBidAccepted`, `ReceiveBidRejected`, `ReceiveAuctionEnded`, `ReceiveTimerUpdate` |
| 5.4 | Server-side countdown | Timer managed server-side (not client JS), broadcast `ReceiveTimerUpdate` every second, finalize on zero |
| 5.5 | Auto-sold / auto-unsold | Timer expiry finalizes sale to highest bidder or marks unsold, broadcasts result to all clients |
| 5.6 | Bid history persistence | `Bid` entity stored in DB, queryable per player/session/team |
| 5.7 | Outbid notifications | SignalR + email (Hangfire background job) when a user is outbid on their nominated player |
| 5.8 | Concurrency handling | `DbUpdateConcurrencyException` → reject stale bid, return current state with retry guidance |
| 5.9 | Auction rounds | Support multiple rounds (Round 1: capped players, Round 2: uncapped/re-auction pool, Round 3: unsold re-entry) |

**Exit criteria:** Live auction where bids are persisted server-side, validated, broadcast in real-time, countdown controlled by server, auction state machine transitions correctly.

---

## Phase 6 — Frontend Modernization (Weeks 11–13)

*Replace inline-CSS Razor views with a professional, responsive, cricket-themed UI.*

| # | Task | Details |
|---|------|---------|
| 6.1 | Choose frontend path | **Option A (Recommended):** React SPA + TypeScript + Vite + Tailwind CSS + React Query + SignalR JS client. **Option B:** Enhanced Razor Views with Tag Helpers, View Components, Alpine.js |
| 6.2 | UI component library | Reusable components: `DataTable`, `Modal`, `BidCard`, `PurseGauge`, `CountdownTimer`, `PlayerCard`, `TeamRoster`, `StatusBadge`, `CurrencyFormatter` |
| 6.3 | Cricket theme | IPL-style color palette (gold, navy, accent), team logo integration, player category icons, auction gavel animations |
| 6.4 | Responsive design | Mobile-first breakpoints, tablet/desktop layouts, touch-friendly bid buttons |
| 6.5 | Accessibility | WCAG 2.1 AA, keyboard navigation, ARIA labels, screen reader support |
| 6.6 | Live bid UI | Bid input with validation, real-time bid list, outbid alerts, timer display, current highest bidder highlight |
| 6.7 | Dashboard analytics | Interactive charts (Chart.js/Recharts): spending distribution, remaining purse per franchise, top acquisitions, category-wise spend |
| 6.8 | Remove inline CSS/JS | All styles in `wwwroot/css/` or Tailwind build, all JS in `site.js` or React components, no `<style>`/`<script>` blocks in views |
| 6.9 | Form handling | React Hook Form + Zod validation (Option A) or Tag Helpers + FluentValidation (Option B) |

---

## Phase 8 — Enterprise Cricket Features (Ongoing)

*Domain-specific features that make this a proper cricket auction app, not a generic CRUD tool.*

| # | Feature | Details |
|---|---------|---------|
| 8.1 | Player categories | Batsman, Bowler, All-rounder, Wicketkeeper with category-specific base price ranges |
| 8.2 | Franchise purse management | ₹100 Cr default purse, per-team budget tracking, unspent purse carry-over rules |
| 8.3 | Retained players | Pre-auction retention: teams can retain X players before auction starts, deducted from purse |
| 8.4 | Right-to-Match (RTM) cards | Franchise can match highest bid for their former player during auction |
| 8.5 | Unsold player pool | Players marked `Unsold` enter re-auction pool in later rounds with reduced base price |
| 8.6 | Auction rounds | Round 1 (capped players), Round 2 (uncapped/re-entry), Round 3 (unsold re-entry, accelerated timer) |
| 8.7 | Squad limits | Max 25 players per squad, max X overseas players, min/max per category |
| 8.8 | Bid increments | Configurable minimum bid increment per price slab |
| 8.9 | Auctioneer controls | Pause/Resume auction, force-sell, skip player, extend timer manually |
| 8.10 | Audit logging | EF Core interceptors + audit table (who changed what, when, old/new values) |
| 8.11 | Soft delete | `IsDeleted` flag on all entities, global query filters, restore capability |
| 8.12 | Reporting | Export auction results to PDF/Excel, franchise purse statements, category-wise spend reports |
| 8.13 | Caching | Redis `IDistributedCache` for team rosters, player catalogs, auction state |
| 8.14 | Background jobs | Hangfire for email/SMS notifications (outbid alerts, auction reminders, daily reports) |

**Exit criteria:** All cricket-specific auction rules implemented, app behaves like a real IPL auction platform.

---

## Execution Order & Dependencies

```
Phase 1 (Stabilization) → Phase 2 (Layering) → Phase 3 (Auth) → Phase 4 (API)
                                                       ↓
                                               Phase 5 (Real-time Auction)
                                                       ↓
                                               Phase 6 (Frontend)
                                                       ↓
                                               Phase 8 (Cricket Features)
```

**Critical path:** Phase 2 must complete before Phase 3 (services need interfaces). Phase 4 must complete before Phase 6 (frontend needs API). Phase 3 must complete before Phase 5 (bid ownership needs authenticated user).

**Do not:** Attempt frontend modernization (Phase 6) before API layer (Phase 4) exists. Do not attempt real-time bidding (Phase 5) before auth (Phase 3) and layering (Phase 2).

---

## Deliverables per Phase

- **Phase 1:** Warning-free build, Serilog logging, health endpoint, security headers, fixed budget decimal
- **Phase 2:** 5-project solution, Domain/Application/Infrastructure separation, DTOs, FluentValidation, AutoMapper, Repository + Unit of Work
- **Phase 3:** ASP.NET Core Identity, JWT auth, 4 roles, `[Authorize]` on all mutations, account views
- **Phase 4:** Versioned REST API (`/api/v1/`), Swagger UI, global `ProblemDetails`, rate limiting
- **Phase 5:** SignalR-powered live auction, server-side countdown, bid persistence, auction state machine, rounds
- **Phase 6:** Modern responsive frontend (React SPA or enhanced Razor), cricket theme, no inline CSS/JS, live bid UI
- **Phase 8:** Player categories, purse management, RTM cards, unsold pool, squad limits, audit logging, reporting

---

## Notes

- **No deployment phase included** per request (Docker, CI/CD, Kubernetes excluded).
- **Connection string:** Move to environment-variable-driven configuration (User Secrets in dev, Key Vault/Secrets Manager in prod).
- **Database:** SQL Server retained; migrations managed via EF Core in Infrastructure project.
- **Files:** Player profile pictures and team logos stored via `IFileStorage` abstraction (local disk now, cloud-swappable later).
- **Timezone:** Auction timers use UTC internally, displayed in local user timezone.
