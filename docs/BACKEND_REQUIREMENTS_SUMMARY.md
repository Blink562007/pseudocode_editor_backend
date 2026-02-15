# Backend Requirements Summary

Implementation-oriented index (endpoints / DB / services / auth) for each user story.
Each row links to the full story spec (which contains detailed **Backend Requirements**).

**Legend:** `NEW …` vs `EXISTING …` indicates whether the endpoint exists in the current backend (Phase 1 baseline).

**Reminder:** pseudocode API payloads should use `content` (not `code`) per the backend API contract and known client mismatch.

## Phase 1

- New endpoints referenced in this phase: **0**

| User Story | Title | Endpoints touched | DB impact | Services | Auth |
|---|---|---|---|---|---|
| [US-1.1](./user-stories/phase-1/US-1.1.md) | Component-based editor layout | `EXISTING GET /api/pseudocode` | None in Phase 1 (current backend uses in-memory storage; pe… | Ensure `GET /api/pseudocode` is deterministic (recommended… | None in Phase 1. After Phase 2, the same endpoint should re… |
| [US-1.2](./user-stories/phase-1/US-1.2.md) | Create a new document | `EXISTING POST /api/pseudocode` | None in Phase 1 (in-memory). In Phase 2 the same create flo… | On create, backend currently auto-formats content; ensure e… | None in Phase 1; post-Phase 2 requires JWT and sets `ownerI… |
| [US-1.3](./user-stories/phase-1/US-1.3.md) | View and switch between saved documents | `EXISTING GET /api/pseudocode`; `EXISTING GET /api/pseudocode/{id}` | None in Phase 1; Phase 2 moves storage to EF Core + DB. | Ensure `updatedAt` changes on update/rename so “last modifi… | None in Phase 1; Phase 2 requires JWT and filters list by `… |
| [US-1.4](./user-stories/phase-1/US-1.4.md) | Save a document | `EXISTING POST /api/pseudocode`; `EXISTING PUT /api/pseudocode/{id}` | None in Phase 1; Phase 2 persists these writes to DB. | Backend currently validates + auto-formats on create/update… | None in Phase 1; post-Phase 2, require JWT and enforce `own… |
| [US-1.5](./user-stories/phase-1/US-1.5.md) | Delete a document | `EXISTING DELETE /api/pseudocode/{id}` | None in Phase 1 (in-memory). In Phase 2, delete must remove… | Delete should be idempotent from the client’s perspective;… | None in Phase 1; post-Phase 2 require JWT and ensure user c… |
| [US-1.6](./user-stories/phase-1/US-1.6.md) | Rename a document | `EXISTING PUT /api/pseudocode/{id}` | None in Phase 1; Phase 2 persists title changes and updates… | Enforce server-side default title if empty/whitespace (`"Un… | None in Phase 1; post-Phase 2, require JWT and enforce owne… |
| [US-1.7](./user-stories/phase-1/US-1.7.md) | Auto-save to localStorage | `EXISTING POST /api/pseudocode`; `EXISTING PUT /api/pseudocode/{id}` | None in Phase 1. | Save endpoints must remain idempotent and return updated `u… | None in Phase 1; post-Phase 2 the save endpoints require JW… |
| [US-1.8](./user-stories/phase-1/US-1.8.md) | Loading and error states | `EXISTING POST /api/pseudocode/validate`; `EXISTING POST /api/pseudocode/format` | None in Phase 1. | Prefer consistent server-side validation messages (e.g., fo… | None in Phase 1; Phase 2 introduces `401`/`403` flows that… |

## Phase 2

- New endpoints referenced in this phase: **4**

| User Story | Title | Endpoints touched | DB impact | Services | Auth |
|---|---|---|---|---|---|
| [US-2.1](./user-stories/phase-2/US-2.1.md) | Register an account | `NEW POST /api/auth/register` | Phase 2 introduces persistent storage (Tasks 2.1–2.4) and I… | — | Registration endpoint is anonymous; subsequent API calls us… |
| [US-2.2](./user-stories/phase-2/US-2.2.md) | Log in to an existing account | `NEW POST /api/auth/login` | Uses ASP.NET Identity user store (Task 2.5). No document sc… | — | Login endpoint is anonymous. |
| [US-2.3](./user-stories/phase-2/US-2.3.md) | Stay logged in across sessions | `NEW GET /api/auth/me` | Uses ASP.NET Identity user store. | — | — |
| [US-2.4](./user-stories/phase-2/US-2.4.md) | Documents persist across server restarts | `EXISTING GET /api/pseudocode`; `EXISTING GET /api/pseudocode/{id}`; `EXISTING POST /api/pseudocode`; `EXISTING PUT /api/pseudocode/{id}`; `EXISTING DELETE /api/pseudocode/{id}` | (Tasks 2.1–2.4) | — | In Phase 2, documents become user-owned; endpoints should b… |
| [US-2.5](./user-stories/phase-2/US-2.5.md) | Documents are scoped to the logged-in user | `EXISTING GET /api/pseudocode`; `EXISTING GET /api/pseudocode/{id}` | Ensure `Documents.ownerId` exists and is indexed; enforce F… | — | Configure JWT bearer auth (Task 2.6). Require valid tokens. |
| [US-2.6](./user-stories/phase-2/US-2.6.md) | Example documents for new users | `NEW POST /api/auth/register` | Persist the 3 seeded rows in `Documents` with `ownerId = ne… | — | Seeding ties documents to the user account; the user must o… |

## Phase 3

- New endpoints referenced in this phase: **10**

| User Story | Title | Endpoints touched | DB impact | Services | Auth |
|---|---|---|---|---|---|
| [US-3.1](./user-stories/phase-3/US-3.1.md) | Run pseudocode and see output | `NEW POST /api/pseudocode/execute` | None required for v1 execution (no persistence of runs). | — | — |
| [US-3.2](./user-stories/phase-3/US-3.2.md) | See runtime errors with line numbers | `NEW POST /api/pseudocode/execute` | None. | — | Same as execute endpoint (likely `[Authorize]` after Phase… |
| [US-3.3](./user-stories/phase-3/US-3.3.md) | Use variables and arithmetic | `NEW POST /api/pseudocode/execute` | None. | Execution engine must support: | Same as execute endpoint. |
| [US-3.4](./user-stories/phase-3/US-3.4.md) | Use control flow (IF, FOR, WHILE, REPEAT) | `NEW POST /api/pseudocode/execute` | None. | Execution engine must support: | Same as execute endpoint. |
| [US-3.5](./user-stories/phase-3/US-3.5.md) | Use procedures and functions | `NEW POST /api/pseudocode/execute` | None. | Execution engine must support: | Same as execute endpoint. |
| [US-3.6](./user-stories/phase-3/US-3.6.md) | Use arrays | `NEW POST /api/pseudocode/execute` | None. | Execution engine must support: | Same as execute endpoint. |
| [US-3.7](./user-stories/phase-3/US-3.7.md) | Provide user input during execution | `NEW POST /api/pseudocode/execute`; `NEW POST /api/pseudocode/execute/{executionId}/input`; `NEW DELETE /api/pseudocode/execute/{executionId}` | None required, but the server must hold in-memory execution… | — | If protected, session ids must be bound to the authenticati… |
| [US-3.8](./user-stories/phase-3/US-3.8.md) | Use string built-in functions | `NEW POST /api/pseudocode/execute` | None. | — | Same as execute endpoint. |

## Phase 4

- New endpoints referenced in this phase: **2**

| User Story | Title | Endpoints touched | DB impact | Services | Auth |
|---|---|---|---|---|---|
| [US-4.1](./user-stories/phase-4/US-4.1.md) | Format code with one click | `EXISTING POST /api/pseudocode/format` | None. | — | — |
| [US-4.2](./user-stories/phase-4/US-4.2.md) | See validation errors inline in the editor | `EXISTING POST /api/pseudocode/validate` | None. | — | — |
| [US-4.3](./user-stories/phase-4/US-4.3.md) | Real-time validation as I type | `EXISTING POST /api/pseudocode/validate` | None. | — | Same as validate endpoint (likely `[Authorize]` after Phase… |
| [US-4.4](./user-stories/phase-4/US-4.4.md) | Keyboard shortcuts | `EXISTING POST /api/pseudocode`; `EXISTING PUT /api/pseudocode/{id}`; `EXISTING POST /api/pseudocode/format`; `NEW POST /api/pseudocode/execute` | None specific to shortcuts. | — | — |
| [US-4.5](./user-stories/phase-4/US-4.5.md) | Colour-coded terminal output | `EXISTING POST /api/pseudocode/validate`; `NEW POST /api/pseudocode/execute` | None. | — | Same as the underlying endpoints (likely `[Authorize]` afte… |
| [US-4.6](./user-stories/phase-4/US-4.6.md) | Keyword auto-completion | — | None. | — | N/A. |
| [US-4.7](./user-stories/phase-4/US-4.7.md) | Format on save | `EXISTING POST /api/pseudocode/format`; `EXISTING POST /api/pseudocode`; `EXISTING PUT /api/pseudocode/{id}` | None (this story persists the setting in `localStorage`). | — | After Phase 2, document endpoints should be `[Authorize]` a… |
