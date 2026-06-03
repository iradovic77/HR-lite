# Multi-stage build

# ── Stage 1: Build ───────────────────────────────────────────────────
FROM node:20-alpine AS build
WORKDIR /app

# Instaliraj zavisnosti (cache sloj — ne mijenja se dok se package.json ne promijeni)
COPY package.json ./
RUN npm install

# Kopiraj izvorni kod i buildi
COPY . .
RUN npm run build

# ── Stage 2: Serve ───────────────────────────────────────────────────
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

# Kopiraj buildani output iz prve faze
COPY --from=build /app/dist .

# nginx konfiguracija za React Router (SPA fallback)
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
