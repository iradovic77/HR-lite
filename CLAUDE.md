# HR-lite — Instrukcije za razvoj

Ovaj fajl definira pravila i konvencije kojih se Claude mora pridržavati pri razvoju HR-lite projekta.
Pravila su obavezna osim ako nije eksplicitno drugačije navedeno.

---

## Kritična pravila — baza podataka

- **Nikad ne pokretati `dotnet ef database update` automatski.** Uvijek generirati migraciju, prikazati SQL preview (`dotnet ef migrations script`), i čekati eksplicitnu potvrdu korisnika prije primjene.
- **Nikad ne dropati tablice, sheme ili bazu bez eksplicitne potvrde korisnika.**
- Svaka destructive operacija (drop, truncate, recreate, seed brisanje) mora biti jasno najavljena s opisom što će se izgubiti — i čekati potvrdu.
- **Migracije se generiraju, ne primjenjuju.** Korisnik sam pokreće `database update` kad je spreman.

---

## Arhitektura

**Modularni monolit** — jedan .NET 8 Web API projekt (`hr-lite-api/`) s modulima po poslovnim domenama.

Stara mikroservisna arhitektura zamijenjena 2026-06-19 (vidi ADR-007). Stari servisi su u `services/*_deprecated/`.

### Struktura projekta

```
hr-lite-api/
  Modules/
    Codebook/    ← šifarnici (controllers, services, repositories, models, dtos)
    Identity/    ← autentikacija/autorizacija
    Employee/    ← placeholder
    Leave/       ← placeholder
    Org/         ← placeholder
  Shared/
    Data/        ← AppDbContext, Migrations
    Models/      ← IAuditable, BaseEntity
    Middleware/  ← error handling, JWT middleware
```

### Granice modula

- Moduli komuniciraju kroz zajednički `AppDbContext` (ista PostgreSQL instanca)
- **Nikad ne koristiti DB foreign key-eve između shema.** Veze su logičke, provode se na aplikativnom nivou putem ID-eva.
- Nema HTTP poziva između modula — isti proces, isti DbContext

---

## Arhitekturalne odluke
- EF Core za standardne CRUD operacije
- Raw SQL za kompleksne upite i izvještaje
- ANSI SQL standard gdje god moguće radi portabilnosti
- Stored procedure se NE koriste
- Impersonation (Act As) funkcionalnost planirana za identity-service:
  * Admin može pregledavati sustav "kao" drugi korisnik
  * Audit log uvijek bilježi stvarnog korisnika + impersoniranog
  * Implementacija kroz JWT claims: sub (stvarni korisnik), acting_as (impersonirani)

--- 

## Identitet vs. profil zaposlenika

- **Autentikacijski podaci (lozinka, refresh token, uloge, permisioni) nikad ne idu u `employee-service`.**
- `identity-service` čuva: `User` (kredencijali, uloge), `Role`, `UserRole`, `Permission`, `RolePermission`.
- `employee-service` čuva: profil zaposlenika (ime, pozicija, odjeljenje, ugovor...).
- Veza između ta dva servisa je isključivo putem `EmployeeId` koji se pohranjuje u `hr_identity.Users` kao logički FK.

---

## Baza podataka
- Jedna PostgreSQL instanca, više shema
- Mogućnost migracije na drugu bazu (SQL Server...) u budućnosti
- Zbog portabilnosti izbjegavati PostgreSQL-specifične funkcije gdje god moguće
  
### Sheme

Svaki servis koristi zasebnu PostgreSQL shemu:

| Servis | Shema |
|--------|-------|
| identity-service | `hr_identity` |
| employee-service | `hr_employee` |
| org-service | `hr_org` |
| leave-service | `hr_leave` |
| document-service | `hr_document` |
| codebook-service | `hr_codebook` |

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

### Šifarnici (codebook tablice)

- **Šifarnici se nikad ne brišu fizički** — samo deaktiviraju postavljanjem `IsActive = false` (soft delete).
- Svaka šifarnik tablica **obavezno** ima kolone `IsActive` (bool, default: `true`) i `Ordinal` (int, default: `0`).
  - `IsActive = false` → zapis je neaktivan i **ne prikazuje se** u combo-ima / dropdown-ima.
  - `Ordinal` → custom redoslijed prikaza u combo-ima; niži broj = više u listi.
