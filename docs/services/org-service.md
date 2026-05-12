# Org Service

**Status:** 🔴 Nije početo  
**Port:** 5003  
**DB schema:** `hr_org`

## Odgovornost

Hijerarhijska struktura organizacije — odjeljenja, timovi i reporting linije.

## Entiteti

### Department

| Polje | Tip | Napomena |
|-------|-----|----------|
| Id | Guid | PK |
| Name | string | |
| ParentDepartmentId | Guid? | Self-referencing hierarhija |
| ManagerEmployeeId | Guid? | Logički FK → employee-service |
| CreatedAt | DateTime | |

## Endpointi

| Metoda | Putanja | Napomena |
|--------|---------|----------|
| GET | `/api/departments` | Lista svih odjeljenja |
| GET | `/api/departments/{id}` | Detalji s pod-odjeljenjem |
| GET | `/api/org-chart` | Hijerarhijska struktura (stablo) |
| POST | `/api/departments` | Kreiranje odjeljenja |
| PUT | `/api/departments/{id}` | Izmjena |
| DELETE | `/api/departments/{id}` | Brisanje (samo prazna) |

## Zadaci

- [ ] .NET Core 8 projekt setup
- [ ] Department entitet + migracija
- [ ] GET /api/departments
- [ ] GET /api/org-chart (rekurzivno stablo)
- [ ] CRUD endpointi
- [ ] Dockerfile + docker-compose integracija
