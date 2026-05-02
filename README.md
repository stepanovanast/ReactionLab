# ReactionLab

ReactionLab is an interactive chemistry education platform where students explore chemical reactions through step-by-step 3D molecular visualizations. Topics unlock sequentially as students complete each one, and a badge system rewards progress and curiosity along the way.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Frontend | Angular 18, TypeScript 5.4, Bootstrap 5 |
| 3D Visualizations | Three.js 0.183 |
| Backend | ASP.NET Core, Entity Framework Core |
| Database | PostgreSQL (via Npgsql) |
| Authentication | JWT Bearer tokens |
| API Docs | Swagger / Swashbuckle |
| Tests | xUnit |

---

## Features

- **4 chemistry topics** with sequential unlocking — complete one to unlock the next
- **3D molecular visualizations** built with Three.js, showing atoms, bonds, and electron transfers step by step
- **8 collectible badges** awarded for completing topics and engaging with the material
- **User accounts** — sign up with a custom avatar, log in, track your progress
- **Confetti celebration** when you earn a new badge
- **Responsive design** — works on desktop and mobile
- **Public pages** — Tutorials, FAQ, Contacts, Terms of Use, Privacy Policy
- **404 page** for unrecognized routes

---

## Topics and Reactions

| # | Topic | Equation |
|---|---|---|
| 1 | Iron sulfide formation | 8 Fe + 8 S → 8 FeS |
| 2 | Production of hydrogen gas | Fe + 2HCl → FeCl₂ + H₂ |
| 3 | Glowing splint test | 2H₂O₂ → 2H₂O + O₂ |
| 4 | Sodium-water reaction | 2Na + 2H₂O → 2NaOH + H₂ |

Each topic contains one reaction broken into multiple steps. Each step shows a different molecular arrangement, temperature range, and narrative description of what is happening at the atomic level.

---

## Badge System

| Badge | How to earn |
|---|---|
| Early Bird | Create an account |
| Bronze Chemist | Complete your first topic |
| Silver Alchemist | Complete your second topic |
| Gold Veteran | Complete your third topic |
| Chemistry Champion | Complete all four topics |
| Molecular Vision | Switch the visualization mode for the first time |
| Full Circuit | Reach the final step of any reaction |
| Curious Mind | Revisit a reaction you have already completed |

Badges are displayed on the Topics dashboard. Click any earned badge to flip it and read its description.

---

## Project Structure

```
ReactionLab/
├── ReactionLab.Frontend/
│   └── ReactionLab/               # Angular 18 application
│       └── src/app/
│           ├── app/               # Navbar component
│           ├── auth/              # Login and Signup pages
│           ├── guards/            # auth, guest, topic-access route guards
│           ├── home/              # Home page sections and static pages
│           │   ├── herosection/
│           │   ├── featuressection/
│           │   ├── librarysection/
│           │   ├── whysection/
│           │   ├── marquee/
│           │   ├── footer/
│           │   ├── docs/          # Tutorials page
│           │   ├── faq/
│           │   ├── contacts/
│           │   ├── termsofuse/
│           │   └── privacypolicy/
│           ├── learning/
│           │   ├── topics/        # Dashboard with topic cards and badges
│           │   └── visualizations/# 3D reaction viewer
│           ├── notfound/          # 404 page
│           ├── services/          # API, auth, toast, confetti services
│           └── shared/            # Shared directives (fade-in on scroll)
└── ReactionLab.Backend/
    ├── ReactionLab.API/           # ASP.NET Core Web API
    │   ├── Controllers/           # Auth, Topics, User, Reactions
    │   └── Services/              # AuthService, BadgeService
    ├── ReactionLab.Data/          # EF Core DbContext, models, migrations
    └── ReactionLab.Tests/         # xUnit test project
```

---

## Getting Started

### Prerequisites

- [Node.js](https://nodejs.org/) (v18 or later) and npm
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) (recommended for the database — see below)

---

### Database (PostgreSQL in Docker)

ReactionLab uses PostgreSQL as its database. Rather than requiring everyone to install and configure PostgreSQL locally, the project expects it to run inside a Docker container. Docker gives every developer the exact same database environment regardless of their OS, with no manual installation or version conflicts.

The backend is configured to connect to PostgreSQL on port **5433** (not the default 5432 — this avoids clashing with any existing local PostgreSQL installation you might have).

**Start the database:**

```bash
docker run -d \
  --name reactionlab-db \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=reactionlab \
  -p 5433:5432 \
  postgres:16
```

Or, if you prefer Docker Compose, a `docker-compose.yml` is included at the project root:

