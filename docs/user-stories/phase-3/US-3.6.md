# US-3.6 · Use arrays
**As a** student,
**I want to** declare and use arrays with custom bounds,
**so that** I can work with collections of data.

**Acceptance Criteria:**
- [ ] `DECLARE names : ARRAY[1:5] OF STRING` creates an array with bounds 1 to 5
- [ ] `names[1] ← "Alice"` assigns to an index
- [ ] `OUTPUT names[1]` reads from an index
- [ ] Accessing an index outside bounds produces: "Index out of bounds"
- [ ] 2D arrays work: `DECLARE grid : ARRAY[1:3, 1:3] OF INTEGER`
- [ ] Array elements are initialised to default values (0 for INTEGER, "" for STRING)

## Backend Requirements

| Endpoints touched | DB impact | Services | Auth |
|---|---|---|---|
| `NEW POST /api/pseudocode/execute` | None | Execution engine must support: | Same as execute endpoint |

- **API endpoints:** Implemented via `POST /api/pseudocode/execute` (new; see US-3.1). No additional endpoints.
- **Database:** None.
- **Service layer logic:** Execution engine must support:
  - Array declarations with custom bounds (including multi-dimensional bounds).
  - Default initialization per element type.
  - Indexing and assignment with bounds checks; out-of-bounds produces a runtime error with a helpful message and line number.
- **Authentication/authorization:** Same as execute endpoint.
- **Error handling / status codes:** Treat bounds issues as user runtime errors (not `500`).

**Traces to:** FR-4.7, Task 3.7


## Screenshot
_No screenshot (no distinct UI change captured)._
