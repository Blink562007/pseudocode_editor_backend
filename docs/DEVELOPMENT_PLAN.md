# Pseudocode Editor — Development Plan

> **Version:** 1.0 · **Date:** 2026-02-15
> **Reference:** See [REQUIREMENTS.md](./REQUIREMENTS.md) for full product requirements.

---

## 1. Current State Assessment

### What exists today

| Area | Status | Notes |
|---|---|---|
| Backend framework | ✅ Working | ASP.NET Core 9, controllers, services, DI |
| Document CRUD | ✅ Working | 7 endpoints (GET/POST/PUT/DELETE + validate + format) |
| Validation service | ✅ Working | Keyword case, indentation, bracket matching; 100+ unit tests |
| Formatting service | ✅ Working | Keyword normalisation, indentation; 70+ unit tests |
| Frontend scaffold | ✅ Working | React 19 + Vite + Monaco Editor |
| Syntax highlighting | ✅ Working | Custom pseudocode language in Monaco |
| API client | ⚠️ Broken | Field-name mismatches (`code` vs `content`); missing CRUD functions |
| Storage | ⚠️ Temporary | In-memory `List<>` — data lost on restart |
| Execution engine | ❌ Missing | No interpreter, no execute endpoint |
| User auth | ❌ Missing | No accounts, no login |
| Code conversion | ❌ Missing | No pseudocode → Python converter |
| AI integration | ❌ Missing | No LLM service |
| PDF features | ❌ Missing | No upload, extraction, or answer generation |
| Frontend tests | ❌ Missing | Zero test files |
| CI/CD | ❌ Missing | No pipeline |

### Critical bugs to fix first
1. `validatePseudocode()` sends `{ code }` — backend expects `{ content }`
2. `executePseudocode()` POSTs to the create-document endpoint with wrong payload
3. No API client functions for CRUD or format operations

---

## 2. Phase Overview

```
Phase 0 ──► Phase 1 ──► Phase 2 ──┬──► Phase 3 ──► Phase 5 ──► Phase 7 ──► Phase 8
  Fix bugs    Core        Auth &   │    Execution    Conversion   PDF         Deploy
              Frontend    Storage  │    Engine       + AI Review  Features
                                   │
                                   └──► Phase 4
                                        Editor UX
```

| Phase | Name | Effort | Dependencies |
|---|---|---|---|
| **0** | Fix API contract mismatches | 0.5 day | None |
| **1** | Frontend architecture + document management | 3–4 days | Phase 0 |
| **2** | Authentication + persistent storage | 3–4 days | Phase 0 |
| **3** | Pseudocode execution engine | 5–8 days | Phase 1 |
| **4** | Editor UX enhancements | 2–3 days | Phase 1 |
| **5** | Code conversion + AI code review | 3–5 days | Phase 3 |
| **6** | Save & share (public links) | 2–3 days | Phase 2 |
| **7** | PDF exam paper features | 5–7 days | Phase 2, Phase 5 |
| **8** | Testing, DevOps, deployment | 3–4 days | All above |
| | **Total estimated effort** | **27–39 days** | |

---

## 3. Phase Details

### Phase 0 — Fix API Contract Mismatches (0.5 day)

> **Goal:** Make the frontend actually communicate with the backend correctly.

| Task | Detail | File(s) |
|---|---|---|
| 0.1 | Fix `validatePseudocode()` — change `{ code }` → `{ content }` | `pseudocodeApi.ts` |
| 0.2 | Remove broken `executePseudocode()` (no backend endpoint yet) | `pseudocodeApi.ts` |
| 0.3 | Add missing API functions: `getAllDocuments`, `getDocument`, `createDocument`, `updateDocument`, `deleteDocument`, `formatContent` | `pseudocodeApi.ts` |
| 0.4 | Add TypeScript interfaces: `PseudocodeDocument`, `ValidationResult`, `ValidationError`, `ValidationWarning`, `FormatResponse` | `types.ts` (new) |
| 0.5 | Add `.env` with `VITE_API_URL=https://localhost:7065` | `.env` |

**Deliverable:** Frontend can validate, format, and CRUD documents against the real backend.

---

### Phase 1 — Frontend Architecture + Document Management (3–4 days)

> **Goal:** Restructure the monolithic App.tsx and build document management UI.

