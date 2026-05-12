# Employee Service

**Status:** 🔴 Nije početo  
**Port:** 5002  
**DB schema:** `hr_employee`

## Odgovornost

Centralni registar zaposlenika. Sadrži lične i poslovne podatke, radno mjesto, ugovorni status i historiju promjena.

## Entiteti

### Employee

| Polje | Tip | Napomena |
|-------|-----|----------|
| Id | Guid | PK |
| FirstName | string | |
| LastName | string | |
| Email | string | Unique |
| Phone | string? | |
| DateOfBirth | DateOnly? | |
| HireDate | DateOnly | |
| TerminationDate | DateOnly? | Null = aktivan |
| EmploymentType | enum | FullTime, PartTime, Contract |
| JobTitle | string | |
| DepartmentId | Guid? | FK → org-service (logički, ne FK u bazi) |
| ManagerId | Guid? | FK → Employee.Id |
| IsActive | bool | Computed ili eksplicitno |
| CreatedAt | DateTime | |
| UpdatedAt | DateTime | |

### EmployeeHistory

Audit log promjena na Employee entitetu (ko je promijenio, šta, kada).

| Polje | Tip | Napomena |
|-------|-----|----------|
| Id | Guid | PK |
| EmployeeId | Guid | FK |
| ChangedByUserId | Guid | iz JWT claims |
| ChangedAt | DateTime | |
| FieldName | string | Naziv promijenjenog polja |
| OldValue | string? | JSON serializirano |
| NewValue | string? | JSON serializirano |

## Endpointi

| Metoda | Putanja | Uloge | Napomena |
|--------|---------|-------|----------|
| GET | `/api/employees` | HR, Admin, Manager | Paginacija + filteri |
| GET | `/api/employees/{id}` | HR, Admin, Manager, sam zaposlenik | |
| POST | `/api/employees` | HR, Admin | Kreiranje |
| PUT | `/api/employees/{id}` | HR, Admin | Puna izmjena |
| PATCH | `/api/employees/{id}` | HR, Admin | Parcijalna izmjena |
| DELETE | `/api/employees/{id}` | Admin | Soft delete (IsActive = false) |
| GET | `/api/employees/{id}/history` | HR, Admin | Audit log promjena |

## Zadaci

- [ ] .NET Core 8 projekt setup
- [ ] EF Core + PostgreSQL (schema: hr_employee)
- [ ] Employee entitet + migracija
- [ ] EmployeeHistory entitet + migracija
- [ ] JWT validacija middleware (dijeli shared logiku s identity-service)
- [ ] GET /api/employees (paginacija, pretraga po imenu/odjeljenju)
- [ ] GET /api/employees/{id}
- [ ] POST /api/employees
- [ ] PUT /api/employees/{id}
- [ ] DELETE /api/employees/{id} (soft delete)
- [ ] GET /api/employees/{id}/history
- [ ] Dockerfile
- [ ] Dodati u docker-compose.yml

## Napomene

- Servis ne čuva lozinku — identitet je u identity-service, ovdje je samo profil
- `DepartmentId` je logički FK prema org-service; nema DB foreign key između shema
- Sve izmjene automatski snimaju unos u `EmployeeHistory`
- Zaposlenik može čitati samo vlastiti profil; HR/Admin može sve
