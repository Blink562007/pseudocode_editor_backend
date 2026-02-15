// Pseudocode Editor — Interactive UI Prototype
// This is a static HTML/CSS/JS prototype for design validation.
// No real backend calls are made.

const SCREENS = {
  LOGIN: 'login',
  REGISTER: 'register',
  EDITOR: 'editor',
};

let currentScreen = SCREENS.EDITOR;
let activeDocIndex = 0;
let sidebarCollapsed = false;

// Screenshot / demo scenarios
const URL_PARAMS = new URLSearchParams(window.location.search);
const SCENARIO_ID = URL_PARAMS.get('scenario');
const IS_SCREENSHOT_MODE = !!SCENARIO_ID;

const uiState = {
  banner: null, // { type: 'error'|'info', message: string, actions?: [{ label, action }] }
  loading: null, // { title: string, subtitle?: string }
  modal: null, // { type: 'newDoc'|'deleteConfirm'|'shortcuts'|'settings', ... }
  renamingDocIndex: null,
  editorDecorations: [], // [{ line, kind: 'error'|'warning'|'runtime', message }]
  problems: null, // { errors: number, warnings: number, message?: string }
  autocomplete: null, // { query: string, items: [{ key, desc }], selected: number }
  saveStatus: { state: 'saved', text: '✓ Saved' },
  autoValidate: false,
  formatOnSave: false,
  toasts: [], // [{ message, type }]
  terminalPrompt: { show: true, label: 'Enter value for x:', placeholder: 'Type here...', focus: false },
};

const documents = [
  { title: 'Bubble Sort', modified: '2 mins ago', content: `// Bubble Sort Algorithm\nDECLARE numbers : ARRAY[1:10] OF INTEGER\nDECLARE temp : INTEGER\nDECLARE n : INTEGER\nn ← 10\n\n// Fill array with random values\nFOR i ← 1 TO n\n   numbers[i] ← INT(RANDOM() * 100)\nNEXT i\n\n// Bubble Sort\nFOR i ← 1 TO n - 1\n   FOR j ← 1 TO n - i\n      IF numbers[j] > numbers[j + 1] THEN\n         temp ← numbers[j]\n         numbers[j] ← numbers[j + 1]\n         numbers[j + 1] ← temp\n      ENDIF\n   NEXT j\nNEXT i\n\n// Output sorted array\nFOR i ← 1 TO n\n   OUTPUT numbers[i]\nNEXT i`, unsaved: true },
  { title: 'Linear Search', modified: '1 hour ago', content: `// Linear Search\nFUNCTION LinearSearch(items : ARRAY, target : INTEGER) RETURNS INTEGER\n   DECLARE i : INTEGER\n   FOR i ← 1 TO LENGTH(items)\n      IF items[i] = target THEN\n         RETURN i\n      ENDIF\n   NEXT i\n   RETURN -1\nENDFUNCTION\n\nDECLARE result : INTEGER\nresult ← LinearSearch(myArray, 42)\nIF result = -1 THEN\n   OUTPUT "Not found"\nELSE\n   OUTPUT "Found at index: " & result\nENDIF`, unsaved: false },
  { title: 'Calculator', modified: '3 hours ago', content: '// Simple Calculator\nDECLARE num1 : REAL\nDECLARE num2 : REAL\nDECLARE op : STRING\n\nOUTPUT "Enter first number:"\nINPUT num1\nOUTPUT "Enter operator (+, -, *, /)"\nINPUT op\nOUTPUT "Enter second number:"\nINPUT num2\n\nCASE OF op\n   "+": OUTPUT num1 + num2\n   "-": OUTPUT num1 - num2\n   "*": OUTPUT num1 * num2\n   "/": IF num2 = 0 THEN\n           OUTPUT "Error: Division by zero"\n        ELSE\n           OUTPUT num1 / num2\n        ENDIF\n   OTHERWISE: OUTPUT "Invalid operator"\nENDCASE', unsaved: false },
  { title: 'Fibonacci Sequence', modified: 'Yesterday', content: '// Fibonacci\nDECLARE n : INTEGER\nOUTPUT "How many terms?"\nINPUT n\n\nDECLARE a : INTEGER\nDECLARE b : INTEGER\na ← 0\nb ← 1\n\nFOR i ← 1 TO n\n   OUTPUT a\n   temp ← a + b\n   a ← b\n   b ← temp\nNEXT i', unsaved: false },
  { title: 'Untitled', modified: 'Just now', content: '', unsaved: false },
];

const terminalLines = [
  { type: 'info', text: '> Running Bubble Sort...' },
  { type: 'normal', text: '> Validating pseudocode...' },
  { type: 'success', text: '> Validation passed ✓' },
  { type: 'normal', text: '> Executing...' },
  { type: 'output', text: '3' },
  { type: 'output', text: '12' },
  { type: 'output', text: '17' },
  { type: 'output', text: '25' },
  { type: 'output', text: '31' },
  { type: 'output', text: '42' },
  { type: 'output', text: '56' },
  { type: 'output', text: '64' },
  { type: 'output', text: '78' },
  { type: 'output', text: '93' },
  { type: 'success', text: '> Program finished (0.02s)' },
];

