# Pseudocode Editor — Product Requirements Document

> **Version:** 1.0 · **Date:** 2026-02-15
> **Target users:** Cambridge IGCSE / AS & A Level Computer Science students and teachers

---

## 1. Product Vision

An online platform where students write, validate, format, execute, and convert Cambridge-style pseudocode — and where teachers can review student work, share model answers, and extract exercises from exam papers. The platform enforces the **Cambridge International pseudocode specification** so students practise with the exact syntax they will encounter in exams.

---

## 2. User Personas

| Persona | Goals | Key Actions |
|---|---|---|
| **Student** | Practise pseudocode, get instant feedback, prepare for exams | Write code → validate → run → fix errors → save |
| **Teacher** | Set exercises, review student work, create model answers | Upload exam paper → extract questions → share with class → review submissions |
| **Self-learner** | Learn pseudocode independently | Browse examples → edit → run → convert to Python to compare |

---

## 3. Functional Requirements

### FR-1 · Pseudocode Editor
| ID | Requirement | Priority |
|---|---|---|
| FR-1.1 | Browser-based code editor with Cambridge pseudocode syntax highlighting | **Must** |
| FR-1.2 | Line numbers, auto-indentation (3-space multiples per Cambridge spec) | **Must** |
| FR-1.3 | Keyword auto-completion (IF/THEN/ENDIF, FOR/NEXT, WHILE/ENDWHILE, etc.) | Should |
| FR-1.4 | Bracket/block matching and auto-closing | Should |
| FR-1.5 | Multiple open documents with tabs | Could |

### FR-2 · Validation & Feedback
| ID | Requirement | Priority |
|---|---|---|
| FR-2.1 | Validate pseudocode against Cambridge keyword-case rules (UPPERCASE keywords) | **Must** |
| FR-2.2 | Validate indentation (multiples of 3 spaces) | **Must** |
| FR-2.3 | Validate syntax structure (matched IF/ENDIF, FOR/NEXT, WHILE/ENDWHILE, etc.) | **Must** |
| FR-2.4 | Validate bracket/parenthesis matching | **Must** |
| FR-2.5 | Display errors inline in the editor (red squiggles) with line numbers | **Must** |
| FR-2.6 | Display warnings inline (yellow squiggles) for style issues | Should |
| FR-2.7 | Real-time validation as the user types (debounced, ~500ms) | Should |

### FR-3 · Code Formatting
| ID | Requirement | Priority |
|---|---|---|
| FR-3.1 | One-click format to Cambridge standard (keyword case, indentation, spacing) | **Must** |
| FR-3.2 | Format-on-save option | Should |
| FR-3.3 | Preview diff before applying format changes | Could |

### FR-4 · Code Execution
| ID | Requirement | Priority |
|---|---|---|
| FR-4.1 | Execute pseudocode and display OUTPUT in a terminal panel | **Must** |
| FR-4.2 | Support INPUT — prompt the user for values during execution | **Must** |
| FR-4.3 | Support variables, constants, assignments (←) | **Must** |
| FR-4.4 | Support arithmetic (+, -, *, /, MOD, DIV), comparison, logical operators | **Must** |
| FR-4.5 | Support control flow: IF/ELSE/ENDIF, CASE, FOR/NEXT, WHILE/ENDWHILE, REPEAT/UNTIL | **Must** |
| FR-4.6 | Support PROCEDURE and FUNCTION with parameters (BYREF, BYVAL) | **Must** |
| FR-4.7 | Support typed arrays with custom bounds (ARRAY[1:10] OF INTEGER) | **Must** |
| FR-4.8 | Support user-defined types (TYPE/ENDTYPE) and records | Should |
| FR-4.9 | Support string operations (LENGTH, SUBSTRING, UCASE, LCASE, etc.) | Should |
| FR-4.10 | Support file operations (OPENFILE, READFILE, WRITEFILE, CLOSEFILE) | Could |
| FR-4.11 | Support OOP (CLASS, INHERITS, NEW, PUBLIC, PRIVATE) | Could |
| FR-4.12 | Execution timeout (max 5 seconds) to prevent infinite loops | **Must** |
| FR-4.13 | Display runtime errors with line number highlighting | **Must** |
| FR-4.14 | Step-by-step debug mode (step over, inspect variables) | Could |

### FR-5 · Code Conversion
| ID | Requirement | Priority |
|---|---|---|
| FR-5.1 | Convert pseudocode → Python with correct syntax mapping | **Must** |
| FR-5.2 | Preserve comments and structure in converted code | Should |
| FR-5.3 | Side-by-side view: pseudocode on left, Python on right | Should |
| FR-5.4 | Convert pseudocode → JavaScript | Could |
| FR-5.5 | Convert pseudocode → Java | Could |
| FR-5.6 | Convert Python → pseudocode (reverse) | Could |

