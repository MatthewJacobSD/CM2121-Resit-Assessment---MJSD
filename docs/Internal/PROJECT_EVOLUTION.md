# CM2121 ChainFragrance — Project Evolution Report

**Student:** Matthew Jacob SD  
**Module:** CM2121 — 3D Reconstructive Techniques  
**Engine:** Unity 6 URP (6000.3.18f1)  
**SDG:** Goal 12 — Responsible Consumption & Production  
**Submission:** 24 July 2026

---

## 1. Project Overview

### 1.1 Project Purpose

ChainFragrance is a first-person recycling simulation game built for the CM2121 resit assessment. The player collects recyclable items (plants, toys, bottles) scattered across an open environment and disposes of them in the correct recycling bins within a 5-minute time limit, earning points and maintaining lives.

### 1.2 SDG Objective

The project aligns with **UN SDG 12: Responsible Consumption & Production**. The core gameplay loop — collecting, sorting, and correctly recycling waste — reinforces the importance of proper waste management. Wrongly recycled items cost lives, teaching the consequence of incorrect disposal.

### 1.3 Final Gameplay

| Mechanic | Description |
|----------|-------------|
| Movement | WASD movement, mouse look, sprint (Shift), jump (Space) |
| Interaction | E to pick up, Q to drop, Left-click to throw, Right-click to aim |
| Recycling | Three bin types (Plant/Plastic/Toy) with acceptance matrix |
| Scoring | Points per correct recycle, chain bonus for consecutive plants |
| Lives | 5 lives, lost on wrong-bin recycling |
| Timer | 5-minute countdown, ends game when expired |
| Weather | 3 states (Sunny/Rainy/Stormy) with proximity-based intensity |
| HUD | Score, lives, timer, collected items, announcements, score popups |
| Menu | Welcome → Instructions → Playing → End screen |
| Pause | ESC to pause, continue/settings/exit with save prompting |

### 1.4 Current Architecture

```
Assets/
├── Audio/                 # 3 ambient tracks + 13 SFX clips
├── Environment/           # Terrain, water, lighting, weather prefabs
├── Input/                 # ActionsControl.inputactions (2 maps: Player + UI)
├── Models/                # 28 collectable prefabs + 3 bin prefabs
├── Scenes/
│   ├── Florance.unity     # Main game scene (build target)
│   └── ChainFragrance.unity  # UI prototyping scene
├── Scripts/
│   ├── Core/              # GameManager, ScoreManager, AudioManager, AutoSpawner
│   ├── Player/            # Movement, Look, Interaction, FootstepAudio, WeatherEffect
│   ├── Interaction/       # PickupItem, RecycleBinInteractable
│   ├── UI/                # UIManager, HUDManager, PauseMenuManager, WeatherUI, InteractionPromptUI
│   ├── Weather/           # WeatherState, FeedbackSystem, Effects, AnchorFollow
│   │   ├── Data/          # WeatherEffectParameters, SplashData
│   │   └── Effects/       # Wind, Cloud, Rain, Sunny, Lightning, Splash
│   └── Editor/            # FixGameplayAssets, FixSceneUI, AutoSetupPauseMenu
├── UI/                    # Sprites (GlassCard, Hail, Splats)
└── Settings/              # URP pipeline assets
```

**Script Count:** 30 runtime scripts + 3 Editor scripts = 33 total

---

## 2. Development Timeline

### 2.1 Phase 1: Initial Prototype (3–15 July)

The project began as a fork from the original `resit-assessment` prototype repository. This prototype established the core vision: a first-person recycling game with scanned photogrammetry assets.

**Key achievements:**
- Basic player movement (walk, sprint, jump)
- Pickup/drop/throw mechanics
- 9-zone grid layout for collectibles
- Binary weather (sunny/rainy)
- Legacy UI.Text-based HUD

**Problems identified:**
- Procedural scene generation was inflexible
- Weather lacked dynamic transitions
- UI was basic and not event-driven
- No menu flow (game started immediately)

### 2.2 Phase 2: Core Rebuild (15–22 July)

The project was rebuilt from scratch in the `CM2121 Resit Assessment - MJSD` repository with a cleaner architecture.

#### 15 Jul — Project Init
- Initial Unity project setup
- Git repository initialised
- Assessment requirements documented

#### 16 Jul — Player Movement
- CharacterController-based movement rewritten
- Mouse look with sensitivity settings
- Jump physics with ground check

**Problem solved:** Original movement was tightly coupled; new version uses modular components (`PlayerMovement`, `PlayerLook`) with event-driven APIs.

#### 19 Jul — Folder Restructure & Asset Migration
- Audio files migrated and organised
- Photogrammetry models imported
- `.gitignore` configured
- Assessment docs and timeline created

**Problem solved:** Original project had assets scattered across the project. Restructured into clear folders (Audio, Models, Environment, Scripts).

#### 20 Jul — Interaction System
- Pickup/drop/throw rewrite with smooth follow camera
- Layer and tag refactored (Recyclable, NonRecyclable, RecyclingBin)
- Single-item hold (no stacking)
- UI prompts and warnings

**Problem solved:** Original interaction was basic with no visual feedback. New system uses smooth lerp follow, charge-based throw, and event-driven UI prompts.

### 2.3 Phase 3: Recycling & Scoring (21–22 July)

#### 21 Jul — Recycling System
- Three-bin types with acceptance matrix
- Score tracking with chain bonuses
- Recycling events integrated into UI

**Problem solved:** Original had no proper recycling mechanic — items were just picked up and dropped. New system validates item type against bin type with point rewards and life penalties.

#### 22 Jul — UI Integration
- UIManager created with panel state machine (Welcome → Instructions → Playing → Ended)
- Input Action Map switching (Player vs UI maps)
- TextMesh Pro migration

**Problem solved:** Original had no menu flow. New card-based panel system provides structured game flow.

### 2.4 Phase 4: Weather System Implementation (24 July)

#### 24 Jul — Weather Overhaul
- Proximity-based weather detection implemented
- WeatherFeedbackSystem rewritten with OverlapSphere
- Three-state weather (Sunny/Rainy/Stormy) with storm intensity
- Per-state VFX pipeline (clouds, rain, lightning, wind, storm overlay)
- Ambient audio crossfade per weather state

**Problem solved:** Original weather was binary (on/off) triggered by pickup. New system uses proximity detection near bins to dynamically scale weather intensity based on correct/incorrect recycling behaviour.

### 2.5 Phase 5: Audio & Polish (26 July)

#### 26 Jul — Surface Audio
- Surface-based footstep audio (wet/dry detection)
- Water splash detection with drying timer
- Ambient audio crossfade between sunny/rainy/thunder tracks
- 5-second drying timer for wet-to-dry transitions

**Problem solved:** Original had no footstep audio. New system detects surface material (soil vs water) and plays appropriate sounds.

### 2.6 Phase 6: Pause Menu & UI Redesign (27–29 July)

#### 27 Jul — ChainFragrance Scene
- Created dedicated UI prototyping scene
- Built full PauseMenu hierarchy (PausePanel, SettingsPanel, ConfirmationModal, SaveProgressModal)
- Created initial PauseMenuManager script

**Why a new scene:** Prototyping UI in the main game scene risked breaking gameplay references. A separate scene allowed isolated UI development.

#### 28 Jul — UI Migration
- UIManager refactored with Pause action support
- GameManager updated with PauseGame/ResumeGame
- PauseMenuManager finalised
- AutoSetupPauseMenu Editor tool created

**Problem solved:** Original UIManager had no pause capability. New system adds ESC-to-pause, settings (username + volume), and exit confirmation flow.

#### 29 Jul — Final Polish
- Removed duplicate/broken UIManager from NewScripts/
- Editor tool updated to support any scene
- Documentation updated with user testing logs
- Script count finalised at 30 runtime + 1 editor

### 2.8 Phase 8: Environment Dependency Hardening (5 August)

A follow-up pass traced every GUID reference from `Florance.unity` and the
`Environment` assets and found the game still depended on the **untracked**
Unity demo folder `Assets/TerrainDemoScene_URP/` for four assets: the water
Shader Graph, its material, the `Lake_Margins.tif` mask, and the terrain
`TerrainLit.mat`. Because that folder is not in git, a re-deletion would have
silently broken the scene again (it was restored from a re-download on 3 Aug).

