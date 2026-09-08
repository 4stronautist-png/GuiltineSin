# GuiltineSin (fka CloverTOS) — Project Context

Private Tree of Savior emulator, forked from the open-source **Melia** server (kenedos/melia)
and iteratively patched to match official **Papaya Play / IMC Games** client behavior via
reverse engineering. Single-developer, private use — not for public redistribution (client
game assets are IMC Games' copyrighted property; only the emulator code here is open-source-derived).

Read `.claude/HISTORY.md` before touching quest/NPC/dialog logic — it has the exact root causes
and fixes for dozens of prior quest-chain bugs. Do not re-derive from scratch what's already there.

## Identity

- Project/product name: **GuiltineSin** (renamed from CloverTOS, 2026-09-07). Guiltine is a
  goddess in-game; the name change is cosmetic/branding only.
- **In-game server name stays `Clover`** (`NAME="Clover"` in serverlist, `ServerName = "Clover"`)
  — intentional exception, do not rename this back to GuiltineSin.
- Windows client install root: `C:\GuiltineSin` (was `C:\CloverTOS-Local`).
- GitHub remote: `github.com/4stronautist-png/GuiltineSin` (public, by explicit user decision on
  2026-09-07 — the *code* repo being public is fine; the boundary that still holds is never
  publishing/automating distribution of the actual Tree of Savior client to third parties — see
  `.claude/skills/git-publish`). All 30 branches from the old
  `github.com/4stronautist-png/CloverTOS` remote (still tracked locally as `origin`) were
  migrated over, including feature branches from prior contributors (Assassin-PRONTO,
  Dragoon-OK, feature/bonemancer_implementation, etc). An earlier throwaway copy at
  `github.com/jeanjcd/GuiltineSin` also exists (wrong account, created while sorting out which
  GitHub identity to use) — safe to delete manually, not the canonical repo.

## Where things live

- Live repo (WSL): `/home/z3ck/Melia-TOS-Server/CloverTOS` (branch `fix/quest-chain-episodes-1-18`,
  large uncommitted working tree from the rename — do not `git reset`/`checkout --` without reading
  `git status` first, real unstaged quest fixes are mixed in).
- Server app: `server/app` (.NET, `GuiltineSin.sln`, was `Melia.sln`).
- Quest/NPC content: `server/app/packages/laima/...` and `server/app/src/ZoneServer/Scripting/Shared/NPCFunctions.cs`.
- Windows client install: `C:\GuiltineSin\release` (`/mnt/c/GuiltineSin/release` from WSL).
- Local DB: MariaDB, database `guiltinesin_local`, app user `guiltinesin` / `guiltinesin123`.
- Test login: account `gotei` / password `gotei123` (10 characters, team `Gotei`).
- Knowledge graph (AST-only, zero LLM cost): `graphify-out/graph.json` +
  `graphify-out/GRAPH_REPORT.md`, scoped to `server/app/src`. Query it (`graphify query "..."`)
  before grepping the whole server tree. Not committed to git (see `.gitignore`); regenerate with
  `/graphify server/app/src` if missing after a fresh clone.

## Skills

- `.claude/skills/quest-chain-repair` — quest progression, missing NPC spawns, static quests,
  dialog chains, episode 1-18 flow.
- `.claude/skills/client-reverse-analysis` — comparing server behavior against the compiled
  Papaya/IMC client (logs, packets, UI frames, crash traces).
- `.claude/skills/local-start-login` — start-server.sh, client launch, serverlist/login failures.
- `.claude/skills/git-publish` — pushing to the private GuiltineSin GitHub remote.

## Hard rules

1. Never make the GitHub repo public, never build/publish an installer that bundles or
   auto-downloads the Tree of Savior client for third parties. Server-emulator code only.
2. Never revert or discard uncommitted changes without checking `git status` first — this
   workspace stays dirty by design across long debugging sessions.
3. After any quest/NPC/dialog change: rebuild Release, run
   `server/app/tools/Run-GuiltineSinQuestRegression.ps1` (aggregates the Papaya/main-quest/level
   simulation validators), and restart via `start-server.sh` before declaring a fix done — a
   quest fix isn't proven until a real client interaction path exists, not just quest-state
   reachability (see HISTORY.md for why this rule exists).