### FR-6 · AI Code Review
| ID | Requirement | Priority |
|---|---|---|
| FR-6.1 | Submit code for AI review; receive suggestions on correctness, style, efficiency | **Must** |
| FR-6.2 | Suggestions reference Cambridge specification rules | Should |
| FR-6.3 | Explain *why* something is wrong, not just *what* is wrong | **Must** |
| FR-6.4 | Rate-limit AI reviews (e.g. 20/hour per user) to manage cost | **Must** |
| FR-6.5 | Teacher can disable AI review for exam-condition practice | Could |

### FR-7 · Save & Share
| ID | Requirement | Priority |
|---|---|---|
| FR-7.1 | User accounts (email/password sign-up, or OAuth with Google) | **Must** |
| FR-7.2 | Save documents to user's account (title, content, language, timestamps) | **Must** |
| FR-7.3 | List, open, rename, delete saved documents | **Must** |
| FR-7.4 | Generate shareable read-only link for any document | **Must** |
| FR-7.5 | Shared link shows formatted code with syntax highlighting (no login required) | **Must** |
| FR-7.6 | "Fork" a shared document into your own account | Should |
| FR-7.7 | Teacher creates a "classroom" and shares exercises with students | Could |
| FR-7.8 | Auto-save to localStorage as fallback when offline | Should |

### FR-8 · PDF Exam Paper Extraction (Advanced)
| ID | Requirement | Priority |
|---|---|---|
| FR-8.1 | Upload a PDF exam paper (Cambridge CS past paper) | **Must** |
| FR-8.2 | Extract individual questions with question numbers | **Must** |
| FR-8.3 | Extract any pseudocode snippets embedded in questions | **Must** |
| FR-8.4 | Display extracted questions in a browsable list | **Must** |
| FR-8.5 | User clicks a question → opens editor pre-filled with any given code | Should |
| FR-8.6 | Support both digital and scanned PDFs (OCR) | Should |
| FR-8.7 | Handle tables, diagrams, and multi-part questions | Could |

### FR-9 · PDF Mark Scheme → Model Answer Generation (Advanced)
| ID | Requirement | Priority |
|---|---|---|
| FR-9.1 | Upload a PDF mark scheme for a specific exam paper | **Must** |
| FR-9.2 | Extract marking criteria (mark allocation, expected answers) | **Must** |
| FR-9.3 | Generate example pseudocode answer based on mark scheme | **Must** |
| FR-9.4 | Provide line-by-line explanation of why each part earns marks | **Must** |
| FR-9.5 | Link generated answer back to the original question (from FR-8) | Should |
| FR-9.6 | Teacher can edit/approve generated answers before sharing | Should |

---

## 4. Non-Functional Requirements

| ID | Requirement | Target |
|---|---|---|
| NFR-1 | Page load time | < 3 seconds on 4G connection |
| NFR-2 | Code execution latency | < 2 seconds for typical programs |
| NFR-3 | AI review response time | < 10 seconds (streaming) |
| NFR-4 | PDF extraction time | < 30 seconds per paper |
| NFR-5 | Concurrent users | Support 100+ simultaneous editors |
| NFR-6 | Data persistence | Documents survive server restarts |
| NFR-7 | Security | Sandboxed execution, no server-side code injection |
| NFR-8 | Accessibility | WCAG 2.1 AA compliance |
| NFR-9 | Browser support | Chrome, Firefox, Safari, Edge (latest 2 versions) |
| NFR-10 | Mobile | Responsive layout; editor usable on tablet (not phone) |



---

## 5. Technical Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                        Frontend                             │
│  React + TypeScript + Vite                                  │
│  ┌──────────┐ ┌──────────┐ ┌────────┐ ┌────────────────┐   │
│  │ Monaco   │ │ Terminal  │ │ PDF    │ │ Document       │   │
│  │ Editor   │ │ Panel    │ │ Viewer │ │ Manager        │   │
│  └────┬─────┘ └────┬─────┘ └───┬────┘ └───────┬────────┘   │
│       └─────────────┴───────────┴──────────────┘            │
│                         │  API Client (fetch)               │
└─────────────────────────┼───────────────────────────────────┘
                          │ HTTPS / JSON
