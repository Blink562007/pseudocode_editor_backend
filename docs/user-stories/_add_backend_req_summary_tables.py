from __future__ import annotations

from pathlib import Path
import re

ROOT = Path("docs/user-stories")

EXISTING_ENDPOINTS = {
    "GET /api/pseudocode",
    "GET /api/pseudocode/{id}",
    "POST /api/pseudocode",
    "PUT /api/pseudocode/{id}",
    "DELETE /api/pseudocode/{id}",
    "POST /api/pseudocode/validate",
    "POST /api/pseudocode/format",
}

RX_ENDPOINT = re.compile(r"`((?:GET|POST|PUT|DELETE)\s+/api/[^`]+)`")

TABLE_HEADER = "| Endpoints touched | DB impact | Services | Auth |"
TABLE_SEPARATOR = "|---|---|---|---|"


def _compact(s: str, n: int, empty: str) -> str:
    s = re.sub(r"\s+", " ", (s or "")).strip()
    if not s:
        return empty
    return s if len(s) <= n else (s[: n - 1].rstrip() + "…")


def _escape_cell(s: str) -> str:
    return (s or "").replace("|", "\\|")


def _norm_db(s: str) -> str:
    t = re.sub(r"\s+", " ", (s or "")).strip().rstrip(".")
    if not t:
        return ""
    if t.lower() in {"none", "n/a", "na"}:
        return "None"
    return t


def _norm_optional(s: str) -> str:
    t = re.sub(r"\s+", " ", (s or "")).strip().rstrip(".")
    if not t:
        return ""
    if t.lower() in {"none", "n/a", "na"}:
        return "—"
    return t


def _extract_backend_section(txt: str) -> str:
    a = txt.find("## Backend Requirements")
    b = txt.find("**Traces to:**")
    if a == -1 or b == -1 or a >= b:
        return ""
    return txt[a:b]


def _extract_field(br: str, label: str) -> str:
    lines = br.splitlines()
    rx = re.compile(r"^\s*-\s+\*\*" + re.escape(label) + r":\*\*\s*(.*)$")

    for i, ln in enumerate(lines):
        m = rx.match(ln)
        if not m:
            continue

        rest = (m.group(1) or "").strip()
        if rest:
            return rest

        # If the bullet has no inline content, take the first meaningful line beneath it
        for ln2 in lines[i + 1 :]:
            if re.match(r"^\s*-\s+\*\*.+?:\*\*", ln2):
                break
            s = ln2.strip()
            if not s:
                continue
            s = re.sub(r"^[-*]\s+", "", s).strip()
            if s:
                return s
        return ""

    return ""


def _build_table(br: str) -> list[str]:
    # Endpoints
    endpoints: list[str] = []
    seen: set[str] = set()
    for ep in RX_ENDPOINT.findall(br):
        ep = ep.strip()
        if ep and ep not in seen:
            endpoints.append(ep)
            seen.add(ep)

    tagged = [
        ("EXISTING " if ep in EXISTING_ENDPOINTS else "NEW ") + ep for ep in endpoints
    ]
    ep_cell = "; ".join(f"`{t}`" for t in tagged) if tagged else "—"

    # DB / Services / Auth
    db = _compact(_norm_db(_extract_field(br, "Database")), 60, "None")
    svc = _compact(
        _norm_optional(_extract_field(br, "Service layer logic")), 60, "—"
    )
    auth = _compact(
        _norm_optional(_extract_field(br, "Authentication/authorization")), 60, "—"
    )

    return [
        TABLE_HEADER + "\n",
        TABLE_SEPARATOR + "\n",
        "| {ep} | {db} | {svc} | {auth} |\n".format(
            ep=_escape_cell(ep_cell),
            db=_escape_cell(db),
            svc=_escape_cell(svc),
            auth=_escape_cell(auth),
        ),
    ]


def _insert_after_backend_heading(txt: str, table_lines: list[str]) -> tuple[str, bool]:
    lines = txt.splitlines(keepends=True)

    for i, ln in enumerate(lines):
        if ln.strip() != "## Backend Requirements":
            continue

        # Find the first non-empty line after the heading
        j = i + 1
        while j < len(lines) and lines[j].strip() == "":
            j += 1

        # If a summary table already exists, replace it (keep the rest untouched)
        if j < len(lines) and lines[j].strip() == TABLE_HEADER:
            k = j
            while k < len(lines) and lines[k].lstrip().startswith("|"):
                k += 1
            while k < len(lines) and lines[k].strip() == "":
                k += 1

            new_txt = "".join(lines[: i + 1] + ["\n"] + table_lines + ["\n"] + lines[k:])
            return new_txt, new_txt != txt

        # Otherwise insert (normalize: remove blank lines directly after heading; we'll re-add one)
        k = i + 1
        while k < len(lines) and lines[k].strip() == "":
            k += 1

        new_txt = "".join(lines[: i + 1] + ["\n"] + table_lines + ["\n"] + lines[k:])
        return new_txt, new_txt != txt

    return txt, False


def main() -> None:
    files = sorted(ROOT.glob("phase-*/*.md"))
    changed = 0

    for p in files:
        txt = p.read_text(encoding="utf-8")
        br = _extract_backend_section(txt)
        table_lines = _build_table(br)
        new_txt, did = _insert_after_backend_heading(txt, table_lines)
        if did:
            p.write_text(new_txt, encoding="utf-8")
            changed += 1

    print(f"UPDATED {changed}/{len(files)} files")


if __name__ == "__main__":
    main()

