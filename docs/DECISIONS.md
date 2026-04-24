# HR-lite — Arhitekturne odluke

Format: **[ADR-NNN] Naslov** | Datum | Status

---

## ADR-001: Mikroservisna arhitektura

**Datum:** 2026-04-24  
**Status:** Prihvaćeno

**Kontekst:** Potrebno odabrati arhitekturni stil za HR aplikaciju.

**Odluka:** Mikroservisna arhitektura s odvojenim servisima po poslovnoj domeni.

**Razlozi:**
- Svaka domena (identity, employee, leave...) ima zasebne promjene i deployment cikluse
- Lakše skaliranje pojedinih servisa
- Jasne granice odgovornosti

**Kompromisi:**
- Veća inicijalna kompleksnost od monolita
- Potreban API Gateway i inter-service komunikacija

---

## ADR-002: .NET Core 8 za backend servise

**Datum:** 2026-04-24  
**Status:** Prihvaćeno

**Odluka:** Svi backend mikroservisi pisani u .NET Core 8 (C#).

**Razlozi:**
- Zrela platforma s odličnim performansama
- Bogat ekosistem za JWT, EF Core, validaciju
- Native Docker podrška

---

## ADR-003: Jedna PostgreSQL instanca, zasebne sheme

**Datum:** 2026-04-24  
**Status:** Prihvaćeno

**Kontekst:** Treba odlučiti: jedna baza sa zasebnim shemama ili zasebne baze po servisu.

**Odluka:** Jedna PostgreSQL instanca, zasebna schema po servisu (`hr_identity`, `hr_employee`, itd.).

**Razlozi:**
- Manji operativni overhead za razvoj i staging
- Izolacija podataka i migracija i dalje postoji
- Produkcijsko skaliranje na zasebne instance je moguće bez izmjene koda

**Kompromisi:**
- Nije potpuna izolacija (shared infrastructure)
- Potencijalni bottleneck na jednoj instanci pri visokom load-u

---

## ADR-004: YARP kao API Gateway

**Datum:** 2026-04-24  
**Status:** Prihvaćeno

**Odluka:** Koristiti Microsoft YARP (Yet Another Reverse Proxy) umjesto Ocelot-a.

**Razlozi:**
- YARP je aktivno razvijan od strane Microsofta
- Bolje performanse i fleksibilnost od Ocelot-a
- Native .NET integracija, isti tech stack kao servisi

---

## ADR-005: JWT autentifikacija bez Identity Servera

**Datum:** 2026-04-24  
**Status:** Prihvaćeno

**Odluka:** Custom identity-service koji izdaje JWT tokene, bez eksternog Identity Servera (Keycloak, Auth0).

**Razlozi:**
- Manji overhead za lite verziju aplikacije
- Puna kontrola nad token logikom
- Jednostavniji setup za development

**Kompromisi:**
- Manje out-of-the-box feature-a (SSO, OAuth flows)
- Veća vlastita odgovornost za sigurnost

---

## ADR-006: React + Vite + TypeScript za frontend

**Datum:** 2026-04-24  
**Status:** Prihvaćeno

**Odluka:** React 18 s Vite bundlerom i TypeScriptom.

**Razlozi:**
- Vite je znatno brži od CRA
- TypeScript povećava sigurnost tipova u API integraciji
- React Query za server state management (manji boilerplate od Redux)

---

_Nove odluke dodavati po formatu ADR-NNN._