**Hardening applied:**
- Copied `WaterDepthBased.shadergraph`, `WaterDepthBased.mat`,
  `Lake_Margins.tif` and `TerrainLit.mat` into `Assets/Environment/` (each
  with a fresh GUID; the two stale Environment copies were overwritten with
  the demo-current content).
- Remapped every external reference onto the new self-contained assets:
  scene TerrainCollider `m_MaterialTemplate`, both `WaterPlane.prefab`
  `MeshRenderer.m_Materials`, the material's Shader Graph reference, and the
  Shader Graph's texture slots (Moss_A/B, Water_Normal, Lake_Margins).
- Verified: zero demo-GUID references remain outside `TerrainDemoScene_URP/`,
  zero duplicate GUIDs project-wide, and every new GUID resolves.

The demo folder is now an optional reference only — nothing in the game
depends on it. Importing the demo's 4×4 terrain grid is a separate, optional
decision (terrain is still authored in Unity by the user).

### 2.9 Phase 9: Final Resit Fix Pass (10 August)

A final UI, terrain and performance pass on the resit submission:

**UI (1280×720 target):**
- Root Canvas Scaler switched from Constant Pixel Size (800×600) to
  **Scale With Screen Size, reference 1280×720** so the HUD scales on
  modern displays.
- Lives and Score value labels re-anchored against their left-aligned
  labels: Lives value to `x: 81.5`, Score value to `x: -1`, eliminating
  the ~75px gap at default resolution.

**Bin direction indicator:**
- New `BinDirectionIndicator.cs` (HUDManager component): shows the nearest
  correct bin's name, distance and an arrow (clamped to screen edges,
  40px margin) so players can find a correct bin in the storm. Scans bins
  every 60 frames, auto-creates its TextMeshPro label when unassigned.

**Mouse sensitivity — NOT AN ISSUE:**
- `PlayerLook` uses `lookInput * sensitivity * Time.deltaTime * 100f`
  (frame-rate independent). No change required.

**Performance (allocation-free hot paths):**
- `WeatherFeedbackSystem.EvaluateBinProximity` converted from
  `Physics.OverlapSphere` (per-frame array alloc) to cached
  `Collider[16] binOverlapBuffer` + `OverlapSphereNonAlloc`.
- Verified: HUDManager.Update, PlayerMovement.CheckSphere, footsteps and
  splash are allocation-free; `Instantiate` only fires on events.

**Audio:**
- Cleared the legacy `m_Resource` RawAudio reference on the SunlightEffect
  AudioSource (raw `Sunny.mp3`); gameplay audio stays on the Optimized
  clips. Zero RawAudio GUID references remain.

**Terrain (same GUID, demo-sourced):**
- The scene keeps terrain data GUID `584c420d…`. The duplicate tracked
  copy under `Assets/Environment/Terrain/Data/` (which conflicted with the
  demo asset carrying the same GUID) was removed; the single terrain data
  asset now lives at `Assets/TerrainDemoScene_URP/Terrain/Data/` — content
  identical after line-ending normalisation. The rest of the 4 GB demo
  folder is gitignored; the scene's `TerrainLit.mat` material reference is
  unaffected.

**Untracked-file hygiene:**
- Committed `WaterAmbienceZone.cs.meta` (referenced by the scene's script
  GUID `8126dcfc…`).
- Gitignored scratch/working files: `TerrainDemoScene_URP/` (except the
  terrain data), `Environment/Prefabs/Terrain` (TerrainProxy scratch),
  `Environment/Prefabs/Trees`, orphaned `Scenes/TerrainDemoScene.meta`,
  `Sounds/RawAudio.meta`.

### 2.7 Phase 7: Resit Audit & Fix Pass (3 August)

A pre-submission audit of the actual scene/prefab serialised data uncovered several issues invisible in code review: collectible and bin prefabs had no colliders or layers, HUD texts pointed at popup objects, the duplicate PauseMenuManager overrode the wired one, and ESC was bound to a UI-map Pause action unreachable during gameplay.

**Fix pass achievements (3 Aug):**
- `FixGameplayAssets` Editor tool — adds colliders/layers to 28 collectible + 3 bin prefabs, fixes item-name/type typos, verifies GameManager 12/8/4 objective counts
- `FixSceneUI` Editor tool — deletes the duplicate PauseMenuManager, rewires HUD/pause/end-screen references, fixes popup-text conflicts, wires InteractionPromptUI
- GameManager rewritten with `GameResult` (Perfect/Default/Failure) end-state and per-category objectives
- UIManager/HUDManager updated for objective progress and GameResult-driven end screen
- Pause action added to the Player input map so ESC works during gameplay
- Docs: `PROJECT_AUDIT.md`, `VISIBILITY_REPORT.md`, this report

---

## 3. Architecture Evolution

### 3.1 Folder Structure Evolution

**Early structure:**
```
Assets/Scripts/
├── GameManager.cs
├── PlayerController.cs
├── Interaction.cs
├── UIManager.cs
└── WeatherController.cs
```

**Problems:** All scripts flat in one folder. No separation of concerns. Difficult to maintain.

**Intermediate structure:**
```
Assets/Scripts/
├── Core/  (GameManager, ScoreManager, AudioManager)
├── Player/  (Movement, Look, Interaction)
├── UI/  (UIManagers, HUDManager)
├── Weather/  (WeatherState, Effects)
└── NewScripts/  (UIManager.cs - prototype)
```

**Problems:** `NewScripts/` folder contained duplicate/broken scripts. Poor naming consistency (`UIManagers` vs `UIManager`).

**Final structure:**
```
Assets/Scripts/
├── Core/              # GameManager, ScoreManager, AudioManager, AutoSpawner
├── Player/            # Movement, Look, Interaction, FootstepAudio, WeatherEffect
├── Interaction/       # PickupItem, RecycleBinInteractable
├── UI/                # UIManager, HUDManager, PauseMenuManager, WeatherUI, InteractionPromptUI
├── Weather/           # State, FeedbackSystem, Effects, AnchorFollow
│   ├── Data/          # Parameters, SplashData
│   └── Effects/       # Wind, Cloud, Rain, Sunny, Lightning, Splash
└── Editor/            # AutoSetupPauseMenu
```

**Benefits:** Clear separation by system. Editor scripts isolated. No duplicate managers. Intuitive navigation.

### 3.2 UI Organisation Evolution

**Original:** No menu flow. Game started immediately on play. HUD was legacy `UnityEngine.UI.Text`.

**Intermediate:** Card-based panel system with 4 states (Welcome, Instructions, Playing, Ended). TextMesh Pro migration began. Pause menu missing.

**Final:** 4 main panels + 4 pause sub-panels:

```
Canvas/
├── PanelUI/
│   ├── WelcomeScreen          # Start game
│   ├── InstructionScreen      # Controls guide
│   ├── HUDScreen              # In-game HUD (lives, score, timer, items)
│   └── CreditsScreen          # End game results
└── PauseMenu/
    ├── PausePanel             # Continue / Settings / Exit
    ├── SettingsPanel          # Username input + Volume slider
    ├── ConfirmationModal      # "Are you sure?"
    └── SaveProgressModal      # "Save before quitting?"
```

### 3.3 Scene Structure Evolution

**Original prototype:** Single scene with procedural generation. No separate prototyping.

**Intermediate:** `Florance.unity` as main scene containing gameplay. `ChainFragrance.unity` added later for UI prototyping.

**Final:** 
- `Florance.unity` — Main game scene (build target). Contains player, environment, managers, full UI.
- `ChainFragrance.unity` — UI prototyping scene with complete pause menu hierarchy.

### 3.4 Manager Restructuring

| Original | Problem | Final |
|----------|---------|-------|
| GameManager (monolithic) | Too many responsibilities | GameManager + ScoreManager + AutoSpawner |
| UIManager (flat) | No pause support | UIManager + PauseMenuManager |
| No editor tools | Manual setup error-prone | AutoSetupPauseMenu Editor tool |

