# Codebook Service (Šifarnici)

**Status:** 🟡 U toku  
**Port:** 5006  
**DB schema:** `hr_codebook`

## Odgovornost

Centralni registar svih šifarnika (lookup tablica) koji se koriste u ostalim servisima.
Svi servisi dohvaćaju šifarnike iz ovog servisa.

---

## Pravila za sve šifarnik tablice

### Standardne kolone

Svaka šifarnik tablica **obavezno** ima sljedeće kolone:

| Kolona | Tip | Default | Napomena |
|--------|-----|---------|----------|
| `Id` | Guid | — | PK |
| `Code` | string | — | Neutralna šifra, nikad se ne prevodi |
| `IsActive` | bool | `true` | `false` = soft delete |
| `Ordinal` | int | `0` | Custom redoslijed u combo-ima |
| `CreatedAt` | DateTime | — | Audit |
| `UpdatedAt` | DateTime | — | Audit |
| `CreatedBy` | Guid | — | Audit — `Guid.Empty` za seed |
| `UpdatedBy` | Guid | — | Audit — `Guid.Empty` za seed |

> **Nema `Name` kolone** — svi nazivi čuvaju se u `translation` tablici.

### IsActive — soft delete

- `IsActive = false` znači **deaktiviran** zapis — ostaje u bazi, ne prikazuje se u combo-ima.
- **Šifarnici se nikad ne brišu fizički.** Uvijek `IsActive = false`.
- API po defaultu vraća samo aktivne zapise; `?includeInactive=true` vraća sve.

### Ordinal — redoslijed u combo-ima

- Niži broj = više u listi.
- Zapisi s istim `Ordinal` sortiraju se po `Code`.

---

## Višejezičnost — Translation sustav

Nazivi šifarnika **ne** čuvaju se u kolonama tablice. Koristi se generički Translation sustav koji omogućava dodavanje novih jezika bez ikakve izmjene sheme.

### Tablica: `language`

| Kolona | Tip | Napomena |
|--------|-----|----------|
| `Id` | Guid | PK |
| `Code` | string(10) | ISO 639-1 — `hr`, `en`, `de`... |
| `Name` | string(100) | Naziv na vlastitom jeziku — `Hrvatski`, `English` |
| + audit kolone | | |

**Seed:**

| Code | Name |
|------|------|
| hr | Hrvatski |
| en | English |

### Tablica: `translation`

Generička tablica prijevoda za **sve** entitete u sustavu.

| Kolona | Tip | Napomena |
|--------|-----|----------|
| `Id` | Guid | PK |
| `EntityType` | string(100) | Naziv tablice — npr. `codebook_gender` |
| `EntityId` | Guid | Id retka koji se prevodi |
| `LanguageId` | Guid | FK → `language.Id` (pravi DB constraint, ista shema) |
| `FieldName` | string(100) | Polje koje se prevodi — npr. `Name` |
| `Value` | string(500) | Prevedeni tekst |
| + audit kolone | | |

**Unique index:** `(EntityType, EntityId, LanguageId, FieldName)`

### Fallback logika (implementira se u servisnom sloju)

```
1. Traženi jezik (npr. "de")  → ako ne postoji prijevod:
2. Hrvatski ("hr")             → ako ne postoji prijevod:
3. Code kolona entiteta        ← uvijek postoji, nikad null
```

### Pravila višejezičnosti

- **Hrvatski prijevod je OBAVEZAN** za svaki šifarnik zapis.
- Ostali jezici su opcionalni.
- Novi jezik = jedan redak u `language` tablici + prijevodi u `translation`. **Bez izmjene sheme.**
- `Code` kolona je neutralni identifikator — nikad se ne prevodi.

---

## Šifarnici

### codebook_gender — Spol

| Code | Ordinal | hr (Name) | en (Name) |
|------|---------|-----------|-----------|
| M | 1 | Muško | Male |
| F | 2 | Žensko | Female |
| O | 3 | Ostalo | Other |

