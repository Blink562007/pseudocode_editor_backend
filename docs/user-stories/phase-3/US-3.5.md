# US-3.5 · Use procedures and functions
**As a** student,
**I want to** define and call PROCEDURE and FUNCTION,
**so that** I can write modular, reusable code.

**Acceptance Criteria:**
- [ ] `PROCEDURE Name(param : INTEGER) ... ENDPROCEDURE` defines a procedure
- [ ] `CALL Name(5)` calls the procedure with arguments
- [ ] `FUNCTION Name(x : INTEGER) RETURNS INTEGER ... ENDFUNCTION` defines a function
- [ ] Functions return values usable in expressions: `y ← Name(10)`
- [ ] BYVAL parameters are copied; BYREF parameters modify the caller's variable
- [ ] Calling an undefined procedure/function produces: "Procedure 'X' is not defined"
- [ ] Recursive calls work correctly (with call stack depth limit)

## Backend Requirements

| Endpoints touched | DB impact | Services | Auth |
|---|---|---|---|
| `NEW POST /api/pseudocode/execute` | None | Execution engine must support: | Same as execute endpoint |

- **API endpoints:** Implemented via `POST /api/pseudocode/execute` (new; see US-3.1). No additional endpoints.
- **Database:** None.
- **Service layer logic:** Execution engine must support:
  - Parsing and storing procedure/function definitions.
  - Call frames / scope handling (locals vs globals).
  - Parameter passing modes: BYVAL copies; `BYREF` aliases the caller’s variable.
  - Return semantics for functions, including type checking.
  - Recursion with a configurable depth limit; exceeding it produces a runtime error.
  - Undefined procedure/function calls produce clear runtime errors with line number.
- **Authentication/authorization:** Same as execute endpoint.
- **Error handling / status codes:** User runtime/semantic errors returned as runtime errors; engine faults as `500`.

**Traces to:** FR-4.6, Task 3.6


## Screenshot
_No screenshot (no distinct UI change captured)._
