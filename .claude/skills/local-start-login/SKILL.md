---
name: local-start-login
description: Use when GuiltineSin local startup, Windows client launch, server list population, or account login fails or regresses. Covers start-server.sh, C:\GuiltineSin, WSL IP changes, serverlist_recent.xml, stale Client_tos_x64.exe processes, Barracks login diagnostics, and the known local gotei login.
---

# Local Start / Login

Use before changing random server/client files. The failure is almost always one of four
surfaces: Windows can't reach WSL, the client files point to the wrong host, the server list
cache is missing/stale, or the account password/login state is wrong.

## Required context

- Read `CLAUDE.md` and `.claude/HISTORY.md` first.
- Work from `/home/z3ck/Melia-TOS-Server/CloverTOS/server/app`.
- Respect the dirty worktree; do not revert unrelated quest/server changes.
- Windows client root: `C:\GuiltineSin\release` (`/mnt/c/GuiltineSin/release` from WSL).
- Known local login: account `gotei`, password `gotei123`.

## Triage workflow

1. Confirm the actual WSL IP: `hostname -I`
2. Confirm `client.xml` and `serverlist_recent.xml` both point to the same current WSL IP:
   ```powershell
   Get-Content C:\GuiltineSin\release\client.xml
   Get-Content C:\GuiltineSin\release\serverlist_recent.xml
   ```
   Both must contain `Clover` (the in-game server name — intentionally NOT renamed),
   `Server0_IP="<current WSL IP>"`, `Server0_Port="2000"`. A missing `serverlist_recent.xml` is a
   real failure, not noise.
3. Confirm the WebServer serves both to Windows/client:
   ```powershell
   curl.exe -sS http://<current WSL IP>:8080/toslive/patch/serverlist.xml
   curl.exe -sS http://<current WSL IP>:8080/toslive/patch/static__Config.txt
   ```
4. Confirm Barracks is reachable: `Test-NetConnection -ComputerName <current WSL IP> -Port 2000 -InformationLevel Quiet`
5. Tail server logs around the current time: `tail -n 120 logs/WebServer.log logs/BarracksServer.log`
   — Web log should show the two GETs above; Barracks log reports the specific rejected-login cause.

## Start script requirements

`start-server.sh` must not claim ready unless ALL of these pass:
- ports `2000 7001 7002 8080 9001 9002` listen inside WSL
- Windows can fetch `serverlist.xml`
- Windows can connect to Barracks `:2000`
- `client.xml` points to the current WSL IP
- `serverlist_recent.xml` exists and contains `Clover` for the current WSL IP
- account creation API writes to MariaDB
- Zone/Social/Web connect to the coordinator
- Zone servers load scripts

For local mode, prefer the direct WSL IP over `127.0.0.1`/portproxy. Use portproxy only when
explicitly configured for loopback.

## Client process rules

Kill stale clients before relaunching. If `Stop-Process` fails with access denied, use elevated
`taskkill` via UAC — don't open a second client and call that success.
```powershell
Get-Process Client_tos_x64,Client_tos -ErrorAction SilentlyContinue
taskkill /IM Client_tos_x64.exe /F
```
Verify the new process `StartTime` after relaunch.

## Server list cache

The client can show a blank Server List even with a `200 OK` remote fetch if
`serverlist_recent.xml` is missing/stale. Keep both sources valid. Write it as UTF-8 with BOM
(matches the historically-working client-side cache format).

## Login repair

Passwords stored as `BCrypt(MD5(plainPassword))`. Use `tools/PasswordHashTool` to generate a
valid hash. If resetting `gotei`, also clear stale login state:
```sql
UPDATE accounts SET password='<bcrypt-md5-hash>', loginState=0 WHERE name='gotei';
SELECT accountId,name,teamName,LEFT(password,12),LENGTH(password),loginState FROM accounts WHERE name='gotei';
```
`LENGTH(password)` must be `60`.

## When it still fails

- Blank server list: inspect `serverlist_recent.xml` first, then WebServer GETs.
- Enter does nothing: inspect `BarracksServer.log` for the rejection reason.
- No Barracks login attempt logged: client hasn't selected a valid server or is using the wrong
  client directory.
- Old process start time: kill the stale client before testing again.