const INITIAL_DOCUMENTS = documents.map(d => ({ ...d }));
const INITIAL_TERMINAL_LINES = terminalLines.map(l => ({ ...l }));

function resetPrototypeData() {
  documents.length = 0;
  INITIAL_DOCUMENTS.forEach(d => documents.push({ ...d }));
  terminalLines.length = 0;
  INITIAL_TERMINAL_LINES.forEach(l => terminalLines.push({ ...l }));
  activeDocIndex = 0;

  uiState.banner = null;
  uiState.loading = null;
  uiState.modal = null;
  uiState.renamingDocIndex = null;
  uiState.editorDecorations = [];
  uiState.problems = null;
  uiState.autocomplete = null;
  uiState.saveStatus = { state: 'saved', text: '✓ Saved' };
  uiState.autoValidate = false;
  uiState.formatOnSave = false;
  uiState.toasts = [];
  uiState.terminalPrompt = { show: true, label: 'Enter value for x:', placeholder: 'Type here...', focus: false };
}

function applyScenario(id) {
  resetPrototypeData();
  if (!id) return;
  currentScreen = SCREENS.EDITOR;

  switch (id) {
    // Phase 1
    case 'US-1.1':
      break;
    case 'US-1.2':
      uiState.modal = { type: 'newDoc', defaultTitle: 'Untitled' };
      uiState.toasts = [{ message: 'Create a new document from the sidebar', type: 'info' }];
      break;
    case 'US-1.3':
      activeDocIndex = 1;
      uiState.toasts = [{ message: 'Switched to "Linear Search"', type: 'info' }];
      break;
    case 'US-1.4':
      documents[0].unsaved = true;
      uiState.saveStatus = { state: 'unsaved', text: 'Unsaved changes' };
      uiState.toasts = [{ message: 'Click Save or press ⌘S / Ctrl+S', type: 'info' }];
      break;
    case 'US-1.5':
      uiState.modal = { type: 'deleteConfirm', docIndex: 2, docTitle: documents[2].title };
      break;
    case 'US-1.6':
      uiState.renamingDocIndex = 1;
      activeDocIndex = 1;
      uiState.toasts = [{ message: 'Renaming… press Enter to save', type: 'info' }];
      break;
    case 'US-1.7':
      documents[0].unsaved = true;
      uiState.saveStatus = { state: 'saving', text: 'Auto-saving to browser…' };
      uiState.toasts = [{ message: 'Auto-save enabled (every 2 seconds)', type: 'info' }];
      break;
    case 'US-1.8':
      uiState.loading = { title: 'Loading documents…', subtitle: 'Fetching your workspace' };
      uiState.banner = { type: 'error', message: 'Failed to load documents (API unreachable).', actions: [{ label: 'Retry', action: 'retry-load' }] };
      terminalLines.length = 0;
      terminalLines.push({ type: 'error', text: '> Error: Network timeout while loading documents' });
      uiState.saveStatus = { state: 'error', text: 'Offline' };
      break;

    // Phase 2
    case 'US-2.1':
      currentScreen = SCREENS.REGISTER;
      break;
    case 'US-2.2':
      currentScreen = SCREENS.LOGIN;
      break;
    case 'US-2.3':
      uiState.toasts = [{ message: 'Session restored — welcome back!', type: 'success' }];
      uiState.saveStatus = { state: 'saved', text: '✓ Synced' };
      break;
    case 'US-2.6':
      documents.push({ title: 'Prime Checker', modified: 'Example', content: '// Example: Prime Checker\nDECLARE n : INTEGER\nINPUT n\n\nDECLARE isPrime : BOOLEAN\nisPrime ← true\n\nFOR i ← 2 TO n - 1\n   IF n MOD i = 0 THEN\n      isPrime ← false\n   ENDIF\nNEXT i\n\nIF isPrime THEN\n   OUTPUT "Prime"\nELSE\n   OUTPUT "Not prime"\nENDIF', unsaved: false, isExample: true });
      uiState.toasts = [{ message: 'Example programs added', type: 'info' }];
      break;

    // Phase 3
    case 'US-3.1':
      terminalLines.length = 0;
      terminalLines.push(
        { type: 'info', text: '> Running Bubble Sort…' },
        { type: 'normal', text: '> Executing…' },
        { type: 'output', text: '3' },
        { type: 'output', text: '12' },
        { type: 'success', text: '> Program finished (0.02s)' },
      );
      uiState.toasts = [{ message: 'Run completed ✓', type: 'success' }];
      break;
    case 'US-3.2':
      activeDocIndex = 2;
      uiState.editorDecorations = [{ line: 16, kind: 'runtime', message: 'Runtime error: Division by zero' }];
      terminalLines.length = 0;
      terminalLines.push(
        { type: 'info', text: '> Running Calculator…' },
        { type: 'error', text: '> Runtime error at line 16: Division by zero' },
      );
      uiState.problems = { errors: 1, warnings: 0, message: 'Runtime error highlighted' };
      break;
    case 'US-3.7':
      activeDocIndex = 3;
      terminalLines.length = 0;
      terminalLines.push(
        { type: 'info', text: '> Running Fibonacci Sequence…' },
        { type: 'normal', text: '> INPUT n' },
      );
      uiState.terminalPrompt = { show: true, label: 'Enter value for n:', placeholder: 'e.g. 10', focus: true };
      uiState.toasts = [{ message: 'Program is waiting for input', type: 'info' }];
      break;

    // Phase 4
    case 'US-4.1':
      uiState.toasts = [{ message: 'Code formatted to Cambridge standard ✓', type: 'success' }];
      break;
    case 'US-4.2':
      uiState.editorDecorations = [
        { line: 2, kind: 'warning', message: 'Warning: ARRAY bounds should be explicit (e.g. [1:10])' },
        { line: 6, kind: 'error', message: 'Error: Variable i not declared' },
      ];
      uiState.problems = { errors: 1, warnings: 1, message: 'Problems detected' };
      uiState.toasts = [{ message: 'Inline validation enabled', type: 'info' }];
      break;
    case 'US-4.3':
      uiState.autoValidate = true;
      uiState.editorDecorations = [{ line: 6, kind: 'error', message: 'Error: Variable i not declared' }];
      uiState.problems = { errors: 1, warnings: 0, message: 'Validating… (500ms debounce)' };
      uiState.toasts = [{ message: 'Auto-validate: ON', type: 'info' }];
      break;
    case 'US-4.4':
      uiState.modal = { type: 'shortcuts' };
      break;
    case 'US-4.5':
      terminalLines.length = 0;
      terminalLines.push(
        { type: 'success', text: '> Validation passed ✓' },
        { type: 'warning', text: '> Warning: Trailing spaces on line 12' },
        { type: 'error', text: '> Error: Unexpected token ENDIF on line 18' },
        { type: 'output', text: '42' },
      );
      break;
    case 'US-4.6':
      uiState.autocomplete = {
        query: 'PRO',
        selected: 0,
        items: [
          { key: 'PROCEDURE', desc: 'Define a procedure' },
          { key: 'PROPERTY', desc: 'OOP member' },
          { key: 'PROGRAM', desc: 'Top-level program (optional)' },
        ],
      };
      uiState.toasts = [{ message: 'Autocomplete suggestions', type: 'info' }];
      break;
    case 'US-4.7':
      uiState.formatOnSave = true;
      uiState.modal = { type: 'settings' };
      break;
  }
}

function render() {
  const app = document.getElementById('app');
  if (currentScreen === SCREENS.LOGIN) { app.innerHTML = renderLogin(); }
  else if (currentScreen === SCREENS.REGISTER) { app.innerHTML = renderRegister(); }
  else { app.innerHTML = renderEditor(); }
  attachEvents();

  // Signal to automation (Playwright) that the UI is ready
  window.__PROTOTYPE_RENDERED = true;
}

function renderLogin() {
  return `
  <div class="auth-overlay">
    <div class="auth-card">
      <div class="auth-header">
        <div class="auth-logo">⟨/⟩ Pseudocode Editor</div>
        <p class="auth-subtitle">Cambridge IGCSE / A Level Pseudocode Platform</p>
      </div>
      <form class="auth-form" onsubmit="return false">
        <div class="auth-field">
          <label>Email</label>
          <input type="email" placeholder="student@school.edu" value="jerry@example.com">
        </div>
        <div class="auth-field">
          <label>Password</label>
          <input type="password" placeholder="••••••••" value="password123">
        </div>
        <button class="auth-submit" data-action="login">Log In</button>
        <div class="auth-footer">
          Don't have an account? <a href="#" data-action="go-register">Sign up</a>
        </div>
        <div class="auth-divider"><span>or</span></div>
        <button class="auth-google" onclick="return false">
          <svg width="18" height="18" viewBox="0 0 18 18"><path fill="#4285F4" d="M17.64 9.2c0-.637-.057-1.251-.164-1.84H9v3.481h4.844a4.14 4.14 0 01-1.796 2.716v2.259h2.908c1.702-1.567 2.684-3.875 2.684-6.615z"/><path fill="#34A853" d="M9 18c2.43 0 4.467-.806 5.956-2.18l-2.908-2.259c-.806.54-1.837.86-3.048.86-2.344 0-4.328-1.584-5.036-3.711H.957v2.332A8.997 8.997 0 009 18z"/><path fill="#FBBC05" d="M3.964 10.71A5.41 5.41 0 013.682 9c0-.593.102-1.17.282-1.71V4.958H.957A8.997 8.997 0 000 9c0 1.452.348 2.827.957 4.042l3.007-2.332z"/><path fill="#EA4335" d="M9 3.58c1.321 0 2.508.454 3.44 1.345l2.582-2.58C13.463.891 11.426 0 9 0A8.997 8.997 0 00.957 4.958L3.964 7.29C4.672 5.163 6.656 3.58 9 3.58z"/></svg>
          Continue with Google
        </button>
      </form>
    </div>
  </div>`;
}

