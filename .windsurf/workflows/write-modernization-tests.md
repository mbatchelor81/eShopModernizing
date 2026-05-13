---
name: write-modernization-tests
description: Add focused validation for a modernization lane when the repo has an existing test pattern
---

# /write-modernization-tests

Use this workflow inside a Worktree-mode local Cascade session.

## Steps

1. Confirm the target solution has an existing test project or established test pattern.
2. If no test project exists, do not invent a large test harness; recommend focused build/manual validation instead.
3. Inspect the target model, controller/page, service, and configuration path.
4. Add the smallest useful test or validation script that covers the requested behavior.
5. Avoid changing production behavior just to make validation easier.
6. Run the relevant restore/build/test command.
7. Summarize coverage, gaps, and how the human can verify the behavior in the UI.
