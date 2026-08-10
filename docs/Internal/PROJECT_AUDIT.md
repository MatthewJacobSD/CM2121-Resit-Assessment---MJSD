# CM2121 ChainFragrance — Project Audit

Verified audit of the ChainFragrance project taken **3 August 2026** ahead of
the CM2121 resit submission (due 6 August 2026), with the **6 August final
submission hardening** and **weather system redesign** passes appended in §11-12.
All facts below were verified by reading the actual project files on disk;
nothing is assumed.

---

## 1. Project facts

| Item | Value |
| --- | --- |
| Project | CM2121 Resit Assessment - MJSD |
| Unity Editor | 6000.3.18f1 (Unity 6) |
| Render Pipeline | Universal Render Pipeline (URP) — `Assets/Settings/PC_RPAsset.asset` assigned in `ProjectSettings/GraphicsSettings.asset` (`m_CustomRenderPipeline` guid `4b83569d...`) |
| Input System | New Input System — `Assets/Input/ActionsControl.inputactions` (guid `e8794d9b...`) |
| Build scene (index 0) | `Assets/Scenes/Florance.unity` (only scene enabled in `EditorBuildSettings.asset`) |
| Git branch | `main` |

## 2. Scenes

| Scene | Role | Notes |
| --- | --- | --- |
| `Assets/Scenes/Florance.unity` | Main game scene | 192 GameObjects, 8 root transforms, 36 prefab instances, ~1.95 MB YAML |
| `Assets/Scenes/ChainFragrance.unity` | Secondary / reference scene | All script GUID references resolve |

## 3. Project scripts (Assets/Scripts)

| Script | Purpose |
| --- | --- |
| `Core/GameManager.cs` | Central state: timer, lives, objectives, end conditions |
| `Core/ScoreManager.cs` | Signed current score + persistent high score (PlayerPrefs `HighScore_Recycling`) |
| `Core/AudioManager.cs` | Centralised audio playback |
| `Core/AutoSpawner.cs` | Spawning helper |
| `Interaction/PickupItem.cs` | Carryable recyclable item (Rigidbody, category, base score) |
| `Interaction/RecycleBinInteractable.cs` | Bin trigger volume; owns the scoring matrix |
| `Player/PlayerInteraction.cs` | Raycast pickup / hold / drop / throw |
| `Player/PlayerMovement.cs` | CharacterController movement, jump, gravity |
| `Player/PlayerLook.cs` | Mouse look |
| `Player/PlayerFootstepAudio.cs` | Surface-based footstep audio |
| `Player/WeatherMovementEffect.cs` | Weather speed modifier |
| `UI/UIManager.cs` | Menu flow (welcome/instructions/playing/end), input maps |
| `UI/HUDManager.cs` | HUD stats, announcements, score popups |
| `UI/PauseMenuManager.cs` | Pause menu, settings, exit flow |
| `UI/InteractionPromptUI.cs` | "Press E to Pick Up" prompts |
| `UI/WeatherUI.cs` | Weather state readout |
| `Weather/*` | Weather state machine, effects, splash/lightning/wind/rain/cloud/sun |
| `Editor/AutoSetupPauseMenu.cs` | Editor tool that (re)builds the UI hierarchy and wiring |
| `Editor/FixGameplayAssets.cs` | Editor tool (this pass) that fixes gameplay prefab/asset data |

## 4. Verified gameplay asset inventory

### Collectibles — 24 items (3 bins)

| Category | Count | Names | Verified data issues |
| --- | --- | --- | --- |
| Plant (`itemType 0`) | 12 | `Bonsay_01–04`, `Vase_Plant_01–04`, `Vase_Pot_Plant_01–04` | none |
| Toy (`itemType 1`) | 8 | `WolfPlushie_01–04` + `ShibaPlushie_02/03/04`, `Shiba_Plushie_01` (all corrected to Toy on 3 Aug) | none remaining |
| Bottle (`itemType 2`) | 4 | `Plastic_Bottle_01–04` (`PlasticBott;e` typo fixed 3 Aug) | none remaining |

All 24 item prefabs are **prefab variants** of the OBJ import prefabs under
`Assets/Models/Blender/RawScans/`. Each variant root carries
`Rigidbody` + `PickupItem` and **no collider** (verified: only `!u!54` and
`!u!114` blocks). None of the prefabs set a layer (defaults to `Default` / 0).

### Bins — 3

| Prefab | BinType | Verified data issues |
| --- | --- | --- |
| `General Waste.prefab` | GeneralWaste (2) | **fixed 6 Aug**: was serialised `binType: 0`; now `2` |
| `Nature Recycling.prefab` | NatureRecycling (0) | correct (was the only one already matching) |
| `Plastic Recycling.prefab` | PlasticRecycling (1) | **fixed 6 Aug**: was serialised `binType: 0`; now `1` |

