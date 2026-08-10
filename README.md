# ChainFragrance

A first-person recycling simulation game built in Unity 6 (URP) for the CM2121 — 3D Reconstructive Techniques module.

**SDG 12: Responsible Consumption & Production**

---

## Gameplay

Collect recyclable items — plants, toys, and bottles — scattered across an open environment and dispose of them in the correct recycling bins within a 5-minute time limit.

### Controls

| Input | Action |
|-------|--------|
| WASD | Move |
| Mouse | Look |
| Shift | Sprint |
| Space | Jump |
| E | Pick up item |
| Q | Drop item |
| Right-click | Aim |
| Left-click (while aiming) | Throw item |
| ESC | Pause |

### Recycling Mechanics

| Bin | Accepts | Rejects |
|-----|---------|---------|
| Nature Recycling | Plants (+20) | Bottles (-15), Toys (-25) |
| Plastic Recycling | Bottles (+20) | Plants (-45), Toys (-15) |
| General Waste | Toys (+25), Bottles (+15) | Plants (-20) |

- **Correct recycle:** Earn points, weather calms
- **Wrong recycle:** Lose a life, score penalty, storm feedback
- **Plant chain bonus:** 2+ consecutive correct plant recycles = +40 bonus
- **Bin indicator:** HUD arrow points to the nearest correct bin (name + distance), clamped to the screen edge while it is off-camera

### Weather System

Weather transitions dynamically based on proximity to recycling bins:

```
No item held → Sunny (no ambient audio)

Item picked up → Stays sunny (no rain on pickup)

Approaching wrong bin (15m) → Light rain (AMB_Rain.wav)
  ↓
Closer to wrong bin (10m) → Heavy rain (AMB_StrongRain.wav)
  ↓
Very close (6m) → Storm (AMB_Storm.wav) + wind push
  ↓
Wrong recycle → Storm feedback (2s) → Heavy rain → Sunny

Approaching correct bin → Calms progressively
  ↓
Correct recycle → Sunny immediately
```

**Weather affects gameplay:**
- Rain/Heavy Rain/Storm slow player movement
- Storm pushes player away from wrong bins
- Lightning flashes during storm (intensity ≥ 0.6)

### Audio

All gameplay audio uses the curated **Optimized** library:

| System | Clips |
|--------|-------|
| Rain ambient | AMB_Rain.wav |
| Heavy rain ambient | AMB_StrongRain.wav |
| Storm ambient | AMB_Storm.wav |
| Water ambience | AMB_WaterFlowing.wav (distance-based fade) |
| Dry footsteps | SFX_DryWalk.wav |
| Wet footsteps | SFX_WetWalk.wav |
| Sprint | SFX_Running.wav |
| Correct recycle | SFX_Correct.wav |
| Wrong recycle | SFX_Buzzer.wav |
| Pickup | SFX_CollectItem.wav |
| Drop | SFX_DropItem.wav |

---

## Project Structure

```
Assets/
├── Environment/           # Terrain, water, lighting, weather prefabs
├── Input/                 # ActionsControl.inputactions (Player + UI maps)
├── Models/                # 24 collectable prefabs + 3 bin prefabs
│   ├── Blender/           # CleanedScans (used by prefabs)
│   ├── Prefabs/           # Bins/, Collectables/, Prop/
│   └── RawScans/          # Original source scans (unreferenced backup)
├── Scenes/
│   ├── Florance.unity     # Main game scene (build target)
│   └── ChainFragrance.unity  # UI prototyping scene
├── TerrainDemoScene_URP/  # Demo terrain data referenced by the scene (tracked)
│   └── Terrain/Data/      # Terrain_1_2…asset (GUID 584c420d); rest of folder gitignored
├── Scripts/
│   ├── Core/              # GameManager, ScoreManager, AudioManager, AutoSpawner
│   ├── Player/            # Movement, Look, Interaction, FootstepAudio, WeatherEffect
│   ├── Interaction/       # PickupItem, RecycleBinInteractable
│   ├── UI/                # UIManager, HUDManager, PauseMenuManager, WeatherUI, InteractionPromptUI
│   ├── Weather/           # State, FeedbackSystem, Effects, AnchorFollow, WaterAmbienceZone
│   │   ├── Data/          # WeatherEffectParameters, SplashData
│   │   └── Effects/       # Wind, Cloud, Rain, Sunny, Lighting, LightningFlash, Splash, SplashSpawner
│   ├── Tests/             # EditMode (11) + PlayMode (10) automated tests
│   └── Editor/            # AutoSetupPauseMenu, FixGameplayAssets, FixSceneUI, BuildScript
├── Sounds/
│   └── Optimized/         # Curated gameplay SFX + ambient clips (LFS-tracked)
├── Settings/              # URP pipeline assets
├── TextMesh Pro/          # Fonts, style sheets
└── ProjectSettings/       # Unity settings
```

---

## Technologies

- **Engine:** Unity 6 URP (6000.3.18f1)
- **Input:** New Input System (Player + UI action maps)
- **UI:** TextMesh Pro, event-driven panel state machine, Scale With Screen Size canvas (1280×720)
- **Audio:** Dual-source ambient crossfade, surface-based footsteps
- **Testing:** 21 automated tests (EditMode + PlayMode)
- **Version Control:** Git + LFS (audio assets)

---

## Building

1. Open in Unity 6000.3.18f1
2. File → Build Settings → Ensure `Florance` is at index 0
3. Build → Choose output folder

---

## Testing

```bash
Unity.exe -batchmode -nographics -projectPath <path> -runTests \
  -testPlatform EditMode -testResults results.xml
```

**EditMode (11):** Bin acceptance matrix, ScoreManager accumulation, penalty clamping, reset, high-score tracking
**PlayMode (10):** StartGame reset, per-category progress, wrong-recycle penalties, win/loss conditions, chain bonus, pause

---

## Documentation

| Document | Location | Purpose |
|----------|----------|---------|
| README | Root | This file — project overview |
| Design | `docs/Public/DESIGN.md` | Architecture and design decisions |
| Game Rules | `docs/Public/GAME_RULES.md` | Gameplay mechanics |
| User Testing | `docs/Public/USER_TESTING.md` | Peer testing results |
| Timeline | `docs/Internal/TIMELINE.md` | Development timeline |
| Project Evolution | `docs/Internal/PROJECT_EVOLUTION.md` | Technical change log |
| Project Audit | `docs/Internal/PROJECT_AUDIT.md` | Complete project audit |
| Assessment Requirements | `docs/Internal/ASSESSMENT_REQUIREMENTS.md` | Requirements checklist |

---

## Known Limitations

- HUD scrolling requires Unity Editor setup (ScrollRect + Mask + Content)
- Audio volume slider uses AudioListener.volume (global) rather than per-source control
- 8 toy instances may need repositioning in the Editor

---

## Credits

- **Module:** CM2121 — 3D Reconstructive Techniques
- **Student:** Matthew Jacob SD (2506116)
- **University:** Robert Gordon University
- **Engine:** Unity 6 URP (6000.3.18f1)
- **Extension Deadline:** 6 August 2026