| Task | Detail |
|---|---|
| 1.1 | Extract components: `EditorPanel`, `TerminalPanel`, `ControlBar`, `Sidebar`, `Header` |
| 1.2 | Add state management with React Context: `DocumentContext` (current doc, doc list, dirty state) |
| 1.3 | Build document sidebar: list all docs, create new, delete, rename |
| 1.4 | Build save/load flow: "Save" → POST (new) or PUT (existing), "Open" → GET by ID |
| 1.5 | Add auto-save to localStorage (debounced, 2s) as offline fallback |
| 1.6 | Remove leftover Vite template CSS (`.logo`, `.card`, `@keyframes`) |
| 1.7 | Add loading states, empty states, error toasts |

**Deliverable:** Multi-document editor with sidebar, save/load, clean component structure.

---

### Phase 2 — Authentication + Persistent Storage (3–4 days)

> **Goal:** Users can create accounts and their documents persist across sessions.

| Task | Detail |
|---|---|
| 2.1 | Add PostgreSQL (or SQLite for dev) with EF Core |
| 2.2 | Create `PseudocodeDbContext` with `Users`, `Documents` tables |
| 2.3 | Run initial EF Core migration |
| 2.4 | Replace in-memory `List<>` with repository pattern using DbContext |
| 2.5 | Add ASP.NET Identity for user registration/login |
| 2.6 | Add JWT token authentication on API endpoints |
| 2.7 | Build frontend login/register pages |
| 2.8 | Add auth token to all API requests (Bearer header) |
| 2.9 | Seed 3 example pseudocode documents for new users |

**Deliverable:** Persistent user accounts with saved documents.

#### 💡 Advice: Don't over-engineer auth early
Start with simple email/password. Add Google OAuth later as a convenience feature.
You can even start Phase 1 with anonymous localStorage usage and add auth in Phase 2 without blocking.

---

### Phase 3 — Pseudocode Execution Engine (5–8 days)

> **Goal:** Users can press "Run" and see OUTPUT in the terminal panel.

| Task | Detail |
|---|---|
| 3.1 | Build **Lexer/Tokenizer** — tokenise keywords, literals, operators, identifiers |
| 3.2 | Build **Parser** — recursive-descent parser producing an AST |
| 3.3 | Build **Interpreter** — tree-walk evaluator with Environment/Scope stack |
| 3.4 | Support core constructs first: variables, constants, assignment (←), arithmetic, comparison, logical |
| 3.5 | Support control flow: IF/ELSE/ENDIF, CASE/OF/ENDCASE, FOR/NEXT, WHILE/ENDWHILE, REPEAT/UNTIL |
| 3.6 | Support PROCEDURE and FUNCTION (BYREF, BYVAL parameters, RETURNS) |
| 3.7 | Support typed arrays with custom bounds: `ARRAY[1:10] OF INTEGER` |
| 3.8 | Support INPUT (prompt user) and OUTPUT (print to terminal) |
| 3.9 | Support string built-ins: LENGTH, SUBSTRING, UCASE, LCASE, LEFT, RIGHT, MID |
| 3.10 | Add `POST /api/pseudocode/execute` endpoint — accepts `{ content }`, returns `{ success, output, error, line }` |
| 3.11 | Add execution timeout (5s via CancellationToken) and iteration cap (100,000) |
| 3.12 | Wire frontend Run button → execute endpoint → display in terminal |
| 3.13 | Write comprehensive unit tests for lexer, parser, and interpreter |

**Deliverable:** Working pseudocode interpreter supporting Cambridge core syllabus constructs.

#### 💡 Advice: The execution engine is the hardest core feature
The Cambridge pseudocode spec is essentially a full programming language. Start with the **MVP subset** (tasks 3.1–3.8) and add advanced features (records, file I/O, OOP) incrementally. An alternative approach is to transpile pseudocode to Python and execute Python — simpler to implement, but loses the educational value of a native interpreter and introduces a Python runtime dependency.

**Recommended approach:** Build an AST-based interpreter. The AST can later be reused for the code conversion feature (Phase 5), making it a high-value investment.

---

### Phase 4 — Editor UX Enhancements (2–3 days)

> **Goal:** Make the editor feel polished and productive.

| Task | Detail |
|---|---|
| 4.1 | Add **Format** button calling `POST /api/pseudocode/format` and replacing editor content |
| 4.2 | Push validation errors/warnings into Monaco's marker API for inline squiggles |
| 4.3 | Add real-time validation (debounced ~500ms as user types) |
| 4.4 | Add keyboard shortcuts: `Ctrl+S` save, `Ctrl+Shift+F` format, `F5` run |
| 4.5 | Improve terminal: colour-coded output (red=error, yellow=warning, green=success), clear button |
| 4.6 | Add pseudocode keyword auto-completion in Monaco |
| 4.7 | Add format-on-save option in settings |

