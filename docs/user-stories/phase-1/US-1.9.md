# US-1.9 · Rename a document with explicit action
**As a** student,
**I want to** rename a document using a clear rename button or icon,
**so that** I can easily organize my documents with meaningful names.

**Acceptance Criteria:**
- [ ] Each document in the sidebar has a visible rename button/icon (🖊️ or similar)
- [ ] Clicking the rename button makes the title editable inline
- [ ] Pressing Enter saves the new title
- [ ] Pressing Escape cancels the rename
- [ ] Empty titles default to "Untitled"
- [ ] Rename works for both saved and unsaved (local) documents
- [ ] For saved documents, rename immediately calls `PUT /api/pseudocode/{id}`
- [ ] For local documents, rename updates in-memory state (persists when document is saved)
- [ ] The header reflects the renamed title immediately
- [ ] Error messages appear as toasts if rename fails

## Backend Requirements

| Endpoints touched | DB impact | Services | Auth |
|---|---|---|---|
| `EXISTING PUT /api/pseudocode/{id}` | None in Phase 1; Phase 2 persists title changes and updates `updatedAt` | Enforce server-side default title if empty/whitespace (`"Untitled"`). Ensure rename updates `updatedAt` so sort-by-last-modified works. | None in Phase 1; post-Phase 2, require JWT and enforce ownership |

- **API endpoints:** Use existing update endpoint:
  - `PUT /api/pseudocode/{id}`
    - Request: `{ "title": string, "content": string, "language": "pseudocode" }`
    - Response: `200 OK` → updated `PseudocodeDocument`
    - Note: When renaming, client sends existing content unchanged
- **Backend validation:** Ensure UpdateDocumentAsync properly handles title-only updates
- **Database:** None in Phase 1; Phase 2 persists title changes
- **Service layer logic:** 
  - If title is empty/whitespace, default to "Untitled" (consistent with create)
  - Update `updatedAt` timestamp on any title change
  - Don't re-process content if it hasn't changed (optimization for rename-only updates)
- **Authentication/authorization:** None in Phase 1; post-Phase 2, require JWT and enforce ownership
- **Error handling:** `400` for invalid request, `404` if id not found, `500` otherwise

**Traces to:** FR-7.3, Task 1.3

## Implementation Notes
- Frontend: Add a rename icon button next to each document in the sidebar
- Frontend: Improve rename state management to handle both local and saved documents
- Backend: Fix UpdateDocumentAsync to default to "Untitled" if title is empty (currently keeps existing title)
- Backend: Add optimization to skip content processing if only title changed