┌─────────────────────────┼───────────────────────────────────┐
│                    Backend API                               │
│  ASP.NET Core (.NET 9)                                      │
│  ┌────────────┐ ┌────────────┐ ┌────────────┐              │
│  │ Pseudocode │ │ Auth       │ │ PDF        │              │
│  │ Controller │ │ Controller │ │ Controller │              │
│  └─────┬──────┘ └─────┬──────┘ └─────┬──────┘              │
│  ┌─────┴──────────────┴──────────────┴──────┐              │
│  │            Service Layer                  │              │
│  │ ┌──────────┐ ┌──────────┐ ┌───────────┐  │              │
│  │ │Validation│ │Formatting│ │Interpreter │  │              │
│  │ └──────────┘ └──────────┘ └───────────┘  │              │
│  │ ┌──────────┐ ┌──────────┐ ┌───────────┐  │              │
│  │ │Converter │ │AI Review │ │PDF Extract │  │              │
│  │ └──────────┘ └──────────┘ └───────────┘  │              │
│  └───────────────────┬───────────────────────┘              │
│                      │                                       │
│  ┌───────────┐ ┌─────┴─────┐ ┌──────────────┐              │
│  │ Database  │ │ LLM API   │ │ Blob Storage │              │
│  │ (Postgres)│ │ (OpenAI)  │ │ (PDF files)  │              │
│  └───────────┘ └───────────┘ └──────────────┘              │
└────────────────────────────────────────────────────────────┘
```

### Technology Stack

| Layer | Technology | Rationale |
|---|---|---|
| Frontend | React 19 + TypeScript + Vite | Already in place; Monaco Editor for code editing |
| Backend API | ASP.NET Core 9 | Already in place; strong typing, good performance |
| Database | PostgreSQL (prod) / SQLite (dev) | Relational data (users, documents, questions); EF Core ORM |
| Authentication | ASP.NET Identity + JWT | Built-in, well-supported; OAuth for Google login |
| AI / LLM | OpenAI API (GPT-4o) | Code review, answer generation, PDF understanding |
| PDF Processing | Azure Document Intelligence or pdf.js + LLM | Structured extraction from exam papers |
| Blob Storage | Local filesystem (dev) / Azure Blob (prod) | PDF file uploads |
| Caching | In-memory (dev) / Redis (prod) | Cache AI responses, session data |
| Hosting | Docker + any cloud (Azure, Railway, Fly.io) | Containerised deployment |

---

## 6. Data Models (Conceptual)

### User
| Field | Type | Notes |
|---|---|---|
| id | UUID | Primary key |
| email | string | Unique, used for login |
| displayName | string | Shown in UI and shared links |
| passwordHash | string | Bcrypt hash |
| role | enum | `student`, `teacher`, `admin` |
| createdAt | datetime | UTC |

### PseudocodeDocument
| Field | Type | Notes |
|---|---|---|
| id | UUID | Primary key |
| ownerId | UUID | FK → User |
| title | string | Default "Untitled" |
| content | string | Raw pseudocode text |
| language | string | Default "pseudocode" |
| isPublic | boolean | Whether shareable link is active |
| shareToken | string? | Unique token for public URL |
| createdAt | datetime | UTC |
| updatedAt | datetime | UTC |

### ExamPaper
| Field | Type | Notes |
|---|---|---|
| id | UUID | Primary key |
| uploadedBy | UUID | FK → User |
| fileName | string | Original PDF filename |
| blobPath | string | Storage path |
| paperYear | int? | e.g. 2024 |
| paperComponent | string? | e.g. "Paper 2" |
| extractedAt | datetime? | When extraction completed |

### ExamQuestion
| Field | Type | Notes |
|---|---|---|
| id | UUID | Primary key |
| examPaperId | UUID | FK → ExamPaper |
| questionNumber | string | e.g. "3(a)(ii)" |
| questionText | string | Extracted question body |
| embeddedCode | string? | Any pseudocode in the question |
| marks | int? | Mark allocation |
| modelAnswer | string? | Generated or teacher-written answer |
| explanation | string? | Line-by-line explanation |

---

## 7. API Surface (Summary)

Detailed endpoint documentation is in [`pseudocode_editor_backend/src/API_CONTRACT.md`](../pseudocode_editor_backend/src/API_CONTRACT.md).

| Group | Endpoints | Status |
|---|---|---|
| **Documents CRUD** | GET/POST/PUT/DELETE `/api/pseudocode` | ✅ Built |
| **Validation** | POST `/api/pseudocode/validate` | ✅ Built |
| **Formatting** | POST `/api/pseudocode/format` | ✅ Built |
| **Execution** | POST `/api/pseudocode/execute` | 🔲 Not built |
| **Conversion** | POST `/api/pseudocode/convert` | 🔲 Not built |
| **AI Review** | POST `/api/pseudocode/review` | 🔲 Not built |
| **Auth** | POST `/api/auth/register`, `/login`, `/me` | 🔲 Not built |
| **Sharing** | GET `/api/share/{token}` | 🔲 Not built |
| **PDF Upload** | POST `/api/papers/upload` | 🔲 Not built |
| **PDF Extract** | POST `/api/papers/{id}/extract` | 🔲 Not built |
| **Questions** | GET `/api/papers/{id}/questions` | 🔲 Not built |
| **Mark Scheme** | POST `/api/papers/{id}/markscheme` | 🔲 Not built |

---

## 8. Out of Scope (v1)

- Real-time collaborative editing (Google Docs style)
- Mobile-native apps (iOS/Android)
- Offline-first PWA with full sync
- Grading / mark tracking system
- Integration with school LMS (Google Classroom, Canvas)
- Multi-language UI (i18n) — English only for v1