**Deliverable:** Professional-feeling editor with inline feedback and shortcuts.

---

### Phase 5 — Code Conversion + AI Code Review (3–5 days)

> **Goal:** Convert pseudocode to Python; get AI-powered code review.

| Task | Detail |
|---|---|
| 5.1 | Build **Pseudocode → Python converter** using the AST from Phase 3 |
| 5.2 | Map Cambridge constructs to Python: ← → `=`, OUTPUT → `print()`, FOR/NEXT → `for...range`, etc. |
| 5.3 | Build side-by-side view: pseudocode on left, Python on right (read-only Monaco) |
| 5.4 | Add `POST /api/pseudocode/convert` endpoint — accepts `{ content, targetLanguage }`, returns `{ convertedCode }` |
| 5.5 | Integrate **OpenAI API** (or configurable LLM provider) for code review |
| 5.6 | Add `POST /api/pseudocode/review` endpoint — accepts `{ content }`, streams review comments |
| 5.7 | Build AI review panel in frontend — show suggestions with Cambridge spec references |
| 5.8 | Add rate limiting on AI endpoints (20 requests/hour per user) |
| 5.9 | Add `IAiService` abstraction so LLM provider can be swapped |

**Deliverable:** Pseudocode ↔ Python conversion and AI-powered code review.

#### 💡 Advice: Use an AI service abstraction
Define `IAiService` with methods like `ReviewCodeAsync(string code)` and `GenerateAnswerAsync(string markScheme)`. Implement `OpenAiService` first, but the abstraction means you can later swap to Anthropic, Google Gemini, or a local model without changing any calling code.

---

### Phase 6 — Save & Share (2–3 days)

> **Goal:** Users can share their code via public links.

| Task | Detail |
|---|---|
| 6.1 | Add `isPublic` and `shareToken` fields to `PseudocodeDocument` |
| 6.2 | Add `POST /api/pseudocode/{id}/share` — generates a unique share token |
| 6.3 | Add `GET /api/share/{token}` — returns document (read-only, no auth required) |
| 6.4 | Build shared document viewer page (syntax-highlighted, read-only) |
| 6.5 | Add "Fork" button — copies shared doc into your account |
| 6.6 | Add "Copy link" button in the editor UI |

**Deliverable:** Shareable read-only links for any document.

#### 💡 Advice: Keep sharing simple for v1
A shareable URL with a unique token is sufficient. Don't build classroom/assignment features yet — that's a separate product feature that can come in v2 once the core platform is stable.

---

### Phase 7 — PDF Exam Paper Features (5–7 days)

> **Goal:** Teachers upload exam papers and mark schemes; the system extracts questions and generates model answers.

| Task | Detail |
|---|---|
| 7.1 | Add file upload endpoint: `POST /api/papers/upload` — accepts PDF, stores to blob storage |
| 7.2 | Add `ExamPaper` and `ExamQuestion` EF Core entities + migration |
| 7.3 | Integrate PDF extraction service (Azure Document Intelligence or pdf.js + LLM) |
| 7.4 | Add `POST /api/papers/{id}/extract` — extracts questions, stores to DB |
| 7.5 | Build frontend PDF upload UI with drag-and-drop |
| 7.6 | Build extracted questions browser (list view, click to open in editor) |
| 7.7 | Add mark scheme upload: `POST /api/papers/{id}/markscheme` |
| 7.8 | Add AI answer generation: parse mark scheme → generate pseudocode answer + explanation |
| 7.9 | Build model answer viewer with line-by-line mark annotation |
| 7.10 | Add teacher approval workflow: teacher can edit generated answers before publishing |

**Deliverable:** End-to-end flow: upload paper → extract questions → upload mark scheme → generate model answers.

#### 💡 Advice: PDF extraction is the riskiest feature
Cambridge exam papers vary in format (digital vs scanned, tables, diagrams, multi-column). Expect to iterate heavily on extraction accuracy. Start with **digital PDFs only** (text-selectable) and add OCR for scanned papers later. Consider a human-in-the-loop review step where the teacher confirms/corrects extracted questions before they're saved.

---

### Phase 8 — Testing, DevOps, Deployment (3–4 days)

> **Goal:** Comprehensive test coverage, containerised deployment, CI/CD pipeline.

