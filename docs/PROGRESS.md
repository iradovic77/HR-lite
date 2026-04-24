# HR-lite — Napredak

## Status: Inicijalizacija projekta

Posljednje ažuriranje: 2026-04-24

---

## Faze

### Faza 1 — Osnovna infrastruktura
- [x] Definiranje arhitekture
- [x] Kreiranje strukture projekta
- [ ] Docker Compose setup (PostgreSQL, servisi)
- [ ] API Gateway (YARP) — osnovna konfiguracija
- [ ] CI/CD pipeline (GitHub Actions)

### Faza 2 — Identity Service
- [ ] .NET Core 8 projekt setup
- [ ] EF Core + PostgreSQL konekcija
- [ ] User entitet + migracija
- [ ] JWT login / refresh endpoint
- [ ] Role-based autorizacija

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
