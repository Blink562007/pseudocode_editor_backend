# Pseudocode Editor API — Frontend Access Guide

> Comprehensive reference for frontend developers implementing an API client against the Pseudocode Editor Backend (.NET 9 / ASP.NET Core).

---

## Table of Contents

1. [Base URL & Environment Configuration](#1-base-url--environment-configuration)
2. [Common Request / Response Conventions](#2-common-request--response-conventions)
3. [Data Models](#3-data-models)
4. [Endpoints](#4-endpoints)
   - [4.1 Get All Documents](#41-get-all-documents)
   - [4.2 Get Document by ID](#42-get-document-by-id)
   - [4.3 Create Document](#43-create-document)
   - [4.4 Update Document](#44-update-document)
   - [4.5 Delete Document](#45-delete-document)
   - [4.6 Validate Content](#46-validate-content)
   - [4.7 Format Content](#47-format-content)
5. [CORS Configuration](#5-cors-configuration)
6. [TypeScript / JavaScript Client Examples](#6-typescript--javascript-client-examples)
7. [Error Handling Patterns](#7-error-handling-patterns)
8. [Notes & Caveats](#8-notes--caveats)

---

## 1. Base URL & Environment Configuration

| Environment | Protocol | URL | Notes |
|---|---|---|---|
| **Development (HTTPS)** | `https` | `https://localhost:7065` | Default profile; requires trusting the ASP.NET dev cert |
| **Development (HTTP)** | `http` | `http://localhost:5062` | No TLS — useful for quick testing |
| **Production** | `https` | *Configured at deploy time* | Set via `ASPNETCORE_URLS` or reverse-proxy |

The frontend should store the base URL in an environment variable so it can be changed per deployment:

```bash
# .env (Vite example)
VITE_API_URL=https://localhost:7065
```

```typescript
const API_BASE_URL = import.meta.env.VITE_API_URL || 'https://localhost:7065';
```

All endpoint paths documented below are **relative to the base URL**.

---

## 2. Common Request / Response Conventions

| Item | Value |
|---|---|
| **Content-Type (requests)** | `application/json` |
| **Content-Type (responses)** | `application/json; charset=utf-8` |
| **Date format** | ISO 8601 / UTC (e.g. `"2026-02-15T10:30:00Z"`) |
| **ID format** | GUID string (e.g. `"d3b07384-d9a0-4e9b-8a0d-4e9b8a0d4e9b"`) |
| **Authentication** | None (no auth required at this time) |

### HTTP Status Codes Used

| Code | Meaning | When |
|---|---|---|
| `200 OK` | Success | GET (found), PUT (updated), POST validate/format |
| `201 Created` | Resource created | POST (new document) |
| `204 No Content` | Deleted successfully | DELETE |
| `400 Bad Request` | Invalid request body | Malformed JSON or missing required fields |
| `404 Not Found` | Resource does not exist | GET / PUT / DELETE with unknown `id` |

---

## 3. Data Models

### 3.1 `PseudocodeDocument` (Response Model)

Returned by GET, POST, and PUT endpoints.

```jsonc
{
  "id":        "string",    // GUID — auto-generated on creation
  "title":     "string",    // Document title (defaults to "Untitled" if blank)
  "content":   "string",    // Pseudocode source text (auto-formatted on save)
  "createdAt": "string",    // ISO 8601 UTC datetime
  "updatedAt": "string",    // ISO 8601 UTC datetime
  "language":  "string"     // Always "pseudocode" unless overridden
}
```

| Field | Type | Default | Description |
|---|---|---|---|
| `id` | `string` (GUID) | Auto-generated | Unique identifier |
| `title` | `string` | `"Untitled"` | Displayed title; whitespace-only values become `"Untitled"` |
| `content` | `string` | `""` | Pseudocode body; auto-formatted to Cambridge standards on create/update |
| `createdAt` | `string` (ISO 8601) | Server UTC now | Set once at creation |
| `updatedAt` | `string` (ISO 8601) | Server UTC now | Updated on every PUT |
| `language` | `string` | `"pseudocode"` | Language tag |

### 3.2 `CreatePseudocodeRequest` (POST body)

```jsonc
{
  "title":    "string",   // optional — defaults to "Untitled"
  "content":  "string",   // optional — defaults to ""
  "language": "string"    // optional — defaults to "pseudocode"
}
```

### 3.3 `UpdatePseudocodeRequest` (PUT body)

```jsonc
{
  "title":    "string",   // optional — blank keeps existing title
  "content":  "string",   // optional — defaults to ""
  "language": "string"    // optional — null keeps existing language
}
```

### 3.4 `ValidateContentRequest` (POST body for `/validate`)

```jsonc
{
  "content": "string"    // The pseudocode to validate
}
```

### 3.5 `ValidationResult` (Response from `/validate`)

```jsonc
{
  "isValid": true,
  "errors": [
    {
      "lineNumber": 3,
      "message": "Unmatched parentheses",
      "code": "UNMATCHED_PARENS"
    }
  ],
  "warnings": [
    {
      "lineNumber": 5,
      "message": "Indentation should be in multiples of 3 spaces (found 4)",
      "code": "INDENTATION"
    }
  ]
}
```

| Field | Type | Description |
|---|---|---|
| `isValid` | `boolean` | `true` when `errors` is empty |
| `errors` | `ValidationError[]` | Blocking issues (wrong case, unmatched brackets, etc.) |
| `warnings` | `ValidationWarning[]` | Non-blocking suggestions (indentation, missing THEN, etc.) |

**Known Validation Codes:**

| Code | Severity | Description |
|---|---|---|
| `KEYWORD_CASE` | Error | Keyword not in UPPERCASE (e.g. `if` instead of `IF`) |
| `IDENTIFIER_CASE` | Warning | Identifier not in PascalCase/camelCase |
| `INDENTATION` | Warning | Indentation not a multiple of 3 spaces |
| `MISSING_THEN` | Warning | `IF` statement without `THEN` on the same line |
| `UNMATCHED_PARENS` | Error | Mismatched `(` and `)` on a single line |
| `UNMATCHED_BRACKETS` | Error | Mismatched `[` and `]` on a single line |

### 3.6 `FormatContentRequest` (POST body for `/format`)

```jsonc
{
  "content": "string"    // The pseudocode to format
}
```

### 3.7 `FormatContentResponse` (Response from `/format`)

```jsonc
{
  "formattedContent": "string"   // Formatted pseudocode text
}
```

### 3.8 Error Response (all endpoints)

```jsonc
{
  "message": "Document not found"
}
```

---

## 4. Endpoints

All paths are prefixed with `/api/pseudocode`.

### 4.1 Get All Documents

| | |
|---|---|
| **Method** | `GET` |
| **Path** | `/api/pseudocode` |
| **Headers** | None required |

**Response `200 OK`**
```json
[
  {
    "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "title": "Bubble Sort",
    "content": "PROCEDURE BubbleSort(BYREF Items : ARRAY OF INTEGER)\n   ...\nENDPROCEDURE",
    "createdAt": "2026-02-15T08:00:00Z",
    "updatedAt": "2026-02-15T08:00:00Z",
    "language": "pseudocode"
  }
]
```

Returns an empty array `[]` when no documents exist.

---

### 4.2 Get Document by ID

| | |
|---|---|
| **Method** | `GET` |
| **Path** | `/api/pseudocode/{id}` |
| **Path Params** | `id` — GUID of the document |

**Response `200 OK`**
```json
{
  "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "title": "Bubble Sort",
  "content": "PROCEDURE BubbleSort(BYREF Items : ARRAY OF INTEGER)\n   ...\nENDPROCEDURE",
  "createdAt": "2026-02-15T08:00:00Z",
  "updatedAt": "2026-02-15T08:00:00Z",
  "language": "pseudocode"
}
```

**Response `404 Not Found`**
```json
{ "message": "Document not found" }
```

---

### 4.3 Create Document

| | |
|---|---|
| **Method** | `POST` |
| **Path** | `/api/pseudocode` |
| **Headers** | `Content-Type: application/json` |

> **Note:** Content is automatically validated and formatted according to Cambridge standards before saving.

**Request Body**
```json
{
  "title": "Binary Search",
  "content": "FUNCTION BinarySearch(List : ARRAY, Target : INTEGER) RETURNS INTEGER\n   DECLARE Low : INTEGER\n   Low ← 0\n   RETURN -1\nENDFUNCTION",
  "language": "pseudocode"
}
```

**Response `201 Created`**

Response headers include `Location: /api/pseudocode/{id}`.

```json
{
  "id": "f47ac10b-58cc-4372-a567-0e02b2c3d479",
  "title": "Binary Search",
  "content": "FUNCTION BinarySearch(List : ARRAY, Target : INTEGER) RETURNS INTEGER\n   DECLARE Low : INTEGER\n   Low ← 0\n   RETURN -1\nENDFUNCTION",
  "createdAt": "2026-02-15T10:30:00Z",
  "updatedAt": "2026-02-15T10:30:00Z",
  "language": "pseudocode"
}
```

**Behavior Notes:**
- If `title` is empty or whitespace-only, it defaults to `"Untitled"`.
- If `language` is null/omitted, it defaults to `"pseudocode"`.
- Content is auto-formatted (keywords uppercased, indentation normalised to 3-space multiples).

---

### 4.4 Update Document

| | |
|---|---|
| **Method** | `PUT` |
| **Path** | `/api/pseudocode/{id}` |
| **Path Params** | `id` — GUID of the document |
| **Headers** | `Content-Type: application/json` |

> **Note:** Content is automatically validated and formatted according to Cambridge standards before saving.

**Request Body**
```json
{
  "title": "Binary Search (Updated)",
  "content": "FUNCTION BinarySearch(List : ARRAY, Target : INTEGER) RETURNS INTEGER\n   RETURN -1\nENDFUNCTION",
  "language": "pseudocode"
}
```

**Response `200 OK`**
```json
{
  "id": "f47ac10b-58cc-4372-a567-0e02b2c3d479",
  "title": "Binary Search (Updated)",
  "content": "FUNCTION BinarySearch(List : ARRAY, Target : INTEGER) RETURNS INTEGER\n   RETURN -1\nENDFUNCTION",
  "createdAt": "2026-02-15T10:30:00Z",
  "updatedAt": "2026-02-15T11:00:00Z",
  "language": "pseudocode"
}
```

**Response `404 Not Found`**
```json
{ "message": "Document not found" }
```

**Behavior Notes:**
- If `title` is empty/whitespace, the existing title is kept.
- If `language` is null, the existing language is kept.
- `updatedAt` is set to the current server UTC time.

---

### 4.5 Delete Document

| | |
|---|---|
| **Method** | `DELETE` |
| **Path** | `/api/pseudocode/{id}` |
| **Path Params** | `id` — GUID of the document |

**Response `204 No Content`** — empty body on success.

**Response `404 Not Found`**
```json
{ "message": "Document not found" }
```

---

### 4.6 Validate Content

Validates pseudocode against Cambridge standards **without saving**.

| | |
|---|---|
| **Method** | `POST` |
| **Path** | `/api/pseudocode/validate` |
| **Headers** | `Content-Type: application/json` |

**Request Body**
```json
{
  "content": "if x > 0 then\n  OUTPUT x"
}
```

**Response `200 OK`**
```json
{
  "isValid": false,
  "errors": [
    {
      "lineNumber": 1,
      "message": "Keyword 'if' should be uppercase: IF",
      "code": "KEYWORD_CASE"
    },
    {
      "lineNumber": 1,
      "message": "Keyword 'then' should be uppercase: THEN",
      "code": "KEYWORD_CASE"
    }
  ],
  "warnings": [
    {
      "lineNumber": 2,
      "message": "Indentation should be in multiples of 3 spaces (found 2)",
      "code": "INDENTATION"
    }
  ]
}
```

Returns `{ "isValid": true, "errors": [], "warnings": [] }` for valid or empty content.

---

### 4.7 Format Content

Formats pseudocode to Cambridge standards **without saving**.

| | |
|---|---|
| **Method** | `POST` |
| **Path** | `/api/pseudocode/format` |
| **Headers** | `Content-Type: application/json` |

**Request Body**
```json
{
  "content": "if x > 0 then\noutput x\nendif"
}
```

**Response `200 OK`**
```json
{
  "formattedContent": "IF x > 0 THEN\n   OUTPUT x\nENDIF"
}
```

Formatting applies:
- Keyword uppercasing (IF, THEN, WHILE, FOR, etc.)
- Indentation normalisation (3-space multiples)
- Proper block nesting

---

## 5. CORS Configuration

The API uses an **"AllowAll"** CORS policy configured in `Program.cs`:

| Setting | Value |
|---|---|
| **Allowed Origins** | `*` (any origin) |
| **Allowed Methods** | `*` (GET, POST, PUT, DELETE, OPTIONS, etc.) |
| **Allowed Headers** | `*` (any header) |

No preflight workarounds are needed during development. For production, consider restricting origins to your deployed frontend domain.

---

## 6. TypeScript / JavaScript Client Examples

### 6.1 Type Definitions

```typescript
// --- Response models ---

interface PseudocodeDocument {
  id: string;
  title: string;
  content: string;
  createdAt: string;   // ISO 8601
  updatedAt: string;   // ISO 8601
  language: string;
}

interface ValidationError {
  lineNumber: number;
  message: string;
  code: string;
}

interface ValidationWarning {
  lineNumber: number;
  message: string;
  code: string;
}

interface ValidationResult {
  isValid: boolean;
  errors: ValidationError[];
  warnings: ValidationWarning[];
}

interface FormatResponse {
  formattedContent: string;
}

// --- Request models ---

interface CreatePseudocodeRequest {
  title?: string;
  content?: string;
  language?: string;
}

interface UpdatePseudocodeRequest {
  title?: string;
  content?: string;
  language?: string;
}
```

### 6.2 API Client

```typescript
const API_BASE = import.meta.env.VITE_API_URL || 'https://localhost:7065';
const ENDPOINT = `${API_BASE}/api/pseudocode`;

// ---------- CRUD Operations ----------

/** GET /api/pseudocode */
async function getAllDocuments(): Promise<PseudocodeDocument[]> {
  const res = await fetch(ENDPOINT);
  if (!res.ok) throw new Error(`GET all failed: ${res.status}`);
  return res.json();
}

/** GET /api/pseudocode/:id */
async function getDocument(id: string): Promise<PseudocodeDocument> {
  const res = await fetch(`${ENDPOINT}/${id}`);
  if (res.status === 404) throw new Error('Document not found');
  if (!res.ok) throw new Error(`GET failed: ${res.status}`);
  return res.json();
}

/** POST /api/pseudocode */
async function createDocument(data: CreatePseudocodeRequest): Promise<PseudocodeDocument> {
  const res = await fetch(ENDPOINT, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data),
  });
  if (!res.ok) throw new Error(`POST failed: ${res.status}`);
  return res.json();
}

/** PUT /api/pseudocode/:id */
async function updateDocument(id: string, data: UpdatePseudocodeRequest): Promise<PseudocodeDocument> {
  const res = await fetch(`${ENDPOINT}/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data),
  });
  if (res.status === 404) throw new Error('Document not found');
  if (!res.ok) throw new Error(`PUT failed: ${res.status}`);
  return res.json();
}

/** DELETE /api/pseudocode/:id */
async function deleteDocument(id: string): Promise<void> {
  const res = await fetch(`${ENDPOINT}/${id}`, { method: 'DELETE' });
  if (res.status === 404) throw new Error('Document not found');
  if (!res.ok) throw new Error(`DELETE failed: ${res.status}`);
}

// ---------- Utility Operations ----------

/** POST /api/pseudocode/validate */
async function validateContent(content: string): Promise<ValidationResult> {
  const res = await fetch(`${ENDPOINT}/validate`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ content }),
  });
  if (!res.ok) throw new Error(`Validate failed: ${res.status}`);
  return res.json();
}

/** POST /api/pseudocode/format */
async function formatContent(content: string): Promise<FormatResponse> {
  const res = await fetch(`${ENDPOINT}/format`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ content }),
  });
  if (!res.ok) throw new Error(`Format failed: ${res.status}`);
  return res.json();
}
```

### 6.3 Usage Example

```typescript
// Create a document
const doc = await createDocument({
  title: 'Hello World',
  content: 'OUTPUT "Hello, World!"',
});
console.log('Created:', doc.id);

// Validate before editing
const result = await validateContent('if x > 0 then\n  output x');
if (!result.isValid) {
  console.error('Errors:', result.errors);
  console.warn('Warnings:', result.warnings);
}

// Format content
const { formattedContent } = await formatContent('if x > 0 then\noutput x\nendif');
console.log('Formatted:', formattedContent);

// Update the document
const updated = await updateDocument(doc.id, {
  title: 'Hello World (v2)',
  content: formattedContent,
});

// Delete when done
await deleteDocument(doc.id);
```

---

## 7. Error Handling Patterns

### 7.1 Network / Connection Errors

```typescript
async function safeFetch<T>(fn: () => Promise<T>): Promise<{ data?: T; error?: string }> {
  try {
    const data = await fn();
    return { data };
  } catch (err) {
    const message = err instanceof Error ? err.message : 'Unknown error';
    // Common: TypeError: Failed to fetch  →  backend is down or CORS blocked
    return { error: message };
  }
}

// Usage
const { data: docs, error } = await safeFetch(getAllDocuments);
if (error) {
  showToast(`Could not load documents: ${error}`);
}
```

### 7.2 HTTPS Certificate Errors (Development)

When using `https://localhost:7065` the browser may reject the self-signed ASP.NET dev certificate. Solutions:
1. Trust the dev cert: `dotnet dev-certs https --trust`
2. Or use the HTTP endpoint: `http://localhost:5062`

---

## 8. Notes & Caveats

| Topic | Detail |
|---|---|
| **Storage** | In-memory only — all data is lost when the server restarts. |
| **Auto-formatting** | Content submitted via POST/PUT is automatically formatted to Cambridge standards before persisting. Use the `/format` endpoint to preview formatting without saving. |
| **Validation on save** | Validation runs during POST/PUT but does **not** block saves — documents are still stored even if validation errors exist. Use the `/validate` endpoint to check before saving. |
| **ID generation** | IDs are server-generated GUIDs; do not send an `id` field in create requests. |
| **OpenAPI / Swagger** | Available in development at `GET /openapi/v1.json` (via `app.MapOpenApi()`). |
| **Framework** | ASP.NET Core on .NET 9 (`net9.0`). |
