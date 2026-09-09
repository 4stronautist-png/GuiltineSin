---
name: client-reverse-analysis
description: Use when comparing GuiltineSin server behavior against a compiled Tree of Savior client (Papaya/IMC), extracting client-side expectations from logs, binaries, packet names, UI frame names, crash traces, and load sequences.
---

# Client Reverse Analysis

Use when a fix depends on what the compiled client expects, not just server quest script logic.
This is for your own private server matching official client behavior — not for redistributing
client assets. Keep findings scoped to server-side compatibility fixes.

## Goals

- Build a reproducible compatibility matrix between GuiltineSin server behavior and the Papaya/IMC client.
- Identify packet/order/property/UI-frame expectations from available client artifacts.
- Keep temporary server-side shims separate from confirmed porting fixes.
- Produce short status notes for future reference.

## Evidence sources

- Client logs: `C:\GuiltineSin\release\log_Client\app_history.log`,
  `C:\GuiltineSin\release\DisconnectLog\LastPacket.log`, crash dumps under
  `C:\GuiltineSin\release\dump`.
- Client binaries/assets: `Client_tos.exe`, `Client_tos_x64.exe`, `.ipf` archives, extracted
  `lua`, `addon_setting`, `userdata`, `languageData`.
- Server-side sequence: `ZC_START_GAME`, `CZ_LOAD_COMPLETE`/`ZC_LOAD_COMPLETE`, `ZC_MYPC_ENTER`,
  `ZC_OBJECT_PROPERTY`, `ZC_NPC_STATE_LIST`, `ZC_ENTER_MONSTER`/`ZC_LEAVE`, addon messages
  (`STAT_UPDATE`, `LEVEL_UPDATE`, `EXP_UPDATE`).

## Workflow

1. Reproduce with the smallest flow possible; record character name/time.
2. Read server logs around that timestamp before changing code.
3. Read client `app_history.log`, `LastPacket.log`, newest dump metadata.
4. Search binary strings for relevant frame names, packet names, addon messages, assert strings.
5. Classify: packet/order mismatch · stale actor handle · UI frame opened without valid context ·
   quest/session objective mismatch · missing asset/model/class data · server-side property missing/wrong.
6. Patch the narrowest confirmed server/client shim first.
7. Rebuild and restart.
8. Record what became confirmed, what remains inferred, what should be re-validated.

## Guardrails

- Do not force-open modal frames (inventory, quest, skill, map, status) as a HUD recovery strategy.
- Do not force-open target frames unless the server has a valid selected target and the client
  created the target context naturally.
- When removing track/cutscene actors, send `ZC_LEAVE` before removing the actor server-side.
- Treat "unknown monster", "non-existent target", and stale handles as actor lifecycle bugs, not combat bugs.
- Keep Papaya client observations as behavioral references; don't patch blindly from string names alone.

## Status note template

- Scenario:
- Expected Papaya behavior:
- Current GuiltineSin behavior:
- Evidence:
- Confirmed cause:
- Patch applied:
- Remaining risk:
