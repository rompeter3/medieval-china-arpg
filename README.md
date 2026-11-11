# Medieval China ARPG

Stylized Unity URP vertical slice: medieval-China hub town + one combat level, inventory/equip/upgrade, 20 weapon defs (8 playable movesets), 2 mobs + 1 telegraph boss.

## Requirements
- **Unity 6 LTS** (project version `6000.0.32f1`) or compatible Unity 6 editor
- Universal Render Pipeline (declared in `Packages/manifest.json`)

## Open & play
1. Clone this repo.
2. Open the project folder in Unity Hub → Unity 6.
3. Allow URP package restore on first open.
4. Open `Assets/Scenes/Bootstrap.unity` (or press Play in any included scene).
5. Flow: **Character Create → Hub Town → Combat Level**.

### Controls
| Input | Action |
|-------|--------|
| WASD | Move |
| Shift | Sprint |
| Mouse | Camera |
| LMB | Light attack / combo |
| RMB | Heavy attack |
| Q | Weapon skill |
| Space | Dodge (i-frames) |
| E | Use portal |
| I | Inventory / equip |
| Tab | Upgrade panel |
| Esc | Close inventory |

## Project layout
```
Assets/Scripts/Player|Combat|Items|UI|AI|World
Assets/Data/Weapons|Armor|Skills|Materials
Assets/Art/Shaders|Materials
Assets/Scenes/
Docs/DESIGN.md
```

## What works in this slice
- Preset character customization (face/hair/colors/outfit)
- Third-person move, sprint, attack, skills, dodge i-frames
- Hub with gate, stalls, lanterns, bamboo, combat portal
- Combat arena with cliffs/bamboo, 2 mob types, boss (3 phases)
- Inventory loot/equip/swap + material upgrades
- 20 weapon definitions; 8 fully playable movesets
- Stylized URP rim + soft shadow shader (optional leaf sway)

## Known gaps
- Placeholder capsule/box art (no bespoke meshes/animations)
- Stub weapons have stats only (locked from equip)
- No authored audio; minimal OnGUI HUD (not production UI)
- Scenes are intentionally thin — `GameBootstrap` builds runtime content
- Unity Editor not required to inspect scripts; full play needs the Editor

## Validate without Editor
```bash
python3 Tools/validate_slice.py
```

## License
Code in this repository is provided for the project owner (`rompeter3`). Add a formal license as desired.
