---
name: resume-context
description: Use when the user asks where we stopped, asks for the point of a previous correction/conversation, just restarted their PC/VS Code, or is frustrated about re-explaining GuiltineSin context. Ported from the old Codex-era `soulsociety-resume` skill and adapted to this repo's single consolidated workspace.
---

# Resume Context

Recover continuity for the GuiltineSin workspace without making the user re-explain history.

## First response

- Acknowledge briefly, don't ask the user to restate the full history.
- Say you'll reconstruct the point from the workspace, then do it.
- If the user wrote in Portuguese, answer in Portuguese.

## Recovery workflow

1. Read `CLAUDE.md` (identity, paths, DB creds, hard rules — usually already loaded automatically
   at session start, but re-confirm if anything looks stale).
2. Read `.claude/HISTORY.md` — the definitive fix/decision log. This is the equivalent of the old
   Codex `.codex/RESUME.md` checkpoint file; update it (or add a dated entry) whenever a meaningful
   milestone is reached, the same way `.codex/RESUME.md` used to be updated.
3. In `/home/z3ck/Melia-TOS-Server/CloverTOS`, run:
   - `git status --short --branch`
   - `git log -8 --oneline`
   - If `.git/MERGE_HEAD` or `.git/rebase-merge`/`.git/rebase-apply` exists, say so explicitly —
     it means a merge/rebase was left mid-flight.
4. If the user asks about "a correção" / "o que fizemos", don't re-derive from scratch — inspect
   targeted diffs first: `git diff --name-only`, then `git diff -- <file>` on the most relevant
   files.
5. Respect a dirty worktree. Never revert/reset/checkout away changes unless the user explicitly
   asks — this workspace goes dirty by design during long debugging sessions.
6. Summarize the recovered point briefly, then continue the work — don't just report status and
   stop if there's an obvious next step.

## Relationship to graphify

This skill and `/graphify` solve different problems and don't overlap directly:

- **graphify** answers "how is the code structured / where is X defined / what calls Y" — a
  structural knowledge graph of the codebase (`graphify-out/graph.json`), rebuilt from source.
- **This skill** answers "what were we doing / where did we leave off" — session and git-history
  continuity, not code structure.

Use `graphify query "..."` for code-navigation questions during a resumed task; use this skill's
workflow (HISTORY.md + git status/log) to figure out what that task even was. They're
complementary, not redundant — don't expect graphify's graph to carry session-state information,
it has none.

## Known checkpoint files

- `CLAUDE.md` — static identity/structure/hard-rules, auto-loaded every session.
- `.claude/HISTORY.md` — dated fix/decision log, read explicitly (not auto-loaded). Append to it
  after any non-trivial multi-step fix, the same discipline the old `.codex/RESUME.md` had.
