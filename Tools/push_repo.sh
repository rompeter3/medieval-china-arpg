#!/usr/bin/env bash
# Create rompeter3/medieval-china-arpg and push main.
# Prerequisites: gh authenticated (gh auth login or GH_TOKEN).
set -euo pipefail
cd "$(dirname "$0")/.."

if ! gh auth status >/dev/null 2>&1; then
  echo "Not authenticated. Run: gh auth login   OR export GH_TOKEN=..."
  exit 1
fi

OWNER="${GITHUB_OWNER:-rompeter3}"
REPO="${GITHUB_REPO:-medieval-china-arpg}"
VIS="${GITHUB_VISIBILITY:-public}" # public|private

if gh repo view "$OWNER/$REPO" >/dev/null 2>&1; then
  echo "Repo exists: https://github.com/$OWNER/$REPO"
else
  gh repo create "$OWNER/$REPO" --"$VIS" --source=. --remote=origin --description "Stylized Unity URP medieval-China ARPG vertical slice"
fi

git push -u origin main
echo "Pushed. URL: https://github.com/$OWNER/$REPO"
echo "Open in Unity 6 → Play Assets/Scenes/Bootstrap.unity"
