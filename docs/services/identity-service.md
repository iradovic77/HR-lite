# Identity Service

**Status:** 🟡 U toku  
**Port:** 5001  
**DB schema:** `hr_identity`

## Odgovornost

Upravljanje korisničkim nalozima, ulogama i autentifikacijom. Jedini servis koji izdaje JWT tokene.

## Entiteti

| Entitet | Tabela | Napomena |
|---------|--------|----------|
| User | `users` | Email, PasswordHash, IsActive |
| Role | `roles` | Admin, HR, Manager, Employee |
| UserRole | `user_roles` | M:N veza |

## Endpointi

| Metoda | Putanja | Status | Napomena |
|--------|---------|--------|----------|
| POST | `/api/auth/login` | ✅ Gotovo | Vraća access token |
| POST | `/api/auth/refresh` | ❌ Nije | Refresh token flow |
| GET | `/api/users` | ❌ Nije | Lista korisnika (Admin/HR) |

## Zadaci

- [x] .NET Core 8 projekt setup
- [x] EF Core + PostgreSQL (schema: hr_identity)
- [x] User, Role, UserRole entiteti
- [x] Seed: 4 uloge + admin korisnik pri prvom pokretanju
- [x] JWT login endpoint (POST /api/auth/login)
- [ ] Refresh token endpoint
- [ ] Role-based autorizacija (zaštićeni endpointi)
- [ ] GET /api/users (paginacija, filtriranje)

## Napomene

- Access token TTL: 60 min (konfigurabilno u appsettings)
- Refresh token: pohraniti u bazi, rotirati pri svakom korištenju