| Task | Detail |
|---|---|
| 8.1 | Add **Vitest** + **React Testing Library** for frontend component tests |
| 8.2 | Add **MSW** (Mock Service Worker) for API client tests without running backend |
| 8.3 | Add **Playwright** end-to-end tests: create doc → edit → validate → run → see output |
| 8.4 | Add backend controller integration tests with `WebApplicationFactory<Program>` |
| 8.5 | Configure **Prettier** + tighten ESLint rules; fix all warnings |
| 8.6 | Create `Dockerfile` for backend (`dotnet publish` → runtime image) |
| 8.7 | Create `Dockerfile` for frontend (`npm run build` → nginx) |
| 8.8 | Create `docker-compose.yml` — backend + frontend + PostgreSQL + Redis |
| 8.9 | Create GitHub Actions CI: build → test → lint on every PR |
| 8.10 | Create GitHub Actions CD: deploy to staging on merge to `main` |

**Deliverable:** Fully tested, containerised, and CI/CD-enabled project.

---

## 4. Risk Register

| # | Risk | Impact | Likelihood | Mitigation |
|---|---|---|---|---|
| R1 | Execution engine scope creep — Cambridge spec is larger than expected | High | High | Start with MVP subset (variables, control flow, I/O). Add advanced features incrementally. Track which spec features are covered. |
| R2 | PDF extraction accuracy — exam papers vary widely in format | High | High | Start with digital PDFs only. Add human review step. Use structured extraction (Azure Doc Intelligence) over raw OCR. |
| R3 | AI API costs — GPT-4o is expensive at scale | Medium | Medium | Cache review results. Rate limit per user. Use GPT-4o-mini for simpler tasks. Add cost monitoring dashboard. |
| R4 | LLM provider lock-in | Medium | Low | Use `IAiService` abstraction from day one. Avoid provider-specific features in business logic. |
| R5 | Security — code execution on server | High | Medium | Sandbox interpreter with timeout + memory + iteration limits. No file system access. No network access from interpreter. |
| R6 | Data loss — in-memory storage | High | Certain | Phase 2 replaces in-memory with database. Until then, localStorage fallback in frontend. |
| R7 | Authentication complexity | Medium | Medium | Start with email/password only. Add OAuth later. Use well-tested library (ASP.NET Identity). |

---

## 5. Key Technical Decisions

| Decision | Choice | Rationale |
|---|---|---|
| **Interpreter approach** | AST-based tree-walk interpreter | Reusable AST for both execution and code conversion; easier to debug than transpilation; educational value |
| **Database** | PostgreSQL (prod) / SQLite (dev) | Relational data fits well; EF Core supports both; PostgreSQL scales for multi-user |
| **AI provider** | OpenAI (GPT-4o) behind abstraction | Best code understanding as of 2026; abstraction allows swapping later |
| **PDF processing** | Azure Document Intelligence | Structured extraction with table/layout understanding; falls back to LLM for interpretation |
| **Auth** | ASP.NET Identity + JWT | Built into the framework; battle-tested; supports OAuth extension |
| **Frontend state** | React Context + useReducer | Sufficient for this app's complexity; avoids Redux overhead |
| **Sharing** | Token-based public URLs | Simple, no auth required for viewers; scales without complexity |

---

## 6. Advice Summary

1. **Build the core loop first** — validate → format → execute → save. This is the daily-use MVP. Don't jump to AI or PDF features before this works perfectly.

2. **The execution engine is the biggest investment** — budget extra time. The Cambridge pseudocode spec includes typed variables, arrays with custom bounds, records, procedures/functions with pass-by-reference, file I/O, and OOP. That's a real programming language.

3. **Use the AST for everything** — the same AST powers the interpreter (Phase 3) and the code converter (Phase 5). Build it well once.

4. **AI features should be behind a service abstraction** — `IAiService` with `ReviewCodeAsync`, `GenerateAnswerAsync`, `ExtractQuestionsAsync`. Swap providers without touching business logic.

5. **PDF extraction will surprise you** — exam papers have headers, footers, page numbers, watermarks, tables, code blocks in monospace, diagrams, and multi-part question numbering. Start with clean digital PDFs and add complexity gradually. Always include a human review step.

6. **Keep sharing dead simple for v1** — a unique URL token that anyone can open. Classroom features, assignments, and group management are v2.

7. **Don't skip tests** — the existing 170+ backend tests are a huge asset. Maintain that standard. Add frontend tests early (Phase 8 could start in parallel with Phase 4).

8. **Fix the API contract first** — the frontend literally cannot talk to the backend correctly right now. Phase 0 is a 2-hour fix that unblocks everything else.