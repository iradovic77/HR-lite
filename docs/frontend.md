# Frontend

**Status:** 🔴 Nije početo  
**Port:** 3000  
**Stack:** React 18 + TypeScript + Vite

## Tehnologije

| Lib | Svrha |
|-----|-------|
| React 18 | UI framework |
| TypeScript | Tipska sigurnost |
| Vite | Bundler (brži od CRA) |
| React Router v6 | Klijentski routing |
| TanStack Query | Server state management |
| Axios | HTTP klijent |
| Tailwind CSS | Stilizacija |

## Routing struktura

```
/login                  — javno
/dashboard              — zaštićeno
/employees              — HR, Admin, Manager
/employees/:id          — HR, Admin, Manager, sam zaposlenik
/employees/new          — HR, Admin
/leave                  — svi zaposlenici
/leave/new              — svi zaposlenici
/org-chart              — svi zaposlenici
/admin/users            — Admin
```

## Auth flow

1. Korisnik unosi email/lozinku → `POST /api/identity/auth/login`
2. Access token + refresh token pohranjeni u memory (access) i httpOnly cookie (refresh)
3. Axios interceptor automatski refresh-uje access token kad istekne
4. Logout briše tokene i preusmjerava na `/login`

## Zadaci

- [ ] Projekt setup (Vite + React + TypeScript)
- [ ] Tailwind CSS konfiguracija
- [ ] React Router v6 struktura
- [ ] Auth context + Axios interceptor
- [ ] Login stranica
- [ ] Employee lista (paginacija, pretraga)
- [ ] Employee detalji
- [ ] Leave request forma
- [ ] Org chart prikaz