function renderRegister() {
  return `
  <div class="auth-overlay">
    <div class="auth-card">
      <div class="auth-header">
        <div class="auth-logo">⟨/⟩ Pseudocode Editor</div>
        <p class="auth-subtitle">Create your account</p>
      </div>
      <form class="auth-form" onsubmit="return false">
        <div class="auth-field">
          <label>Display Name</label>
          <input type="text" placeholder="Your name">
        </div>
        <div class="auth-field">
          <label>Email</label>
          <input type="email" placeholder="student@school.edu">
        </div>
        <div class="auth-field">
          <label>Password</label>
          <input type="password" placeholder="Min. 8 characters">
        </div>
        <div class="auth-field">
          <label>I am a...</label>
          <select><option>Student</option><option>Teacher</option></select>
        </div>
        <button class="auth-submit" data-action="register">Create Account</button>
        <div class="auth-footer">
          Already have an account? <a href="#" data-action="go-login">Log in</a>
        </div>
      </form>
    </div>
  </div>`;
}

function renderEditor() {
  const doc = documents[activeDocIndex];
  const contentPadding = uiState.banner ? 'style="padding-top: 42px;"' : '';
  return `
  <div class="header">
    <div class="header-left">
      <div class="header-logo">⟨/⟩ Pseudocode Editor <span>Cambridge</span></div>
      <div class="header-nav">
        <button data-action="go-login" title="View login screen">Auth Demo</button>
      </div>
    </div>
    <div class="header-right">
      ${renderSaveStatus()}
      <button class="header-user">
        <div class="avatar">JL</div>
        Jerry Li
      </button>
    </div>
  </div>
  <div class="main">
    ${renderSidebar()}
    <div class="content" ${contentPadding}>
      ${renderBanner()}
      ${renderControlBar(doc)}
      ${renderEditorArea(doc)}
      ${renderProblemsBar()}
      ${renderTerminal()}
      ${renderLoadingOverlay()}
    </div>
  </div>
  ${renderModal()}
  ${renderToasts()}`;
}

function renderSidebar() {
  return `
  <div class="sidebar">
    <div class="sidebar-header">
      <div class="sidebar-title">Documents</div>
      <button class="btn-new" data-action="new-doc" title="New Document">+</button>
    </div>
    <div class="doc-list">
      ${documents.map((doc, i) => `
        <div class="doc-item ${i === activeDocIndex ? 'active' : ''}" data-action="select-doc" data-index="${i}">
          <div class="doc-item-title">
            ${uiState.renamingDocIndex === i
              ? `<input class="doc-rename-input" data-role="rename-input" data-index="${i}" value="${escapeHtml(doc.title)}">`
              : `${escapeHtml(doc.title)}${doc.isExample ? '<span class="badge badge-example">Example</span>' : ''}${doc.unsaved ? ' •' : ''}`
            }
          </div>
          <div class="doc-item-meta">${doc.modified}</div>
          ${doc.unsaved ? '<div class="unsaved-dot"></div>' : ''}
          <button class="doc-rename" data-action="rename-doc" data-index="${i}" title="Rename">✎</button>
          <button class="doc-delete" data-action="delete-doc" data-index="${i}" title="Delete">×</button>
        </div>
      `).join('')}
    </div>
  </div>`;
}

function renderControlBar(doc) {
  return `
  <div class="controlbar">
    <div class="controlbar-left">
      <button class="btn btn-run" data-action="run">▶ Run<span class="shortcut">F5</span></button>
      <button class="btn btn-format" data-action="format">⬡ Format<span class="shortcut">⇧⌘F</span></button>
      <button class="btn btn-validate" data-action="validate">✓ Validate</button>
      <button class="btn btn-convert" data-action="convert">↗ Python</button>
      <button class="btn btn-review" data-action="review">✦ AI Review</button>
    </div>
    <div class="controlbar-right">
      <button class="btn btn-save" data-action="save">💾 Save<span class="shortcut">⌘S</span></button>
      <button class="btn btn-share" data-action="share">🔗 Share</button>
    </div>
  </div>`;
}

