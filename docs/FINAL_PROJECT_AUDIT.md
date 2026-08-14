# ChainFragrance — Final Project Audit & Preservation Report

**Date:** 14 August 2026
**Student:** Matthew Jacob SD (2506116)
**University:** Robert Gordon University
**Module:** CM2121 — 3D Reconstructive Techniques
**Unity Version:** 6000.3.18f1 (Unity 6)
**Repository:** https://github.com/MatthewJacobSD/CM2121-Resit-Assessment---MJSD.git

---

## 1. Project Identity

| Field | Value |
|-------|-------|
| Project name | CM2121 Resit Assessment - MJSD |
| Game title | ChainFragrance |
| Unity version | 6000.3.18f1 (revision 5ebeb53e4c07) |
| Render pipeline | Universal Render Pipeline (URP) 17.3.0 |
| Input system | New Input System 1.19.0 |
| Build target | StandaloneWindows64 |
| Build output | `Build/CM2121_EcoRescue.exe` |
| Reference resolution | 1280x720 (Scale With Screen Size) |
| Color space | Linear |
| SDG alignment | SDG 12 — Responsible Consumption & Production |

---

## 2. Project Architecture

### 2.1 System Overview

```
┌─────────────────────────────────────────────────────────────┐
│                        GameManager                          │
│  (Singleton: lives, timer, win/lose, game state)           │
│  Events: OnGameStarted, OnGameOver, OnGameWon, etc.        │
└─────────┬───────────────────────────┬───────────────────────┘
          │                           │
    ┌─────▼──────┐            ┌───────▼────────┐
    │ScoreManager│            │ UIManager      │
    │(Singleton) │            │(Panel state    │
    │Events:     │            │ machine)       │
    │OnScore     │            └───────┬────────┘
    │Changed     │                    │
    └────────────┘            ┌───────▼────────┐
                              │ HUDManager     │
                              │(Lives, timer,  │
                              │ score, popup)  │
                              └────────────────┘

┌─────────────────────────────────────────────────────────────┐
│                     Player System                           │
│  PlayerMovement (WASD, sprint, jump, gravity)              │
│  PlayerLook (mouse, sensitivity, clamping)                 │
│  PlayerInteraction (raycast, pickup/drop/throw)            │
│  PlayerFootstepAudio (surface-based, drying timer)         │
│  WeatherMovementEffect (speed modifier per weather)        │
└─────────┬───────────────────────────┬───────────────────────┘
          │                           │
    ┌─────▼──────┐            ┌───────▼────────┐
    │PickupItem  │            │WeatherFeedback │
    │(ItemType,  │            │System          │
    │ score,     │◄───────────│(proximity,     │
    │ physics)   │            │ storm intensity│
    └─────┬──────┘            │ bin detection) │
          │                   └───────┬────────┘
    ┌─────▼──────────┐          ┌─────▼──────────┐
    │RecycleBin      │          │WeatherState    │
    │Interactable    │          │(Sunny/Rainy/   │
    │(BinType,       │          │ HeavyRain/     │
    │ acceptance,    │          │ Stormy)        │
    │ scoring)       │          └─────┬──────────┘
    └────────────────┘          ┌─────▼──────────┐
                                │WeatherEffects  │
                                │(VFX orchestr.) │
                                └─────┬──────────┘
                          ┌───────────┼───────────┐
                    ┌─────▼──┐  ┌─────▼──┐  ┌────▼─────┐
                    │Sunny   │  │Cloud   │  │Rain      │
                    │Effect  │  │Effect  │  │Effect    │
                    └────────┘  └────────┘  └──────────┘
                    ┌────────┐  ┌────────┐  ┌──────────┐
                    │Wind    │  │Lighting│  │Splash    │
                    │Effect  │  │Effect  │  │Spawner   │
                    └────────┘  └────────┘  └──────────┘

┌─────────────────────────────────────────────────────────────┐
│                     Audio System                            │
│  AudioManager (Singleton: ambient crossfade, SFX)          │
│  WaterAmbienceZone (distance-based water ambience)         │
│  PlayerFootstepAudio (context-based step sounds)           │
└─────────────────────────────────────────────────────────────┘
```