### Lokacijska hijerarhija

```
Država (codebook_country)
  └── Županija (codebook_county)
        └── Općina (codebook_municipality)
              └── Naselje (codebook_settlement)
```

> Relacije između razina su **logički FK-ovi** (Guid polje) — bez DB foreign key-eva.  
> U inicijalnoj implementaciji pouzdani podaci postoje samo za Hrvatsku i Bosnu i Hercegovinu.

### codebook_country — Država

ISO 3166-1 alpha-2 za `Code` (npr. `BA`, `HR`, `RS`).

### codebook_county — Županija

| Extra kolona | Tip | Napomena |
|--------------|-----|----------|
| `CountryId` | Guid | Logički FK → codebook_country |

### codebook_municipality — Općina

| Extra kolona | Tip | Napomena |
|--------------|-----|----------|
| `CountyId` | Guid | Logički FK → codebook_county |

### codebook_settlement — Naselje

| Extra kolona | Tip | Napomena |
|--------------|-----|----------|
| `MunicipalityId` | Guid | Logički FK → codebook_municipality |

### codebook_employment_type — Vrsta zaposlenja

| Code | hr | en |
|------|----|----|
| FT | Puno radno vrijeme | Full-time |
| PT | Nepuno radno vrijeme | Part-time |
| CT | Ugovor o djelu | Contract |

### codebook_leave_type — Vrsta odsustva

| Code | hr | en |
|------|----|----|
| AL | Godišnji odmor | Annual leave |
| SL | Bolovanje | Sick leave |
| PL | Roditeljski dopust | Parental leave |
| UL | Neplaćeno odsustvo | Unpaid leave |

---

## Endpointi (generički pattern)

| Metoda | Putanja | Uloge | Napomena |
|--------|---------|-------|----------|
| GET | `/api/codebook/{type}` | Svi autorizirani | Aktivni, sortirani po Ordinal/Code |
| GET | `/api/codebook/{type}?includeInactive=true` | Admin, HR | Svi zapisi |
| GET | `/api/codebook/{type}/{id}` | Svi autorizirani | Jedan zapis |
| POST | `/api/codebook/{type}` | Admin | Kreiranje |
| PUT | `/api/codebook/{type}/{id}` | Admin | Izmjena |
| DELETE | `/api/codebook/{type}/{id}` | Admin | Soft delete (IsActive = false) |

`{type}` = `gender`, `country`, `county`, `municipality`, `settlement`, `employment-type`, `leave-type`

Response uključuje nazive za traženi jezik (header `Accept-Language` ili query param `?lang=hr`).

---

## Zadaci

- [x] Definirati strukturu šifarnik dokumenta
- [x] `CodebookBase` apstraktna klasa (Code, IsActive, Ordinal, audit)
- [x] `IAuditable` interface za automatske audit kolone
- [x] `Language` model + seed (hr, en)
- [x] `Translation` model + seed za Gender
- [x] Gender model + seed (M, F, O)
- [x] Country, County, Municipality, Settlement modeli
- [x] EF Core migracija s punim seed data i Translation FK-om
- [x] Dockerfile
- [ ] .NET Core 8 projekt pokrenut i testiran lokalno
- [ ] EmploymentType, LeaveType modeli + seed data + prijevodi
- [ ] Generic controller pattern za CRUD s Translation dohvatom
- [ ] Swagger dokumentacija
- [ ] docker-compose integracija

---

## Napomene

- Šifarnici su **read-heavy** — razmotriti cache u kasnijoj fazi.
- Seed data ide kroz **EF Core migracije** (`HasData`) — ne kroz SQL skripte.
- Sistemski seed zapisi koriste `CreatedBy = Guid.Empty`.
- `LanguageId` FK u `translation` je **pravi DB constraint** jer su obje tablice u istoj shemi (`hr_codebook`).
- FK-ovi između šifarnika (npr. County → Country) su **logički** — bez DB constraint-a.