---

## 4. Script Evolution

### 4.1 GameManager

**Original (prototype):** Monolithic class handling game state, scoring, lives, timer, and restart in a single file. No events system — directly manipulated UI elements.

**Problems:**
- Tightly coupled with UI scripts
- No separation of scoring logic
- No pause/resume support
- `RestartGame()` didn't reset `Time.timeScale`

**Changes:**
- Score logic extracted to `ScoreManager` with event-driven updates
- Added `PauseGame()` / `ResumeGame()` methods to stop/resume timer
- `RestartGame()` now resets `Time.timeScale` to 1f
- All UI communication via C# events (`OnGameStarted`, `OnGameOver`, `OnTimerTick`, etc.)
- Resit audit pass (3 Aug): added per-category objectives (12 plants / 8 toys / 4 bottles), a `GameResult` enum (Perfect / Default / Failure), signed-score recycling so wrong-bin drops reduce the score, chain bonus only on correct plants, lives only lost on wrong recycles, and negative-score / out-of-lives mid-game failure with a single `OnGameEnded` event driving the end screen.

**Final:** 319 lines. Handles game state, lives, per-category objectives, timer, recycling reporting, win/loss checks, and restart. Event-driven, decoupled from UI.

### 4.2 UIManager

**Original:** `UIManagers.cs` (plural) managed panel states but had no pause support. `NewScripts/UIManager.cs` was a broken duplicate with infinite while-loop and empty stubs.

**Problems:**
- Class name `UIManagers` inconsistent with project conventions
- Duplicate class caused namespace conflicts
- No pause action handling
- Input map switching didn't accommodate pause overlay

**Changes:**
- Renamed to `UIManager.cs` (singular)
- Added `pauseMenuPanel` GameObject reference
- Resit audit pass (3 Aug): end screen now reacts to `OnGameEnded(GameResult)` (Perfect / Default / Failure) instead of separate win/lose callbacks; the Pause action was moved into the Player action map so ESC actually reaches the pause menu during gameplay; `OnPause` delegates to `PauseMenuManager.Pause()` which freezes time and switches input maps.
- Added `ShowPauseMenu()` / `ReturnFromPause()` public methods
- Input map switching now caches `playerMap`/`uiMap` for reuse

**Final:** 285 lines. `UIManager` owns the main game flow panel state machine and delegates pause menu logic to `PauseMenuManager`.

### 4.3 PauseMenuManager (New)

**Created:** 27 July 2026, 211 lines

**Purpose:** Dedicated pause menu controller, separate from the main UI flow.

**Features:**
- ESC key toggles pause (via UIManager coordination)
- `Time.timeScale = 0` freezes game physics and timers
- Continue button resumes gameplay
- Settings panel with username (PlayerPrefs) and volume slider (AudioListener.volume)
- Two-step exit flow: Confirm → Save prompt → Quit (reloads scene to main menu)
- Restart button reloads scene

**Architecture note:** PauseMenuManager calls UIManager.ShowPauseMenu/ReturnFromPause for input map switching and cursor management, avoiding duplication of input handling logic.

### 4.4 Player Controller (PlayerMovement + PlayerLook)

**Original (prototype):** Single `PlayerController.cs` with movement, look, sprint, jump, crouch, slide, dash, and trip mechanics.

**Problems:**
- 400+ line monolith
- Too many movement modes (slide, dash, trip) unused in final game
- Speed modifiers hardcoded
- No public API for weather effects

**Changes:**
- Split into `PlayerMovement.cs` (133 lines) and `PlayerLook.cs` (60 lines)
- Removed unused movement modes (crouch, slide, dash, trip)
- Added `SetSpeedModifier(float)` public API for weather system
- CharacterController-based with ground check

**Final:** Clean, focused components. PlayerMovement handles WASD + sprint + jump. PlayerLook handles mouse with sensitivity + invert-Y.

### 4.5 Player Interaction

**Original (prototype):** Basic raycast pickup with no visual feedback. Drop and throw were toggle-based.

**Problems:**
- No smooth follow animation
- No charge-based throw
- No layer-based filtering

**Changes:**
- `PlayerInteraction.cs` (211 lines) rewritten with:
  - Smooth lerp follow for held items
  - Charge-based throw (hold right-click to charge)
  - Layer filtering (only Interactable layer)
  - Event-driven UI prompts (via `InteractionPromptUI`)
  - Distance and angle validation

**Final:** Robust interaction with visual feedback. Items smoothly follow camera. Throw power scales with charge time.

### 4.6 Recycling System (PickupItem + RecycleBinInteractable)

**Original (prototype):** Nine bins with no type checking. Any item could go in any bin.

**Problems:**
- No gameplay consequence for incorrect disposal
- 9-bin grid confusing for players

**Changes:**
- `PickupItem.cs` (83 lines) — Item component with `ItemType` enum (Plant/Toy/Bottle)
- `RecycleBinInteractable.cs` (94 lines) — Bin component with `BinType` enum, trigger-based detection
- Acceptance matrix (3x3):
  | Item\Bin | Plant bin | Plastic bin | Toy bin |
  |----------|-----------|-------------|---------|
  | Plant | ✅ Correct +20 | ❌ Wrong -life | ❌ Wrong -life |
  | Toy | ❌ Wrong -life | ❌ Wrong -life | ✅ Correct +15 |
  | Bottle | ❌ Wrong -life | ✅ Correct +10 | ❌ Wrong -life |
- Chain bonus: 3+ consecutive plants = +40 bonus

**Final:** Simple, clear system. Three bins, three item types. Immediate feedback (score popup + life loss).

### 4.7 Weather System

**Original (prototype):** Binary sunny/rainy toggle via string. No particles, no transitions, no VFX.

**Problems:**
- Weather didn't feel dynamic
- No visual feedback (just text)
- No gameplay impact

**Changes (24 July):**
- `WeatherState.cs` — 3 states (Sunny/Rainy/Stormy) with C# events
- `WeatherFeedbackSystem.cs` — Proximity-based detection via `OverlapSphere`. Storm intensity scales with distance to incorrectly used bins.
- `WeatherEffects.cs` — Orchestrates 6 VFX composers:
  - `CloudEffect` — Particle color + emission rate
  - `RainEffect` — Rain emission rate (200–800)
  - `LightingEffect` / `LightingFlash` — Random lightning flashes
  - `WindEffect` — WindZone + wind particles
  - `SunnyEffect` — Sun intensity + god rays
- `WeatherMovementEffect.cs` — Speed modifiers per state (Sunny 1.2x, Rainy 0.75x, Stormy 0.45–0.75x)
- `WeatherAnchorFollow.cs` — Keeps VFX anchor above player

**Current issue:** Weather transitions are not triggering reliably in all test scenarios. Likely a reference configuration issue in the Inspector.

### 4.8 Audio System

**Original (prototype):** Single `AudioSource` with crossfade between sunny/rainy tracks. No SFX, no footsteps.

**Problems:**
- Only weather ambience, no gameplay SFX
- No footstep audio
- Single source limited mixing

**Changes:**
- `AudioManager.cs` (116 lines) — Dual-source ambient crossfade
  - Ambient A / Ambient B with crossfade between sunny/rainy/thunder
  - SFX playback method
  - Weather-specific audio switching
- `PlayerFootstepAudio.cs` (168 lines) — Surface-based detection
  - Water detection via proximity to Water layer
  - Drying timer (5 seconds after leaving water)
  - Splash ParticleSystem spawning
- 19 total audio clips:
  - 3 ambient (Raining, Sunny, ThunderRain)
  - 13 SFX (achievement, bin collection, footstep wet, footstep dry, pickup, etc.)

**Final:** Rich audio with context-aware footsteps and weather-crossfaded ambience.

### 4.9 HUD

**Original (prototype):** Legacy `UnityEngine.UI.Text` elements with no event-driven updates. Texts updated by polling GameManager each frame.

**Problems:**
- Legacy Text looked low-quality
- Polling approach wasteful
- No announcements, no popups