function renderSaveStatus() {
  const status = uiState.saveStatus || { state: 'saved', text: '✓ Saved' };
  const cls = status.state && status.state !== 'saved' ? ` ${status.state}` : '';
  const icon = status.state === 'saving'
    ? '<span class="spinner" aria-hidden="true"></span>'
    : '<span class="save-dot" aria-hidden="true"></span>';

  return `<span class="save-status${cls}">${icon}${escapeHtml(status.text)}</span>`;
}

function renderBanner() {
  if (!uiState.banner) return '';
  const type = uiState.banner.type || 'info';
  const actions = (uiState.banner.actions || []).map(a => (
    `<button data-action="${a.action}">${escapeHtml(a.label)}</button>`
  )).join('');

  return `
    <div class="banner banner-${type}" role="status">
      <div>${escapeHtml(uiState.banner.message || '')}</div>
      ${actions ? `<div class="banner-actions">${actions}</div>` : ''}
    </div>`;
}

function renderProblemsBar() {
  const problems = uiState.problems;
  const errors = problems?.errors ?? 0;
  const warnings = problems?.warnings ?? 0;
  const msg = problems?.message ?? (errors || warnings ? 'Problems detected' : 'No problems');
  const mode = uiState.autoValidate ? 'Auto-validate: ON' : 'Auto-validate: OFF';

  return `
    <div class="problems-bar">
      <div class="problems-left">
        <span class="pill pill-error">${errors} errors</span>
        <span class="pill pill-warning">${warnings} warnings</span>
        <span>${escapeHtml(msg)}</span>
      </div>
      <div class="problems-right">${escapeHtml(mode)}</div>
    </div>`;
}

function renderLoadingOverlay() {
  if (!uiState.loading) return '';
  const title = uiState.loading.title || 'Loading…';
  const subtitle = uiState.loading.subtitle || '';
  return `
    <div class="loading-overlay" role="status" aria-live="polite">
      <div class="spinner" style="width: 18px; height: 18px; border-width: 3px;"></div>
      <div class="loading-title">${escapeHtml(title)}</div>
      ${subtitle ? `<div class="loading-sub">${escapeHtml(subtitle)}</div>` : ''}
    </div>`;
}

function renderToasts() {
  const toasts = uiState.toasts || [];
  return `
    <div class="toast-container" id="toasts">
      ${toasts.map(t => `<div class="toast toast-${t.type || 'info'}">${escapeHtml(t.message || '')}</div>`).join('')}
    </div>`;
}

function renderModal() {
  if (!uiState.modal) return '';
  const m = uiState.modal;

  if (m.type === 'newDoc') {
    return `
      <div class="modal-overlay" data-action="modal-dismiss">
        <div class="modal" role="dialog" aria-modal="true" onclick="event.stopPropagation()">
          <div class="modal-header"><div class="modal-title">New Document</div></div>
          <div class="modal-body">
            <div>Create a new pseudocode document.</div>
            <input data-role="newdoc-title" value="${escapeHtml(m.defaultTitle || 'Untitled')}">
          </div>
          <div class="modal-actions">
            <button class="btn-ghost" data-action="modal-cancel">Cancel</button>
            <button class="btn-primary" data-action="confirm-new-doc">Create</button>
          </div>
        </div>
      </div>`;
  }

  if (m.type === 'deleteConfirm') {
    return `
      <div class="modal-overlay" data-action="modal-dismiss">
        <div class="modal" role="dialog" aria-modal="true" onclick="event.stopPropagation()">
          <div class="modal-header"><div class="modal-title">Delete Document</div></div>
          <div class="modal-body">
            <div>Are you sure you want to delete <b>${escapeHtml(m.docTitle || '')}</b>?</div>
            <div>This action cannot be undone.</div>
          </div>
          <div class="modal-actions">
            <button class="btn-ghost" data-action="modal-cancel">Cancel</button>
            <button class="btn-danger" data-action="confirm-delete-doc" data-index="${m.docIndex}">Delete</button>
          </div>
        </div>
      </div>`;
  }

  if (m.type === 'shortcuts') {
    return `
      <div class="modal-overlay" data-action="modal-dismiss">
        <div class="modal" role="dialog" aria-modal="true" onclick="event.stopPropagation()">
          <div class="modal-header"><div class="modal-title">Keyboard Shortcuts</div></div>
          <div class="modal-body">
            <div><b>Run</b> — <span class="pill">F5</span></div>
            <div><b>Save</b> — <span class="pill">⌘S / Ctrl+S</span></div>
            <div><b>Format</b> — <span class="pill">⇧⌘F / Shift+Ctrl+F</span></div>
          </div>
          <div class="modal-actions">
            <button class="btn-primary" data-action="modal-cancel">Done</button>
          </div>
        </div>
      </div>`;
  }

  if (m.type === 'settings') {
    return `
      <div class="modal-overlay" data-action="modal-dismiss">
        <div class="modal" role="dialog" aria-modal="true" onclick="event.stopPropagation()">
          <div class="modal-header"><div class="modal-title">Editor Settings</div></div>
          <div class="modal-body">
            <label style="display:flex; align-items:center; gap:10px;">
              <input type="checkbox" data-action="toggle-format-on-save" ${uiState.formatOnSave ? 'checked' : ''}>
              Format on Save
            </label>
            <div style="color: var(--text-muted); font-size: 12px;">
              When enabled, the editor formats code automatically on save.
            </div>
          </div>
          <div class="modal-actions">
            <button class="btn-primary" data-action="modal-cancel">Close</button>
          </div>
        </div>
      </div>`;
  }

  return '';
}

