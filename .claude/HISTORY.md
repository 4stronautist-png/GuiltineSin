# Fix History (carried over from .codex/RESUME.md)

Naming note: this log predates the 2026-09-07 rename. Where it says `CloverTOS`/`Melia.sln`/
`C:\CloverTOS-Local`, read that as `GuiltineSin`/`GuiltineSin.sln`/`C:\GuiltineSin` today — the
code/behavior described is unchanged, only the names moved. Left verbatim below for accuracy.

---

Last updated: 2026-05-23 (Codex session, migrated to Claude Code 2026-09-07)

This workspace is a multi-project SoulSocietyTOS / CloverTOS / Melia setup. Start by reading
this file, then inspect the working trees, before asking the user to re-explain history.

## Latest local-start correction

On 2026-05-19, the Clover local client startup failure after reboot was traced to stale Windows
`netsh interface portproxy` entries. WSL had moved to a new IP, while Windows still forwarded
`127.0.0.1:8080` to the old WSL address, so the launcher could not load
`http://127.0.0.1:8080/toslive/patch/serverlist.xml` and exited before the game opened.

Fixes applied:
- `client/patches/loading-screen/release/Start-CloverTOS-Local.bat` now calls
  `CloverTOS-LoadingScreen.ps1` when present, with a direct EXE fallback only if the loader is missing.
- `server/app/start-server.sh` launches the client through the validated launcher first, then
  falls back to direct EXE start.
- `start-server.sh` excludes its own PowerShell PID when killing/checking stale loading-screen
  PowerShell processes, avoiding self-termination.
- `start-server.sh` revalidates Windows portproxy after an elevated update attempt, because the
  elevated process can update the table even if the caller times out waiting for it.

## Quest regression workflow

Single quest regression runner: `server/app/tools/Run-CloverQuestRegression.ps1` (now
`Run-GuiltineSinQuestRegression.ps1`). Run it after quest-chain changes or before claiming a
quest flow is fixed. It aggregates:
- `Validate-PapayaMainQuestFlow.ps1`
- `Validate-MainQuestChain.ps1`
- `Simulate-QuestProgressionToLevel.ps1 -TargetLevel 500`
- `Validate-TosKnowledge.ps1`
- a recent server/client log scan for quest/client runtime symptoms

Last known-good result: Papaya captured main flow passed for 44 links; main quest chain coverage
1435 reachable MAIN quests, 0 errors; player-flow simulation passed to level 501 with 177
completed quests (109 MAIN, 68 SUB); TOS knowledge consistency 0 errors (many fallback-dialog
warnings remain for later content — queue for Papaya-accurate static NPC placement). 639 MAIN
quests use ids above 65535; keep script lookup guarded and raw/static progression paths intact.

## Local login correction

Database had real local accounts (`gotei`, `gotei2`, `gotei3`, `gotei4`) plus API check accounts.
Primary: `gotei` / team `Gotei`, 10 characters, password reset to `gotei123` using the
Melia-compatible format `BCrypt(MD5(plainPassword))`. Added Barracks login diagnostics in
`server/app/src/BarracksServer/Network/PacketHandler.cs` (reports empty account name, unsupported
service nation, missing account, password mismatch, or stale logged-in state).

## Blank server-list correction

Client showed login screen but blank Server List even though `serverlist.xml`/
`static__Config.txt` fetches succeeded — the installed client had no active
`serverlist_recent.xml`, only a stale `.stale` cache, and would not reliably trust a remote-only
fetch. Fix: `start-server.sh` now writes `serverlist_recent.xml` during client config generation
and validates both `client.xml` and `serverlist_recent.xml` before reporting ready; the loading
screen script no longer deletes the cache and instead writes the validated remote serverlist back
using UTF-8 with BOM. `start-server.sh` now force-kills (elevated `taskkill` via UAC) a resistant
stale `Client_tos_x64.exe` and fails rather than declaring success with a stale client running.

## Local IP handling

For local mode, prefer the current WSL IP directly over `127.0.0.1`/portproxy —
`LOCAL_CLIENT_HOST` defaults to `LOCAL_HOST` (the live WSL IP), disabling automatic portproxy by
default. Windows/client connectivity validation is mandatory by default; the script fails loudly
on stale portproxy instead of printing "ready" while the client would time out.

## Goddess Saule (Veja Ravine, f_huevillage_58_1)