- API endpointi za šifarnike po defaultu vraćaju samo aktivne zapise; query param `?includeInactive=true` vraća sve.
- Seed podaci za standardne šifarnike idu kroz **EF Core migracije** (`HasData`) — ne kroz SQL skripte.
- Sistemski seed zapisi koriste `CreatedBy = Guid.Empty` kao oznaku da ih je kreirao sistem, ne korisnik.
- Hijerarhija lokacija: Država → Županija → Općina → Naselje
- Relacije između lokacijskih šifarnika nisu obavezne — pouzdane samo za Hrvatsku inicijalno

### Raw SQL

- Kad je raw SQL neophodan (rijetki slučajevi u migracijama ili složenim upitima), pisati što standardniji **ANSI SQL** — izbjegavati PostgreSQL-specifičnu sintaksu gdje god je moguće.
- Cilj: lakša eventualna migracija na drugu bazu bez masovnih izmjena SQL-a.
- Primjer: koristiti `COALESCE` umjesto `ISNULL`, standardne JOIN-ove, izbjegavati `::` cast operator.

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

## Frontend

- **Framework:** React 18 + TypeScript
- **Build tool:** Vite
- Ant Design kao UI framework
- Podrška za svjetlu i tamnu temu (light/dark mode toggle u topbaru)
- React + TypeScript + Vite
- **HTTP klijent:** Axios
- **Routing:** React Router v6
- **i18n:** i18next + react-i18next

### Teme (light/dark mode)
- Ant Design `ConfigProvider` s `theme.darkAlgorithm` / `theme.defaultAlgorithm`
- Toggle u topbaru — stanje persistirano u `localStorage`

### Višejezičnost (UI)
- Jezik se bira kroz **dropdown (combo)**, ne kroz tipke
- Razlog: jezici će se dodavati naknadno — dropdown skalira bolje od tipki
- Implementacija: i18next + react-i18next
- Početni jezici: `hr` (default), `en`
- Odabrani jezik persistiran u `localStorage`

---

## Višejezičnost — Translation sustav
Višejezičnost se implementira kroz generički Translation sustav. **Nikad ne dodavati `Name`, `NameEn`, `NameDe`... kolone** u tablice — to ne skalira.

## Višejezičnost — detalji implementacije
- Generički Translation sustav kroz Language i Translation tablice (NE kolone NameHr/NameEn)
- Language tablica: Code (hr, en...) kao PK, Name (Hrvatski, English...)
- Translation tablica: Id, EntityType, EntityId, LanguageCode, FieldName, Value
- Hrvatski prijevod je OBAVEZAN za sve unose — bez njega se ne može spremiti
- Fallback logika: traženi jezik → hrvatski → Code
- Code kolona je neutralni identifikator, nikad se ne prevodi
- Frontend i18n: JSON format s hijerarhijskom strukturom po modulima (common, codebook, employee, errors...)
- Frontend odabir jezika kroz dropdown (combo), ne kroz tipke
### Tablice

**`language`** — podržani jezici:

| Kolona | Napomena |
|--------|----------|
| `Code` | ISO 639-1 — `hr`, `en`, `de` — **PK** |
| `Name` | Naziv na vlastitom jeziku |
| + audit kolone | |

**`translation`** — generička tablica prijevoda za SVE entitete:

| Kolona | Napomena |
|--------|----------|
| `Id` | Guid, PK |
| `EntityType` | Naziv tablice — identičan PostgreSQL nazivu (npr. `codebook_gender`) |
| `EntityId` | Id retka koji se prevodi |
| `LanguageCode` | FK → `language.Code` |
| `FieldName` | Polje koje se prevodi — npr. `Name` |
| `Value` | Prevedeni tekst |
| + audit kolone | |

### Pravila

