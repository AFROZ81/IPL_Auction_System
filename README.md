# IPL Auction System

A web-based **IPL player auction** application built with **ASP.NET Core MVC**. It lets admins manage franchises and players, run a live auction war room, broadcast real-time bids via SignalR, and track team purse spending.

## Tech Stack

- **.NET 10** / ASP.NET Core MVC (Razor views)
- **Entity Framework Core** (Code-First + Migrations) backed by **SQL Server**
- **ASP.NET Core SignalR** for real-time bid updates
- **Bootstrap** for UI

## Key Features

- **Players & Teams CRUD** — create, edit, delete, and browse players and franchises, including image uploads (player profile photos and team logos saved to `wwwroot/images`).
- **Auction War Room** — walks through players one at a time, sells to the highest-bidding team, or marks them unsold.
- **Real-Time Bidding** — bids are pushed to all connected viewers instantly through `AuctionHub`.
- **Budget Enforcement** — purchases run inside a DB transaction that validates and deducts from each team's remaining purse (default ₹100 Cr).
- **Dashboard** — overview of team rosters, remaining budgets, and the top-priced player.

## Project Structure

```
IPLAuctionSystem/
├── Controllers/      # MVC controllers (Home, Players, Teams, Auction, Dashboard)
├── Data/             # ApplicationDbContext (EF Core)
├── Hubs/             # AuctionHub (SignalR real-time bids)
├── Models/           # Entity models (Player, Team)
├── Migrations/       # EF Core migrations
└── Views/            # Razor views + wwwroot static assets
```

## Getting Started

### Prerequisites

- .NET 10 SDK
- SQL Server (or SQL Server Express / LocalDB)

### Setup

1. **Configure the connection string** in `IPLAuctionSystem/appsettings.json`:

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER;Database=IPLAuctionDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
   }
   ```

2. **Apply database migrations** (from the project directory):

   ```bash
   dotnet ef database update
   ```

3. **Run the app:**
   ```bash
   dotnet run
   ```

The app launches over HTTPS using the URL printed in the console (see `Properties/launchSettings.json`).

## Core Routes

| Route        | Description                                     |
| ------------ | ----------------------------------------------- |
| `/Home`      | Landing page with player feed and purse tracker |
| `/Players`   | Manage players (CRUD + sell)                    |
| `/Teams`     | Manage franchises (CRUD)                        |
| `/Auction`   | Live auction war room                           |
| `/Dashboard` | Summary of teams, budgets, and top buys         |

## How the Auction Works

1. The war room loads the next `Pending` player.
2. Teams place bids, which `AuctionHub` broadcasts to everyone in real time.
3. On **Sell**, the app checks the team's budget, deducts the winning amount, and records the player's sold price and team — all within a single transaction.
4. On **Pass**, the player is marked unsold and the next player loads.

## Developer Notes

- **Machine-specific connection string**: `appsettings.json` ships with the original developer's SQL Server instance (`Server=LRMC-20-PC\SQLEXPRESS01;...`). Every contributor must edit `DefaultConnection` to point at their own SQL Server / Express / LocalDB instance before running the app.
- **`AuctionStatus` enum**: Each `Player` carries an `AuctionStatus` of `Pending`, `Sold`, or `Unsold` (replacing the old boolean `IsSold`, which was ambiguously set to `true` for both sold _and_ passed players). The war room surfaces the next `Pending` player; **Sell** sets `Sold` (with team + price) and **Pass** sets `Unsold` (clearing team + price). `Unsold` players can still be re-auctioned.
