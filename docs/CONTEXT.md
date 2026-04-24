# HR-lite — Kontekst projekta

## Svrha

HR-lite je interna HR aplikacija namijenjena manjim i srednje velikim organizacijama. Pokriva osnovne HR procese: upravljanje zaposlenicima, organizacijsku strukturu, odsustava i dokumentaciju.

## Ciljni korisnici

- **HR menadžeri** — unos i upravljanje podacima zaposlenika, odobravanje odsustva
- **Rukovodioci** — pregled tima, odobravanje zahtjeva
- **Zaposlenici** — pregled vlastitih podataka, podnošenje zahtjeva za odsustvo

## Poslovne domene

### Identity
Upravljanje korisničkim nalozima, ulogama i permisijama. Svaki zaposlenik ima korisnički nalog sa jednom ili više uloga (Admin, HR, Manager, Employee).

### Employee
Centralni registar zaposlenika. Sadrži lične podatke, radno mjesto, ugovorni status i historiju promjena.

### Organisation
Hijerarhijska struktura organizacije — odjeljenja, timovi, i rukovodioci. Definira ko komu reportira.

### Leave
Upravljanje vrstama odsustva (godišnji odmor, bolovanje, slobodni dani). Tok odobrenja: zaposlenik podnosi → rukovodilac odobrava → HR evidentira.

### Document
Centralno skladište HR dokumenata — ugovori, certifikati, politike. Vezano za konkretan profil zaposlenika.

## Ograničenja i pretpostavke

- Aplikacija nije namijenjena za payroll (platni spisak).
- Početni opseg ne uključuje integracije s eksternim sistemima (ERP, Active Directory).
- Višejezičnost nije u prvom koraku, ali arhitektura to treba podržati.
- Sve operacije su audit-logged.