```bash
docker compose up -d
```

Once the container is running you can stop and restart it without losing data:

```bash
docker stop reactionlab-db    # stop
docker start reactionlab-db   # start again
```

To connect to the database directly (e.g. with psql or a GUI like TablePlus / pgAdmin):

| Field | Value |
|---|---|
| Host | `localhost` |
| Port | `5433` |
| Database | `reactionlab` |
| Username | `postgres` |
| Password | `postgres` |

> The schema is managed by EF Core migrations. You do not need to create tables or run any SQL manually — the backend does this automatically on startup.

---

### Backend

```bash
cd ReactionLab.Backend/ReactionLab.API
dotnet run
```

On first run, EF Core migrations are applied automatically and the database is seeded with topics, reactions, and badges. A default admin account is created at `admin@reactionlab.com` / `admin`.

Swagger UI is available at `http://localhost:{port}/swagger` in development.

### Frontend

```bash
cd ReactionLab.Frontend/ReactionLab
npm install
ng serve
```

The app runs at `http://localhost:4200`.

---

## Configuration

Backend configuration lives in `ReactionLab.Backend/ReactionLab.API/appsettings.json`.

| Key | Default | Description |
|---|---|---|
| `ConnectionStrings:DefaultConnection` | `Host=localhost;Port=5433;Database=reactionlab;Username=postgres;Password=postgres` | PostgreSQL connection string |
| `Jwt:Key` | *(change before deploying)* | Secret key for signing JWT tokens |
| `Jwt:ExpirationMinutes` | `60` | Token lifetime |
| `Admin:Email` | `admin@reactionlab.com` | Seeded admin account email |
| `Admin:Password` | `admin` | Seeded admin account password |

---

## API Reference

All endpoints are prefixed with `/api`.

### Auth

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/auth/signup` | — | Create a new account. Returns JWT token. |
| POST | `/auth/login` | — | Log in. Returns JWT token. |
| POST | `/auth/logout` | — | Acknowledge logout (client clears token). |

**Signup request body:**
```json
{ "name": "string", "email": "string", "password": "string", "avatarId": 1 }
```

**Login request body:**
```json
{ "email": "string", "password": "string" }
```

Both return:
```json
{ "token": "JWT string", "user": { ... } }
```

---

### Topics

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/topics` | Optional | List all topics with status (`locked`, `available`, `completed`) for the current user. Supports `?search=` query param. |
| GET | `/topics/{id}` | — | Get a single topic including its reactions and step data. |
| POST | `/topics` | Admin | Create a topic. |
| PUT | `/topics/{id}` | Admin | Update a topic. |
| DELETE | `/topics/{id}` | Admin | Delete a topic. |
| POST | `/topics/{id}/reactions` | Admin | Add a reaction to a topic. |

---

### User

All user endpoints require a valid JWT token in the `Authorization: Bearer <token>` header.

| Method | Endpoint | Description |
|---|---|---|
| GET | `/user/profile` | Get the current user's name, email, and account creation date. |
| GET | `/user/badges` | Get all badges with `earned: true/false` for the current user. |
| PUT | `/user/progress` | Update progress on a topic. Completing a topic unlocks the next one and awards the relevant badge. |
| POST | `/user/badges/molecular-vision` | Award Molecular Vision badge. |
| POST | `/user/badges/full-circuit` | Award Full Circuit badge. |
| POST | `/user/badges/curious-mind` | Award Curious Mind badge. |
| DELETE | `/user` | Delete the current user's account. |

**Progress request body:**
```json
{ "topicId": 1, "status": "completed" }
```

---

## Route Guards

| Guard | Behavior |
|---|---|
| `authGuard` | Redirects unauthenticated users to `/login` |
| `guestGuard` | Redirects already-authenticated users to `/topics` |
| `topicAccessGuard` | Checks the topic is not locked before allowing access to `/visualizations/:topicId`; redirects to `/topics` if locked |

---

## Pages

| Route | Title | Access |
|---|---|---|
| `/` | ReactionLab | Public |
| `/login` | Log in | Guests only |
| `/signup` | Sign up | Guests only |
| `/topics` | My dashboard | Auth required |
| `/visualizations/:topicId` | *(topic title)* | Auth + topic unlocked |
| `/tutorials` | Tutorials | Public |
| `/faq` | FAQ | Public |
| `/contacts` | Contacts | Public |
| `/terms-of-use` | Terms of use | Public |
| `/privacy-policy` | Privacy policy | Public |
| `/**` | 404 | Public |
