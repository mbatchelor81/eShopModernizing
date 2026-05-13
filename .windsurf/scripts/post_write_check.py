#!/usr/bin/env python3
import json
import pathlib
import sys


TEXT_EXTENSIONS = {
    ".cs",
    ".cshtml",
    ".aspx",
    ".ascx",
    ".config",
    ".json",
    ".md",
    ".ps1",
    ".sh",
    ".yml",
    ".yaml",
}


def main():
    try:
        payload = json.load(sys.stdin)
    except json.JSONDecodeError:
        return 0

    if payload.get("agent_action_name") != "post_write_code":
        return 0

    file_path = payload.get("tool_info", {}).get("file_path", "")
    if not file_path:
        return 0

    path = pathlib.Path(file_path)
    if path.suffix.lower() not in TEXT_EXTENSIONS or not path.exists():
        return 0

    try:
        text = path.read_text(encoding="utf-8-sig")
    except UnicodeDecodeError:
        return 0

    cleaned = "\n".join(line.rstrip() for line in text.splitlines())
    if text.endswith(("\n", "\r\n")):
        cleaned += "\n"

    if cleaned != text:
        path.write_text(cleaned, encoding="utf-8")

    return 0


if __name__ == "__main__":
    sys.exit(main())
