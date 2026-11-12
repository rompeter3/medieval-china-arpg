# Player builds (Windows + macOS)

Automated standalone players via [GameCI](https://game.ci/) GitHub Actions.

## Expected download URLs

After the first successful workflow run these stabilize:

- Windows: https://github.com/rompeter3/medieval-china-arpg/releases/download/player-builds/MedievalChinaARPG-Windows-x64.zip
- macOS: https://github.com/rompeter3/medieval-china-arpg/releases/download/player-builds/MedievalChinaARPG-macOS.zip

Release page: https://github.com/rompeter3/medieval-china-arpg/releases/tag/player-builds

## Required GitHub Actions secrets (user action)

GameCI needs a Unity Personal (or Pro) license. This repo **does not** invent or store license material. Add these under
**Settings → Secrets and variables → Actions**:

| Secret | Value |
|--------|--------|
| `UNITY_LICENSE` | Full contents of your local `Unity_lic.ulf` |
| `UNITY_EMAIL` | Unity account email |
| `UNITY_PASSWORD` | Unity account password (prefer alphanumeric; special characters can break activation) |

### How to create `Unity_lic.ulf` (Personal)

1. Install [Unity Hub](https://unity.com/download) on your machine.
2. Sign in with the Unity account you want CI to use.
3. **Unity Hub → Preferences → Licenses → Add** → get a free Personal license (must click Add so the `.ulf` is written).
4. Copy the file contents into the `UNITY_LICENSE` secret:
   - Windows: `C:\ProgramData\Unity\Unity_lic.ulf`
   - macOS: `/Library/Application Support/Unity/Unity_lic.ulf`
   - Linux: `~/.local/share/unity3d/Unity/Unity_lic.ulf`

Pro/Plus: set `UNITY_SERIAL` + email/password instead (and adjust the workflow env if you switch).

## Run the build

1. Merge/push the workflow on `main`.
2. Add the three secrets above.
3. **Actions → Build Players (Windows + macOS) → Run workflow**.
4. On success, assets attach to the `player-builds` release (URLs above).

Artifacts are also uploaded on the workflow run for 30 days.

## Local Editor builds

With Unity `6000.0.32f1` open:

- Menu **Build → Players → Windows + macOS**, or
- `-executeMethod MedievalChinaARPG.Editor.BuildPlayers.BuildAll`

Outputs: `Builds/Windows/`, `Builds/macOS/`.