### Bin scoring matrix (source of truth: `RecycleBinInteractable.CalculateScore`)

| Item type | Nature | Plastic | General |
| --- | --- | --- | --- |
| Plant | **+20** | −45 | −20 |
| Bottle | −15 | **+20** | +15 |
| Toy | −25 | −15 | **+25** |
| anything else | −10 | −10 | −10 |

Correct = `CalculateScore(...) > 0`. Note General Waste *rewards* Bottles (+15)
and Toys (+25); this is the author's intended design and is preserved.

### GameManager scene override values (verified on disk)

| Field | Scene value | Required after this pass |
| --- | --- | --- |
| `maxLives` | 5 | 5 (keep) |
| `gameDuration` | 300 s | 300 (keep) |
| `plantsRequired` | 6 | **12** (scene inventory) |
| `chainBonus` | 40 | 40 (keep) |
| `chainThreshold` | 2 | 2 (keep) |

## 5. Scene layers (ProjectSettings/TagManager.asset)

| Layer | Name | Used by |
| --- | --- | --- |
| 6 | Environment | environment objects |
| 7 | Interactable | items (must be set — `PlayerInteraction.interactLayer` mask = `m_Bits 128` = layer 7) |
| 8 | Bin | bins (must be set) |
| 9 | Player | player |

## 6. Confirmed gameplay bugs (root causes) identified during audit

1. **No colliders on any item or bin prefab.** Pickup is a 3 m raycast on
   layer 7 (`PlayerInteraction.cs:151,202`); bin recycling is `OnTriggerEnter`
   (`RecycleBinInteractable.cs:64`). Both require colliders that do not exist —
   the core loop (pick up → throw → recycle) is impossible as-is.
2. **Items are on the wrong layer.** All item instances are `Default` (0);
   the interaction raycast only hits layer 7 (`Interactable`). Even with
   colliders, items could not be targeted.
3. **No walkable ground in the play area.** The player spawns at
   (592, 94.2, 2995). The only scene colliders are 4 wall slabs and the
   player's capsule/controller. There is no `Terrain` component (terrain data
   `Environment/Terrain/Soil.asset` and the user's new `New Terrain*.asset`
   exist but are not applied), and the "Valley/Gulley/Lake/Global" prefab
   instances are **light-probe / reflection-probe rigs only** — no terrain
   geometry exists. The player free-falls. (Resolution is the user's terrain
   work; this pass does **not** add placeholder floors.)
4. **Score never changes.** `GameManager.ReportRecycled` never calls
   `ScoreManager.AddScore/AddPenalty`; `scoreValue` is only forwarded to the
   UI popup event. High score therefore stays 0.
5. **Lives logic inverted.** `GameManager.cs:152–163` decrements a life for
   *any* Toy or Bottle (even correctly recycled) and never for wrong-bin
   Plants. Correct rule: `scoreValue > 0` = correct, `scoreValue < 0` = wrong.
6. **Win condition only counts plants** (`GameManager.cs:203–211`) and counts
   wrong-bin plants too. Toys/bottles never matter for winning.
7. **8 toy instances + WaterPlane are stacked at one world point**
   (1264.6, 283.5, 530.0), far outside the play corridor (walls at
   z ≈ 2957–3957). All 8 toys share identical transform overrides; the toys
   are unreachable. Scene-level issue — user repositions in Unity.
8. **Shiba plushies mislabelled** as Plants (`itemType 0`,
   `itemName "DogPlushie2"`); bottles have the `PlasticBott;e` typo.
9. **UI wiring broken:** duplicate `PauseMenuManager` (comp 1854047160, dead)
   next to the live one (comp 1037489632); all button `onClick` lists empty
   (or pointing at the script asset with empty `m_MethodName`); `OnPressignSettings`
   typo present; HUD stat texts `{0}`; `finalScoreText` wired to the
   deactivated HUD `Count` text; `toysText`/`bottlesText` fall back to popup
   labels; `InteractionPromptUI` texts `{0}`; bin VFX/audio `{0}`.

## 7. Confirmed non-issues (do not waste time on these)

- **No missing scripts.** All MonoBehaviour script GUID references resolve
  (project scripts, TextMeshPro `f4688fdb`, uGUI `4e29b1a8`, Input System
  `e8794d9b`).
- **The 4 "unresolved" GUIDs are harmless:**
  - `0b6d251b…` (`Volume.m_MaterialTemplate`) — **resolved 5 Aug**: the
    referenced `TerrainLit.mat` was copied into
    `Assets/Environment/Terrain/Materials/` with a fresh GUID (`8e36950e…`)
    and the scene remapped; no longer a demo-folder reference.
  - `241f7368…` (`Volume.m_LightmapParameters`) on the SoilPlane volume —
    cosmetic inspector-only reference.
  - `3d2422e4…` and `c7439120…` — stale prefab-override targets on two
    PrefabInstance blocks (missing-override warnings, non-blocking).
