# API Gateway

**Status:** 🔴 Nije početo  
**Port:** 5000  
**Tehnologija:** YARP (Yet Another Reverse Proxy)

## Odgovornost

Jedina ulazna točka za sve klijente. Prosljeđuje zahtjeve odgovarajućim mikroservisima, validira JWT tokene i primjenjuje rate limiting.

## Routing tablica

| Prefix | Cilj | Port |
|--------|------|------|
| `/api/identity/*` | identity-service | 5001 |
| `/api/employees/*` | employee-service | 5002 |
| `/api/departments/*` | org-service | 5003 |
| `/api/org-chart/*` | org-service | 5003 |
| `/api/leave-*` | leave-service | 5004 |
| `/api/documents/*` | document-service | 5005 |

## JWT validacija

- Gateway validira potpis JWT tokena koristeći isti `JwtSettings:Secret` kao i identity-service
- Nema poziva prema identity-service za svaki zahtjev (stateless validacija)
- Zahtjevi bez validnog tokena vraćaju `401 Unauthorized` na gateway nivou

## Rate limiting

- Per-IP: 100 zahtjeva / minuta
- Per-User (iz JWT claims): 500 zahtjeva / minuta

## Zadaci

- [ ] .NET Core 8 projekt setup s YARP paketom
- [ ] Osnovna routing konfiguracija (appsettings.json)
- [ ] JWT validacija middleware
- [ ] Rate limiting
- [ ] Health check endpoint (`/health`)
- [ ] Dockerfile + docker-compose integracija
