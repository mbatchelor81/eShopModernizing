#!/usr/bin/env python3
import json
import pathlib
import sys


BOM_UTF8 = b"\xef\xbb\xbf"

TEXT_EXTENSIONS = {
    ".cs",
    ".csproj",
    ".cshtml",
    ".aspx",
    ".ascx",
    ".config",
    ".json",
    ".props",
    ".md",
    ".ps1",
    ".sln",
    ".targets",
    ".xml",
    ".sh",
    ".yml",
    ".yaml",
}


def trim_trailing_whitespace(text):
    cleaned_parts = []
    for line in text.splitlines(keepends=True):
        if line.endswith("\r\n"):
            cleaned_parts.append(line[:-2].rstrip(" \t") + "\r\n")
        elif line.endswith("\n"):
            cleaned_parts.append(line[:-1].rstrip(" \t") + "\n")
        elif line.endswith("\r"):
            cleaned_parts.append(line[:-1].rstrip(" \t") + "\r")
        else:
            cleaned_parts.append(line.rstrip(" \t"))

    return "".join(cleaned_parts)


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

    raw = path.read_bytes()
    had_bom = raw.startswith(BOM_UTF8)

    try:
        text = raw.decode("utf-8-sig")
    except UnicodeDecodeError:
        return 0

    cleaned = trim_trailing_whitespace(text)
    if cleaned != text:
        encoding = "utf-8-sig" if had_bom else "utf-8"
        path.write_bytes(cleaned.encode(encoding))

    return 0


if __name__ == "__main__":
    sys.exit(main())
