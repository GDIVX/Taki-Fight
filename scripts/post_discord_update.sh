#!/bin/bash
set -euo pipefail

CHANGELOG_FILE=${CHANGELOG_FILE:-CHANGELOG.md}
WEBHOOK_URL=${PLAYTEST_DISCORD_UPDATE:?"PLAYTEST_DISCORD_UPDATE not set"}

# Extract latest changelog entry
header=$(grep -m 1 '^## ' "$CHANGELOG_FILE" | sed 's/^## //')
notes=$(awk '/^## /{if(found) exit; found=1; next} found{print}' "$CHANGELOG_FILE")

content="**$header**\n$notes"

payload=$(printf '%s' "$content" | \
    python3 -c "import sys, json; print(json.dumps({'content': sys.stdin.read()}))")

curl -H 'Content-Type: application/json' -X POST -d "$payload" "$WEBHOOK_URL"