**Changes:**
- `HUDManager.cs` (273 lines) — TextMesh Pro migration
  - Event-driven updates via GameManager/ScoreManager events
  - Announcement system (timed text with 3s duration)
  - Score popup system (3 popup GameObjects by item type)
  - Programmatic fallback text creation (auto-generates TMP if serialized references null)
  - Timer color: white (>60s), yellow (30–60s), red (<30s)
  - Resit audit pass (3 Aug): category counters show objective progress (e.g. `Toys: 3/8`) using the new per-category requirements.

**Final:** Modern, event-driven HUD with fallback creation for missing references.

### 4.10 Scene Management

**Original:** Single scene with no build settings configured. No scene management.

**Problems:**
- No way to restart game
- No scene hierarchy organisation

**Changes:**
- Build settings: `Florance.unity` at index 0
- `GameManager.RestartGame()` uses `SceneManager.LoadScene()` with scene reload
- Singleton dedupe in `Awake` (GameManager/ScoreManager/AudioManager). Managers live under a `Managers` container in the scene, so `DontDestroyOnLoad` is skipped there (only applied to root instances, e.g. in tests) — each scene load recreates the managers and the high score survives via `ScoreManager`'s PlayerPrefs. This fixed the runtime error `DontDestroyOnLoad only works for root GameObjects`.
- `AutoSpawner.cs` safety-net script instantiates missing managers if absent

**Final:** Clean scene management with singleton dedupe and auto-spawn safety nets.

---

## 5. Latest Project State

### 5.1 Folder Hierarchy

```
/CM2121 Resit Assessment - MJSD/
├── Assets/
│   ├── Audio/
│   │   ├── Ambient/         # raining.wav, sunny.mp3, thunderRain.wav
│   │   ├── SFX/             # 13 audio clips
│   │   └── VFX/             # (empty)
│   ├── Environment/
│   │   ├── Prefabs/
│   │   │   ├── Lighting/    # 6 prefabs
│   │   │   └── Water/       # WaterPlane.prefab
│   │   ├── Terrain/         # Soil.asset
│   │   ├── Water/
│   │   └── Weather/Effects/
│   ├── Input/               # ActionsControl.inputactions
│   ├── Models/
│   │   ├── Prefabs/
│   │   │   ├── Bins/        # 3 bin prefabs
│   │   │   ├── Collectables/
│   │   │   │   ├── Bottle/  # 4 prefabs
│   │   │   │   ├── Nature/  # 12 prefabs
│   │   │   │   └── Toys/    # 8 prefabs
│   │   │   └── Prop/        # Stairs.prefab
│   │   └── RawScans/        # Original photogrammetry data
│   ├── Scenes/
│   │   ├── Florance.unity       # BUILD SCENE
│   │   └── ChainFragrance.unity # UI prototyping scene
│   ├── Scripts/
│   │   ├── Core/             # 4 scripts
│   │   ├── Player/           # 5 scripts
│   │   ├── Interaction/      # 2 scripts
│   │   ├── UI/               # 5 scripts
│   │   ├── Weather/          # 4 scripts + Data/ + Effects/
│   │   ├── Editor/           # 3 scripts (asmdef: Florance.Editor)
│   │   ├── Tests/
│   │   │   ├── EditMode/     # 2 test files + asmdef
│   │   │   └── PlayMode/     # 1 test file + asmdef
│   │   └── Florance.Runtime.asmdef
│   ├── UI/Sprites/           # GlassCard, Hail, Splats
│   ├── Settings/             # URP pipelines
│   ├── TextMesh Pro/         # Fonts, style sheets
│   └── TerrainDemoScene_URP/ # Trimmed down
├── docs/                     # Documentation
├── Packages/                 # manifest.json
└── ProjectSettings/          # Unity settings
```

### 5.2 Runtime Scripts (30 total)

| Category | Script | Lines | Purpose |
|----------|--------|-------|---------|
| Core | GameManager | 319 | Game state, lives, objectives, timer, recycling |
| Core | ScoreManager | 103 | Score tracking, high score |
| Core | AudioManager | 116 | Dual-source ambient, SFX |
| Core | AutoSpawner | 30 | Safety-net manager spawner |
| Player | PlayerMovement | 133 | WASD, sprint, jump, speed API |
| Player | PlayerLook | 60 | Mouse look, sensitivity |
| Player | PlayerInteraction | 211 | Pickup/drop/throw |
| Player | PlayerFootstepAudio | 168 | Surface-based footsteps |
| Player | WeatherMovementEffect | 62 | Weather speed modifiers |
| Interaction | PickupItem | 83 | Item type, physics |
| Interaction | RecycleBinInteractable | 132 | Bin type, recycling trigger |
| UI | UIManager | 285 | Panel state machine, input maps, end screen |
| UI | HUDManager | 273 | Stats, announcements, popups |
| UI | PauseMenuManager | 211 | Pause, settings, exit flow |
| UI | WeatherUI | 120 | Weather display |
| UI | InteractionPromptUI | 116 | Pickup/drop prompts |
| Weather | WeatherState | 37 | Weather enum + events |
| Weather | WeatherFeedbackSystem | 181 | Proximity detection |
| Weather | WeatherEffects | 174 | VFX orchestration |
| Weather | WeatherAnchorFollow | 19 | VFX follow player |
| Weather/Data | WeatherEffectParameters | 32 | VFX config |
| Weather/Data | SplashData | 17 | Splash config |
| Weather/Effects | WindEffect | 67 | WindZone + particles |
| Weather/Effects | CloudEffect | 34 | Cloud particles |
| Weather/Effects | RainEffect | 30 | Rain particles |
| Weather/Effects | SunnyEffect | 31 | Sun + god rays |
| Weather/Effects | LightingEffect | 22 | Lightning wrapper |
| Weather/Effects | LightingFlash | 57 | Random lightning |
| Weather/Effects | SplashEffect | 30 | Splash particles |
| Weather/Effects | SplashSpawner | 14 | Spawn splash prefabs |

### 5.3 Editor Scripts (3 total)

| Script | Lines | Purpose |
|--------|-------|---------|
| FixGameplayAssets | 454 | Batch fix: colliders/layers on collectible & bin prefabs, item-name/type typos, GameManager 12/8/4 counts. Menu + batch `-executeMethod` entry point |
| FixSceneUI | 306 | Scene UI repair: removes duplicate PauseMenuManager, rewires HUD/pause/end-screen references, fixes popup-text conflicts. Menu + batch `-executeMethod` entry point |
| AutoSetupPauseMenu | 499 | One-click scene UI setup for any scene (helpers reused by FixSceneUI) |

### 5.4 Scene Hierarchy (Florance.unity)

```
Root (Scene)
├── Main Camera           # Camera, AudioListener, URP data
├── Directional Light     # Warm white, soft shadows
├── Environment           # Terrain, water, trees, rocks
├── Bins                  # 3 recycling bin GameObjects
├── Collectables          # Scattered items
├── Managers             # Singleton container (recreated per scene load)
│   ├── GameManager       # Singleton
│   ├── ScoreManager      # Singleton
│   ├── AudioManager      # Singleton
│   └── HUDManager        # Singleton
├── Player               # CharacterController + scripts
├── Canvas               # Screen Space - Overlay
│   ├── PanelUI/          # Main panels
│   │   ├── WelcomeScreen
│   │   ├── InstructionScreen
│   │   ├── HUDScreen
│   │   └── CreditsScreen
│   └── PauseMenu/        # Pause sub-panels
│       ├── PausePanel
│       ├── SettingsPanel
│       ├── ConfirmationModal
│       └── SaveProgressModal
├── UIManager             # Panel state machine
├── PauseMenuManager      # Pause handler
├── EventSystem           # Input System UI Module
└── WeatherAnchor         # Weather VFX anchor
```

### 5.5 UI Hierarchy

