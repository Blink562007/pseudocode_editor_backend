# Prototype screenshots index

This folder contains one PNG screenshot for each user story that introduces a visible UI change/new UI element.

- Source: `docs/USER_STORIES.md`
- Prototype: `docs/prototype/index.html`

Tip: you can also view the live prototype state in a browser via:
- `docs/prototype/index.html?scenario=US-x.y`

## Phase 1 — Frontend Architecture + Document Management

| User story | Title | Screenshot |
|---|---|---|
| US-1.1 | Component-based editor layout | [US-1.1-component-layout.png](US-1.1-component-layout.png) |
| US-1.2 | Create a new document | [US-1.2-new-document-modal.png](US-1.2-new-document-modal.png) |
| US-1.3 | View and switch between saved documents | [US-1.3-switch-document.png](US-1.3-switch-document.png) |
| US-1.4 | Save a document | [US-1.4-save-status-unsaved.png](US-1.4-save-status-unsaved.png) |
| US-1.5 | Delete a document | [US-1.5-delete-confirmation-dialog.png](US-1.5-delete-confirmation-dialog.png) |
| US-1.6 | Rename a document | [US-1.6-rename-document-inline.png](US-1.6-rename-document-inline.png) |
| US-1.7 | Auto-save to localStorage | [US-1.7-auto-save-localstorage.png](US-1.7-auto-save-localstorage.png) |
| US-1.8 | Loading and error states | [US-1.8-loading-error-state.png](US-1.8-loading-error-state.png) |

## Phase 2 — Authentication + Persistent Storage

| User story | Title | Screenshot |
|---|---|---|
| US-2.1 | Register an account | [US-2.1-registration-screen.png](US-2.1-registration-screen.png) |
| US-2.2 | Log in to an existing account | [US-2.2-login-screen.png](US-2.2-login-screen.png) |
| US-2.3 | Stay logged in across sessions | [US-2.3-session-restored.png](US-2.3-session-restored.png) |
| US-2.6 | Example documents for new users | [US-2.6-example-documents.png](US-2.6-example-documents.png) |

## Phase 3 — Pseudocode Execution Engine

| User story | Title | Screenshot |
|---|---|---|
| US-3.1 | Run pseudocode and see output | [US-3.1-run-program-output.png](US-3.1-run-program-output.png) |
| US-3.2 | See runtime errors with line numbers | [US-3.2-runtime-error-highlight.png](US-3.2-runtime-error-highlight.png) |
| US-3.7 | Provide user input during execution | [US-3.7-stdin-input-prompt.png](US-3.7-stdin-input-prompt.png) |

## Phase 4 — Editor UX Enhancements

| User story | Title | Screenshot |
|---|---|---|
| US-4.1 | Format code with one click | [US-4.1-format-one-click.png](US-4.1-format-one-click.png) |
| US-4.2 | See validation errors inline in the editor | [US-4.2-inline-validation-squiggles.png](US-4.2-inline-validation-squiggles.png) |
| US-4.3 | Real-time validation as I type | [US-4.3-problems-bar-errors-warnings.png](US-4.3-problems-bar-errors-warnings.png) |
| US-4.4 | Keyboard shortcuts | [US-4.4-keyboard-shortcuts-modal.png](US-4.4-keyboard-shortcuts-modal.png) |
| US-4.5 | Colour-coded terminal output | [US-4.5-terminal-colour-coded-output.png](US-4.5-terminal-colour-coded-output.png) |
| US-4.6 | Keyword auto-completion | [US-4.6-autocomplete-dropdown.png](US-4.6-autocomplete-dropdown.png) |
| US-4.7 | Format on save | [US-4.7-settings-format-on-save.png](US-4.7-settings-format-on-save.png) |

## Stories intentionally skipped (no new UI)

These stories are primarily backend/engine/data-focused and don’t introduce a distinct UI element beyond what’s already shown above:

- US-2.4, US-2.5
- US-3.3, US-3.4, US-3.5, US-3.6, US-3.8

