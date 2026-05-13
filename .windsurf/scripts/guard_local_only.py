#!/usr/bin/env python3
import json
import re
import sys


BLOCKED_PATTERNS = [
    (r"\bdevin\b", "This demo is local-only; do not invoke Devin or Devin for Terminal."),
    (r"app\.devin\.ai", "This demo is local-only; do not open Devin cloud sessions."),
    (r"devin\s+for\s+terminal", "This demo is local-only; do not use Devin for Terminal."),
    (r"\breset\b.*--hard(?![-\w])", "Destructive git resets are blocked during demo worktree sessions."),
    (r"\bclean\b.*(?:-[A-Za-z]*f[A-Za-z]*|--force)(?![-\w])", "Destructive git clean commands are blocked during demo worktree sessions."),
    (r"\bpush\b.*(?:--force(?![-\w])|\s-f(?:\s|$))", "Force-push is blocked during local demo work."),
]


def main():
    try:
        payload = json.load(sys.stdin)
    except json.JSONDecodeError:
        return 0

    if payload.get("agent_action_name") != "pre_run_command":
        return 0

    command = payload.get("tool_info", {}).get("command_line", "")
    normalized = command.lower()

    for pattern, message in BLOCKED_PATTERNS:
        if re.search(pattern, normalized):
            print(message, file=sys.stderr)
            return 2

    return 0


if __name__ == "__main__":
    sys.exit(main())
