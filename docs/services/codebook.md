# Codebook Service — Specifikacija

Centralni servis za sve šifrarnike. Svi servisi dohvaćaju šifrarnike iz ovog servisa.

## Pravila
- Svi šifarnici podržavaju višejezičnost (hr, en)
- Dodavanje novih jezika moguće bez promjene arhitekture
- Šifarnici se ne brišu — deaktiviraju se (IsActive flag)

## Šifarnici identificirani uz Person_data

| Naziv | Opis | Napomena |
|---|---|---|
| codebook_gender | Spol | hr: Muško/Žensko/Ostalo, en: Male/Female/Other |
| codebook_city | Naselja/Gradovi | Sadrži relaciju prema codebook_municipality |
| codebook_municipality | Općine | Sadrži relaciju prema codebook_country |
| codebook_country | Države | Sadrži naziv države I državljanstvo (npr. "hrvatska" / "hrvatsko") |

## Relacije između šifarnika
codebook_city → codebook_municipality → codebook_country

Relacije nisu obavezne — pouzdane samo za Hrvatsku u inicijalnoj implementaciji.

## Napomena
Lista šifarnika će se proširivati kako definiramo ostale grupe podataka.
Svaki novi šifarnik se dodaje ovdje.