Quest `HUEVILLAGE_58_1_MQ01` showed a marker but the map only spawned a generic `Villager` — the
level-500 simulation had proven quest-state reachability, not that a human could see/click the
actor. Added visible `Goddess Saule` actor, dialog bridge, a real 7-item `Collect` objective from
`Tanu`, and a manual-interaction objective for Saule; hardened
`Validate-PapayaMainQuestFlow.ps1`/`Simulate-QuestProgressionToLevel.ps1` to require this going
forward — a recurring pattern, see next section.

## Vieta Gorge / "Activate the Obelisk" saga (f_huevillage_58_2, quest 20277) — 11 iterations

This is the canonical example of "quest state looks right, player still can't act" bugs in this
codebase. Read this in full before touching any collect/interact objective.

Papaya hidden NPC ids: `HUEVILLAGE_58_2_MQ01_NPC`=571, `MQ02_NPC`=572, `OBELISK_BEFORE`=573,
`OBELISK_AFTER`=574. Real bucket actors 575/576/577 (`Tree Sap Collection Container`, model
147354), altar 578.

1. **No visible actor** → spawned real static actors for elder/priest/obelisk/buckets/altar in
   `f_huevillage_58_2.cs`, added bucket click handling + `HUEVILLAGE_58_2_MQ02_BUCKET03` in
   `NPCFunctions.cs`, extended quest/session data through MQ04.
2. **Bucket click didn't advance objective** → `TryHandleStaticQuestNpcDialogAtStart` was
   consuming the click via the generic dialog-start shortcut before the bucket-specific
   `DialogFunction` ran. Fixed by skipping the shortcut when the active quest has an unfinished
   no-drop collect objective referencing that dialog via session `mapPointGroup`.
3. **Shortcut still intercepted** (follow-up) → the intercept must be scoped to exactly
   `NPCFunctions.COMMON_QUEST_HANDLER`; any NPC with a real specific `DialogFunction` runs that
   first, always.
