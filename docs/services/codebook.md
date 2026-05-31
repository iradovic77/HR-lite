# Codebook Service (Šifarnici)

**Status:** 🟡 U toku  
**Port:** 5006  
**DB schema:** `hr_codebook`

## Odgovornost

Centralni registar svih šifarnika (lookup tablica) koji se koriste u ostalim servisima.
Šifarnici su dijeljeni podaci — npr. spol, vrsta ugovora, razlog odsustva, država, županija.

Svi servisi dohvaćaju šifarnike iz ovog servisa.

---

## Pravila za sve šifarnik tablice

Svaka šifarnik tablica **obavezno** ima sljedeće kolone:

| Kolona | Tip | Default | Napomena |
|--------|-----|---------|----------|
| `Id` | Guid | — | PK |
| `Code` | string | — | Kratka šifra (npr. `M`, `HR`, `BA`) |
| `Name` | string | — | Naziv na lokalnom jeziku |
| `NameEn` | string? | null | Naziv na engleskom (i18n) |
| `IsActive` | bool | `true` | `false` = soft delete |
| `Ordinal` | int | `0` | Custom redoslijed u combo-ima |
| `CreatedAt` | DateTime | — | Audit |
| `UpdatedAt` | DateTime | — | Audit |
| `CreatedBy` | Guid | — | Audit |
| `UpdatedBy` | Guid | — | Audit |

### IsActive
- `IsActive = false` znači **soft delete** — zapis ostaje u bazi, ali se **ne prikazuje** u combo-ima / dropdown-ima na UI-u.
- Šifarnici se **nikad ne brišu fizički** iz baze.
- API endpointi po defaultu vraćaju samo aktivne zapise; query param `?includeInactive=true` vraća sve.

### Ordinal
- Definira custom redoslijed prikaza u combo-ima, neovisno o abecednom redu ili ID-u.
- Niži broj = više u listi. Zapisi s istim `Ordinal` sortiraju se po `Name`.

### Višejezičnost
- Svi šifarnici podržavaju višejezičnost putem `Name` (hr) i `NameEn` (en) kolona.
- Arhitektura mora podržavati dodavanje novih jezika naknadno bez promjene strukture tablice.

---

## Šifarnici

### codebook_gender — Spol

| Code | Name | NameEn |
|------|------|--------|
| M | Muško | Male |
| F | Žensko | Female |
| O | Ostalo | Other |

### Lokacijska hijerarhija

Hijerarhija lokacija ide od najveće prema najmanjoj administrativnoj jedinici:

```
Država (codebook_country)
  └── Županija (codebook_county)
        └── Općina (codebook_municipality)
              └── Naselje (codebook_settlement)
```

> **Napomena:** Relacije između razina nisu DB foreign key-evi — provode se na aplikativnom nivou.
> U inicijalnoj implementaciji pouzdani podaci postoje samo za Hrvatsku i Bosnu i Hercegovinu.
> Lista šifarnika proširivat će se kako se definiraju ostale grupe podataka.

### codebook_country — Država

Matična lista država. Koristi ISO 3166-1 alpha-2 za `Code` (npr. `BA`, `HR`, `RS`).
Čuva i naziv državljanstva (npr. "hrvatska" / "hrvatsko") u `Name` polju po potrebi.

### codebook_county — Županija

Administrativna jedinica unutar države.

| Kolona | Napomena |
|--------|----------|
| `CountryId` | Logički FK → codebook_country |

### codebook_municipality — Općina

Administrativna jedinica unutar županije.

| Kolona | Napomena |
|--------|----------|
| `CountyId` | Logički FK → codebook_county |

### codebook_settlement — Naselje

Najniža administrativna jedinica (grad, selo, naseljeno mjesto).

| Kolona | Napomena |
|--------|----------|
| `MunicipalityId` | Logički FK → codebook_municipality |

### codebook_employment_type — Vrsta zaposlenja

| Code | Name | NameEn |
|------|------|--------|
| FT | Puno radno vrijeme | Full-time |
| PT | Nepuno radno vrijeme | Part-time |
| CT | Ugovor o djelu | Contract |

### codebook_leave_type — Vrsta odsustva

Početni seed — može se proširivati kroz admin sučelje.

| Code | Name | NameEn |
|------|------|--------|
| AL | Godišnji odmor | Annual leave |
| SL | Bolovanje | Sick leave |
| PL | Roditeljski dopust | Parental leave |
| UL | Neplaćeno odsustvo | Unpaid leave |

---

## Endpointi (generički pattern)

Svaki šifarnik izlaže iste endpointe:

| Metoda | Putanja | Uloge | Napomena |
|--------|---------|-------|----------|
| GET | `/api/codebook/{type}` | Svi autorizirani | Aktivni zapisi, sortirani po Ordinal/Name |
| GET | `/api/codebook/{type}?includeInactive=true` | Admin, HR | Svi zapisi |
| GET | `/api/codebook/{type}/{id}` | Svi autorizirani | Jedan zapis |
| POST | `/api/codebook/{type}` | Admin | Kreiranje |
| PUT | `/api/codebook/{type}/{id}` | Admin | Izmjena |
| DELETE | `/api/codebook/{type}/{id}` | Admin | Soft delete (IsActive = false) |

`{type}` = `gender`, `country`, `county`, `municipality`, `settlement`, `employment-type`, `leave-type`

---

## Zadaci

- [x] Definirati strukturu šifarnik dokumenta
- [x] `CodebookBase` apstraktna klasa s zajedničkim kolonama
- [x] Gender model + EF Core seed data (primjer za učenje)
- [x] Country, County, Municipality, Settlement modeli
- [x] EF Core migracija s InsertData za Gender
- [x] Dockerfile
- [ ] .NET Core 8 projekt pokrenut i testiran lokalno
- [ ] EmploymentType, LeaveType modeli + seed data
- [ ] Generic controller pattern za CRUD
- [ ] Swagger dokumentacija
- [ ] docker-compose integracija

---

## Napomene

- Šifarnici su **read-heavy** — razmotriti cache na gateway ili servis nivou u kasnijoj fazi.
- Seed data za standardne šifarnike (spol, vrste odsustva) ide kroz **EF Core migracije** (`HasData`), ne kroz SQL skripte.
- Admin korisnici mogu dodavati nove zapise kroz API; sistemski seed zapisi imaju `CreatedBy = Guid.Empty` (sistem).
