from __future__ import annotations

from pathlib import Path
import re

DOCS = Path("docs")
ROOT = DOCS / "user-stories"
OUT = DOCS / "BACKEND_REQUIREMENTS_SUMMARY.md"

EXISTING = {
    "GET /api/pseudocode",
    "GET /api/pseudocode/{id}",
    "POST /api/pseudocode",
    "PUT /api/pseudocode/{id}",
    "DELETE /api/pseudocode/{id}",
    "POST /api/pseudocode/validate",
    "POST /api/pseudocode/format",
}

RX_PHASE = re.compile(r"phase-(\d+)")
RX_H1 = re.compile(r"^#\s+(US-\d+\.\d+)\s+·\s+(.*)\s*$", re.M)
RX_EP = re.compile(r"`((?:GET|POST|PUT|DELETE)\s+/api/[^`]+)`")


def _field(br: str, name: str) -> str:
    prefix = f"- **{name}:**"
    for ln in br.splitlines():
        s = ln.strip()
        if s.startswith(prefix):
            return s[len(prefix) :].strip()
    return ""


def _compact(s: str, n: int) -> str:
    s = re.sub(r"\s+", " ", s).strip()
    if not s:
        return "—"
    return s if len(s) <= n else (s[: n - 1].rstrip() + "…")


def _tag(ep: str) -> str:
    return ("EXISTING " if ep in EXISTING else "NEW ") + ep


def main() -> None:
    rows_by_phase: dict[int, list[dict[str, str]]] = {1: [], 2: [], 3: [], 4: []}
    new_counts: dict[int, int] = {1: 0, 2: 0, 3: 0, 4: 0}

    for p in sorted(ROOT.glob("phase-*/*.md")):
        txt = p.read_text(encoding="utf-8")
        m_phase = RX_PHASE.search(p.as_posix())
        m_h1 = RX_H1.search(txt)
        if not m_phase or not m_h1:
            continue
        phase = int(m_phase.group(1))
        us_id, title = m_h1.group(1), m_h1.group(2)

        a = txt.find("## Backend Requirements")
        b = txt.find("**Traces to:**")
        br = txt[a:b] if (a != -1 and b != -1 and a < b) else ""

        endpoints: list[str] = []
        seen: set[str] = set()
        for ep in RX_EP.findall(br):
            if ep not in seen:
                endpoints.append(ep)
                seen.add(ep)

        tagged = [_tag(ep) for ep in endpoints]
        new_counts[phase] += sum(1 for t in tagged if t.startswith("NEW "))
        ep_cell = "; ".join(f"`{t}`" for t in tagged) if tagged else "—"

        link = p.as_posix().replace("docs/", "./")
        rows_by_phase[phase].append(
            {
                "id": us_id,
                "title": title,
                "link": link,
                "endpoints": ep_cell,
                "db": _compact(_field(br, "Database"), 60),
                "services": _compact(_field(br, "Service layer logic"), 60),
                "auth": _compact(_field(br, "Authentication/authorization"), 60),
            }
        )

    lines: list[str] = []
    lines += [
        "# Backend Requirements Summary",
        "",
        "Implementation-oriented index (endpoints / DB / services / auth) for each user story.",
        "Each row links to the full story spec (which contains detailed **Backend Requirements**).",
        "",
        "**Legend:** `NEW …` vs `EXISTING …` indicates whether the endpoint exists in the current backend (Phase 1 baseline).",
        "",
        "**Reminder:** pseudocode API payloads should use `content` (not `code`) per the backend API contract and known client mismatch.",
    ]

    for phase in (1, 2, 3, 4):
        lines += [
            "",
            f"## Phase {phase}",
            "",
            f"- New endpoints referenced in this phase: **{new_counts[phase]}**",
            "",
            "| User Story | Title | Endpoints touched | DB impact | Services | Auth |",
            "|---|---|---|---|---|---|",
        ]
        for r in rows_by_phase[phase]:
            us = f"[{r['id']}]({r['link']})"
            lines.append(
                "| {us} | {title} | {endpoints} | {db} | {services} | {auth} |".format(
                    us=us,
                    title=r["title"],
                    endpoints=r["endpoints"],
                    db=r["db"],
                    services=r["services"],
                    auth=r["auth"],
                )
            )

    OUT.write_text("\n".join(lines) + "\n", encoding="utf-8")
    print(f"WROTE {OUT}")


if __name__ == "__main__":
    main()