function highlightSyntax(code) {
  if (!code) return '<span class="cmt">// Start typing your pseudocode here...</span>';
  return code.split('\n').map(line => {
    // Comments
    if (line.trim().startsWith('//')) return `<span class="cmt">${escapeHtml(line)}</span>`;
    let result = escapeHtml(line);
    // Strings
    result = result.replace(/"([^"]*)"/g, '<span class="str">"$1"</span>');
    // Numbers
    result = result.replace(/\b(\d+\.?\d*)\b/g, '<span class="num">$1</span>');
    // Types
    result = result.replace(/\b(INTEGER|REAL|STRING|CHAR|BOOLEAN|ARRAY|OF)\b/g, '<span class="type">$1</span>');
    // Keywords
    result = result.replace(/\b(DECLARE|CONSTANT|IF|THEN|ELSE|ENDIF|FOR|TO|STEP|NEXT|WHILE|DO|ENDWHILE|REPEAT|UNTIL|CASE|OF|OTHERWISE|ENDCASE|PROCEDURE|ENDPROCEDURE|FUNCTION|ENDFUNCTION|RETURNS|RETURN|CALL|INPUT|OUTPUT|OPENFILE|READFILE|WRITEFILE|CLOSEFILE|TYPE|ENDTYPE|CLASS|ENDCLASS|INHERITS|NEW|PUBLIC|PRIVATE|BYREF|BYVAL)\b/g, '<span class="kw">$1</span>');
    // Built-in functions
    result = result.replace(/\b(LENGTH|SUBSTRING|UCASE|LCASE|LEFT|RIGHT|INT|RANDOM|MOD|DIV|AND|OR|NOT)\b/g, '<span class="fn">$1</span>');
    // Arrow operator
    result = result.replace(/←/g, '<span class="op">←</span>');
    return result;
  }).join('\n');
}

