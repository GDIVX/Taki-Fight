#!/bin/bash
set -euo pipefail

CHANGELOG_FILE=${CHANGELOG_FILE:-CHANGELOG.md}
WEBHOOK_URL=${PLAYTEST_DISCORD_UPDATE:?"PLAYTEST_DISCORD_UPDATE not set"}

# Extract latest changelog entry
header=$(grep -m 1 '^## ' "$CHANGELOG_FILE" | sed 's/^## //')
notes=$(awk '/^## /{if(found) exit; found=1; next} found{print}' "$CHANGELOG_FILE")

content="**$header**\n$notes"

payload=$(jq -n --arg content "$content" '{content:$content}')

curl -H 'Content-Type: application/json' -d "$payload" "$WEBHOOK_URL"
