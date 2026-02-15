# US-3.8 · Use string built-in functions
**As a** student,
**I want to** use built-in string functions like LENGTH, SUBSTRING, UCASE,
**so that** I can manipulate text in my programs.

**Acceptance Criteria:**
- [ ] `LENGTH("hello")` returns 5
- [ ] `SUBSTRING("hello", 2, 3)` returns "llo" (start index, length)
- [ ] `UCASE("hello")` returns "HELLO"
- [ ] `LCASE("HELLO")` returns "hello"
- [ ] `LEFT("hello", 3)` returns "hel"
- [ ] `RIGHT("hello", 3)` returns "llo"
- [ ] Calling with wrong argument types produces a clear error

## Backend Requirements

| Endpoints touched | DB impact | Services | Auth |
|---|---|---|---|
| `NEW POST /api/pseudocode/execute` | None | Implement built-ins: `LENGTH`, `SUBSTRING`, `UCASE`, `LCASE… | Same as execute endpoint |

- **API endpoints:** Implemented via `POST /api/pseudocode/execute` (new; see US-3.1). No additional endpoints.
- **Database:** None.
- **Service layer logic:**
  - Implement built-ins: `LENGTH`, `SUBSTRING`, `UCASE`, `LCASE`, `LEFT`, `RIGHT`.
  - Validate argument count/types and return clear runtime errors when invalid.
  - Ensure indexing semantics match the Cambridge expectation (the AC example implies `SUBSTRING("hello", 2, 3)` returns "llo").
- **Authentication/authorization:** Same as execute endpoint.
- **Error handling / status codes:** Wrong types/counts are runtime errors with line numbers.

**Traces to:** FR-4.9, Task 3.9


## Screenshot
_No screenshot (no distinct UI change captured)._
