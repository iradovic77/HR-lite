# Document Service

**Status:** 🔴 Nije početo  
**Port:** 5005  
**DB schema:** `hr_document`

## Odgovornost

Centralno skladište HR dokumenata. Upload, preuzimanje i kategorizacija dokumenata vezanih za zaposlenika.

## Entiteti

### Document

| Polje | Tip | Napomena |
|-------|-----|----------|
| Id | Guid | PK |
| EmployeeId | Guid | Logički FK → employee-service |
| FileName | string | Originalni naziv fajla |
| StoredPath | string | Putanja na disku / S3 key |
| ContentType | string | MIME type |
| SizeBytes | long | |
| Category | enum | Contract, Certificate, Policy, Other |
| UploadedByUserId | Guid | |
| UploadedAt | DateTime | |
| Description | string? | |

## Endpointi

| Metoda | Putanja | Napomena |
|--------|---------|----------|
| GET | `/api/documents` | Lista dokumenata (filtriranje po employeeId) |
| GET | `/api/documents/{id}` | Metadata dokumenta |
| GET | `/api/documents/{id}/download` | Preuzimanje fajla |
| POST | `/api/documents` | Upload (multipart/form-data) |
| DELETE | `/api/documents/{id}` | Brisanje (Admin/HR) |

## Storage strategija

- **Lokalni razvoj:** fajlovi na disku unutar Docker volumena
- **Produkcija (budući korak):** S3-compatible storage (MinIO ili AWS S3)
- Fizičko ime fajla = `{Guid}{extension}` da se izbjegne kolizija

## Zadaci

- [ ] .NET Core 8 projekt setup
- [ ] Document entitet + migracija
- [ ] Upload endpoint (multipart)
- [ ] Download endpoint (stream)
- [ ] Brisanje + fizičko uklanjanje fajla
- [ ] Docker volume za storage
- [ ] Dockerfile + docker-compose integracija
