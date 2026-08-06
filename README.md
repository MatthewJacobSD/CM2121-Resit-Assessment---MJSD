# ChainFragrance

A first-person recycling simulation game built in Unity 6 (URP) for the CM2121 — 3D Reconstructive Techniques module.

**SDG 12: Responsible Consumption & Production**

---

## Gameplay

Collect recyclable items — plants, toys, and bottles — scattered across an open environment and dispose of them in the correct recycling bins within a 5-minute time limit.

- **WASD** movement, mouse look, sprint (Shift), jump (Space)
- **E** to pick up items, **Q** to drop, **Left-click** to throw (while aiming with Right-click)
- Three bin types: Nature Recycling, Plastic Recycling, General Waste
- Correct recycles earn points; wrong recycles cost lives
- Plant chain bonus: 2+ consecutive correct plant recycles = +40 bonus
- Storm weather pushes you away from wrong bins — get to the right one

### Weather System

Weather transitions dynamically based on your recycling behaviour:

| Action | Weather |
|--------|---------|
| Carrying an item | Mild rain |
| Near a correct bin | Calm (rain stops) |
| Near a wrong bin | Storm (rain, wind, lightning) |
| Wrong recycle | Storm persists briefly as feedback |
| Correct recycle | Sunny, calm ambience |
| Drop item | Sunny |

### Audio

- Ambient crossfade between weather states (sunny, rainy, stormy)
- Surface-based footstep audio (dry/wet/sprint)
- Water ambience fades in/out based on distance to the WaterPlane
- SFX for pickup, drop, correct recycle, and wrong recycle

---

## Project Structure

```
Assets/
├── Audio/                 # Legacy ambient + SFX (deprecated, use Sounds/)
├── Environment/           # Terrain, water, lighting, weather prefabs
├── Input/                 # ActionsControl.inputactions (Player + UI maps)
├── Models/                # 28 collectable prefabs + 3 bin prefabs
├── Scenes/
│   ├── Florance.unity     # Main game scene (build target)
│   └── ChainFragrance.unity  # UI prototyping scene
├── Scripts/
│   ├── Core/              # GameManager, ScoreManager, AudioManager, AutoSpawner
│   ├── Player/            # Movement, Look, Interaction, FootstepAudio, WeatherEffect
│   ├── Interaction/       # PickupItem, RecycleBinInteractable
│   ├── UI/                # UIManager, HUDManager, PauseMenuManager, WeatherUI, InteractionPromptUI
│   ├── Weather/           # State, FeedbackSystem, Effects, AnchorFollow, WaterAmbienceZone
│   │   ├── Data/          # WeatherEffectParameters, SplashData
│   │   └── Effects/       # Wind, Cloud, Rain, Sunny, Lighting, LightningFlash, Splash, SplashSpawner
│   └── Editor/            # AutoSetupPauseMenu, FixGameplayAssets, FixSceneUI, BuildScript
├── Sounds/
│   ├── Optimized/         # Curated gameplay SFX + ambient clips (LFS-tracked)
│   └── RawAudio/          # Original source audio (not used in-game)
├── Settings/              # URP pipeline assets
├── TextMesh Pro/          # Fonts, style sheets
└── ProjectSettings/       # Unity settings
```

---

## Scripts Overview (30 runtime + 1 editor)

| Script | Lines | Purpose |
|--------|-------|---------|
| GameManager | 325 | Game state, timer, lives, category objectives, win/lose |
| ScoreManager | 103 | Current score + persistent high score (PlayerPrefs) |
| AudioManager | 161 | Ambient crossfade + SFX playback |
| AutoSpawner | 47 | Safety-net: spawns missing manager prefabs |
| PlayerMovement | 193 | WASD, sprint, jump, speed modifier, wind push |
| PlayerLook | 83 | Mouse look, sensitivity, invert-Y |
| PlayerInteraction | 261 | Raycast pickup, carry, drop, throw, aim |
| PlayerFootstepAudio | 202 | Surface-based footstep audio + splash effects |
| WeatherMovementEffect | 92 | Speed modifier per weather state |
| PickupItem | 125 | Carryable item: type, score, physics |
| RecycleBinInteractable | 132 | Bin trigger, scoring matrix, VFX/audio feedback |
| UIManager | 296 | Panel flow (Welcome→Instructions→Playing→Ended), input maps |
| HUDManager | 273 | Stats, announcements, score popups, timer colour |
| PauseMenuManager | 241 | Pause/resume, settings, exit confirmation |
| WeatherUI | 146 | Weather state display (text + icon/emoji) |
| InteractionPromptUI | 116 | "Press [E] to Pick Up" prompts, warnings |
| WeatherState | 75 | Weather enum + change events |
| WeatherFeedbackSystem | 272 | Proximity-based weather + wind push |
| WeatherEffects | 244 | VFX orchestration + ambient crossfade |
| WeatherAnchorFollow | 32 | VFX container follows player |
| WaterAmbienceZone | 112 | Distance-based water ambience fade |
| WeatherEffectParameters | 45 | Per-state VFX config |
| SplashData | 25 | Splash visual config |
| WindEffect | 93 | Wind zone + particles |
| CloudEffect | 53 | Cloud particles |
| RainEffect | 53 | Rain particles |
| SunnyEffect | 54 | Sun intensity + god rays |
| LightingEffect | 40 | Lightning toggle |
| LightingFlash | 83 | Random lightning flashes |
| SplashEffect | 46 | One-shot splash burst |
| SplashSpawner | 27 | Splash factory with auto-destroy |

---

## Building

1. Open in Unity 6000.3.18f1
2. File → Build Settings → Ensure `Florance` is at index 0
3. Build → Choose output folder

Or use the menu: **Build → Build Windows** (after adding the BuildScript menu item).

---

## Testing

Automated test suite (21 tests):

```bash
Unity.exe -batchmode -nographics -projectPath <path> -runTests \
  -testPlatform EditMode -testResults results.xml
```

EditMode tests (11): bin acceptance matrix, ScoreManager accumulation, penalty clamping, reset, high-score tracking
PlayMode tests (10): StartGame reset, per-category progress, wrong-recycle penalties, win/loss conditions, chain bonus, pause

---

## Known Limitations

- 8 toy instances + WaterPlane need repositioning in the Editor
- HUD scrolling requires Unity Editor setup (ScrollRect + Mask + Content)
- Audio volume slider uses AudioListener.volume (global) rather than per-source control
- Legacy ambient AudioSources on inactive GameObjects (SunlightEffect, RainyParams, StormyParams) still reference RawAudio originals

---

## Credits

- **Module:** CM2121 — 3D Reconstructive Techniques
- **Student:** Matthew Jacob SD (2506116)
- **University:** Robert Gordon University
- **Engine:** Unity 6 URP (6000.3.18f1)
- **Extension Deadline:** 6 August 2026