### 2.2 Script Inventory (40 C# files)

| Module | Scripts | Key Classes |
|--------|---------|-------------|
| **Core** (4) | GameManager, ScoreManager, AudioManager, AutoSpawner | Singletons managing game state |
| **Player** (5) | PlayerMovement, PlayerLook, PlayerInteraction, PlayerFootstepAudio, WeatherMovementEffect | First-person controller |
| **Interaction** (2) | PickupItem, RecycleBinInteractable | Item/bin mechanics |
| **UI** (7) | UIManager, HUDManager, PauseMenuManager, InteractionPromptUI, WeatherUI, WeatherStatusUI, BinDirectionIndicator | All UI panels and HUD |
| **Weather** (5) | WeatherState, WeatherEffects, WeatherFeedbackSystem, WeatherAnchorFollow, WaterAmbienceZone | Weather state machine and effects |
| **Weather/Data** (2) | WeatherEffectParameters, SplashData | Configuration data |
| **Weather/Effects** (8) | SunnyEffect, CloudEffect, RainEffect, WindEffect, LightingEffect, LightingFlash, SplashEffect, SplashSpawner | Individual weather effects |
| **Editor** (4) | AutoSetupPauseMenu, FixSceneUI, FixGameplayAssets, BuildScript | Editor tools |
| **Tests** (3) | RecycleBinMatrixTests, ScoreManagerTests, GameManagerPlayTests | Automated tests |

### 2.3 Assembly Definitions

| Assembly | References | Platform |
|----------|------------|----------|
| `Florance.Runtime.asmdef` | TMPro, InputSystem, UnityEngine.UI | All |
| `Florance.Editor.asmdef` | Florance.Runtime, TMPro, InputSystem, UnityEngine.UI | Editor only |
| `EditModeTests.asmdef` | TestRunners, Florance.Runtime | Editor only |
| `PlayModeTests.asmdef` | TestRunners, Florance.Runtime | All (test constraint) |

---

## 3. Scene Structure

| Scene | Purpose | Build Index | Status |
|-------|---------|-------------|--------|
| `Florance.unity` | Main game scene (build target) | 0 | Active, serialized |
| `ChainFragrance.unity` | UI prototyping scene | Not in build | Backup/reference |
| `TerrainDemoScene/` | Empty directory (orphaned) | — | Stale, gitignored |

---

## 4. Asset Structure

### 4.1 Critical Assets (must be preserved)

| Category | Location | Count | LFS |
|----------|----------|-------|-----|
| C# Scripts | `Assets/Scripts/` | 40 | No |
| Scenes | `Assets/Scenes/Florance.unity` | 1 | No |
| Input Actions | `Assets/Input/ActionsControl.inputactions` | 1 | No |
| Prefabs (collectables) | `Assets/Models/Prefabs/Collectables/` | 20 | No |
| Prefabs (bins) | `Assets/Models/Prefabs/Bins/` | 3 | No |
| Lighting prefabs | `Assets/Environment/Prefabs/Lighting/` | 7 | No |
| Terrain prefabs | `Assets/Environment/Terrain/Prefabs/` | 5 | No |
| Water shader | `Assets/Environment/Water/Shaders/WaterDepthBased.shadergraph` | 1 | No |
| URP settings | `Assets/Settings/` | 7 | No |
| Weather materials | `Assets/Environment/Weather/Effects/` | 11 | No |
| UI sprites | `Assets/UI/Sprites/` | 15 | Yes |
| 3D Models (cleaned) | `Assets/Models/Blender/CleanedScans/` | 8 | Yes |
| 3D Models (raw) | `Assets/Models/RawScans/` | 8 | Yes |
| Audio (optimized) | `Assets/Sounds/Optimized/` | 16 | Yes |
| Fonts | `Assets/TextMesh Pro/Fonts/` | 3 | Yes |
| TMP resources | `Assets/TextMesh Pro/Resources/` | Multiple | No |

### 4.2 External Packages (Unity Package Manager)

