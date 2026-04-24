# HR-lite — Arhitektura

## Pregled

```
┌─────────────┐
│   Frontend  │  React + TypeScript (port 3000)
└──────┬──────┘
       │ HTTP/REST
┌──────▼──────┐
│ API Gateway │  YARP (port 5000)
└──────┬──────┘
       │ HTTP/REST (internal)
┌──────┴──────────────────────────────────────┐
│  identity  │ employee │  org  │ leave │ doc  │
│  :5001     │  :5002   │ :5003 │ :5004 │:5005 │
└──────┬──────────────────────────────────────┘
       │
┌──────▼──────┐
│  PostgreSQL │  Zasebna schema po servisu
└─────────────┘
```

## Mikroservisi

### identity-service
- **Odgovornost:** JWT izdavanje, validacija tokena, upravljanje korisnicima i ulogama
- **Baza:** `hr_identity` schema
- **Ključni endpointi:** `POST /auth/login`, `POST /auth/refresh`, `GET /users`

### employee-service
- **Odgovornost:** CRUD zaposlenika, historija promjena, radni status
- **Baza:** `hr_employee` schema
- **Ključni endpointi:** `GET /employees`, `POST /employees`, `PUT /employees/{id}`

### org-service
- **Odgovornost:** Odjeljenja, timovi, hijerarhija, reporting linije
- **Baza:** `hr_org` schema
- **Ključni endpointi:** `GET /departments`, `GET /org-chart`

### leave-service
- **Odgovornost:** Tipovi odsustva, zahtjevi, tok odobrenja, bilans dana
- **Baza:** `hr_leave` schema
- **Ključni endpointi:** `POST /leave-requests`, `PUT /leave-requests/{id}/approve`

### document-service
- **Odgovornost:** Upload, preuzimanje i kategorizacija HR dokumenata
- **Baza:** `hr_document` schema, fajlovi u lokalnom storage / S3-compatible

## API Gateway

- Tehnologija: **YARP** (Yet Another Reverse Proxy)
- Routing po prefiksu: `/api/identity/*` → identity-service, itd.
- JWT validacija na gateway nivou
- Rate limiting po IP i korisniku

## Autentifikacija

- JWT Bearer tokeni (access + refresh)
- Tokeni izdan od identity-service
- Gateway validira potpis tokena bez poziva identity-service (shared secret ili JWKS endpoint)

## Baza podataka

- Jedna PostgreSQL instanca, zasebne **sheme** po servisu (ne zasebne baze)
- Migracije putem **EF Core Migrations** unutar svakog servisa
- Svaki servis ima vlastiti `DbContext` i vlastite migracije

## Komunikacija između servisa

- **Sinhrona:** HTTP REST (samo gdje je neophodna)
- Direktni pozivi između servisa su minimalni; svaki servis je vlasnički nad svojim podacima

## Frontend

- React 18 + TypeScript
- React Router v6 za navigaciju
- React Query (TanStack) za server state
- Axios za HTTP pozive
- Tailwind CSS za stilizaciju

## Containerizacija

- Docker slike za svaki servis
- `docker-compose.yml` za lokalni razvoj
- Svaki servis ima vlastiti `Dockerfile`
