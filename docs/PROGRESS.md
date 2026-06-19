# HR-lite — Napredak

Posljednje ažuriranje: 2026-06-19

## Arhitektura

**Modularni monolit** (migracija završena 2026-06-19)

Projekt je prešao s mikroservisne na modularnu monolitnu arhitekturu. Razlog: team od 1 osobe, kompleksni cross-domain upiti, previše overhead-a za trenutnu fazu.

Novi API: `hr-lite-api/` (.NET 8 Web API, port 5000)
- Modul Codebook — `/api/codebook/*`
- Modul Identity — `/api/auth/*`
- Modul Employee, Leave, Org — prazni placeholderi

Stari servisi premješteni u `services/*_deprecated/` ili označeni s `_DEPRECATED.md`.

## Faze — pregled

| Faza | Naziv | Status |
|------|-------|--------|
| 1 | Osnovna infrastruktura | ✅ Završeno |
| 2 | Modularni monolit scaffold | ✅ Završeno |
| 3 | Codebook modul | ✅ Završeno |
| 4 | Identity modul | ✅ Završeno |
| 5 | Employee modul | 🔴 Nije početo |
| 6 | Org modul | 🔴 Nije početo |
| 7 | Leave modul | 🔴 Nije početo |
| 8 | Frontend nadogradnja | 🟡 Sidebar menu (završeno) |

## Implementirano

### Backend — hr-lite-api

- AppDbContext — merged codebook + identity (hr_codebook + hr_identity sheme)
- EF Core migracija: `Shared/Data/Migrations/InitialCreate`
- JWT Bearer autentikacija (HMAC SHA256, 1h expiry)
- Swagger UI na `/swagger`
- Admin seed pri prvom startu (admin@hr-lite.com / Admin123!)
- Docker: single container `hr-lite-api:5000`

### Codebook modul

- Gender CRUD + prijevodi (hr/en)
- Country CRUD + prijevodi (hr/en) + državljanstvo
- County CRUD + prijevodi + hierarhija (Country → County)
- Municipality CRUD + prijevodi + JOPPD kod
- Settlement CRUD + prijevodi + poštanski broj
- Soft delete (IsActive) za sve šifarnike
- Translation tablica — generički prijevodi

### Identity modul

- JWT login (`POST /api/auth/login`)
- BCrypt hashiranje lozinki
- Role: Admin, HR, Manager, Employee (seed)
- UserRole many-to-many

### Frontend

- React 18 + TypeScript + Vite + Ant Design
- Sidebar menu — data-driven, 46 stavki, filter, scroll
- Clickable breadcrumbs s rekurzivnim dropdownima
- i18n (hr/en) kroz i18next
- Light/dark mode toggle
- AG Grid za sve tablice šifarnika

## Docker Compose

3 servisa: `hr-lite-api:5000`, `postgres:5433`, `frontend:3000`

```
docker-compose up -d
```

API: http://localhost:5000/swagger
Frontend: http://localhost:3000
PgAdmin: http://localhost:5050

---

_Ovaj fajl se ažurira na kraju svake radne sesije._