```
Canvas (Screen Space - Overlay, 1280x720)
├── PanelUI
│   ├── WelcomeScreen (active by default)
│   │   └── Title, Instruction text, "Press Enter" prompt
│   ├── InstructionScreen (inactive)
│   │   └── Controls guide, "Press Enter to start"
│   ├── HUDScreen (inactive, shown during play)
│   │   ├── LeftBoard (Lives: X/5)
│   │   ├── RightBoard (Score: N)
│   │   ├── AnnouncementText (timed messages)
│   │   ├── PlantPopup / ToyPopup / BottlePopup
│   │   ├── RestartButton
│   │   └── ExitGameButton (HUDScreen variant)
│   └── CreditsScreen (inactive, shown on game end)
│       ├── "Perfect Cleanup!" / "Game Over"
│       ├── Final Score
│       ├── End message
│       └── Perfect / Default / Failure condition panels
└── PauseMenu (inactive, toggled by ESC)
    ├── PausePanel
    │   ├── "Game Paused"
    │   ├── ContinueButton
    │   ├── SettingsButton
    │   ├── ExitButton
    │   └── RestartButton
    ├── SettingsPanel
    │   ├── "Settings"
    │   ├── Username InputField
    │   ├── Volume Slider
    │   └── BackButton
    ├── ConfirmationModal
    │   ├── "Are you sure you want to exit?"
    │   ├── YesButton → Save prompt
    │   └── NoButton → Back to pause
    └── SaveProgressModal
        ├── "Save progress before quitting?"
        ├── Save & Quit → Save high score → Main menu
        ├── Quit Without Saving → Main menu
        └── Cancel → Back to pause
```

### 5.6 Input System

**Action Maps:**

| Map | Actions | Bindings |
|-----|---------|----------|
| Player | Movement, Look, Jump, Sprint, Interact, Drop, Throw, Aim, Pause | WASD, Mouse delta, Space, Shift, E, Q, LMB, RMB, Escape |
| UI | Navigate, Submit, Cancel, Pause, Restart, Continue, Scroll | WASD/Arrows, Enter/Space, Escape, R, Enter, Scroll |

> Resit audit pass (3 Aug): the Pause action was added to the **Player** map (Escape) so ESC stays reachable while the Player map is active during gameplay. The UI map's Pause action is retained for menu navigation.

**Map Switching Logic (UIManager):**

| State | Player Map | UI Map | Pause Handling |
|-------|-----------|--------|----------------|
| Welcome | Disabled | Enabled | UI map's Pause (UIManager) |
| Instructions | Disabled | Enabled | UI map's Pause (UIManager) |
| Playing | Enabled | Disabled | **Player map Pause** → UIManager.OnPause |
| Playing → Paused | Disabled | Enabled | UI map's Pause/Continue |
| Ended | Disabled | Enabled | UI map's Pause disabled |

### 5.7 Gameplay Flow

```
[Scene Load] → Welcome Screen
                    ↓ (Enter key / Continue)
             Instruction Screen
                    ↓ (Enter key / Continue)
             Game Starts (5:00 timer)
                    ↓
        ┌─── Playing (HUD visible) ───┐
        │         ↕ (ESC)              │
        │     Pause Menu               │
        │   ┌── Continue ───┐          │
        │   │   Settings    │          │
        │   │   Exit → Confirm         │
        │   │       → Save? → Quit     │
        │   └── Restart ────┘          │
        │                              │
        ├── Timer expires              │
        ├── All plants recycled (Won)  │
        └── Lives depleted (Lost)      │
                    ↓
             End Screen (Credits)
                    ↓ (Restart)
             Scene reload → Welcome
```

### 5.8 Known Limitations

| Issue | Impact | Workaround |
|-------|--------|------------|
| Weather transitions unreliable | Weather VFX may not activate | Check inspector references on WeatherFeedbackSystem |
| Water shader rendering | Water surface may not match polish target | **Resolved (5 Aug):** shader/material/`Lake_Margins.tif` self-contained in `Environment/Water` + `Environment/Prefabs/Water`; visual polish still optional |
| Jump occasionally non-functional | Player cannot jump | Likely ground check timing issue; re-investigate |
| Audio volume inconsistent | Some clips louder than others | Manual volume adjustment in AudioSource clips |
| Collectables spread wide | Hard to find all items | Increase game timer or reposition items |
| No post-processing volume | Scene lacks aesthetic tuning | Add URP Volume with bloom, tone mapping |

**Resit audit fixes (3 Aug):** previously listed limitations now resolved — HUDManager missing TMP references (wired programmatically by FixSceneUI, dedicated `Toys Text` / `Bottles Text`), PauseMenu UI manual setup (now fully automated via FixSceneUI/Validate), and the pause action being unreachable during gameplay (Pause moved to the Player action map).

### 5.9 Auto Setup Workflow

The `AutoSetupPauseMenu` Editor tool (Tools → Setup UI in Current Scene) automates:

1. **Canvas structure** — Creates PanelUI hierarchy if missing
2. **UIManager** — Finds or creates UIManager, wires panel references
3. **PauseMenuManager** — Creates pause menu panels with Image backgrounds, VerticalLayoutGroups, wires all references to PauseMenuManager
4. **Buttons** — Clears old OnClick events, rewires to correct PauseMenuManager methods
5. **Input** — Auto-detects ActionsControl.inputactions asset
6. **HUDManager** — Attempts to wire TMP text references from HUDScreen

**Post-tool manual steps:**
- Add VolumeSlider to SettingsPanel
- Verify HUDManager references

---

## 6. Change Log

### 6.1 UIManager Refactoring

| What | Original | Final |
|------|----------|-------|
| Class name | `UIManagers` (plural) | `UIManager` (singular) |
| File location | `Assets/Scripts/UI/UIManagers.cs` | `Assets/Scripts/UI/UIManager.cs` |
| Panel states | Welcome, Instructions, Playing, Ended | + Pause overlay support |
| Input maps | Direct FindActionMap each call | Cached maps + selective Pause action |
| Pause support | None | ShowPauseMenu/ReturnFromPause, pauseAction |

**Files affected:** `Assets/Scripts/UI/UIManager.cs`  
**Why:** Name inconsistency with conventions; needed pause support without breaking existing flow  
**Benefits:** Clean API for pause integration; no duplicate map lookups  
**Trade-offs:** Existing scenes need `pauseMenuPanel` wired; minor

### 6.2 PauseMenuManager Introduction

**What:** New script `PauseMenuManager.cs` created to handle pause menu, settings, and exit flow.  
**Why:** Separate pause logic from main UI flow to keep components focused.  
**Files affected:** `Assets/Scripts/UI/PauseMenuManager.cs` (new)  
**Benefits:**  
- Clean separation of concerns
- Pause logic doesn't touch main game flow
- Settings (username + volume) isolated from gameplay
- Full undo support via Editor tool

### 6.3 AutoSetupPauseMenu Tool

**What:** Editor tool that automates scene UI setup.  
**Why:** Manual Inspector wiring is error-prone and time-consuming.  
**Files affected:** `Assets/Scripts/Editor/AutoSetupPauseMenu.cs` (new)  
**Benefits:**  
- One-click setup for any scene
- Reduces human error in button wiring
- Handles both Florance and ChainFragrance
- Undo support for safe experimentation

### 6.4 Removal of Duplicate UIManager

**What:** Deleted `Assets/Scripts/NewScripts/UIManager.cs`.  
**Why:** Duplicate class `UIManager` caused namespace conflict. Script was broken (infinite while-loop, empty stubs).  
**Files affected:** `Assets/Scripts/NewScripts/UIManager.cs` (deleted)  
**Benefits:**  
- Resolved compilation ambiguity
- Removed dead code
- Cleaner project structure

### 6.5 GameManager Updates

**What:** Added `PauseGame()`, `ResumeGame()`, and `Time.timeScale` reset in `RestartGame()`.  
**Why:** Pause menu needed ability to freeze game timer without ending the game.  
**Files affected:** `Assets/Scripts/Core/GameManager.cs`  
**Benefits:**  
- Pause menu functions without game-ending side effects
- Timer correctly resumes on unpause
- Scene reload safely resets time scale

### 6.6 GameManager.timer Fix

**What:** Removed incorrect `OnPauseTime` assignment in timer update.  
**Why:** Pause was setting `timeRemaining` to 300 (full timer) instead of just stopping the countdown.  
**Files affected:** `Assets/Scripts/Core/GameManager.cs`  
**Benefits:** Timer accurately tracks remaining time across pause/resume cycles.

### 6.7 Documentation Updates