function escapeHtml(text) {
  return String(text ?? '')
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/\"/g, '&quot;')
    .replace(/'/g, '&#39;');
}

function renderEditorArea(doc) {
  const raw = doc.content || '';
  const rawLines = raw.length ? raw.split('\n') : [''];
  const highlightedLines = highlightSyntax(raw).split('\n');
  const decorationsByLine = new Map((uiState.editorDecorations || []).map(d => [d.line, d]));
  const lineNums = rawLines.map((_, i) => {
    const ln = i + 1;
    const dec = decorationsByLine.get(ln);
    const cls = dec ? `ln ln-${dec.kind}` : 'ln';
    return `<div class="${cls}">${ln}</div>`;
  }).join('');

  const codeHtml = rawLines.map((_, i) => {
    const ln = i + 1;
    const dec = decorationsByLine.get(ln);
    const lineWrapperClasses = ['code-line'];
    let textSpanClass = '';
    let title = '';

    if (dec) {
      title = dec.message || '';
      if (dec.kind === 'runtime') {
        lineWrapperClasses.push('err-line', 'line-marker');
      }
      if (dec.kind === 'error') textSpanClass = 'squiggle-error';
      if (dec.kind === 'warning') textSpanClass = 'squiggle-warning';
    }

    const lineHtml = highlightedLines[i] ?? '';
    const spanOpen = textSpanClass ? `<span class="${textSpanClass}">` : '<span>';
    return `<div class="${lineWrapperClasses.join(' ')}" data-line="${ln}" title="${escapeHtml(title)}">${spanOpen}${lineHtml || '&nbsp;'}</span></div>`;
  }).join('');

  return `
  <div class="editor-area">
    <div class="line-numbers">${lineNums || '<div class="ln">1</div>'}</div>
    <div class="editor-content" contenteditable="true" spellcheck="false" data-role="editor">${codeHtml}</div>
    ${renderAutocomplete()}
  </div>`;
}

function renderAutocomplete() {
  const ac = uiState.autocomplete;
  if (!ac) return '';
  const items = (ac.items || []).map((it, idx) => (
    `<div class="ac-item ${idx === ac.selected ? 'active' : ''}">
      <div class="ac-key">${escapeHtml(it.key)}</div>
      <div class="ac-desc">${escapeHtml(it.desc || '')}</div>
    </div>`
  )).join('');

  return `
    <div class="autocomplete" role="listbox">
      <div class="ac-header">Suggestions for: <b>${escapeHtml(ac.query || '')}</b></div>
      ${items}
    </div>`;
}

function renderTerminal() {
  const prompt = uiState.terminalPrompt;
  return `
  <div class="terminal">
    <div class="terminal-header">
      <div class="terminal-title">Terminal</div>
      <div class="terminal-actions">
        <button data-action="clear-terminal" title="Clear">Clear</button>
        <button data-action="toggle-terminal" title="Minimize">─</button>
      </div>
    </div>
    <div class="terminal-body">
      ${terminalLines.map(l => `<div class="term-line term-${l.type}">${escapeHtml(l.text)}</div>`).join('')}
      ${prompt?.show ? `
        <div class="term-line term-input-row">
          <span class="term-info">${escapeHtml(prompt.label || 'Enter value:')}</span>
          <input class="term-input" placeholder="${escapeHtml(prompt.placeholder || '')}" data-role="term-input">
        </div>
      ` : ''}
    </div>
  </div>`;
}


// ─── Event Handling ───

function showToast(message, type = 'info') {
  if (IS_SCREENSHOT_MODE) {
    uiState.toasts.push({ message, type });
    return;
  }
  const container = document.getElementById('toasts');
  if (!container) return;
  const toast = document.createElement('div');
  toast.className = `toast toast-${type}`;
  toast.textContent = message;
  container.appendChild(toast);
  setTimeout(() => toast.remove(), 3000);
}

function addTerminalLine(text, type = 'normal') {
  terminalLines.push({ type, text });
  const body = document.querySelector('.terminal-body');
  if (body) {
    const line = document.createElement('div');
    line.className = `term-line term-${type}`;
    line.textContent = text;
    // Insert before the input row
    const inputRow = body.querySelector('.term-input-row');
    if (inputRow) body.insertBefore(line, inputRow);
    else body.appendChild(line);
    body.scrollTop = body.scrollHeight;
  }
}

function attachEvents() {
  document.querySelectorAll('[data-action]').forEach(el => {
    el.addEventListener('click', (e) => {
      e.stopPropagation();
      const action = el.getAttribute('data-action');
      const index = parseInt(el.getAttribute('data-index'));

      switch (action) {
        case 'go-login':
          currentScreen = SCREENS.LOGIN;
          render();
          break;
        case 'go-register':
          currentScreen = SCREENS.REGISTER;
          render();
          break;
        case 'login':
          currentScreen = SCREENS.EDITOR;
          render();
          showToast('Logged in successfully', 'success');
          break;
        case 'register':
          currentScreen = SCREENS.EDITOR;
          render();
          showToast('Account created! Welcome 🎉', 'success');
          break;
        case 'select-doc':
          activeDocIndex = index;
          render();
          break;
        case 'new-doc':
          uiState.modal = { type: 'newDoc', defaultTitle: 'Untitled' };
          render();
          break;
        case 'delete-doc':
          if (documents.length <= 1) {
            showToast('Cannot delete the last document', 'error');
            return;
          }
          uiState.modal = { type: 'deleteConfirm', docIndex: index, docTitle: documents[index].title };
          render();
          break;
        case 'rename-doc':
          uiState.renamingDocIndex = index;
          render();
          break;
        case 'confirm-new-doc': {
          const input = document.querySelector('[data-role="newdoc-title"]');
          const title = (input?.value || 'Untitled').trim() || 'Untitled';
          documents.unshift({ title, modified: 'Just now', content: '', unsaved: true });
          activeDocIndex = 0;
          uiState.modal = null;
          uiState.saveStatus = { state: 'unsaved', text: 'Unsaved changes' };
          render();
          showToast('New document created', 'success');
          break;
        }
        case 'confirm-delete-doc': {
          const deleteIndex = index;
          const title = documents[deleteIndex]?.title || 'Document';
          documents.splice(deleteIndex, 1);
          if (activeDocIndex >= documents.length) activeDocIndex = documents.length - 1;
          uiState.modal = null;
          render();
          showToast(`"${title}" deleted`, 'info');
          break;
        }
        case 'modal-cancel':
        case 'modal-dismiss':
          uiState.modal = null;
          render();
          break;
        case 'retry-load':
          uiState.banner = { type: 'info', message: 'Retrying…' };
          uiState.loading = { title: 'Retrying…', subtitle: 'Fetching documents' };
          render();
          setTimeout(() => {
            uiState.loading = null;
            uiState.banner = null;
            uiState.saveStatus = { state: 'saved', text: '✓ Synced' };
            render();
            showToast('Connected', 'success');
          }, 700);
          break;
        case 'toggle-format-on-save':
          uiState.formatOnSave = !uiState.formatOnSave;
          render();
          showToast(`Format on Save: ${uiState.formatOnSave ? 'ON' : 'OFF'}`, 'info');
          break;
        case 'run':
          addTerminalLine(`> Running ${documents[activeDocIndex].title}...`, 'info');
          el.disabled = true;
          el.innerHTML = '⏳ Running...';
          setTimeout(() => {
            addTerminalLine('> Execution complete (0.03s)', 'success');
            el.disabled = false;
            el.innerHTML = '▶ Run<span class="shortcut">F5</span>';
          }, 1500);
          break;
        case 'format':
          showToast('Code formatted ✓', 'success');
          addTerminalLine('> Code formatted to Cambridge standard', 'success');
          break;
        case 'validate':
          addTerminalLine('> Validating pseudocode...', 'normal');
          setTimeout(() => {
            addTerminalLine('> Validation passed — 0 errors, 0 warnings ✓', 'success');
            showToast('Validation passed ✓', 'success');
          }, 600);
          break;
        case 'convert':
          addTerminalLine('> Converting to Python...', 'info');
          setTimeout(() => {
            addTerminalLine('> Python code generated (23 lines)', 'success');
            showToast('Converted to Python', 'success');
          }, 800);
          break;
        case 'review':
          addTerminalLine('> Requesting AI code review...', 'info');
          setTimeout(() => {
            addTerminalLine('> AI Review: Good use of modular design. Consider adding input validation.', 'success');
            showToast('AI review complete', 'success');
          }, 1200);
          break;
        case 'save':
          documents[activeDocIndex].unsaved = false;
          documents[activeDocIndex].modified = 'Just now';
          uiState.saveStatus = { state: 'saved', text: '✓ Saved' };
          render();
          showToast('Document saved ✓', 'success');
          break;
        case 'share':
          showToast('Share link copied to clipboard!', 'info');
          break;
        case 'clear-terminal':
          terminalLines.length = 0;
          const body = document.querySelector('.terminal-body');
          if (body) body.innerHTML = '<div class="term-line term-info">> Terminal cleared</div>';
          break;
        case 'toggle-terminal':
          const terminal = document.querySelector('.terminal');
          if (terminal) {
            const isMinimised = terminal.style.height === '30px';
            terminal.style.height = isMinimised ? '' : '30px';
            terminal.style.overflow = isMinimised ? '' : 'hidden';
            el.textContent = isMinimised ? '─' : '□';
          }
          break;
      }
    });
  });

  // Modal / rename focus helpers
  const newDocInput = document.querySelector('[data-role="newdoc-title"]');
  if (newDocInput) newDocInput.focus();

  const renameInput = document.querySelector('[data-role="rename-input"]');
  if (renameInput) {
    renameInput.focus();
    renameInput.addEventListener('keydown', (e) => {
      if (e.key === 'Enter') {
        e.preventDefault();
        const idx = parseInt(renameInput.getAttribute('data-index'));
        const value = renameInput.value.trim() || 'Untitled';
        documents[idx].title = value;
        uiState.renamingDocIndex = null;
        render();
        showToast('Document renamed', 'success');
      }
      if (e.key === 'Escape') {
        uiState.renamingDocIndex = null;
        render();
      }
    });
  }

  const termInput = document.querySelector('[data-role="term-input"]');
  if (termInput) {
    if (uiState.terminalPrompt?.focus) termInput.focus();
    termInput.addEventListener('keydown', (e) => {
      if (e.key === 'Enter') {
        const value = termInput.value;
        termInput.value = '';
        addTerminalLine(`> ${value}`, 'output');
        showToast('Input submitted', 'info');
      }
    });
  }

  // Keyboard shortcuts (attach once)
  if (!window.__KEYBOARD_SHORTCUTS_ATTACHED) {
    window.__KEYBOARD_SHORTCUTS_ATTACHED = true;
    document.addEventListener('keydown', (e) => {
      if ((e.metaKey || e.ctrlKey) && e.key === 's') {
        e.preventDefault();
        documents[activeDocIndex].unsaved = false;
        documents[activeDocIndex].modified = 'Just now';
        uiState.saveStatus = { state: 'saved', text: '✓ Saved' };
        render();
        showToast('Document saved ✓', 'success');
      }
      if (e.key === 'F5') {
        e.preventDefault();
        addTerminalLine(`> Running ${documents[activeDocIndex].title}...`, 'info');
        setTimeout(() => addTerminalLine('> Execution complete (0.03s)', 'success'), 1500);
      }
      if ((e.metaKey || e.ctrlKey) && e.key.toLowerCase() === 'k') {
        uiState.modal = { type: 'shortcuts' };
        render();
      }
    });
  }
}

// ─── Initial render ───
if (IS_SCREENSHOT_MODE) document.body.classList.add('screenshot-mode');
applyScenario(SCENARIO_ID);
render();