# Medieval China ARPG — Vertical Slice Design

## Pitch
Stylized / semi-realistic third-person ARPG set in a medieval Chinese frontier: a small hub town opens onto a single combat grounds. Systems first, world later.

## Loop (4h slice)
1. **Character create** — pick face/hair/outfit preset + starting playable weapon.
2. **Hub town** — walk gate/stalls/lanterns; open inventory to equip/swap/upgrade; enter portal.
3. **Combat level** — clear melee + ranged trash, learn telegraphs, defeat the warlord boss.
4. **Loot → upgrade** — scrap/jade mats feed flat weapon/armor tiers; return to hub via portal.

## Visual target
URP stylized look (custom `MedievalChinaARPG/StylizedRim`: soft shadow + rim; bamboo leaf sway). Tiled roofs, bamboo/pine, stone, lanterns. **Not** photoreal.

## Weapons (20 data / 8 live)

| # | Weapon | Status | Notes |
|---|--------|--------|-------|
| 1 | Katana | Playable | Light combo, heavy, iaido-style skill |
| 2 | Jian | Playable | Fast thrust/slash kit |
| 3 | Dao | Playable | Heavier cleaves |
| 4 | Composite Bow | Playable | Projectile skill |
| 5 | Crossbow | Playable | Slower, harder projectile |
| 6 | Three-Section Staff | Playable | Wide flail control |
| 7 | Qiang Spear | Playable | Long poke + thrust skill |
| 8 | Guandao | Playable | Slow, high damage sweeps |
| 9 | Twin Hooks | Stub | Catch-and-slash |
| 10 | Rope Dart | Stub | Flexible reach |
| 11 | War Fan | Stub | Concealed blades |
| 12 | Butterfly Swords | Stub | Paired CQC |
| 13 | Nine-Ring Saber | Stub | Disrupt rings |
| 14 | Meteor Hammer | Stub | Chain smash |
| 15 | Deer Horn Knives | Stub | Trapping |
| 16 | Monk Spade | Stub | Temple polearm |
| 17 | Hidden Sleeve Knives | Stub | Assassin burst |
| 18 | Chain Whip | Stub | Segment control |
| 19 | Short Bow | Stub | Mobile skirmish |
| 20 | Pu Dao Glaive | Stub | Infantry sweeps |

Each **playable** weapon: light 3-hit combo + heavy + 1 weapon skill. Shared **dodge with i-frames**.

## Combat content
- **Mob A:** melee bandit
- **Mob B:** ranged archer
- **Boss:** warlord with 3 telegraph phases — Sweep → Lunge → Storm (HP thresholds)

## Inventory
- Pickup mats/weapons/armor
- Equip weapon + head/chest/legs/hands
- Upgrade consumes mats for flat +damage / +armor

## Scenes
- `Bootstrap` → routes to character create
- `CharacterCreate`
- `HubTown`
- `CombatLevel`

## Out of scope (this slice)
Open-world streaming, 20 unique full animation sets, multiplayer, crafting trees, quests, ultra graphics, audio production, full city interiors.

## Architecture
```
Assets/Scripts/Player|Combat|Items|UI|AI|World
Assets/Data (weapons/armor/skills/materials)
Assets/Art (materials/shaders/placeholder props)
```

Runtime `GameBootstrap` + `ItemCatalog` construct playable content if editor wiring is incomplete, so empty scenes still run after opening in Unity.