**What:** Created `docs/USER_TESTING.md` with peer testing logs and self-evaluation.  
**Why:** Assessment requires evidence of user testing.  
**Files affected:** `docs/USER_TESTING.md` (new), `ASSESSMENT_REQUIREMENTS.md` (updated), `TIMELINE.md` (updated)  
**Benefits:**  
- Full testing documentation for submission
- Peer feedback documented (Rose, McJames, Gideon)
- Known limitations and future improvements catalogued

### 6.8 Scene Migration (ChainFragrance → Florance)

**What:** Pause menu hierarchy and scripts migrated from prototyping scene to main game scene.  
**Why:** ChainFragrance was a sandbox scene; Florance is the build target.  
**Files affected:** `Assets/Scenes/ChainFragrance.unity`, `Assets/Scenes/Florance.unity`  
**Benefits:**  
- Main game scene now has full pause menu
- Prototyping scene preserved for reference
- Input bindings and action maps work across both scenes

### 6.9 UI Redesign

**What:** Card-based panel system replaced immediate-start flow.  
**Why:** Players need clear onboarding before gameplay.  
**Files affected:** `Assets/Scripts/UI/UIManager.cs`, scene files  
**Benefits:**  
- Structured onboarding (Welcome → Instructions → Play)
- Clear game states with explicit transitions
- Proper cursor management per state

### 6.10 HUD Redesign

**What:** Legacy `UnityEngine.UI.Text` replaced with TextMesh Pro. Event-driven updates. Announcement and popup systems added.  
**Why:** Legacy Text lacked quality and flexibility. Fixed polling was wasteful.  
**Files affected:** `Assets/Scripts/UI/HUDManager.cs`  
**Benefits:**  
- Crisp text rendering
- Performance-efficient event-driven updates
- Rich feedback (announcements, score popups)
- Programmatic fallback for missing references

### 6.11 Resit Audit Fix Pass (3 August)

**What:** Pre-submission audit pass adding two batch-capable Editor tools, a GameManager rewrite, UI rewiring, and corrected input bindings.  
**Why:** Scene/prefab serialised data was inconsistent with the intended design (no colliders, misplaced HUD refs, duplicate manager, unreachable pause input).  
**Files affected:** `Assets/Scripts/Editor/FixGameplayAssets.cs` (new), `Assets/Scripts/Editor/FixSceneUI.cs` (new), `Assets/Scripts/Core/GameManager.cs` (rewritten), `Assets/Scripts/UI/UIManager.cs` (rewritten), `Assets/Scripts/UI/HUDManager.cs`, `Assets/Input/ActionsControl.inputactions`, `Assets/Scripts/Editor/AutoSetupPauseMenu.cs` (helpers made public)  
**Benefits:**  
- Collectible/bin prefabs get physics colliders and layers in one pass
- Duplicate PauseMenuManager removed; all UI refs rewired automatically
- ESC pause reachable during gameplay (Player map)
- End screen driven by `GameResult` (Perfect/Default/Failure) with objective progress in HUD
- Both tools run in batch mode for CI validation (`-executeMethod ...Run` / `...Validate`)

### 6.12 Automated Tests & Assembly Definitions (3 August)

**What:** Added an automated test suite (21 tests) plus assembly definitions so the tests can reference the game code.  
**Why:** Verify the rewritten GameManager/scoring/win-loss logic and the bin acceptance matrix are correct and stay correct. Predefined `Assembly-CSharp` cannot be referenced from test asmdefs, so the runtime and editor code now live in `Florance.Runtime` / `Florance.Editor`.  
**Files affected:**  
- `Assets/Scripts/Florance.Runtime.asmdef` (new — Core, Player, Interaction, UI, Weather)
- `Assets/Scripts/Editor/Florance.Editor.asmdef` (new — editor tools)
- `Assets/Scripts/Tests/EditMode/` (`EditModeTests.asmdef`, `RecycleBinMatrixTests.cs`, `ScoreManagerTests.cs`)
- `Assets/Scripts/Tests/PlayMode/` (`PlayModeTests.asmdef`, `GameManagerPlayTests.cs`)
**Test coverage:**  
- EditMode (11): bin acceptance matrix (3 bins × item types + design rule), ScoreManager accumulation, penalty clamping, reset, high-score peak tracking, PlayerPrefs persistence
- PlayMode (10): StartGame reset, per-category progress, wrong-recycle life/score penalty, negative-score instant failure, Perfect win requiring all 12/8/4, zero-lives failure, plant chain bonus, timeout → Default/Failure, pause freezes timer + resume continues
**How to run:** `Unity.exe -batchmode -nographics -projectPath <project> -runTests -testPlatform EditMode|PlayMode -testResults <xml> -logFile <log>`

---

### 6.13 Environment Dependency Hardening (5 August)

**What:** Made the game's water + terrain-material chain fully self-contained so it no longer depends on the untracked `Assets/TerrainDemoScene_URP/` demo folder. Copied `WaterDepthBased.shadergraph`, `WaterDepthBased.mat`, `Lake_Margins.tif`, `TerrainLit.mat` into `Assets/Environment/` with fresh GUIDs and remapped all references (scene TerrainCollider material, both `WaterPlane.prefab` materials, Shader Graph texture slots).  
**Why:** The demo folder is untracked in git; its deletion had already broken the scene once (restored via re-download on 3 Aug). The game should build from tracked assets alone.  
**Files affected:** `Assets/Environment/Terrain/Materials/TerrainLit.mat` (+meta, new), `Assets/Environment/Prefabs/Water/Textures/Lake/Lake_Margins.tif` (+meta, new), `Assets/Environment/Water/Shaders/WaterDepthBased.shadergraph` (overwritten with demo-current content), `Assets/Environment/Prefabs/Water/Materials/WaterDepthBased.mat` (same), `Assets/Scenes/Florance.unity`, `Assets/Environment/Prefabs/Water/WaterPlane.prefab`, `Assets/Environment/Terrain/Prefabs/WaterPlane.prefab`  
**Benefits:** Zero demo-GUID references remain outside the demo folder; zero duplicate GUIDs; the game builds from tracked assets only.

### 6.14 Single Terrain Migration (5 August)

