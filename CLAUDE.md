# HR-lite — Instrukcije za razvoj

Ovaj fajl definira pravila i konvencije kojih se Claude mora pridržavati pri razvoju HR-lite projekta.
Pravila su obavezna osim ako nije eksplicitno drugačije navedeno.

---

## Arhitektura i granice servisa

- Svaki mikroservis je autonoman i vlasnički nad svojom PostgreSQL shemom (`hr_identity`, `hr_employee`, `hr_org`, `hr_leave`, `hr_document`).
- **Nikad ne koristiti DB foreign key-eve između shema.** Veze između servisa su logičke i provode se na aplikativnom nivou putem ID-eva.
- Direktni HTTP pozivi između servisa su dozvoljeni samo gdje je neophodna sinhrona konzistentnost. Svaki takav poziv mora biti dokumentovan u `docs/services/<ime-servisa>.md`.

---

## Identitet vs. profil zaposlenika

- **Autentikacijski podaci (lozinka, refresh token, uloge, permisioni) nikad ne idu u `employee-service`.**
- `identity-service` čuva: `User` (kredencijali, uloge), `Role`, `UserRole`, `Permission`, `RolePermission`.
- `employee-service` čuva: profil zaposlenika (ime, pozicija, odjeljenje, ugovor...).
- Veza između ta dva servisa je isključivo putem `EmployeeId` koji se pohranjuje u `hr_identity.Users` kao logički FK.

---

## Baza podataka

### Sheme

Svaki servis koristi zasebnu PostgreSQL shemu:

| Servis | Shema |
|--------|-------|
| identity-service | `hr_identity` |
| employee-service | `hr_employee` |
| org-service | `hr_org` |
| leave-service | `hr_leave` |
| document-service | `hr_document` |

### Audit kolone

**Svaka tablica mora imati sljedeće kolone:**

```csharp
public DateTime CreatedAt  { get; set; }
public DateTime UpdatedAt  { get; set; }
public Guid     CreatedBy  { get; set; }  // UserId iz JWT claims
public Guid     UpdatedBy  { get; set; }  // UserId iz JWT claims
```

Postavljaju se automatski u `DbContext.SaveChanges()` override-u — nikad ručno u kontrolerima ili servisima.

### Povijesni podaci

Entiteti koji zahtijevaju praćenje povijesti promjena (npr. `EmployeeHistory`) koriste **ValidFrom / ValidTo** obrazac:

```csharp
public DateTime  ValidFrom  { get; set; }
public DateTime? ValidTo    { get; set; }  // null = trenutno aktivan zapis
```

Novi zapis se kreira pri svakoj promjeni; prethodni dobiva `ValidTo = DateTime.UtcNow`.

### Audit log

- **Svaka promjena podataka mora se logirati u `AuditLog` tablicu kroz aplikacijski sloj.**
- Logiranje se implementira u servisnom sloju (ne u kontroleru, ne u repozitoriju).
- `AuditLog` tablica bilježi: `EntityName`, `EntityId`, `Action` (Created/Updated/Deleted), `ChangedByUserId`, `ChangedAt`, `OldValues` (JSON), `NewValues` (JSON).
- **Nikad koristiti database triggere za audit log** — promjena mora biti vidljiva i testabilna u C# kodu.

### Zabrane u bazi

- **Nikad stored procedures.** Sva poslovna logika je u C# kodu.
- **Nikad triggers.** Reakcije na promjene podataka implementiraju se u aplikativnom sloju.
- Nikad pogledi (views) koji enkapsuliraju poslovnu logiku.

---

## Pristup bazi — Repository pattern

Svaki servis koristi Repository pattern:

```
Services/<ime-servisa>/
  Repositories/
    IEmployeeRepository.cs   ← interface
    EmployeeRepository.cs    ← EF Core implementacija
```

- Kontroleri ne pristupaju `DbContext`-u direktno — uvijek kroz repozitorij.
- Repozitoriji ne sadrže poslovnu logiku — samo CRUD i upite.
- Poslovna logika ide u zasebni servisni sloj (`Services/EmployeeService.cs`).

---

## API dizajn

### DTOs

- **Svaki ulaz i izlaz iz API-ja mora biti DTO** — nikad direktno izlagati entitete.
- Konvencija imenovanja:

```
CreateEmployeeRequest   ← ulaz za kreiranje
UpdateEmployeeRequest   ← ulaz za izmjenu
EmployeeResponse        ← izlaz (jedan zapis)
EmployeeListResponse    ← izlaz (lista, s paginacijom)
```

