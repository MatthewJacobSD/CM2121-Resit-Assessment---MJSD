# ChainFragrance

A first-person recycling simulation game built in **Unity 6 (URP)** for the **CM2121 — 3D Reconstructive Techniques** module at **Robert Gordon University**.

**Student:** Matthew Jacob SD (2506116)
**SDG:** Goal 12 — Responsible Consumption & Production

---

## Overview

Collect recyclable items — plants, toys, and bottles — scattered across a terrain environment and dispose of them in the correct recycling bins within a 5-minute time limit. Weather transitions dynamically based on your proximity to bins and recycling behaviour: calm and sunny when safe, escalating to storms when holding items near the wrong bins.

---

## Core Gameplay

| Feature | Detail |
|---------|--------|
| **Objective** | Sort all items into the correct bins before time runs out |
| **Time limit** | 5 minutes |
| **Lives** | 5 (lost on wrong recycle) |
| **Scoring** | Correct recycle: +15 to +25 pts. Wrong recycle: −15 to −45 pts. |
| **Chain bonus** | +40 for 2+ consecutive correct plant recycles |

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

### Recycling Bins

| Bin | Accepts | Rejects |
|-----|---------|---------|
| Nature Recycling | Plants (+20) | Bottles (−15), Toys (−25) |
| Plastic Recycling | Bottles (+20) | Plants (−45), Toys (−15) |
| General Waste | Toys (+25), Bottles (+15) | Plants (−20) |

---

## Weather System

Weather transitions dynamically based on proximity to recycling bins:

- **No item held** → Sunny (speed boost 1.2x, no ambient audio)
- **Near wrong bin (15m)** → Light rain (AMB_Rain.wav, speed 0.75x)
- **Closer (10m)** → Heavy rain (AMB_StrongRain.wav, speed 0.6x)
- **Very close (6m)** → Storm (AMB_Storm.wav, speed 0.45–0.75x, wind push)
- **Wrong recycle** → Storm feedback (2s) → heavy rain → sunny
- **Correct recycle** → Sunny immediately

---

## Audio

All gameplay audio uses the curated **Optimized** library:

- **Ambient:** Rain, heavy rain, storm, water flowing
- **Footsteps:** Dry walk, wet walk, running (surface-based detection)
- **SFX:** Correct recycle, wrong recycle, pickup, drop, achievement

---

## Technology

| Component | Detail |
|-----------|--------|
| **Engine** | Unity 6 URP (6000.3.18f1) |
| **Input** | New Input System (Player + UI action maps) |
| **UI** | TextMesh Pro, event-driven panel state machine, Scale With Screen Size (1280×720) |
| **Audio** | Dual-source ambient crossfade, surface-based footsteps |
| **Testing** | 21 automated tests (EditMode + PlayMode) |
| **Version Control** | Git + LFS (audio, models, images, fonts) |

---

## Repository Structure

```
Assets/
├── Environment/           # Terrain, water, lighting, weather prefabs/materials
├── Input/                 # ActionsControl.inputactions (Player + UI maps)
├── Models/                # 20 collectable prefabs + 3 bin prefabs
│   ├── Blender/           # CleanedScans (used by prefabs)
│   ├── Prefabs/           # Bins/, Collectables/, Prop/
│   └── RawScans/          # Original source scans (backup)
├── Scenes/
│   ├── Florance.unity     # Main game scene (build target)
│   └── ChainFragrance.unity  # UI prototyping scene
├── Scripts/
│   ├── Core/              # GameManager, ScoreManager, AudioManager, AutoSpawner
│   ├── Player/            # Movement, Look, Interaction, FootstepAudio, WeatherEffect
│   ├── Interaction/       # PickupItem, RecycleBinInteractable
│   ├── UI/                # UIManager, HUDManager, PauseMenuManager, WeatherUI, etc.
│   ├── Weather/           # State, FeedbackSystem, Effects, AnchorFollow, WaterAmbience
│   ├── Tests/             # EditMode + PlayMode automated tests
│   └── Editor/            # AutoSetupPauseMenu, FixGameplayAssets, BuildScript
├── Sounds/Optimized/      # Curated gameplay SFX + ambient clips (LFS-tracked)
├── Settings/              # URP pipeline assets (PC + Mobile)
├── TextMesh Pro/          # Fonts, shaders, resources
└── UI/Sprites/            # HUD sprites
```

---

## Documentation

| Document | Location | Purpose |
|----------|----------|---------|
| README | Root | This file — project overview |
| Design | `docs/Public/DESIGN.md` | Architecture and design decisions |
| Game Rules | `docs/Public/GAME_RULES.md` | Gameplay mechanics |
| User Testing | `docs/Public/USER_TESTING.md` | Peer testing results |
| Pitch | `docs/Public/PITCH.md` | Game pitch document |
| Final Audit | `docs/FINAL_PROJECT_AUDIT.md` | Preservation audit report |
| Bug Report | `docs/Internal/BUG_REPORT.md` | Known issues and risks |
| Timeline | `docs/Internal/TIMELINE.md` | Development timeline |

---

## Building

1. Open in Unity **6000.3.18f1**
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

## Recovery (After Deleting Local Copy)

1. Clone repository:
   ```bash
   git clone https://github.com/MatthewJacobSD/CM2121-Resit-Assessment---MJSD.git
   ```
2. Pull LFS assets:
   ```bash
   cd CM2121-Resit-Assessment---MJSD
   git lfs pull
   ```
3. Open with Unity **6000.3.18f1** via Unity Hub
4. Allow Unity to import all assets
5. Open `Assets/Scenes/Florance.unity`
6. Press Play to verify

---

## Known Limitations

- HUD scrolling requires Unity Editor setup (ScrollRect + Mask + Content)
- Audio volume slider uses AudioListener.volume (global) rather than per-source control
- Weather effect GameObjects activate on demand (lifecycle fix applied in code)
- Items and bins are spread across a large terrain (200–430m)

---

## Credits

- **Module:** CM2121 — 3D Reconstructive Techniques
- **Student:** Matthew Jacob SD (2506116)
- **University:** Robert Gordon University
- **Engine:** Unity 6 URP (6000.3.18f1)

---

## Gen AI Acknowledgement

I acknowledge use of opencode from https://opencode.ai to assist with project setup, code scaffolding, bug fixing, documentation generation, and refactoring. Content was used to accelerate development while maintaining code quality and assessment alignment.