- **Hrvatski (`hr`) prijevod je OBAVEZAN** za sve šifarnike i prevedive entitete.
- Ostali jezici su opcionalni.
- **Fallback logika** (implementira se u servisnom sloju): traženi jezik → `hr` → `Code`.
- `Code` kolona entiteta je neutralni identifikator — **nikad se ne prevodi**.
- Novi jezik = jedan redak u `language` tablici. **Bez izmjene sheme** ostalih tablica.
- Seed data za prijevode ide kroz EF Core migracije (`HasData` na `Translation` entitetu).
- **`EntityType` u `translation` tablici mora biti identičan PostgreSQL nazivu tablice entiteta** (npr. `codebook_country`, `codebook_gender`). Nikad kratke forme, aliasi ili drugačiji formati. Fallback logika koja prihvaća alternativne vrijednosti nije dozvoljena.

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

- `hr-lite-api` ima vlastiti `Dockerfile` (multi-stage .NET 8 build).
- Lokalni razvoj pokreće se putem `docker-compose.yml` iz roota (3 containera: hr-lite-api:5000, postgres:5433, frontend:3000).
- Connection stringovi i tajni idu u `appsettings.Development.json` ili Docker environment varijable — nikad u `appsettings.json` koji ide u Git.

### Poredak pokretanja

```
docker-compose up -d
```

- postgres zdravi → hr-lite-api starta → `db.Database.Migrate()` + admin seed → API dostupan
- API: http://localhost:5000/swagger
- Frontend: http://localhost:3000

---

## Ažuriranje dokumentacije

- Nakon svake sesije razvoja ažurirati `docs/PROGRESS.md` i odgovarajući `docs/services/<ime>.md`.
- Arhitekturne odluke dokumentovati u `docs/DECISIONS.md` po ADR formatu.
- `CLAUDE.md` se ažurira samo kada se mijenjaju temeljna pravila projekta.

## Struktura menija (sidebar navigacija)

Aplikacija koristi data-driven sidebar (menuConfig.ts), grupiran prema:

Root
├── Osobno
│   ├── Moji podaci
│   ├── Moji godišnji odmori
│   └── ...
├── Poslovanje
│   ├── Podaci o osobama
│   ├── Zaposlenici
│   ├── Agencijski radnici
│   ├── Studenti i učenici
│   ├── Ugovori o djelu
│   ├── Organizacija
│   │   ├── Organizacijska hijerarhija
│   │   ├── Katalog poslova
│   │   ├── Radna mjesta
│   │   └── ...
│   ├── Godišnji odmori
│   │   ├── Parametri za obračun
│   │   ├── Kalendar prisutnosti
│   │   └── ...
│   ├── Upravljanje ugovorima
│   │   ├── Masovno produženje ugovora
│   │   └── ---
│   ├── Upravljanje dokumentima
│   │   ├── Katalog dokumenata
│   │   └── ...
│   └── Upravljanje zaduženjima
│       ├── Katalog opreme
│       └── ---
├── Šifrarnici
│   ├── Osnovni
│   │   ├── Spolovi
│   │   └── ...
│   ├── Adrese
│   │   ├── Države
│   │   ├── Županije
│   │   ├── Općine
│   │   └── Naselja
│   ├── Zaposlenje
│   │   ├── Tipovi radnog odnosa
│   │   └── ...
│   ├── Godišnji odmori
│   │   ├── Praznici
│   │   └── ...
│   ├── Dokumenti
│   │   └── ...
│   └── Organizacija
│       ├── Vrste organizacijskih jedinica
│       └── ...
├── Administracija sustava
└── Upute

NAPOMENA: "Organizacija" i "Godišnji odmori" postoje i pod Poslovanje (operativni 
moduli) i pod Šifrarnici (šifarnici vezani uz te module) - to su različite stavke 
s istim nazivom, ne duplikat.

Trenutno implementirano (Faza 1): Šifrarnici → Osnovni (Spolovi), Adrese (Države, 
Županije, Općine, Naselja). Ostatak strukture su placeholderi za buduće module.

## Git workflow - veće dorade

Za dorade koje diraju širi dio aplikacije (npr. refaktor navigacije, promjene 
arhitekture) - rad na zasebnom branchu (npr. feature/menu-grouping), PR se kreira 
ali se ne merge-a automatski - čeka review.

Za pokretanje branch verzije lokalno:
git checkout feature/naziv-brancha
(Vite hot-reload automatski pokupi promjene)

Povratak na main:
git checkout main