| Package | Version | Used By |
|---------|---------|---------|
| com.unity.inputsystem | 1.19.0 | PlayerMovement, PlayerLook, PlayerInteraction, UIManager |
| com.unity.render-pipelines.universal | 17.3.0 | Render pipeline |
| com.unity.test-framework | 1.6.0 | Automated tests |
| com.unity.timeline | 1.8.12 | Available (may not be used) |
| com.unity.visualeffectgraph | 17.3.0 | Available (may not be used) |
| com.unity.visualscripting | 1.9.12 | Available (may not be used) |
| com.unity.ai.navigation | 2.0.14 | Available (may not be used) |

### 4.3 Git-Ignored Assets (not in repository)

| Asset | Reason |
|-------|--------|
| `Library/` | Unity cache, regenerated on open |
| `Temp/` | Unity temporary files |
| `Logs/` | Unity log files |
| `UserSettings/` | Editor user preferences |
| `*.csproj`, `*.sln` | Auto-generated by Unity |
| `Assets/Sounds/RawAudio/` | Raw audio originals (102MB), not used in-game |
| `Assets/TerrainDemoScene_URP/` | Demo terrain data (mostly), only one terrain data file tracked |
| `Assets/Environment/Prefabs/Trees/` | Scratch tree prefabs, unreferenced |
| `Assets/Environment/Prefabs/Terrain/` | Scratch terrain prefabs, unreferenced |
| `Assets/New Terrain 2.asset*` | Unreferenced terrain duplicates |
| `Assets/New Terrain 3.asset*` | Unreferenced terrain duplicates |

---

## 5. Gameplay Flow

```
1. Scene loads → Welcome Screen displayed
2. Player presses Continue → Instructions Screen
3. Player presses Continue → Gameplay starts
   - GameManager.StartGame(): resets lives (5), timer (300s), score
   - Cursor locked, Time.timeScale = 1
   - Player can move, look, interact
4. During gameplay:
   - Player picks up items (E key)
   - Weather system responds to held item + bin proximity
   - Player throws/drops items (Q / right-click + left-click)
   - Items entering bin triggers process: score + or -, weather adjusts
5. Game ends when:
   - All category objectives met → Win screen
   - Lives depleted → Lose screen
   - Timer expires → End screen
6. End screen shows: final score, high score, restart button
7. Player can return to menu or restart
```

---

## 6. Weather Flow

```
No item held → Sunny (no ambient, speed 1.2x)
    ↓
Item picked up → Stays Sunny (no rain on pickup)
    ↓
Player approaches wrong bin (15m) → Light Rain (AMB_Rain.wav, speed 0.75x)
    ↓
Closer to wrong bin (10m) → Heavy Rain (AMB_StrongRain.wav, speed 0.6x)
    ↓
Very close (6m) → Storm (AMB_Storm.wav, speed 0.45-0.75x, wind push)
    ↓
Wrong recycle → Storm feedback (2s) → Heavy Rain → Sunny
    ↓
Approaching correct bin → Calms progressively
    ↓
Correct recycle → Sunny immediately
```

---

## 7. Audio Flow

```
AudioManager (Singleton)
├── Ambient crossfade: dual-source crossfade between weather clips
│   ├── Sunny: null (silence)
│   ├── Rainy: AMB_Rain.wav
│   ├── Heavy Rain: AMB_StrongRain.wav
│   └── Stormy: AMB_Storm.wav
├── SFX playback:
│   ├── Correct recycle: SFX_Correct.wav
│   ├── Wrong recycle: SFX_Buzzer.wav
│   ├── Pickup: SFX_CollectItem.wav
│   ├── Drop: SFX_DropItem.wav
│   └── Achievement: SFX_Achievement.wav
└── PlayerFootstepAudio
    ├── Dry footsteps: SFX_DryWalk.wav
    ├── Wet footsteps: SFX_WetWalk.wav
    └── Running: SFX_Running.wav

WaterAmbienceZone
└── AMB_WaterFlowing.wav (distance-based fade near water)
```

---

## 8. UI Flow

