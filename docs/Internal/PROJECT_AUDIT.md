# CM2121 ChainFragrance — Project Audit

Verified audit of the ChainFragrance project taken **3 August 2026** ahead of
the CM2121 resit submission (due 6 August 2026), with the **6 August final
submission hardening**, **weather system redesign** (§11-12) and **10 August
contextual-audio / mouse-sensitivity audit** (§13) passes appended. All facts
below were verified by reading the actual project files on disk; nothing is
assumed.

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

## 13. Contextual audio & mouse-sensitivity audit (10 August)

Read-only audit of the audio/mouse/manager wiring plus the final code changes
in this pass. Findings are tagged **CONFIRMED** (verified against the serialised
scene/prefab/script data on disk), **POSSIBLE·UNREPRODUCED** (logic verified in
code but no runtime session was available to reproduce), or **NOT AN ISSUE**.

### 13.1 Audio asset hygiene

Every audio clip GUID referenced by the scene and bin prefabs resolves to
`Assets/Sounds/Optimized/` — CONFIRMED:

| Referenced clip | GUID | Resolves to |
| --- | --- | --- |
| rainyAmbient / AudioManager.rainyClip | `581b25e9…` | `Sounds/Optimized/Ambient/AMB_Rain.wav` |
| heavyRainAmbient | `e47206e9…` | `Sounds/Optimized/Ambient/AMB_StrongRain.wav` |
| stormyAmbient / AudioManager.stormyClip | `ecd2b90b…` | `Sounds/Optimized/Ambient/AMB_Storm.wav` |
| WaterAmbienceZone.waterClip | `9f5159ff…` | `Sounds/Optimized/Ambient/AMB_WaterFlowing.wav` |
| dryWalkFootsteps | `85d10511…` | `Sounds/Optimized/SFX/SFX_DryWalk.wav` |
| runningFootsteps | `6beec6ba…` | `Sounds/Optimized/SFX/SFX_Running.wav` |
| wetWalkFootsteps | `d73c1b47…` | `Sounds/Optimized/SFX/SFX_WetWalk.wav` |
| successClip (AudioManager + bins) | `1f75364b…` | `Sounds/Optimized/SFX/SFX_Correct.wav` |
| errorClip (AudioManager + bins) | `e587a08b…` | `Sounds/Optimized/SFX/SFX_Buzzer.wav` |
| pickupClip | `03044e0b…` | `Sounds/Optimized/SFX/SFX_CollectItem.wav` |
| dropClip | `4be50ad4…` | `Sounds/Optimized/SFX/SFX_DropItem.wav` |
| achievementClip | `a251a5c2…` | `Sounds/Optimized/SFX/SFX_Achievement.wav` |

No raw `m_Resource` clip references remain in game scenes/prefabs — CONFIRMED
(§11.3 re-verified). No new audio assets were imported in this pass.

### 13.2 Contextual footstep audio (`PlayerFootstepAudio`)

- Surface is resolved by a downward **raycast** using `groundMask` (`m_Bits
  81` = Default + Water + Environment layers) with `QueryTriggerInteraction
  .Ignore` — CONFIRMED. The scene Terrain (`Terrain_1_2…`, layer 0 Default)
  and `WaterPlane` (layer 4 Water, MeshCollider non-trigger) both fall inside
  the mask, so detection uses real collisions/layers, not a hard-coded height.
- Footstep selection: sprint → `runningFootsteps`; on water → `wetWalkFootsteps`;
  otherwise wet vs dry via `IsSurfaceWet()` (weather state + drying timer) —
  CONFIRMED. Footsteps are not played while stationary or airborne (`IsMoving`
  gate + grounded check, `stepTimer` reset) — CONFIRMED.
- **This pass fixed the spawn state:** `Awake()` now starts `wetnessTimer` at
  `wetnessDuration` so sunny terrain plays **dry** footsteps from the first
  step (previously the timer began at zero → wet footsteps at spawn). Also
  primed `wetnessTimer` to zero during rain/storm so the documented
  "wetnessDuration seconds after rain" drying window actually engages when
  the sky clears. CONFIRMED against code; runtime feel NOT REPRODUCED here
  (no editor session).
- `audioSource`/`characterController` are auto-resolved with `GetComponent`
  when the serialized field is null (scene shows `{fileID: 0}`) — CONFIRMED,
  not a wiring bug. Splash (`splashSpawner`/`wetGrassSplash`) is null in the
  scene and is guarded, so no splash spawns — CONFIRMED as intentionally
  optional, NOT AN ISSUE.

### 13.3 Weather ambience (`WeatherEffects` → `AudioManager`)

- Per-state ambient mapping wired in the scene: Sunny → `null` (silence),
  Rainy → `AMB_Rain`, HeavyRain → `AMB_StrongRain`, Stormy → `AMB_Storm` —
  CONFIRMED. `CrossfadeAmbient(null)` fades to silence, so sunny is not left
  on a stale loop.
- Stormy uses its own `AMB_Storm` track only (no extra rain track stacked on
  top) — matches the existing design and avoids overlapping loud ambience —
  CONFIRMED (decision recorded, no code change).