- **Water + terrain material chain self-contained (5 Aug):** `WaterDepthBased
  .shadergraph`, `WaterDepthBased.mat`, `Lake_Margins.tif`, `TerrainLit.mat`
  all now live under `Assets/Environment/` with fresh GUIDs; zero references
  to `Assets/TerrainDemoScene_URP/` remain in the game.
- **Correction (10 Aug):** the *terrain data* was intentionally re-sourced
  from the demo folder. The scene's `m_TerrainData` keeps GUID `584c420d…`,
  now owned by the single tracked
  `Assets/TerrainDemoScene_URP/Terrain/Data/Terrain_1_2_….asset`; the
  duplicate copy under `Assets/Environment/Terrain/Data/` was removed. All
  other demo-folder references remain eliminated.
- **URP is correctly assigned** (`PC_RPAsset.asset`); the water material uses
  the included `WaterDepthBased.shadergraph` (see water shader review).
- **Camera culling mask = all layers** (`m_Bits 4294967295`) — nothing culled.
- **All transform scales are 1**; input/EventSystem references are consistent.
- The "empty" environment containers are serialisation artefacts of prefab
  instances (children connect via `m_TransformParent` inside `!u!1001`
  blocks) — see `VISIBILITY_REPORT.md`.

## 8. Build settings & project settings

- Single build scene: `Assets/Scenes/Florance.unity`.
- Custom layers present: 6 Environment, 7 Interactable, 8 Bin, 9 Player.
- GraphicsSettings `m_CustomRenderPipeline` → `PC_RPAsset.asset` (URP).
- Quality settings referenced from `ProjectSettings/QualitySettings.asset`.

## 9. Git state (end of final pass, 6 August)

- Branch `main`; HEAD `d09bfe0` ("Reorganisation code structure arrangement…").
- **Staged (ready for final submission commit):**
  - Scene YAML fixes (playerControls/groundCheck), rewritten
    `AutoSetupPauseMenu.cs`, `InteractionPromptUI.cs`.
  - Deletion of unused `Assets/TerrainDemoScene_URP/` (`git rm -r`, user-approved).
  - Terrain rebuild: `New Terrain.asset`, `New Terrain 1.asset`,
    `New Terrain 4.asset` added; `New Terrain 4` is the active scene terrain.
  - Old `Assets/Audio/*` removals.
- **5 Aug hardening (staged):** self-contained water/terrain assets
  (`Environment/Terrain/Materials/TerrainLit.mat`, `Environment/Prefabs/Water/
  Textures/Lake/Lake_Margins.tif`, overwritten `WaterDepthBased.shadergraph` +
  `.mat`) with fresh GUIDs; scene + both `WaterPlane.prefab` + shadergraph
  remapped off the demo folder; docs updated.
- **6 Aug gameplay fixes (staged):** player grounding (CC center + groundMask),
  storm wind-push, audio remap to Optimized, bin binType fix, HUD numeric texts,
  FootstepAudio wiring.
- **Untracked (do not commit):** `New Terrain 2/3.asset` (unreferenced
  duplicates), whole `Assets/Sounds/` folder (102 MB `RawAudio` + 14 MB
  `Optimized`). Note: the Optimized clips referenced by the scene are **not**
  in git — if they are delivered from the repo clone they must be added or the
  SFX will silently miss.

## 10. Fix pass status (applied 3 August)

The confirmed bugs above were addressed as follows. State = intended fix;
apply and verify with the two batch tools (`FixGameplayAssets.Run` /
`FixSceneUI.Run`), then re-run `FixGameplayAssets.Validate` /
`FixSceneUI.Validate`.

| Bug | Fix | Status |
| --- | --- | --- |
| 1. No colliders on items/bins | `FixGameplayAssets` adds BoxCollider (items) / CapsuleCollider triggers (bins) to prefab roots | Implemented |
| 2. Wrong layer on items | Tool sets `Interactable` (7) on item roots, `Bin` (8) on bins, via `LayerMask.NameToLayer` | Implemented |
| 3. No walkable ground | **Resolved 6 Aug** — active flat `New Terrain 4` at origin, player grounded (CC center `(0,0,0)`, groundMask `81`); migrated 54 MB tile kept inactive | Implemented |
| 4. Score never changes | `GameManager.ReportRecycled` now calls `ScoreManager.AddScore/AddPenalty` with signed scores | Implemented |
| 5. Lives logic inverted | Lives lost only when `scoreValue < 0` (wrong bin); correct recycles never cost a life | Implemented |
| 6. Win condition plants-only | Now 12/8/4 per-category objectives; wrong-bin items excluded | Implemented |
| 7. Toys stacked far away | **Not automated** — scene-level; user repositions in Unity | User |
| 8. Shiba/Bottle labels | Item names/types corrected (Shiba → Toy, `PlasticBott;e` → `Plastic Bottle`) | Implemented |
| 9. UI wiring | `FixSceneUI` removes duplicate PauseMenuManager, rewires HUD/pause/end-screen/InteractionPromptUI references, fixes popup-text conflict and `OnPressignSettings` | Implemented |

