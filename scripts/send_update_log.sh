#!/bin/bash
set -euo pipefail

# Extract latest release notes from CHANGELOG.md
entry=$(awk 'BEGIN{capture=0}/^## \[Unreleased\]/{next}/^## \[/{if(capture)exit; capture=1}capture{print}' CHANGELOG.md)

if [ -z "$entry" ]; then
    echo "No changelog entry found" >&2
    exit 1
fi

payload=$(jq -n --arg content "$entry" '{content: $content}')

curl -H "Content-Type: application/json" -d "$payload" "$PLAYTEST_DISCORD_UPDATE"