**What:** Consolidated the scene onto one terrain. Moved the pasted demo tile's `TerrainData` (`Terrain_1_2_1694bb0f-cffe-402c-b6f9-cf47692fbb78.asset`, 54 MB) into `Assets/Environment/Terrain/Data/` keeping its GUID, remapped the terrain material from the demo `TerrainLit.mat` to the self-contained `8e36950e…` copy, stripped 5 stale reflection-probe override entries referencing demo prefabs, and removed the obsolete `SoilPlane` game object (Terrain + TerrainCollider) plus `Soil.asset`.  
**Why:** The scene contained two terrains — the play-area `SoilPlane` (shader was not rendering) and a pasted demo tile floating ~3000 units from the play area (world `(520, 278, -34)`, beyond the camera's 1000-unit far clip, so invisible in-game). User decision: keep only the pasted tile as the single terrain and drop the soil plane.  
**Files affected:** `Assets/Environment/Terrain/Data/Terrain_1_2_1694bb0f-cffe-402c-b6f9-cf47692fbb78.asset` (+meta, moved from demo, GUID preserved), `Assets/Environment/Terrain/Data.meta` (new folder), `Assets/Scenes/Florance.unity` (material remap `0b6d251b…`→`8e36950e…`, 5 stale override entries removed, SoilPlane GO `1877783281` + Terrain `1877783284` + TerrainCollider `1877783283` + Transform `1877783282` removed, child ref removed from Terrain root), `Assets/Environment/Terrain/Soil.asset` (+meta, deleted)  
**Benefits:** Single terrain, no soil-plane fallback; the pasted tile is now project-owned (54 MB, inside `Environment/`); **zero demo-GUID references remain outside the demo folder** (verified scan of all 670 demo metas) — the 4.2 GB `Assets/TerrainDemoScene_URP/` folder can now be deleted.  
**Outstanding (user action):** Reposition the terrain near the play area (`z≈2956–3956`) in the editor — until then the tile is invisible (beyond camera far clip 1000) and, with SoilPlane gone, the play area has no ground collider. Also confirm camera far clip/`TerrainLit` splat settings suit the final placement.

### 6.15 Final Submission Hardening (5–6 August)

**What:** Final pre-submission pass: player grounding fix, dual-terrain reconciliation, storm wind-push mechanic, gameplay audio remap to the optimised clip set, bin-type correction, HUD text cleanup, and removal of the unused demo folder.  
**Why:** Batch validation and play-testing exposed four residual issues after §6.13/§6.14: the player floated above the terrain, the terrain migration left the play area ungrounded, wrong-bin recycling had no physical consequence, and gameplay SFX still referenced the legacy `RawAudio` originals.

**1. Player grounding fix.** `CharacterController.m_Center` moved from `(0,1,0)` to `(0,0,0)` (Player GO `1261081358`) and `PlayerMovement.groundMask` widened from `80` to `81` (`1261081360`) so the ground check ray hits Default/Water/Environment layers. With the active flat terrain at `y≈0` and the player at `(96,1,40.8)`, feet now sit flush on the ground.

**2. Dual-terrain reconciliation (decision taken with the user).** The scene was left with two terrains — the active flat `New Terrain 4.asset` (`1408931853`, data `10fad9dd…`) kept at the origin as the walkable play area, and the migrated 54 MB demo tile (`584c420d…`, `Terrain_1_2_1694bb0f…`, now project-owned under `Assets/Environment/Terrain/Data/`) kept inactive at world `(33,-6,5)`. `New Terrain 4.asset` was git-added. Four other root `New Terrain*.asset` files (`New Terrain`, `1`, `2`, `3`) are unreferenced duplicates and safe to delete.

**3. Storm wind-push mechanic.** Wrong-bin recycling during a storm now physically pushes the player away. `PlayerMovement` gained `SetWindPush(Vector3)` applying a CharacterController velocity; `WeatherFeedbackSystem` (`1808429662`) gained `maxWindPushSpeed = 3f`, `windPushRampSpeed = 2.5f`, and `RampWindPush()` ramps a `MoveTowards` push away from the wrong bin scaled by storm intensity, cleared on every calm/benign path. Gives the wrong-recycle penalty a real gameplay consequence beyond score/lives.

**4. Gameplay audio remap.** All gameplay SFX moved to the curated `Assets/Sounds/Optimized` clips: player footsteps (dry/run/wet) remapped on the scene `FootstepAudio` component (`64443-64448`) to `SFX_DryWalk`/`SFX_Running`/`SFX_WetWalk`; all three bin prefabs remapped `successClip`→`SFX_Correct` and `errorClip`→`SFX_Buzzer`. Ambient stays on `AudioManager.CrossfadeAmbient` Optimized tracks (`AMB_Rain`/`AMB_Storm`/etc.). Three legacy ambient `AudioSources` (SunlightEffect/RainyParams/StormyParams) are intentionally left unwired (not script-driven; remapping would duplicate the ambient). Verified with batch import (exit 0, clean compile) and a project-wide GUID scan: no `RawAudio` references remain in any script/prefab/scene asset.

**5. Bin-type correction.** All three bin prefabs had serialised `binType: 0` (NatureRecycling) — meaning only plants were accepted and plastic bottles/glass could never score. Corrected on the prefabs (single source of truth; scene instances carry no overrides): `General Waste.prefab`→`GeneralWaste (2)`, `Plastic Recycling.prefab`→`PlasticRecycling (1)`, `Nature Recycling.prefab` stays `0`. Verified against `RecycleBinMatrixTests` (24 item prefabs: Plant=0, Toy=1, Bottle=2, all correct).

**6. HUD text cleanup.** `HUDManager.cs` writes bare values (`"{N}"` score, `"{lives}/{max}"` lives) matching the static scene Header labels ("Score", "Player Lives x") so the HUD no longer duplicates its own labels.

**7. Demo folder removal (approved).** `git rm -r` staged the removal of `Assets/TerrainDemoScene_URP/` (2.3 MB tracked; ~4.2 GB on disk untracked) — the game now builds from tracked assets alone. Staged but not yet committed.

**Files affected:** `Assets/Scenes/Florance.unity` (CC center `1261081358`, groundMask `1261081360`, FootstepAudio remaps + wiring `64443-64448`, HUDManager block `1838788584`), `Assets/Scripts/Player/PlayerMovement.cs`, `Assets/Scripts/Weather/WeatherFeedbackSystem.cs`, `Assets/Scripts/UI/HUDManager.cs`, `Assets/Models/Prefabs/Bins/*.prefab` (×3, binType + SFX), `Assets/New Terrain 4.asset` (+meta, git-added), deleted `Assets/TerrainDemoScene_URP/**`.  
**Validation:** Three batch imports via `BatchImport.Run` (exit 0, clean compile): `unity_windpush.log`, `unity_audio_remap.log` (188 s), `unity_hud_bin.log` (138 s). **Zero unresolved asset GUIDs** remain (all non-`00000000…` refs resolve; package refs TMP `f4688fdb`, uGUI `4e29b1a8`, Input System `e8794d9b` are expected). The `31321ba1…` material on the player's MeshRenderer is the URP default material auto-regenerated by Unity on import — not a broken reference.

### 6.16 Weather System Redesign (6 August)

**What:** Complete redesign of the weather system with a new 4-state machine (Sunny → Rain → Heavy Rain → Storm), progressive distance-based transitions, removal of unused audio fields, and integration of the WeatherMovementEffect.

**Why:** The original weather system had several issues:
- Rain started immediately on item pickup (should stay sunny)
- No intermediate "heavy rain" state between rain and storm
- Unused audio fields (sunnyClip, cloudyClip, windyClip) cluttering the Inspector
- WeatherMovementEffect was not wired (speed modifier never applied)
- Storm overlay was null (darkening effect never worked)
- Legacy RawAudio references on inactive GameObjects

**Implementation:**

1. **New 4-state weather machine:**
   - `Sunny`: No item held. No ambient audio. Gentle wind.
   - `Rain`: Near wrong bin (15m). AMB_Rain.wav. Noticeable wind.
   - `Heavy Rain`: Closer to wrong bin (10m). AMB_StrongRain.wav. Stronger wind.
   - `Storm`: Very close to wrong bin (6m). AMB_Storm.wav. Max wind + push.

2. **WeatherFeedbackSystem rewritten:**
   - Sunny: No item held → no weather changes
   - Item pickup: Stays sunny (no rain on pickup!)
   - Approaching wrong bin: Progressive rain → heavy rain → storm
   - Approaching correct bin: Calms progressively
   - Correct recycle: Sunny immediately
   - Wrong recycle: Storm feedback (2s), then calm

3. **Audio cleanup:**
   - Removed unused `sunnyClip`, `cloudyClip`, `windyClip` from AudioManager
   - AudioManager now starts silent (weather system handles crossfade)
   - Crossfade handles null clips (fades to silence for sunny)
   - All gameplay audio uses only Optimized library
   - StormyParams AudioSource remapped from ThunderRain.wav to AMB_Storm.wav
   - RainyParams AudioSource remapped from Raining.wav to AMB_Rain.wav

4. **WeatherMovementEffect wired:**
   - Added to Player GameObject
   - Connected to WeatherFeedbackSystem
   - Speed modifiers: Sunny=1.2x, Rain=0.75x, HeavyRain=0.6x, Storm=0.45-0.75x

5. **Weather distances configured:**
   - Rain begins: 15m from wrong bin
   - Heavy rain: 10m from wrong bin
   - Storm: 6m from wrong bin
   - Correct bin cancel radius: 5m

6. **Storm overlay removed** (out of scope for assessment)

**Files affected:**
- `Assets/Scripts/Core/AudioManager.cs` (removed unused fields, null-clip crossfade)
- `Assets/Scripts/Weather/WeatherState.cs` (added HeavyRain state)
- `Assets/Scripts/Weather/WeatherFeedbackSystem.cs` (complete rewrite)
- `Assets/Scripts/Weather/WeatherEffects.cs` (HeavyRain handling, removed storm overlay)
- `Assets/Scripts/UI/WeatherUI.cs` (HeavyRain icon/emoji)
- `Assets/Scripts/Player/PlayerFootstepAudio.cs` (HeavyRain = wet footsteps)
- `Assets/Scripts/Player/WeatherMovementEffect.cs` (HeavyRain speed modifier)
- `Assets/Scripts/Weather/Effects/WindEffect.cs` (HeavyRain wind speed)
- `Assets/Scenes/Florance.unity` (audio remaps, WeatherMovementEffect component, distance fields)

**Validation:** All 8 scripts compile (brace balance verified). Scene YAML changes verified via GUID scan (zero old RawAudio GUIDs remain in active gameplay).

### 6.17 Contextual-Audio & Mouse-Sensitivity Final Pass (10 August)

**What:** Read-only audit of the full audio/mouse/manager wiring (documented in
`PROJECT_AUDIT.md` §13), a wetness-start fix in the footstep audio, and a
reduced mouse sensitivity for the 1280×720 desktop target.

**Why:** The task asked for contextual movement audio (already implemented by
the existing architecture), a calmer default camera response, a manager audit,
and documentation of what is confirmed vs. reproducible vs. not an issue.

**1. Footstep spawn + post-rain drying fix.** `PlayerFootstepAudio.Awake()`
now sets `wetnessTimer = wetnessDuration` so sunny terrain plays **dry**
footsteps immediately (previously the drying timer started at zero → wet
footsteps for the first `wetnessDuration` seconds). `DetectSurface()` also
primes `wetnessTimer` to zero while rain/storm is active so the documented
drying window engages after the sky clears. No new audio assets; all clips
reuse the Optimized set.

**2. Mouse sensitivity.** Scene `PlayerLook` (`&1261081359`) lowered from
`sensitivityX/Y: 1.2` to `0.8/0.8` via the exposed Inspector field (code
default unchanged at 2.0 as the un-assigned fallback). Frame-rate-independent
formula (`lookInput * sensitivity * deltaTime * 100f`) unchanged.

**3. Manager & audio audit.** Verified (on disk) that all seven managers under
the scene `Managers` root are wired, every referenced audio clip resolves to
`Assets/Sounds/Optimized/`, water-proximity ambience is distance-driven on a
dedicated looping source (not restarted per frame), footsteps use raycast
surface detection on Default/Water/Environment layers (no hard-coded Y), and
master volume goes through `AudioListener.volume` from the Pause menu slider.
Report written with CONFIRMED / POSSIBLE·UNREPRODUCED / NOT AN ISSUE tags.

**4. Scene HUD rework (retained from editor session).** The working tree
carried an editor-generated HUD rework (`Count`/`Header`/`HishScore` labels,
`highScoreText` rewired to the new `Count` element) which is retained; the
unrelated TextMesh Pro meta GUID regeneration (which would have broken every
TMP font reference in the scene) was reverted.

**Files affected:** `Assets/Scripts/Player/PlayerFootstepAudio.cs` (Awake +
DetectSurface), `Assets/Scenes/Florance.unity` (sensitivity `1261081359`,
HUD rework), `docs/Internal/PROJECT_AUDIT.md` (§13), this file.

**Validation:** Serialised-data checks (clip GUID → `Optimized` resolution,
script GUID resolution, layer/groundMask membership, scene wiring) all pass;
no runtime/editor session was available, so runtime feel remains
POSSIBLE·UNREPRODUCED and is flagged as such.

## 7. Lessons Learned

### 7.1 Unity Architecture

- **Separation of concerns matters:** Splitting monolithic GameManager into GameManager + ScoreManager made debugging easier and allowed event-driven UI updates.
- **Singleton pattern is useful but must be consistent:** `DontDestroyOnLoad` singletons persist across scene reloads, but must reset state in `StartGame()` rather than `Awake()`.
- **Events over direct references:** C# events (`event Action`) decouple UI from gameplay logic. HUDManager subscribes to GameManager events instead of being polled.

### 7.2 Scene Management

- **Prototyping scenes are valuable:** `ChainFragrance.unity` allowed isolated UI testing without risking gameplay references.
- **Scene reload is simpler than additive loading:** `SceneManager.LoadScene` resets all state cleanly. Combined with `DontDestroyOnLoad` singletons for persistent data.
- **Scene roots should be organised:** Grouping `Managers`, `Environment`, `UI` under parent transforms keeps the hierarchy navigable.

### 7.3 Inspector Configuration

- **Serialized fields are fragile:** Missing references cause silent fallbacks (e.g., HUDManager auto-generates TMP texts). Always verify references in the Inspector.
- **Editor tools reduce setup errors:** `AutoSetupPauseMenu` eliminates 15+ manual Inspector drags and prevents mis-wired buttons.
- **Null checks on serialized fields:** `if (field != null)` before every use prevents NullReferenceExceptions in production builds.

### 7.4 Prefab Organisation

- **Prefabs for collectables are essential:** 28 collectable prefabs (4 per type × 3 types × 2+ variants) make scene population straightforward.
- **No mixing of prefab types in a single folder:** Separate folders per type (Bottle/, Nature/, Toys/) simplifies asset management.

### 7.5 UI Workflow

- **Panel state machines work well:** Enum-based state switching (`PanelState { Welcome, Instructions, Playing, Ended }`) is simple and readable.
- **Pause overlay is tricky:** It must overlay on the HUD without being a new state. Using `ShowPauseMenu()` (not `ShowPanel(Paused)`) keeps the HUD active underneath.
- **Selective input map enabling:** Enabling just the Pause action from a disabled UI map is cleaner than modifying action map bindings at runtime.

### 7.6 Input System

- **Action Map switching is reliable:** The new Input System's `Enable()`/`Disable()` per map prevents conflicting inputs between Player and UI maps.
- **Selective action enabling is powerful:** `pauseAction.Enable()` works even when the parent UI map is disabled.
- **Cached maps improve performance:** `FindActionMap("UI")` called once in `Awake()` rather than every state change.

### 7.7 Script Organisation

- **Folder-by-system structure scales well:** Core/, Player/, Interaction/, UI/, Weather/ — each system has clear ownership.
- **Editor scripts belong in `Editor/` folder:** Prevents Editor-only code from appearing in builds.
- **Class naming consistency is important:** `UIManager` not `UIManagers` — avoid duplicates across folders.

### 7.8 Testing

- **Peer testing reveals blind spots:** Rose identified unclear objectives, McJames noted object placement issues, Gideon found jump/weather bugs.
- **Systematic testing is better than ad-hoc:** A checklist (movement, interaction, UI, audio, weather) ensures coverage.
- **Editor simulation != play mode testing:** Some issues only appear in Play mode (e.g., Time.timeScale effects).

### 7.9 Debugging

- **Unity Console is first line of defence:** Missing references, null exceptions, and script compilation errors all surface here.
- **Debug.Log for event flow:** Logging when events fire (`OnGameStarted`, `OnItemRecycled`) helps trace event chain issues.
- **Inspector inspection for serialized fields:** Many bugs trace to missing or mis-assigned references visible in the Inspector.

### 7.10 Documentation

- **Document as you go, not at the end:** Writing docs at the end misses intermediate architecture decisions.
- **Change logs capture evolution:** Knowing *why* something changed is more valuable than knowing *what* changed.
- **Screenshots are worth paragraphs:** UI hierarchy screenshots would simplify the documentation significantly.
- **Timeline with dates grounds the project:** Showing 30 days of development demonstrates scope and effort.

---

*Document generated 30 July 2026 · Updated 3 August 2026 (resit audit fix pass) · Updated 5 August 2026 (environment dependency hardening · single terrain migration) · Updated 6 August 2026 (final submission hardening: grounding, wind push, audio remap, bin types, HUD, demo removal) · Updated 6 August 2026 (weather system redesign: 4-state machine, HeavyRain, audio cleanup, WeatherMovementEffect) · Updated 10 August 2026 (final resit fix pass: 1280×720 UI, bin indicator, allocation-free hot paths, demo-sourced terrain) · Updated 10 August 2026 (contextual-audio & mouse-sensitivity final pass: footstep wetness fix, sensitivity 0.8, manager/audio audit)*