## 11. Final submission hardening (6 August)

- **Player grounding:** `CharacterController.m_Center` `(0,1,0)`→`(0,0,0)`
  (GO `1261081358`); `PlayerMovement.groundMask` `80`→`81` (`1261081360`).
- **Storm wind push:** `PlayerMovement.SetWindPush(Vector3)` + ramped
  `MoveTowards` away from the wrong bin (`maxWindPushSpeed 3f`,
  `windPushRampSpeed 2.5f`) in `WeatherFeedbackSystem` (`1808429662`).
- **Audio remap:** player footsteps + all 3 bin prefab SFX now use
  `Assets/Sounds/Optimized` clips; project-wide GUID scan confirms **no
  `RawAudio` references** remain in scene/prefabs/scripts. Three legacy
  ambient AudioSources (SunlightEffect `71494726` inactive, RainyParams
  `803733999`, StormyParams `1185070769`) are intentionally unwired — ambient
  is managed by `AudioManager.CrossfadeAmbient`.
- **Bin types:** all 3 bin prefabs were serialised `binType: 0`; corrected to
  2 / 1 / 0. Verified against `RecycleBinMatrixTests` (24 items correct).
- **HUD:** `HUDManager.cs` now writes bare values matching the static scene
  Headers ("Score", "Player Lives x").
- **Validation:** 3× batch import via `BatchImport.Run` — exit 0, clean
  compile (`unity_windpush.log` 90 s, `unity_audio_remap.log` 188 s,
  `unity_hud_bin.log` 138 s). Zero unresolved asset GUIDs; `31321ba1…` is the
  URP default material (auto-regenerated on import), **not** a broken ref.

**Remaining manual steps (user):**
- Reposition the 8 toy instances + WaterPlane (bug 7).
- Water shader polish review (visual polish optional).
- Optional: delete unreferenced `New Terrain.asset` + `New Terrain 1/2/3.asset`
  and `Assets/Models/RawScans/` (all confirmed unreferenced).

## 12. Weather system redesign (6 August)

### Weather state machine

4 states: `Sunny` → `Rain` → `HeavyRain` → `Storm`

| State | Trigger | Audio | Wind | Player Speed |
|-------|---------|-------|------|-------------|
| Sunny | No item held | Silence | 2 m/s | 1.2x |
| Rain | Near wrong bin (15m) | AMB_Rain.wav | 8 m/s | 0.75x |
| HeavyRain | Closer to wrong bin (10m) | AMB_StrongRain.wav | 14 m/s | 0.6x |
| Storm | Very close to wrong bin (6m) | AMB_Storm.wav | 8-20 m/s + push | 0.45-0.75x |

### Key design decisions

1. **No rain on item pickup** — weather stays sunny until player approaches a wrong bin
2. **Progressive calming** — approaching a correct bin calms weather progressively
3. **Wrong recycle feedback** — storm persists 2s after wrong recycle, then calms
4. **Correct recycle** — immediate return to sunny
5. **Wind push** — only active during storm, max 3 m/s, scales with intensity

### Audio migration

All gameplay audio now uses only the Optimized library:
- StormyParams AudioSource: ThunderRain.wav → AMB_Storm.wav
- RainyParams AudioSource: Raining.wav → AMB_Rain.wav
- AudioManager: removed unused sunnyClip, cloudyClip, windyClip fields
- AudioManager starts silent (weather system handles crossfade)
- Crossfade handles null clips (fades to silence for sunny)

### WeatherMovementEffect

Added to Player GameObject and wired to WeatherFeedbackSystem:
- Sunny: 1.2x speed (faster)
- Rain: 0.75x speed
- HeavyRain: 0.6x speed
- Storm: 0.45-0.75x speed (scales with intensity)

### Weather distances

| Distance | State | Rationale |
|----------|-------|-----------|
| 15m | Rain begins | Early warning |
| 10m | Heavy rain | Clear escalation |
| 6m | Storm | Urgency, but retreat possible |
| 5m | Correct bin cancel | Player is at the right place |

### Validation

- All 8 modified scripts compile (brace balance verified)
- Scene YAML changes verified via GUID scan
- Zero old RawAudio GUIDs in active gameplay
- WeatherMovementEffect component added and wired