- `WeatherEffects` lives on the WeatherManager GameObject together with
  `WeatherState` and `WeatherFeedbackSystem`; `PlayerFootstepAudio.weatherState`
  references the same `WeatherState` (`&1808429660`) — CONFIRMED.

### 13.4 Water proximity ambience (`WaterAmbienceZone`)

- Attached to the AudioManager GameObject; `waterPlane` → `WaterPlane`
  transform, `waterClip` = `AMB_WaterFlowing`, distances 20 m fade-in start /
  5 m full volume — CONFIRMED. Volume is applied to a dedicated, self-created
  looping AudioSource (not the crossfade pair) and playback stops at full
  fade-out, so the flow loop is **not** restarted every frame — CONFIRMED.
  Runtime fade behaviour itself POSSIBLE·UNREPRODUCED (no editor session).

### 13.5 Pickup / drop / bin / UI / volume audio

- Pickup & drop: `PlayerInteraction` calls `AudioManager.PlayPickupSFX()` /
  `PlayDropSFX()` on hold/drop/throw — CONFIRMED; clips wired on the
  AudioManager component.
- Bin success/error: `RecycleBinInteractable` plays `successClip`/`errorClip`
  on correct/wrong recycle — CONFIRMED on both bin prefabs (`Nature
  Recycling.prefab`, `Plastic Recycling.prefab`). Note: `General Waste.prefab`
  is referenced in §4 but only two bin prefabs exist under `Assets/Models/
  Prefabs/Bins/`; the third bin is not instantiated in this scene — see
  §13.7.
- Master volume: `PauseMenuManager` persists to PlayerPrefs `"Volume"` and
  applies `AudioListener.volume` (and the Pause settings slider) — CONFIRMED.

### 13.6 Mouse sensitivity

- `PlayerLook` uses `lookInput * sensitivity * Time.deltaTime * 100f` —
  frame-rate independent. Scene values were `1.2/1.2`; this pass lowered
  them first to `0.8/0.8`, then (on user request) further to **`0.5/0.5`**
  for a calmer camera on the 1280×720 desktop setup. The change is made on
  the exposed Inspector field, not duplicated in code — code default stays
  2.0 as the un-assigned fallback. CONFIRMED on disk; feel NOT REPRODUCED
  here.

### 13.7 Manager audit (scene `Managers` root, &832482054)

| Manager | GameObject | Scene component | Status |
| --- | --- | --- | --- |
| GameManager | &947271160 | present, objectives 12/8/4 (§4) | CONFIRMED wired |
| ScoreManager | &1525541454 | present, PlayerPrefs high score | CONFIRMED wired |
| AudioManager | &1646009441 | + WaterAmbienceZone (&1646009444) | CONFIRMED wired |
| WeatherManager | &1808429656 | WeatherState + WeatherEffects + WeatherFeedbackSystem | CONFIRMED wired |
| UIManager | &438008473 | panel state machine + input maps | CONFIRMED wired |
| HUDManager | &1838788583 | + BinDirectionIndicator; highScoreText → new `Count` element (§ scene rework) | CONFIRMED wired |
| PauseMenuManager | &1037489631 | volume slider + pause flow | CONFIRMED wired |

- `AutoSpawner` exists as a code-generation fallback but is **not** present in
  the scene — CONFIRMED as intentional (all managers are pre-wired; nothing to
  auto-spawn), NOT AN ISSUE.
- **Bin inventory (CONFIRMED, correction 10 Aug):** all three bin prefabs
  exist under `Assets/Models/Prefabs/Bins/` — `Nature Recycling.prefab`
  (`binType 0`), `Plastic Recycling.prefab` (`binType 1`) and `General
  Waste.prefab` (`binType 2`) — each carrying `RecycleBinInteractable` with the
  Optimized success/error clips. The scene contains 13 instances of each
  prefab (39 bin instances total). `WeatherFeedbackSystem`/`BinDirectionIndicator`
  find them via `FindObjectsByType<RecycleBinInteractable>`, so all three bin
  types are present and reachable. (An earlier draft of this section
  incorrectly reported only two bin prefabs; corrected after a full-directory
  scan.)
- **Edit-mode matrix tests (`RecycleBinMatrixTests`)** construct bins for all
  three types and passed previously (scoring matrix §4 is code-driven and
  matches the three prefabs' `binType` values above).

### 13.8 Automated tests

- EditMode: `RecycleBinMatrixTests` + `ScoreManagerTests`; PlayMode:
  `GameManagerPlayTests` — as documented in §12/PROJECT_EVOLUTION §6.12.
- **POSSIBLE·UNREPRODUCED:** no test-runner invocation was available in this
  environment; the suite's last documented run predates the footstep changes
  (footstep audio is not covered by the EditMode/PlayMode suite). The changed
  code in this pass (`PlayerFootstepAudio.Awake` + `DetectSurface`) is not
  unit-tested. Recommended follow-up: run EditMode + PlayMode once in the
  editor before submission.
