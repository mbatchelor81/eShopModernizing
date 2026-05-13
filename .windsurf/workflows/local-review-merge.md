---
name: local-review-merge
description: Review and merge completed local worktree agents from Agent Command Center
---

# /local-review-merge

Use this workflow from the main workspace after local worktree agents finish.

## Steps

1. Open Agent Command Center and review only local Cascade sessions in the current Space.
2. For each completed agent card, inspect the diff before merging.
3. Reject or send back any session that touched unrelated files, introduced secrets, skipped validation, or mixed multiple lanes.
4. Merge one worktree at a time.
5. After each merge, run `git status --short` and the narrowest relevant validation command.
6. Resolve straightforward conflicts in the main workspace; if the conflict reflects incompatible design choices, pause and choose one lane deliberately.
7. Run final aggregate validation for the changed solution(s).
8. Commit only reviewed source, configuration, and demo-support files.