- DTOs su u `DTOs/` folderu unutar projekta servisa.
- Mapiranje između entiteta i DTO-a radi se u servisnom sloju (bez AutoMapper-a osim ako nije eksplicitno dogovoreno).

### Paginacija

Svaki list endpoint mora podržavati paginaciju:

```json
{
  "data": [...],
  "page": 1,
  "pageSize": 20,
  "totalCount": 150
}
```

### Swagger

- **Swagger dokumentacija je obavezna za sve endpointe.**
- Svaki endpoint mora imati `[ProducesResponseType]` atribute za sve moguće HTTP statuse.
- Koristiti XML komentare (`/// <summary>`) za opis endpointa i parametara.
- Swagger UI dostupan na `/swagger` u development okruženju.

---

## Autorizacija — RBAC

- **Uloge i permisioni se nikad ne hardkodiraju u kodu.**
- Uloge i permisioni su dinamički — definirani u bazi (`hr_identity` shema) i učitavaju se iz JWT claims.
- Provjera permisiona se radi putem policy-based autorizacije u .NET-u, ne putem `if (role == "Admin")` provjera.

### Struktura permisiona

Permisioni se definišu kao `resurs:akcija`:

```
employees:read
employees:write
employees:delete
employees:history:read
departments:read
leave-requests:approve
documents:upload
```

### Employee service — grupe podataka kao resursi

Podaci u `employee-service` su grupisani u permission resurse:

| Grupa | Permission resurs | Primjer polja |
|-------|-------------------|---------------|
| Lični podaci | `employees:personal` | Ime, prezime, datum rođenja, kontakt |
| Ugovorni podaci | `employees:contract` | Tip ugovora, datum zapošljavanja, plata |
| Organizacijski podaci | `employees:org` | Odjeljenje, rukovodilac, pozicija |
| Historija promjena | `employees:history` | Audit log svih izmjena |

Servis provjerava permission za konkretnu grupu podataka, ne samo za entitet u cjelini.
Npr. Manager može čitati `employees:org`, ali ne i `employees:contract`.

---

## Testiranje

- **Svaki servis mora imati projekt za unit testove** (`<ServisName>.Tests`).
- Minimalno pokriće: svi servisi (business logic sloj) i repozitoriji (mock DbContext).
- Koristiti **xUnit** + **Moq**.
- Integracijski testovi su poželjni ali nisu obavezni u prvoj iteraciji.
- Test projekat smješten u: `services/<ime-servisa>/<ime-servisa>.Tests/`

---

## Struktura projekta (konvencija)

```
services/<ime-servisa>/
  Controllers/        ← HTTP sloj, bez logike
  DTOs/               ← request/response modeli
  Models/             ← EF Core entiteti
  Data/               ← DbContext, migracije
  Repositories/       ← interfejsi + implementacije
  Services/           ← poslovna logika
  Middleware/         ← JWT validacija, error handling
  <ServisName>.csproj
  Program.cs
  appsettings.json
  appsettings.Development.json
  Dockerfile
<ime-servisa>.Tests/
  Services/           ← unit testovi za servisni sloj
  Repositories/       ← unit testovi za repozitorije
```

---

## Docker i okruženje

- Svaki servis ima vlastiti `Dockerfile` (multi-stage build).
- Lokalni razvoj pokreće se isključivo putem `docker-compose.yml` iz roota.
- Connection stringovi i tajni idu u `appsettings.Development.json` ili Docker environment varijable — nikad u `appsettings.json` koji ide u Git.
- `.env` fajlovi se ne commituju; postoji `.env.example` kao predložak.

---

## Ažuriranje dokumentacije

- Nakon svake sesije razvoja ažurirati `docs/PROGRESS.md` i odgovarajući `docs/services/<ime>.md`.
- Arhitekturne odluke dokumentovati u `docs/DECISIONS.md` po ADR formatu.
- `CLAUDE.md` se ažurira samo kada se mijenjaju temeljna pravila projekta.

  
## Buduće funkcionalnosti
- Impersonation (Act As): Admin mora moći pregledavati 
  sustav "kao" drugi korisnik. Audit log uvijek bilježi 
  stvarnog korisnika + impersoniranog korisnika.
  Implementira se kroz JWT claims: 'sub' (stvarni), 
  'acting_as' (impersonirani).
