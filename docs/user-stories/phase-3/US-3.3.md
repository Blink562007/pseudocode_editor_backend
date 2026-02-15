# US-3.3 · Use variables and arithmetic
**As a** student,
**I want to** declare variables, assign values with ←, and do arithmetic,
**so that** I can write computation-based programs.

**Acceptance Criteria:**
- [ ] `DECLARE x : INTEGER` creates a typed variable
- [ ] `x ← 5` assigns a value
- [ ] `CONSTANT Pi = 3.14` creates an immutable constant
- [ ] Arithmetic: `+`, `-`, `*`, `/`, `MOD`, `DIV` work correctly
- [ ] `OUTPUT x + 10` prints the computed result to the terminal
- [ ] Assigning to a constant produces an error: "Cannot reassign CONSTANT 'Pi'"
- [ ] Using an undeclared variable produces an error

## Backend Requirements

| Endpoints touched | DB impact | Services | Auth |
|---|---|---|---|
| `NEW POST /api/pseudocode/execute` | None | Execution engine must support: | Same as execute endpoint |

- **API endpoints:** Implemented via `POST /api/pseudocode/execute` (new; see US-3.1). No additional endpoints.
- **Database:** None.
- **Service layer logic:** Execution engine must support:
  - Declarations: `DECLARE <id> : <TYPE>` with a typed symbol table.
  - Assignment operator `←` with type checking/conversion rules.
  - Constants: `CONSTANT <id> = <expr>`; prohibit reassignment.
  - Arithmetic operators, including integer division (`DIV`) and modulo (`MOD`) with correct type semantics.
  - Runtime errors for undeclared identifiers and constant reassignment, including line number.
- **Authentication/authorization:** Same as execute endpoint.
- **Error handling / status codes:** User program faults should be returned as runtime errors (not `500`).

**Traces to:** FR-4.3, FR-4.4, Task 3.4


## Screenshot
_No screenshot (no distinct UI change captured)._
