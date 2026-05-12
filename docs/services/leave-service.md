# Leave Service

**Status:** 🔴 Nije početo  
**Port:** 5004  
**DB schema:** `hr_leave`

## Odgovornost

Upravljanje tipovima odsustva, zahtjevima i tokom odobrenja. Prati bilans dana po zaposleniku.

## Entiteti

### LeaveType

| Polje | Tip | Napomena |
|-------|-----|----------|
| Id | Guid | PK |
| Name | string | Godišnji odmor, Bolovanje, Slobodan dan... |
| DefaultDaysPerYear | int? | Null = neograničeno |
| RequiresApproval | bool | |

### LeaveRequest

| Polje | Tip | Napomena |
|-------|-----|----------|
| Id | Guid | PK |
| EmployeeId | Guid | Logički FK → employee-service |
| LeaveTypeId | Guid | FK |
| StartDate | DateOnly | |
| EndDate | DateOnly | |
| Status | enum | Pending, Approved, Rejected, Cancelled |
| RequestedAt | DateTime | |
| ReviewedByUserId | Guid? | Rukovodilac koji je odobrio/odbio |
| ReviewedAt | DateTime? | |
| Note | string? | Komentar zaposlenika ili rukovodioca |

### LeaveBalance

| Polje | Tip | Napomena |
|-------|-----|----------|
| Id | Guid | PK |
| EmployeeId | Guid | |
| LeaveTypeId | Guid | |
| Year | int | |
| TotalDays | int | |
| UsedDays | int | |
| RemainingDays | int | Computed |

## Tok odobrenja

```
Zaposlenik podnosi (Pending)
    → Rukovodilac odobrava (Approved) ili odbija (Rejected)
    → HR evidentira (opciono, za bolovanje i sl.)
```

## Endpointi

| Metoda | Putanja | Napomena |
|--------|---------|----------|
| GET | `/api/leave-types` | Lista tipova odsustva |
| GET | `/api/leave-requests` | Filtriranje po zaposleniku/statusu |
| POST | `/api/leave-requests` | Podnošenje zahtjeva |
| PUT | `/api/leave-requests/{id}/approve` | Odobravanje |
| PUT | `/api/leave-requests/{id}/reject` | Odbijanje |
| PUT | `/api/leave-requests/{id}/cancel` | Otkazivanje (sam zaposlenik) |
| GET | `/api/leave-balance/{employeeId}` | Bilans po zaposleniku |

## Zadaci

- [ ] .NET Core 8 projekt setup
- [ ] LeaveType, LeaveRequest, LeaveBalance entiteti + migracije
- [ ] Seed: osnovni tipovi odsustva
- [ ] CRUD za LeaveRequest
- [ ] Tok odobrenja endpointi
- [ ] Bilans izračun
- [ ] Dockerfile + docker-compose integracija