```
UIManager (Panel State Machine)
├── WelcomeScreen → InstructionsScreen → HUDScreen → EndScreen
├── PauseMenu (ESC key)
│   ├── PausePanel → SettingsPanel / ConfirmationModal / SaveProgressModal
│   └── Input map switching: Player ↔ UI
└── HUDManager
    ├── Lives display
    ├── Timer display (color-coded: red ≤30s, yellow ≤60s)
    ├── Score display (Score: N / Best: N)
    ├── Announcement text (temporary messages)
    └── Score popups (per item type)
```

---

## 9. Git/GitHub Preservation Strategy

### 9.1 What is tracked

| Category | Status |
|----------|--------|
| All C# scripts | ✅ Tracked |
| All scenes | ✅ Tracked (Florance.unity, ChainFragrance.unity) |
| All prefabs | ✅ Tracked (41 prefabs) |
| All materials | ✅ Tracked (13 materials) |
| All shaders | ✅ Tracked (19 shader files) |
| Input actions | ✅ Tracked |
| ProjectSettings/ | ✅ Tracked (26 files) |
| Packages/ | ✅ Tracked (manifest.json + packages-lock.json) |
| 3D models (OBJ) | ✅ LFS-tracked (76 files) |
| Audio (WAV/MP3/OGG) | ✅ LFS-tracked (16 optimized files) |
| Images (PNG/JPG/TIF) | ✅ LFS-tracked |
| Fonts (TTF/OTF) | ✅ LFS-tracked |
| .meta files | ✅ Tracked (335 files) |
| Documentation | ✅ Tracked |

### 9.2 What is NOT tracked (by design)

| Category | Reason |
|----------|--------|
| Library/ | Unity cache, regenerated |
| Temp/ | Unity temporary files |
| Logs/ | Unity log files |
| UserSettings/ | Editor user preferences |
| *.csproj, *.sln | Auto-generated |
| RawAudio/ | Raw audio originals (102MB) |
| Trees/ | Scratch prefabs, unreferenced |
| Terrain/ (scratch) | Scratch prefabs, unreferenced |
| TerrainDemoScene_URP/ (most) | Demo terrain data, only one file tracked |
| __pycache__/ | Python cache |
| output/~$* | Word lock files |

### 9.3 LFS Configuration

- **76 files** tracked by LFS (models, audio, images, fonts)
- **LFS version:** git-lfs/3.7.1
- **Endpoint:** GitHub LFS (basic auth)
- **Transfer:** basic, lfs-standalone-file, ssh

### 9.4 `.meta` File Importance

Every Unity asset requires a corresponding `.meta` file containing:
- **GUID** (Globally Unique Identifier) — links asset references across scenes/prefabs
- **Import settings** — how Unity processes the asset
- **Plugin settings** — platform-specific configuration

**Without `.meta` files, Unity regenerates new GUIDs**, breaking all serialized references in scenes and prefabs. The repository tracks all 335 `.meta` files.

### 9.5 Serialized Scene References

The `Florance.unity` scene file (YAML format) contains:
- GameObject hierarchy with fileID references
- Component serialization (MonoBehaviour fields with fileID/GUID links)
- Prefab instance references (GUID → prefab → fileID)
- Material/shader references (GUID)
- Terrain data references

**All references use GUIDs from `.meta` files.** If `.meta` files are lost, the scene will have missing references.

---

## 10. Required Files for Recovery

To reconstruct this project from GitHub:

### Must-have files

