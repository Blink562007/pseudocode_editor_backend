# US-2.5 · Documents are scoped to the logged-in user
**As a** student,
**I want to** see only my own documents in the sidebar,
**so that** my work is private and I'm not confused by other people's files.

**Acceptance Criteria:**
- [ ] `GET /api/pseudocode` filters by the authenticated user's ID
- [ ] `PUT /api/pseudocode/{id}` and `DELETE /api/pseudocode/{id}` return 403 if the document doesn't belong to the user
- [ ] Unauthenticated requests to protected endpoints return 401
- [ ] A user cannot see another user's documents in the sidebar

## Backend Requirements

| Endpoints touched | DB impact | Services | Auth |
|---|---|---|---|
| `EXISTING GET /api/pseudocode`; `EXISTING GET /api/pseudocode/{id}` | Ensure `Documents.ownerId` exists and is indexed; enforce F… | Derive `ownerId` from JWT claims (do not accept `ownerId` f… | Configure JWT bearer auth (Task 2.6). Require valid tokens |

- **API endpoints:** No new endpoints; tighten authZ on existing document endpoints.
  - Protect `GET/POST/PUT/DELETE /api/pseudocode*` with `[Authorize]`.
- **Database:** Ensure `Documents.ownerId` exists and is indexed; enforce FK to users.
- **Service layer logic:**
  - Derive `ownerId` from JWT claims (do not accept `ownerId` from the client).
  - `GET /api/pseudocode`: return only documents where `ownerId == currentUserId`.
  - `PUT`/`DELETE`: if document exists but `ownerId != currentUserId`, return `403 Forbidden`.
  - Recommended: `GET /api/pseudocode/{id}` should return `404 Not Found` when the doc is not owned (prevents ID probing), unless you explicitly want `403` for reads too.
- **Authentication/authorization:** Configure JWT bearer auth (Task 2.6). Require valid tokens.
- **Error handling / status codes:**
  - `401 Unauthorized` for missing/invalid token
  - `403 Forbidden` for ownership violations (per AC)
  - `404 Not Found` for missing documents

**Traces to:** FR-7.2, Task 2.6


## Screenshot
_No screenshot (no distinct UI change captured)._
