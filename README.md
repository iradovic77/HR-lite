# HR-lite

Lagana HR aplikacija razvijena u mikroservisnoj arhitekturi.

## Tech Stack

- **Backend:** .NET Core 8, C#
- **Baza podataka:** PostgreSQL
- **Frontend:** React (TypeScript)
- **API Gateway:** YARP / Ocelot
- **Autentifikacija:** JWT, Identity Server / Keycloak
- **Kontejnerizacija:** Docker, Docker Compose

## Mikroservisi

| Servis | Opis | Port |
|--------|------|------|
| `identity-service` | Autentifikacija i autorizacija | 5001 |
| `employee-service` | Upravljanje zaposlenicima | 5002 |
| `org-service` | Organizacijska struktura | 5003 |
| `leave-service` | Upravljanje odsustvima | 5004 |
| `document-service` | Upravljanje dokumentima | 5005 |

## Struktura projekta

```
HR-lite/
├── gateway/             # API Gateway
├── services/
│   ├── identity-service/
│   ├── employee-service/
│   ├── org-service/
│   ├── leave-service/
│   └── document-service/
├── frontend/            # React aplikacija
├── docs/                # Dokumentacija
└── docker-compose.yml
```

## Pokretanje

```bash
docker-compose up -d
```

Frontend: http://localhost:3000  
API Gateway: http://localhost:5000