1. **All tracked files** (661 files at time of audit)
2. **LFS objects** (76 binary assets — models, audio, images, fonts)
3. **Unity version** 6000.3.18f1 (exact version required)
4. **ProjectSettings/** (26 files — defines project configuration)
5. **Packages/manifest.json** (defines package dependencies)
6. **All `.meta` files** (335 files — preserves GUID references)

### Nice-to-have files

- `docs/` (documentation, not required for gameplay)
- `output/` (generated Word documents, can be regenerated from Python scripts)
- `README.md` (project description)

---

## 11. Known Issues

See `docs/Internal/BUG_REPORT.md` for complete issue list.

### Summary

| Issue | Severity | Status |
|-------|----------|--------|
| Weather VFX inactive GOs | Medium | Fixed in pending commit |
| Bins are triggers | Medium | Fixed in pending commit |
| heavyRainParameters null | Low | Acceptable fallback |
| Wind particles null | Low | WindZone works |
| WeatherUI not in scene | Low | WeatherStatusUI provides alternative |
| Items spread 200-430m | Low | Design decision |
| HUD scrolling missing | Low | Minor |
| Global audio volume | Low | Minor |

---

## 12. Unity Version Control Status

| Aspect | Status |
|--------|--------|
| Unity Version Control mode | `Visible Meta Files` (configured in `VersionControlSettings.asset`) |
| Plastic SCM / UVCS | **Not connected** — no remote workspace configured |
| Pending changes in Unity VCS | **Unknown** — cannot verify from CLI |
| Scene checked in | **Unknown** — requires Unity Editor verification |
| Workspace clean | **Unknown** — requires Unity Editor verification |

**Important:** The project uses `Visible Meta Files` mode (standard file-based version control), NOT Unity Version Control (Plastic SCM). This means Unity is configured to use `.meta` files for asset tracking, which is compatible with Git. There is no separate Unity Version Control system to checkpoint.

---

## 13. Potential Unity Import/Opening Problems

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| Wrong Unity version | High | Project won't open | Use exactly 6000.3.18f1 |
| LFS not installed | Medium | Binary assets show as LFS pointers | Install Git LFS before cloning |
| LFS pull not run | Medium | Binary assets are placeholders | Run `git lfs pull` after clone |
| Missing packages | Low | Compilation errors | Packages resolved from manifest.json |
| Terrain data mismatch | Low | Terrain looks different | One terrain data file is tracked |
| Scene references broken | Very Low | Missing components | All .meta files tracked |
| Assembly definition mismatch | Very Low | Compilation errors | All .asmdef files tracked |

---

## 14. Recovery Instructions

1. **Clone repository:**
   ```bash
   git clone https://github.com/MatthewJacobSD/CM2121-Resit-Assessment---MJSD.git
   ```

2. **Pull LFS assets:**
   ```bash
   cd CM2121-Resit-Assessment---MJSD
   git lfs pull
   ```

3. **Open with Unity 6000.3.18f1:**
   - Install Unity 6000.3.18f1 via Unity Hub
   - Open the cloned folder as a Unity project
   - Allow Unity to import all assets (may take 5-15 minutes)

4. **Verify scene:**
   - Open `Assets/Scenes/Florance.unity`
   - Verify all objects are present (no pink/missing materials)
   - Verify all scripts compile (no errors in Console)

5. **Verify packages:**
   - Check Window → Package Manager
   - Ensure all packages from manifest.json are resolved

6. **Verify gameplay:**
   - Press Play
   - Verify player movement (WASD + mouse)
   - Verify item pickup (E key near items)
   - Verify recycling (throw into bins)
   - Verify weather transitions
   - Verify audio plays

---

## 15. Final State

| Metric | Value |
|--------|-------|
| Unity version | 6000.3.18f1 |
| Total tracked files | 661 |
| C# scripts | 40 |
| Scenes | 2 (1 active) |
| Prefabs | 41 |
| Materials | 13 |
| Shader files | 19 |
| LFS files | 76 |
| Automated tests | 21 (EditMode + PlayMode) |
| Documentation files | 19 Markdown |
| Git branch | main |
| Last meaningful commit | aac35e7 (10 Aug 2026) |
| Uncommitted changes | 15 files (11 modified, 4 new) |

---

## 16. Recommended Future Development Boundaries

If development continues beyond this checkpoint:

1. **Do not change the Unity version** without thorough testing
2. **Do not remove `.meta` files** — they preserve all serialized references
3. **Do not modify the assembly definitions** without understanding cross-references
4. **Do not change the Input System action map names** — scripts reference them by string
5. **Do not change the scoring matrix** in `RecycleBinInteractable` without updating tests
6. **Keep LFS tracking** for all binary assets
7. **Keep the `Optimized/` audio folder** as the single source of gameplay audio
8. **Test weather system changes in-play** — the lifecycle fix is code-based, not Inspector-based
9. **Back up the scene** before major changes — `Florance.unity` is the single game scene

---

*Audit completed 14 August 2026. All findings are evidence-based from direct file inspection.*
