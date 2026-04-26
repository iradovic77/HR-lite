# HR-lite — Napredak

## Status: Identity Service — u toku

Posljednje ažuriranje: 2026-04-26

---

## Faze

### Faza 1 — Osnovna infrastruktura
- [x] Definiranje arhitekture
- [x] Kreiranje strukture projekta
- [x] Docker Compose setup (PostgreSQL + identity-service)
- [ ] API Gateway (YARP) — osnovna konfiguracija
- [ ] CI/CD pipeline (GitHub Actions)

### Faza 2 — Identity Service
- [x] .NET Core 8 projekt setup
- [x] EF Core + PostgreSQL konekcija (shema: hr_identity)
- [x] User, Role, UserRole entiteti + EnsureCreated (seed: 4 role)
- [x] JWT login endpoint (POST /api/auth/login)
- [ ] JWT refresh token endpoint
- [ ] Role-based autorizacija (zaštićeni endpointi)

### Faza 3 — Employee Service
- [ ] .NET Core 8 projekt setup
- [ ] Employee entitet + migracija
- [ ] CRUD API endpoints
- [ ] JWT validacija (middleware)
- [ ] Audit log

### Faza 4 — Org Service
- [ ] Department / Team entiteti
- [ ] Org chart endpoint
- [ ] Linking zaposlenika s odjeljenjem

### Faza 5 — Leave Service
- [ ] LeaveType, LeaveRequest entiteti
- [ ] Tok odobrenja (submit → approve/reject)
- [ ] Bilans odsustva po zaposleniku

### Faza 6 — Document Service
- [ ] Metadata entitet
- [ ] File upload/download (lokalni storage)
- [ ] Vezanje dokumenta za zaposlenika

### Faza 7 — React Frontend
- [ ] Projekt setup (Vite + React + TypeScript)
- [ ] Routing struktura
- [ ] Auth flow (login, token refresh)
- [ ] Employee lista i detalji
- [ ] Leave request forma

---

## Blokatori

_Nema trenutnih blokatora._

## Napomene

_Ovaj fajl se ažurira ručno na kraju svake radne sesije._
