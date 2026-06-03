# Frontend

**Status:** 🟡 U toku  
**Port:** 3000  
**Stack:** React 18 + TypeScript + Vite

## Tehnologije

| Lib | Verzija | Svrha |
|-----|---------|-------|
| React | 18 | UI framework |
| TypeScript | 5 | Tipska sigurnost |
| Vite | 5 | Bundler |
| Ant Design | 5 | UI komponente |
| React Router | v6 | Klijentski routing |
| i18next + react-i18next | 23/14 | Višejezičnost |
| Axios | 1.7 | HTTP klijent |

## Teme

- Ant Design `ConfigProvider` s `theme.darkAlgorithm` / `theme.defaultAlgorithm`
- Toggle switch u topbaru (Sun/Moon ikone)
- Odabrana tema persistirana u `localStorage` pod ključem `hr-lite-theme`

## Višejezičnost

- Jezik se bira kroz **dropdown** u topbaru (ne tipke — skalira za više jezika)
- Implementacija: i18next + react-i18next
- Prijevodi u `src/i18n/locales/hr.json` i `en.json`
- Odabrani jezik persistiran u `localStorage` pod ključem `hr-lite-lang`
- Default: `hr`

## Routing struktura

```
/                       → redirect na /codebook/gender
/codebook/gender        → Šifarnik spolova ✅
```

## Layout

```
┌─────────────────────────────────────────────┐
│ Sidebar          │ Topbar                    │
│                  │  Naziv stranice | Lang | 🌙 | Avatar │
│  HR-lite         ├───────────────────────────┤
│  > Šifarnici     │                           │
│    > Spol        │   <Outlet />              │
│                  │   (sadržaj stranice)       │
│                  │                           │
└─────────────────────────────────────────────┘
```

## API komunikacija

- `src/api/axios.ts` — centralna Axios instanca
- Request interceptor: automatski dodaje `Authorization: Bearer <token>`
- Response interceptor: 401 → redirect na `/login`
- Vite proxy u dev modu: `/api/*` → backend servisi

## Zadaci

- [x] Projekt setup (Vite + React + TypeScript)
- [x] Ant Design + light/dark mode
- [x] i18n setup (hr, en)
- [x] Axios instanca s interceptorima
- [x] MainLayout (sidebar + topbar)
- [x] Šifarnik spolova — `/codebook/gender` (mock data)
- [x] Dockerfile + docker-compose integracija
- [ ] Login stranica + auth context
- [ ] Refresh token flow
- [ ] Ostali šifarnici (country, county...)
- [ ] Employee lista i detalji
- [ ] Leave request forma
- [ ] Org chart prikaz
