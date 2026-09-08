---
name: quest-chain-repair
description: Use when repairing GuiltineSin quest-chain progression, especially the Steam-like early-game flow in West Siauliai, missing quest NPC spawns, cutscene-triggered starts, static quests from quests.txt, or NPC dialog chains that stop after the first line. Covers episodes 1-18.
---

# Quest Chain Repair

Use when quest data exists but the gameplay chain is partially broken. Read `.claude/HISTORY.md`
first — the Vieta Gorge saga there documents 11 iterations of the exact failure class this skill
exists to prevent repeating.

## Focus areas

- Tutorial/opening chain for new characters in `f_siauliai_west`
- Cutscene tracks such as `SIAU_WEST_START_TRACK`
- Static quests from `server/app/system/db/quests.txt`
- NPC dialog functions in `server/app/src/ZoneServer/Scripting/Shared/NPCFunctions.cs`
- Static field NPC spawns in `server/app/packages/laima/scripts/zone/content/laima/npcs/fields/`
- Quest sync logic in `server/app/src/ZoneServer/World/Actors/Characters/Components/QuestComponent.cs`

## Working assumptions

The client may already show quest icons/objectives even when the NPC actor is missing. Early bugs
are usually one of:
- missing static NPC spawn for a `startNPC` / `progressNPC` / `endNPC`
- wrong `DialogName`
- static quest progressing through `quests.txt` without a `QuestScript`
- cutscene/track property not being set or cleared
- bad actor model/template causing the scene to play with the wrong NPC
- track start attempted while an NPC dialog is still active (throws, blocks the handoff)
- client left in cutscene UI state after fallback completion — needs explicit gameplay-HUD restore

## The rule this skill exists to enforce

**A quest being state-reachable in `Simulate-QuestProgressionToLevel.ps1` does NOT mean a real
player can act on it.** For every collect/interact objective, require: a spawned static actor,
a client-reachable `DialogFunction` (or the fallback-only `COMMON_QUEST_HANDLER`), a native
tracker (`QuestInfoValue#`, `QuestMapPointGroup#`) that reflects real progress, and — for objects
referenced only via an active objective's `mapPointGroup` — highlighted/interactible state with
`Range` >= 250. See `.claude/HISTORY.md` for the full failure chain and why each of these matters
independently; skipping any one of them reproduces a bug that has already been found and fixed
once.

## Fast workflow

1. Check the current quest row in `server/app/system/db/quests.txt`.
2. Confirm `startNPC`, `progressNPC`, `endNPC`, start/end mode, required quest names.
3. Search for the matching `DialogFunction` in `NPCFunctions.cs`.
4. Search the map field script for a static `AddNpc(...)` using that exact dialog name.
5. If the objective exists but the NPC doesn't appear, add/fix the static spawn.
6. If clicking only shows one line, inspect `COMMON_QUEST_HANDLER` and static quest flow in `QuestComponent`.
7. If a cutscene launches from an NPC dialog, defer the track or close the active dialog before `TrackComponent.Start`.
8. After forced/fallback track end, restore gameplay UI: cutscene off, core HUD properties, object
   property sync, skill/sysmenu addon refresh.
9. Rebuild Release, restart, validate with `logs/ZoneServer1.log` AND a real client interaction —
   not just the simulator (see HISTORY.md item 11: state can look right while the click is silently ignored).

## West Siauliai notes

- Map `f_siauliai_west`; opening chain starts `SIAUL_WEST_MEET_TITAS`.
- Key dialog names: `SIAUL_WEST_CAMP_MANAGER`, `SIALUL_WEST_DRASIUS`, `SIAUL_WEST_SOL3`, `SIAUL_WEST_NAGLIS2`.
- Spread across `packages/laima/scripts/zone/content/laima/npcs/fields/f_siauliai_west.cs`,
  `packages/laima/scripts/zone/content/laima/tracks/fields/siaul_west_meet_titas_track.cs`,
  `NPCFunctions.cs`, `QuestComponent.cs`.
