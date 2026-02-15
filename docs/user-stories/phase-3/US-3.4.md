# US-3.4 · Use control flow (IF, FOR, WHILE, REPEAT)
**As a** student,
**I want to** use IF/ELSE, FOR loops, WHILE loops, and REPEAT/UNTIL,
**so that** I can write programs with branching and repetition.

**Acceptance Criteria:**
- [ ] `IF condition THEN ... ELSE ... ENDIF` executes the correct branch
- [ ] `FOR i ← 1 TO 10 ... NEXT i` iterates correctly (with optional STEP)
- [ ] `WHILE condition DO ... ENDWHILE` loops while true
- [ ] `REPEAT ... UNTIL condition` loops until true (executes at least once)
- [ ] `CASE OF ... OTHERWISE ... ENDCASE` matches values correctly
- [ ] Nested control structures work (e.g. FOR inside IF inside WHILE)
- [ ] Infinite loops are terminated after 5 seconds with: "Execution timeout — possible infinite loop"

## Backend Requirements

| Endpoints touched | DB impact | Services | Auth |
|---|---|---|---|
| `NEW POST /api/pseudocode/execute` | None | Execution engine must support: | Same as execute endpoint |

- **API endpoints:** Implemented via `POST /api/pseudocode/execute` (new; see US-3.1). No additional endpoints.
- **Database:** None.
- **Service layer logic:** Execution engine must support:
  - Control structures: `IF/ELSE/ENDIF`, `FOR/NEXT` (optional `STEP`), `WHILE/ENDWHILE`, `REPEAT/UNTIL`, `CASE/OTHERWISE/ENDCASE`.
  - Nested scopes and correct variable visibility.
  - Safety limits: timeout and/or instruction limit; on termination emit a runtime error with the specified message and a best-effort line number.
- **Authentication/authorization:** Same as execute endpoint.
- **Error handling / status codes:** Timeout/limit breaches are user runtime errors returned to the client (not server `500`).

**Traces to:** FR-4.5, FR-4.12, Task 3.5, 3.11


## Screenshot
_No screenshot (no distinct UI change captured)._