4. **Item granted but UX wrong** (no collect animation, stale "0/3", tracker didn't advance) →
   root cause: server updated inventory/custom progress but not the native session-object values
   the client reads for objective text/map guidance. Added `TimeAction("Collecting...", "COLLECT",
   1.4s)` before granting the item, then `UpdateClient` + `SyncStaticQuestNpcStates` + HUD
   restore; `QuestInfoValue#` now derives from live quest progress instead of defaulting to zero;
   no-drop collect objectives filter remaining `mapPointGroup` sources by collected count.
5. **Tracker still showed "0/3" after collecting** → item grant alone isn't enough; must
   explicitly call `character.Quests.UpdateObjectives<CollectItemObjective>` for the quest/item
   BEFORE `UpdateClient`, so `progress.Count`/`Done` match inventory before `QuestInfoValue#`/
   `QuestMapPointGroup#` recompute.
6. **Tracker pointed at the wrong/old marker after relog** → named `mapPointGroup` actor-group
   points (e.g. `f_huevillage_58_2 HUEVILLAGE_58_2_MQ02_BUCKET02 100`) are rejected by
   `FilterClientSafeQuestMapPointGroups` (client-safe path wants numeric coordinates only), so the
   fallback silently discarded the correct bucket list. Fix: resolve named groups to numeric
   coordinates via `ResolveClientSafeQuestMapPointGroups`/`AddResolvedQuestMapPointGroups` BEFORE
   filtering/fallback. Known coordinates: bucket1 `-210 41 1115`, bucket2 `-85 41 1215`, bucket3
   `30 41 1080` (radius 100).
7. **Marker correct, object still not interactible** → `SyncStaticQuestNpcStates` only considered
   static start/progress/end NPCs "relevant"; collection sources referenced only via the active
   objective's `mapPointGroup` were visible but never promoted to highlighted/interactible. Added
   `StaticQuestActiveObjectiveReferencesDialog` so uncollected sources for active no-drop collect
   objectives get `SetMapNPCState(Highlighted)`.
8. **Highlighted but no `CZ_CLICK_TRIGGER`** → interaction range too small.
   `EnsureStaticQuestNpcInteractionSurface` raises `PropertyName.Range` to at least 250 for active
   objective NPCs before re-entry/highlight.
9. **Still no `CZ_CLICK_TRIGGER`** → some MISC visual models are never client-clickable no matter
   the state/range/highlight. Added per-character `ActorVisibility.Individual` personal objective
   click-target actors at the active position (`EnsureStaticQuestObjectiveInteractionActors`),
   sent via `ZC_ENTER_MONSTER`, cleaned up via `ZC_LEAVE` when progress advances. Initially reused
   the visual bucket model (147354) — still silent — then switched the interaction-surface model
   specifically to `HiddenTrigger2` (`20041`) while keeping the real dialog name, so the existing
   `DialogFunction` still runs. `ResolveStaticQuestNpcMonsterId`/`ResolveStaticQuestNpcName` map
   Vieta dialogs to correct models so fallback actors don't spawn as generic humans.
10. **Personal overlay made it worse** (extra NPC-looking actors appeared, still not clickable) →
    the invisible personal overlay was stealing/obscuring target selection from the real visible
    actor. New rule: **do not create personal fallback actors for real static Papaya objects**.
    `TryArmExistingStaticQuestObjectiveActor` arms the existing real actor first
    (highlight+range); `EnsureStaticQuestObjectiveInteractionActors` skips personal fallback
    entirely for Vieta bucket/altar/obelisk dialogs. Live logs then confirmed real genTypes
    575/576/577 armed correctly with no personal overlay spawned.
11. **Click finally reaches the server, but gets ignored** → log:
    `Vieta Gorge: player '10' activated Tree Sap Collection Container bucket 01.` followed by
    `bucket 01 ignored ... already collected or inventory already has enough sap.` — stale/
    divergent state: inventory already had the item from earlier attempts, quest tracker still
    said 0/3, and the handler blocked on inventory state before repairing the objective. Fix:
    `TryCollectVietaWhiteOakSap` now recalculates `CollectItemObjective` progress from
    `Inventory.CountItem(650617)` immediately once quest 20277 is confirmed active, and if
    inventory already satisfies it, repairs quest/client/session state and syncs instead of
    no-op'ing the click (logs `repaired White Oak Sap objective...`).

**Standing rules extracted from this saga** (apply to every collect/interact objective, not just
Vieta):
- A quest being state-reachable in simulation does NOT mean a real player can act on it. Require
  a spawned actor AND a client-reachable `DialogFunction`/interaction path.
- `TryHandleStaticQuestNpcDialogAtStart` may only intercept true fallback dialogs
  (`COMMON_QUEST_HANDLER` exactly) — never a dialog with its own specific `DialogFunction`.
- Collection UX must update native client guidance (`TimeAction`, `QuestInfoValue#`,
  `QuestMapPointGroup#`), not just inventory.
- Recalculate `CollectItemObjective` from real inventory count rather than trusting cumulative
  `AddItem` side effects — divergence between the two is the recurring root cause class here.
- Named `mapPointGroup` anchors must be resolved to numeric coordinates before the client-safe
  filter/fallback runs, or the fallback silently discards the correct target.
- Objects referenced only via an active objective's `mapPointGroup` must be promoted to
  relevant/highlighted in `SyncStaticQuestNpcStates`, with `Range` >= 250 for object-style
  interaction.
- Never overlay a personal per-character click-target actor on top of an already-real static
  actor — arm the real one first; personal overlays are fallback-only for genuinely missing
  actors, and when used, must use a `HiddenTrigger2` (`20041`) interaction surface, not the
  visual model.

## West Siauliai / opening flow notes (see `.claude/skills/quest-chain-repair` for the live version)

- Map `f_siauliai_west`, opening chain starts `SIAUL_WEST_MEET_TITAS`.
- Scout/Kepa track handoff must not run while `CurrentDialog` is still active — hard blocker for
  `SIAUL_WEST_DRASIUS1_TRACK`.
- Some Papaya MAIN quests are client-hidden bridge steps (e.g. `SIAUL_EAST_REQUEST3`): keep them
  server-only, auto-complete once objectives reach Success, continue to the next visible quest.
- After `SOUT_Q_14`, do not expose the native Crystal Mines/device chain unless session objects,
  devices, actors, cutscenes, and monster objectives are fully spawned — safe repair is to
  complete/cleanup Miners optional quests + all `MINE_*_CRYSTAL_*`/`MINE_3_*`/
  `CMINE6_TO_KATYN7_*` bridge quests, then start the next visible main quest (e.g. `GELE572_MQ_01`).

## Next-session protocol (still applies)

1. Read `CLAUDE.md`, then this file, before editing quest/NPC/dialog code.
2. Run `git status --short` — this repo stays intentionally dirty across long sessions; never
   revert unrelated changes.
3. Inspect focused diffs before editing.
4. After a fix: rebuild, run the quest regression tool, restart via `start-server.sh`, and prefer
   proving the fix via live server log lines (e.g. `Vieta Gorge: player '...' activated...`) over
   simulator-only validation — the simulator has repeatedly missed real-client interaction gaps
   that only show up in live logs.
