# US-1.1 · Component-based editor layout
**As a** student,
**I want** the editor, toolbar, and terminal to be visually distinct panels,
**so that** I can focus on writing code without the interface feeling cluttered.

**Acceptance Criteria:**
- [ ] The page is divided into: Header, Sidebar (left), Editor Panel (centre), Terminal Panel (bottom)
- [ ] Each panel can be identified visually with clear borders/backgrounds
- [ ] The editor panel takes up the majority of screen space
- [ ] The layout does not break on window resize (down to 1024×768)
- [ ] No leftover Vite template CSS (logo animation, `.card`, `.read-the-docs`)

## Backend Requirements

| Endpoints touched | DB impact | Services | Auth |
|---|---|---|---|
| `EXISTING GET /api/pseudocode` | None in Phase 1 (current backend uses in-memory storage; pe… | Ensure `GET /api/pseudocode` is deterministic (recommended… | None in Phase 1. After Phase 2, the same endpoint should re… |

- **API endpoints:** No new endpoints for layout itself. The editor shell should assume document loading uses existing `GET /api/pseudocode`.
  - Response: `200 OK` → `PseudocodeDocument[]` (each includes `id`, `title`, `content`, `language`, `createdAt`, `updatedAt`).
- **Database:** None in Phase 1 (current backend uses in-memory storage; persistence is introduced in Phase 2).
- **Service layer logic:** Ensure `GET /api/pseudocode` is deterministic (recommended sort by `updatedAt DESC` server-side, or document that the client sorts).
- **Authentication/authorization:** None in Phase 1. After Phase 2, the same endpoint should require `Authorization: Bearer <jwt>` and return only the caller’s documents.
- **Error handling / status codes:** `500` for unexpected errors; prefer returning RFC7807 `ProblemDetails` for unhandled exceptions.

**Traces to:** FR-1.1, Task 1.1, 1.6


## Screenshot
![US-1.1 Screenshot](../../prototype/screenshots/US-1.1-component-layout.png)
