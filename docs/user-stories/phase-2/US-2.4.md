# US-2.4 · Documents persist across server restarts
**As a** student,
**I want** my saved documents to still be there after the server restarts,
**so that** I don't lose my work.

**Acceptance Criteria:**
- [ ] Documents are stored in a PostgreSQL (or SQLite) database, not in-memory
- [ ] Creating, updating, and deleting documents write to the database
- [ ] After restarting the backend, `GET /api/pseudocode` returns previously saved documents
- [ ] The database schema includes: id, ownerId, title, content, language, createdAt, updatedAt

## Backend Requirements

| Endpoints touched | DB impact | Services | Auth |
|---|---|---|---|
| `EXISTING GET /api/pseudocode`; `EXISTING GET /api/pseudocode/{id}`; `EXISTING POST /api/pseudocode`; `EXISTING PUT /api/pseudocode/{id}`; `EXISTING DELETE /api/pseudocode/{id}` | (Tasks 2.1–2.4) | Replace the in-memory `List<>` document store with a reposi… | In Phase 2, documents become user-owned; endpoints should b… |

- **API endpoints:** No new endpoints; existing document endpoints must move from in-memory to EF Core-backed persistence.
  - `GET /api/pseudocode` → `200 OK` (`PseudocodeDocument[]`)
  - `GET /api/pseudocode/{id}` → `200 OK` / `404 Not Found`
  - `POST /api/pseudocode` → `201 Created`
  - `PUT /api/pseudocode/{id}` → `200 OK` / `404 Not Found`
  - `DELETE /api/pseudocode/{id}` → `204 No Content` / `404 Not Found`
- **Database:** (Tasks 2.1–2.4)
  - Add EF Core provider (PostgreSQL recommended; SQLite acceptable for local dev).
  - Create `PseudocodeDbContext` with a `Documents` table including: `id`, `ownerId`, `title`, `content`, `language`, `createdAt`, `updatedAt`.
  - Add FK relationship `Documents.ownerId → AspNetUsers.Id` (or your `Users` table).
  - Create initial migration and verify persistence across process restarts.
- **Service layer logic:**
  - Replace the in-memory `List<>` document store with a repository pattern backed by `DbContext`.
  - Preserve existing behavior: auto-format/validate on create/update (align with current `PseudocodeService`, `ValidationService`, `FormattingService`).
- **Authentication/authorization:** In Phase 2, documents become user-owned; endpoints should be protected with JWT (see US-2.5).
- **Error handling / status codes:**
  - `500` for DB connectivity/migration issues (log details server-side)
  - `400` for invalid payloads

**Traces to:** NFR-6, Task 2.1, 2.2, 2.3, 2.4


## Screenshot
_No screenshot (no distinct UI change captured)._
