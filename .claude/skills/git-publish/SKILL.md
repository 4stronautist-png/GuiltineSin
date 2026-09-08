---
name: git-publish
description: Push the GuiltineSin repository to its private GitHub remote when the user asks to send/publish/sync GuiltineSin changes, or gives a branch name after @gitpublish. Targets /home/z3ck/Melia-TOS-Server/CloverTOS pushing to https://github.com/4stronautist-png/GuiltineSin.git (private). If normal authentication fails, ask for a new GitHub token during execution and use it temporarily for the push.
---

# Git Publish

Push the local `CloverTOS` working tree (product name GuiltineSin) to its GitHub remote quickly
and safely.

## Repository

- Working tree: `/home/z3ck/Melia-TOS-Server/CloverTOS`
- Remote: `origin -> https://github.com/4stronautist-png/GuiltineSin.git` (**must stay private**)
- Prior/legacy remote (do not push here going forward unless explicitly asked):
  `https://github.com/4stronautist-png/CloverTOS.git`

## Hard rule

**Never make this repository public, and never add a workflow/installer that bundles or
auto-downloads the Tree of Savior client for third parties.** This is a private, closed-group
project — confirmed explicitly by the user. If asked to make it public or to build a
public-facing client installer, stop and ask again before proceeding; that decision was
deliberately scoped to "no" once already.

## Workflow

1. Interpret the user's branch name input.
2. Inspect: `git status --short --branch`, `git remote -v`, `git log -1 --oneline`.
3. If the requested branch doesn't exist locally, create/switch only if clearly wanted; otherwise
   stop and say the branch is missing.
4. Run `git push -u origin <branch>` first.
5. On success, report the branch and latest commit hash.
6. On auth failure: ask for a new token, use it only for that one push, then immediately restore
   the remote URL back to the clean `https://github.com/4stronautist-png/GuiltineSin.git`.
7. Warn the user to revoke/regenerate any token pasted into chat after use.

## Safety rules

- Never leave a token embedded in `git remote -v`.
- Never commit tokens, passwords, generated askpass files, or machine-local secrets.
- Keep the push scoped to `CloverTOS`; don't publish sibling repos (`Melia`, `Melia-TOS-Client`)
  unless explicitly asked.
- If the repo is dirty in unexpected ways, summarize before pushing.
- Repo must be **private**. If `gh repo view 4stronautist-png/GuiltineSin --json isPrivate` ever
  reports `false`, flag it immediately rather than silently pushing more code to it.

## Expected final report

Keep it short: branch pushed, latest commit hash, whether auth fallback was needed, reminder to
revoke any pasted token.
