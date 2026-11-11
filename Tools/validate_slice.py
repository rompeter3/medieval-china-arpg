#!/usr/bin/env python3
"""Structural validation for the medieval-china-arpg vertical slice (no Unity required)."""
from __future__ import annotations

import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []
warnings: list[str] = []


def ok(msg: str) -> None:
    print(f"  OK  {msg}")


def err(msg: str) -> None:
    errors.append(msg)
    print(f" FAIL {msg}")


def warn(msg: str) -> None:
    warnings.append(msg)
    print(f" WARN {msg}")


def main() -> int:
    print(f"Validating {ROOT}")

    required_dirs = [
        "Assets/Scripts/Player",
        "Assets/Scripts/Combat",
        "Assets/Scripts/Items",
        "Assets/Scripts/UI",
        "Assets/Scripts/AI",
        "Assets/Scripts/World",
        "Assets/Data/Weapons",
        "Assets/Data/Armor",
        "Assets/Data/Skills",
        "Assets/Data/Materials",
        "Assets/Art/Shaders",
        "Assets/Scenes",
        "Docs",
        "Packages",
        "ProjectSettings",
    ]
    for d in required_dirs:
        if (ROOT / d).is_dir():
            ok(f"dir {d}")
        else:
            err(f"missing dir {d}")

    for f in [
        "README.md",
        "Docs/DESIGN.md",
        ".gitignore",
        "Packages/manifest.json",
        "ProjectSettings/ProjectVersion.txt",
        "Assets/Art/Shaders/StylizedRim.shader",
        "Assets/Scenes/Bootstrap.unity",
        "Assets/Scenes/CharacterCreate.unity",
        "Assets/Scenes/HubTown.unity",
        "Assets/Scenes/CombatLevel.unity",
    ]:
        if (ROOT / f).is_file():
            ok(f"file {f}")
        else:
            err(f"missing file {f}")

    weapons = list((ROOT / "Assets/Data/Weapons").glob("*.asset"))
    if len(weapons) == 20:
        ok("20 weapon assets")
    else:
        err(f"expected 20 weapon assets, found {len(weapons)}")

    scripts = list((ROOT / "Assets/Scripts").rglob("*.cs"))
    if len(scripts) >= 25:
        ok(f"{len(scripts)} C# scripts")
    else:
        err(f"too few scripts: {len(scripts)}")

    # Basic C# sanity: braces balanced, no obvious bad tokens
    for cs in scripts:
        text = cs.read_text(encoding="utf-8")
        if text.count("{") != text.count("}"):
            err(f"unbalanced braces in {cs.relative_to(ROOT)}")
        if "new Color(0.12f, 0.1," in text:
            err(f"non-float Color literal in {cs.relative_to(ROOT)}")
        if re.search(r"\busing\s+UnityEngine\.SceneManagement\s*;", text) and "SceneManager" not in text and "LoadSceneMode" not in text:
            warn(f"unused SceneManagement import? {cs.relative_to(ROOT)}")

    # Playable weapons named in catalog
    catalog = (ROOT / "Assets/Scripts/Items/ItemCatalog.cs").read_text(encoding="utf-8")
    playable = [
        "Katana", "Jian", "Dao", "Bow", "Crossbow",
        "ThreeSectionStaff", "SpearQiang", "Guandao",
    ]
    for name in playable:
        if name in catalog:
            ok(f"catalog has {name}")
        else:
            err(f"catalog missing {name}")

    stubs = [
        "TwinHooks", "RopeDart", "WarFan", "ButterflySwords", "NineRingSaber",
        "MeteorHammer", "DeerHornKnives", "MonkSpade", "HiddenSleeveKnives",
        "ChainWhip", "ShortBow", "GlaivePuDao",
    ]
    for name in stubs:
        if name in catalog:
            ok(f"stub {name}")
        else:
            err(f"missing stub {name}")

    manifest = (ROOT / "Packages/manifest.json").read_text(encoding="utf-8")
    if "com.unity.render-pipelines.universal" in manifest:
        ok("URP in manifest")
    else:
        err("URP missing from Packages/manifest.json")

    version = (ROOT / "ProjectSettings/ProjectVersion.txt").read_text(encoding="utf-8")
    if "6000." in version or "2022." in version or "2023." in version:
        ok(f"Unity version line present")
    else:
        warn("unexpected ProjectVersion.txt")

    shader = (ROOT / "Assets/Art/Shaders/StylizedRim.shader").read_text(encoding="utf-8")
    for token in ["_RimPower", "_LeafSway", "UniversalPipeline", "GetMainLight"]:
        if token in shader:
            ok(f"shader has {token}")
        else:
            err(f"shader missing {token}")

    print()
    print(f"Warnings: {len(warnings)}  Errors: {len(errors)}")
    return 1 if errors else 0


if __name__ == "__main__":
    sys.exit(main())
