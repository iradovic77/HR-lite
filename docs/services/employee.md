# Employee Service — Specifikacija

## Grupe podataka

Svaka grupa podataka je zasebna tablica u bazi. Kolona "Povijesna komponenta" označava da li tablica ima ValidFrom/ValidTo datume.

| Naziv grupe | Povijesna komponenta | Opis |
|---|---|---|
| Person_data | NE | Osobni podaci zaposlenika. Jedan zapis po osobi bez obzira na broj zaposlenja. |
| Employment_data | DA | Podaci zaposlenja. Jedna osoba može imati više redaka (paralelna zaposlenja, različiti statusi). |
| Position_data | DA | Podaci o radnom rasporedu (poziciji) u razdoblju. |
| Salary_coefficient_data | DA | Podaci o plaći/koeficijentu. Tip (Bruto/Neto/koeficijent) ovisi o radnom mjestu. |
| CostCenter_data | DA | Podaci o mjestu troška u razdoblju. |
| Project_data | DA | Podaci o rasporedu na projekt u razdoblju. |
| Location_data | DA | Podaci o lokaciji zaposlenika u razdoblju. |
| Contact_data | NE | Kontakt podaci (email, telefoni...). Višestruki unos. |
| Address_data | DA | Podaci o svim vrstama adresa. |
| Contract_data | DA | Podaci o ugovorima. Veže se na raspored i plaću. |
| Disability_data | DA | Podaci o invalidnosti. |
| BankAccount_data | DA | Podaci o računima isplate. |
| PreviousEmployment_data | NE | Prethodni staž ili beneficirani staž. Višestruki unos. |
| Education_data | NE | Podaci o školovanju. Višestruki unos. |
| AddOns_data | DA | Dodaci za zaposlenika. |
| Transport_data | DA | Podaci o prijevozu. |
| FamilyMembers_data | NE | Članovi obitelji. Višestruki unos. |
| HealthInsurance_data | DA | Podaci o zdravstvenom osiguranju. |
| PersonalEquipment_data | DA | Zaduženja opreme. |
| PersonalDocuments_data | DA | Dokumenti zaposlenika. Veže se na katalog dokumenata. |

---

## Person_data

Osobni podaci zaposlenika. Jedna osoba = jedan zapis, bez obzira na broj zaposlenja ili povrataka u sustav.

### Pravila
- Jedan zapis po osobi (unique constraint na PersonId)
- Person_code: slobodni broj, moguće autogeneriranje kroz UI gumb "Generiraj automatski" ili ručni unos ili izmjena generiranog
- Person_taxNumber: nije unique na razini Person_data — unique constraint je na kombinaciji (taxNumber + companyId) u Employment_data tablici
- Validacija poreznog broja ovisi o državi poduzeća (definira se u Employment_data) — OTVORENO PITANJE: radnik može raditi u 2 poduzeća u različitim državama, što znači potencijalno 2 porezna broja
- Relacije Naselje → Općina → Država postoje ali nisu obavezne (pouzdano samo za Hrvatsku). Ako su definirane, odabirom naselja automatski se popunjavaju općina i država.
- ProfileImageUrl: URL do slike, sama slika se čuva u document-serviceu

### Atributi

| Naziv polja | Tip | Obavezno | Opis |
|---|---|---|---|
| Person_code | nvarchar(50) | DA | Šifra osobe. Autogeneriranje slobodnog broja ili ručni unos. |
| Person_name | nvarchar(150) | DA | Ime osobe. |
| Person_surname | nvarchar(150) | DA | Prezime osobe. |
| Person_gender | int | DA | FK → codebook_gender |
| Person_birthDate | datetime | DA | Datum rođenja. |
| Person_birthPlace | int | NE | Mjesto rođenja. FK → codebook_city |
| Person_birthMunicipality | int | NE | Općina rođenja. FK → codebook_municipality. Auto-popunjava se ako postoji relacija s codebook_city. |
| Person_birthCountry | int | NE | Država rođenja. FK → codebook_country. Auto-popunjava se ako postoji relacija. |
| Person_citizenship | int | NE | Državljanstvo. FK → codebook_country (isti šifarnik, sadrži i državljanstvo). |
| Person_taxNumber | varchar(100) | NE | Porezni broj (OIB za HR). Validacija po državi poduzeća. Nije unique na ovoj razini. |
| Person_parentName | nvarchar(150) | NE | Ime jednog roditelja. |
| Person_maidenName | nvarchar(150) | NE | Djevojačko prezime. |
| Person_profileImageUrl | nvarchar(500) | NE | URL do slike zaposlenika. Slika se čuva u document-serviceu. |

### Indeksi
- Composite index: (Person_surname, Person_name) — česta pretraga
- Index: Person_taxNumber — česta pretraga (unique constraint je u Employment_data)
- Index: Person_code

### Validacija poreznog broja

#### Hrvatska (OIB)
```csharp
public static bool ValidateOibHR(string oib)
{
    if (string.IsNullOrEmpty(oib) || oib.Length != 11) return false;
    int broj = 10;
    for (int i = 0; i < 10; i++)
    {
        broj += int.Parse(oib[i].ToString());
        broj %= 10;
        if (broj == 0) broj = 10;
        broj *= 2;
        broj %= 11;
    }
    int kontrolni = 11 - broj;
    kontrolni %= 10;
    return kontrolni == int.Parse(oib[10].ToString());
}
```

#### Slovenija (EMŠO/davčna)
```csharp
public static bool ValidateOibSI(string oib)
{
    if (string.IsNullOrEmpty(oib)) return false;
    int kontrolni = int.Parse(oib[^1].ToString());
    int broj = 0;
    for (int i = 0; i < oib.Length - 1; i++)
        broj += int.Parse(oib[i].ToString()) * (oib.Length - i);
    broj %= 11;
    broj = 11 - broj;
    if (broj == 10) broj = 0;
    return kontrolni == broj;
}
```

### Otvorena pitanja
- [ ] Radnik koji radi u 2 poduzeća u različitim državama — može li imati 2 porezna broja? Gdje se to vodi?
- [ ] Unique constraint za Person_taxNumber i Person_code — implementirati na razini Employment_data u kombinaciji s companyId
