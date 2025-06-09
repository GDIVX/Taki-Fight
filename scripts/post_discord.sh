#!/usr/bin/env bash
set -e

artifact_url="$1"
drive_url="$2"
target="$3"

if [ -z "$PLAYTEST_DISCORD_BUILD" ]; then
    echo "PLAYTEST_DISCORD_BUILD not set"
    exit 1
fi

payload=$(cat <<JSON
{
  "content": "New $target build available\nGitHub: $artifact_url\nDrive: $drive_url"
}
JSON
)

curl -H "Content-Type: application/json" -X POST -d "$payload" "$PLAYTEST_DISCORD_BUILD"
