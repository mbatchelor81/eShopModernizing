#!/usr/bin/env python3
import json
import re
import shlex
import sys


BLOCKED_PATTERNS = [
    (r"app\.devin\.ai", "This demo is local-only; do not open Devin cloud sessions."),
    (r"devin\s+for\s+terminal", "This demo is local-only; do not use Devin for Terminal."),
]

GIT_VALUE_OPTIONS = {
    "-c",
    "-C",
    "--config-env",
    "--exec-path",
    "--git-dir",
    "--namespace",
    "--super-prefix",
    "--work-tree",
}


def normalize_token(token):
    return token.rstrip(";&|")


def has_shell_boundary(token):
    return normalize_token(token) != token


def is_git_token(token):
    executable = normalize_token(token).replace("\\", "/").rsplit("/", 1)[-1]
    return executable in {"git", "git.exe"}


def is_devin_invocation(command):
    try:
        tokens = shlex.split(command)
    except ValueError:
        return False

    return any(normalize_token(token).replace("\\", "/").rsplit("/", 1)[-1] == "devin" for token in tokens)


def iter_git_invocations(command):
    try:
        tokens = shlex.split(command)
    except ValueError:
        return

    for git_index, token in enumerate(tokens):
        if not is_git_token(token):
            continue

        index = git_index + 1
        while index < len(tokens):
            current = normalize_token(tokens[index])
            if current in GIT_VALUE_OPTIONS:
                index += 2
                continue
            if any(current.startswith(option + "=") for option in GIT_VALUE_OPTIONS):
                index += 1
                continue
            if current.startswith("-"):
                index += 1
                continue

            arguments = []
            for argument in tokens[index + 1 :]:
                arguments.append(normalize_token(argument))
                if has_shell_boundary(argument):
                    break

            yield current, arguments
            break


def has_short_force_flag(arguments):
    return any(
        argument.startswith("-")
        and not argument.startswith("--")
        and "f" in argument[1:]
        for argument in arguments
    )


def is_blocked_git_invocation(command):
    for subcommand, arguments in iter_git_invocations(command):
        if subcommand == "reset" and "--hard" in arguments:
            return "Destructive git resets are blocked during demo worktree sessions."
        if subcommand == "clean" and ("--force" in arguments or has_short_force_flag(arguments)):
            return "Destructive git clean commands are blocked during demo worktree sessions."
        if subcommand == "push" and (
            any(argument.startswith("--force") for argument in arguments)
            or has_short_force_flag(arguments)
        ):
            return "Force-push is blocked during local demo work."

    return None


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

    if is_devin_invocation(normalized):
        print("This demo is local-only; do not invoke Devin or Devin for Terminal.", file=sys.stderr)
        return 2

    blocked_git_message = is_blocked_git_invocation(normalized)
    if blocked_git_message:
        print(blocked_git_message, file=sys.stderr)
        return 2

    return 0


if __name__ == "__main__":
    sys.exit(main())