- Scout/Kepa track handoff must not run while `CurrentDialog` is active — hard blocker for `SIAUL_WEST_DRASIUS1_TRACK`.

## Miners Village / Crystal Mines notes

- Handoff from `SIAUL_EAST_REQUEST7` into `f_siauliai_out`.
- First Miners NPC dialog `SIAULIAIOUT_Q01` needs a `DialogFunction` bridge to `COMMON_QUEST_HANDLER`.
- Goddess statue dialog `WARP_F_SIAULIAI_OUT` should repair Papaya main quest state and restore
  gameplay HUD before/after the statue warp menu.
- After `SOUT_Q_14`, don't expose the native Crystal Mines/device chain unless session objects,
  devices, actors, cutscenes, and monster objectives are fully spawned. Safe repair: complete/
  cleanup Miners optional quests plus all `MINE_1_CRYSTAL_*`, `MINE_2_CRYSTAL_*`, `MINE_3_*`,
  `CMINE6_TO_KATYN7_*` bridge quests, then start the next visible main quest (e.g. `GELE572_MQ_01`).
- If HUD disappears after a Miners quest/warp, verify `RestoreCoreHudState` includes `mainstatus`,
  `buff`, `buff_separatedlist`, `questinfoset_2`, `quickslotnexpbar`, and that generic client-native
  tracks are aborted on map transition via `AbortGenericTrackAfterMapTransition`.

## Validation

Watch `logs/ZoneServer1.log` and `logs/BarracksServer.log` for quest start lines, cutscene start
lines, dialog exceptions, and missing actor/spawn symptoms after objective creation.

Run `server/app/tools/Run-GuiltineSinQuestRegression.ps1` after any change (aggregates
`Validate-PapayaMainQuestFlow.ps1`, `Validate-MainQuestChain.ps1`,
`Simulate-QuestProgressionToLevel.ps1 -TargetLevel 500`, `Validate-TosKnowledge.ps1`, and a log scan).

## Known standing fixes (do not regress these)

- Static quest NPC dialogs bridge through `COMMON_QUEST_HANDLER`.
- `QuestComponent` has generic static NPC-dialog advancement logic.
- `Character.Dialog.TryHandleStaticQuestNpcDialogAtStart` only intercepts true fallback dialogs
  (exactly `COMMON_QUEST_HANDLER`) — any dialog with a specific `DialogFunction` runs that first.
- Inactive/possible bridge or side quests must not emit native Mission Objectives updates; client
  visibility goes through `QuestShouldBeVisibleInClientList` and native notification filters.
- Static NPCs belonging only to unavailable static quests stay hidden until the main-chain state
  reaches them.
- No-drop collect objectives with named `mapPointGroup` sources require BOTH a spawned static
  actor AND a client-reachable `DialogFunction` that grants the item and syncs quest state.
- Player-like collection must use `TimeAction` for feedback, sync `QuestInfoValue#` from real
  objective progress, and advance `QuestMapPointGroup#` to the remaining source.
- Recalculate `CollectItemObjective` from `Inventory.CountItem(...)` rather than trusting
  cumulative `AddItem` side effects alone.
- Named `mapPointGroup` anchors (`map DialogName radius`) must be resolved to numeric coordinates
  via `ResolveClientSafeQuestMapPointGroups`/`AddResolvedQuestMapPointGroups` before the
  client-safe filter/fallback runs.
- Objects referenced only via an active objective's `mapPointGroup` must be promoted to
  relevant/highlighted in `SyncStaticQuestNpcStates`, with `PropertyName.Range` >= 250.
- Never overlay a personal per-character click target on a real existing static actor — arm the
  real actor first (`TryArmExistingStaticQuestObjectiveActor`); personal fallback actors are for
  genuinely missing actors only, and must use `HiddenTrigger2` (`20041`) as the interaction model,
  never the visual model.
