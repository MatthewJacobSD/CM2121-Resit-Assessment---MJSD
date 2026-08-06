# CM2121 Eco Rescue FPS — Migration Checklist

## Audio Migration

| File | Old Project | New Project | Status |
|------|-------------|-------------|--------|
| 16429__agarwalparag__dropobject.wav | ✅ | ✅ | Migrated |
| 186103__marcel_farres__recycle-bin.wav | ✅ | ✅ | Migrated |
| 194993__soundmary__splashing-footsteps.mp3 | ✅ | ✅ | Migrated |
| 346946__vincent2cent__mediumhard-rain-indoors.wav | ✅ | ✅ | Migrated |
| 362950__alexanderche__bin-trash-recycled.wav | ✅ | ✅ | Migrated |
| 366556__scotthurly__throwing-away-an-item.wav | ✅ | ✅ | Migrated |
| 442261__jonastisell__plastic-bottle-drop.mp3 | ✅ | ✅ | Migrated |
| 469070__hawkeye_sprout__stumble-steps.wav | ✅ | ✅ | Migrated |
| 477393__nuff3__steps-dirt-falling_1a.ogg | ✅ | ✅ | **Migrated today** |
| 628810__the_underdog__binaural-suburban-footsteps-2.mp3 | ✅ | ✅ | Migrated |
| 699923__8bitmyketison__multimedia-sfx-error-01.wav | ✅ | ✅ | **Migrated today** |
| 717771__1bob__victory-chime.wav | ✅ | ✅ | **Migrated today** |
| 728687__kristoffer_andersson__amb_forest_thunder_rain.wav | ✅ | ✅ | Migrated |
| 733684__randomrecord19__spring-forest-at-the-stream.mp3 | ✅ | ✅ | Migrated |
| 73583__jzazvurek__body-fall-in-grass-close.mp3 | ✅ | ✅ | Migrated |
| 73584__jzazvurek__body-fall-in-grass-distant.wav | ✅ | ✅ | Migrated |
| 79177__nathan_lomeli__bottle-dropped-in-trashcan.wav | ✅ | ✅ | Migrated |
| 835112__yoshikamiyafuji__fail-sound-wa-wa-wah.wav | ✅ | ✅ | Migrated |
| 859080__coghezzi__fantasy-achievement-unlock.wav | ✅ | ✅ | Migrated |

**Total:** 19/19 audio files migrated ✅

**Audio note (6 Aug):** all in-game gameplay SFX now reference the curated
`Assets/Sounds/Optimized` clips (player footsteps, bin success/error). The
legacy `RawAudio` originals remain on disk (untracked, 102 MB) but are not
referenced by any scene/prefab/script (project-wide GUID scan verified).

---

## Scanned Models Migration

| Model | Old Project | New Project | Status |
|-------|-------------|-------------|--------|
| Bonsay | ✅ | ✅ | Present |
| Dog Plushie | ✅ | ✅ | Present |
| Dog Plushie 2 | ✅ | ✅ | Present |
| Nature Recycle Bin | ✅ | ✅ | Present |
| Plastic Bottle | ✅ | ✅ | Present |
| Stairs | ✅ | ✅ | Present |
| Vase Plant | ✅ | ✅ | Present |
| Vase Plant Pot | ✅ | ✅ | Present |

**Total:** 8/8 scanned models present ✅

---

## Folder Structure Migration

| Item | Old Project | New Project | Status |
|------|-------------|-------------|--------|
| Audio/SFX/ | ✅ | ✅ | Migrated |
| Models/ScannedObjects/ | ScannedModels/ | ✅ | Renamed |
| Environment/Terrain/ | TerrainDemoScene_URP/ | ✅ | Simplified + self-contained (5 Aug) |
| Input/PlayerControl.inputactions | ✅ | ✅ | Present |
| Scripts/Player/ | UnityEcoFPSController/ | ✅ | Simplified |
| Scripts/Objects/ | — | ✅ | New |
| UI/Prefabs/ | — | ✅ | New (card system) |
| Settings/ | ✅ | ✅ | Present |

---

## Scripts Migration

| Script | Old Project | New Project | Status |
|--------|-------------|-------------|--------|
| PlayerMovement.cs | PlayerController.cs | ✅ | Rewritten |
| PlayerLook.cs | PlayerCamera.cs | ✅ | Rewritten |
| PlayerInteraction.cs | PlayerInteraction.cs | ✅ | Rewritten |
| PickupObject.cs | PlayerPickupTrigger.cs | ✅ | Rewritten |
| GameManager.cs | GameManager.cs | ✅ | Created + rewritten (3 Aug) |
| AudioManager.cs | AudioManager.cs | ✅ | Created |
| HUD.cs | HUD.cs | ✅ | Created (3 Aug rewrite) |
| BinCollector.cs | BinCollector.cs | ✅ | Created as RecycleBinInteractable.cs |
| CollectibleItem.cs | CollectibleItem.cs | ✅ | Created as PickupItem.cs |
| WeatherWindController.cs | WeatherWindController.cs | ✅ | Created as WindEffect + WeatherFeedbackSystem |

---

## Assets NOT Migrated (Re-importable from Asset Store)

| Asset | Size | Reason |
|-------|------|--------|
| Rocks and Boulders 2 | 386MB | Re-importable from Asset Store |
| TerrainDemoScene_URP | 4GB | Re-importable from Asset Store — **removed 6 Aug** (`git rm -r`); game is now fully self-contained |

---

## Documentation Files (Gitignored)

| File | Purpose | Status |
|------|---------|--------|
| TIMELINE.md | Combined project timeline | ✅ Created |
| PROJECT_AUDIT.md | Full project inventory | ✅ Created |
| MIGRATION_CHECKLIST.md | This file | ✅ Created |
| ASSESSMENT_REQUIREMENTS.md | Assessment brief mapping | ✅ Created |
| generate_docs.py | Auto-generate docs at end | ✅ Created (output/) |
