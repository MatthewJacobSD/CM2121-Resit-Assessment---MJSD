# CM2121 ChainFragrance — Design Document

**Student:** Matthew Jacob SD (2506116)
**University:** Robert Gordon University
**Module:** CM2121 — 3D Reconstructive Techniques
**SDG:** Goal 12 — Responsible Consumption & Production
**Engine:** Unity 6 URP (6000.3.18f1)
**Date:** 6 August 2026 (updated)

---

## 1. Contents Page

1. UN SDG Theme Overview
2. Project Plan
3. Mood Boards
4. Initial Sketches
5. Task List
6. Scanned Objects
7. System Architecture
8. Asset References
9. Project Comparison: Current vs Reference Prototype

---

## 2. UN SDG Theme Overview

### SDG 12: Responsible Consumption & Production

The project addresses SDG 12 by creating an interactive recycling game where players sort waste into correct bins. The game educates players about proper recycling habits and raises awareness about waste management.

**Key targets:**
- **12.5:** By 2030, substantially reduce waste generation through prevention, reduction, recycling, and reuse
- **12.2:** By 2030, achieve the sustainable management and efficient use of natural resources

**Game mechanics reinforcing SDG:**
- Players pick up scattered items (plants, toys, bottles) and sort them into correct recycling bins
- Correct sorting earns points; incorrect sorting costs a life and deducts points
- A chain bonus rewards consecutive correct plant recycling, encouraging focused recycling behaviour
- Weather changes reflect environmental consequences: stormy weather when holding wrong items near bins, sunny weather when no threat is detected
- Movement speed is affected by weather: sunny grants a speed boost, rain slows the player, storm imposes heavy slowdown with visual obscurity

---

## 3. Project Plan

### Timeline

| Phase | Dates | Tasks | Status |
|-------|-------|-------|--------|
| Setup | 19 Jul | Folder restructure, migrate audio/models, .gitignore | Done |
| Core | 20 Jul | Player movement, camera, jump, sprint | Done |
| Core | 20 Jul | Pickup/drop/throw mechanics, interaction raycast | Done |
| Core | 21 Jul | Scoring, lives, game manager, win/lose conditions | Done |
| Core | 21 Jul | Recycling bin type-checking with 3 separate bins | Done |
| Environment | 22 Jul | Terrain, trees, rocks, skybox, lighting | Done |
| Environment | 22 Jul | Proximity-based weather system (sunny/rainy/stormy) | Done |
| Polish | 23 Jul | HUD with lives/timer/score/announcements, UI flow | Done |
| Polish | 23 Jul | AudioManager with crossfade ambient, SFX for all actions | Done |
| Polish | 23 Jul | Movement effects (weather-based speed modifiers) | Done |
| Polish | 23 Jul | WindZone and wind particles per weather state | Done |
| Final | 24 Jul | User testing, documentation, demo video | In Progress |

### Milestones

- [x] Project setup and folder structure
- [x] Player controller (movement, camera, jump, sprint)
- [x] Pickup/drop/throw mechanics
- [x] Scoring system with chain bonuses (+40 bonus for 2+ consecutive plants)
- [x] Lives system (5 lives)
- [x] 5-minute countdown timer
- [x] Three separate recycling bins (Plant, Toy, Bottle) with type-checking
- [x] Win/lose conditions
- [x] Proximity-based weather system (threat detection from nearby non-recyclables)
- [x] Weather VFX (rain, clouds, lightning, wind, sun rays, storm overlay)
- [x] Weather audio crossfade
- [x] Movement effects (sunny speed boost, rainy slowdown, storm heavy slowdown)
- [x] Footstep audio (surface-based: water=splashing, soil+drying effect after leaving water)
- [x] WindZone and wind particles per weather state
- [x] Splash VFX on wet surfaces while walking
- [x] HUD (lives, timer, score, high score, announcements)
- [x] UI flow (welcome -> instructions -> play -> end screen)
- [x] Audio integration (ambient, SFX)
- [x] User testing
- [x] Documentation
- [x] Demo video

---

## 4. Mood Boards

### Visual Style
- Realistic photogrammetry objects placed in a natural terrain environment
- Dynamic weather system creating atmospheric variation (sunny to stormy)
- Forest environment with trees, rocks, and organic terrain
- Clean, modern UI with card-based layout and TextMesh Pro

### Color Palette
- **Greens:** Forest, nature, plants (recycling targets)
- **Grays:** Overcast sky, concrete bins, storm clouds
- **Blues:** Water, rain, clean energy
- **Browns:** Earth, wood, organic waste
- **Red/Yellow:** Warning indicators for wrong bin, low time, low lives

### References
- Prototype project (resit-assessment) — v14 layout with 9 zones simplified to 3
- Asset Store: Rocks and Boulders 2 (environment props with 24 rock variants)
- Asset Store: URP Terrain Demo (terrain base with grassland textures)

---

## 5. Initial Sketches

### Environment Layout
```
[Forest Border - Trees & Rocks]
         |
[Rocky Area] --- [Open Field] --- [Bin Zone - Plants]
    |                |                    |
[Trees]        [Collectibles]       [Bin Zone - Toys]
    |                |                    |
[Stream]       [Path]              [Bin Zone - Bottles]
```

### UI Layout
```
+------------------------------------------+
| [Lives: 5/5]  [Time: 05:00]  [Score: 0] |
|                                          |
|                                          |
|              [Game View]                |
|                                          |
|          [Announcement Text]            |
+------------------------------------------+
```

### Game Flow
```
Welcome Screen -> Instructions Screen -> Gameplay (5 min)
                                              |
                                     [Win: All plants recycled]
                                     [Lose: 0 lives or time up]
                                              |
                                          End Screen
                                     (Score + Restart button)
```

### Proximity Weather Flow (Redesigned 6 August)

```
No item held → Sunny (no ambient audio, speed 1.2x)
  ↓
Item picked up → Stays sunny (no rain on pickup!)
  ↓
Approaching wrong bin (15m) → Light rain (AMB_Rain.wav, speed 0.75x)
  ↓
Closer to wrong bin (10m) → Heavy rain (AMB_StrongRain.wav, speed 0.6x)
  ↓
Very close (6m) → Storm (AMB_Storm.wav, speed 0.45-0.75x, wind push)
  ↓
Wrong recycle → Storm feedback (2s) → Heavy rain → Sunny
  ↓
Approaching correct bin → Calms progressively
  ↓
Correct recycle → Sunny immediately
```
```

---

## 6. Task List

### Completed
- [x] Player movement (WASD, sprint, jump) with speed modifier API
- [x] Camera look (mouse, sensitivity, clamping)
- [x] Pickup/drop/throw mechanics (E/Q/right-click)
- [x] Three recycling bins (Plant/Toy/Bottle) with type-checking
- [x] Scoring system with chain bonuses (+40 for 2+ consecutive plants)
- [x] Lives system (5 lives, lose on wrong bin)
- [x] 5-minute countdown timer
- [x] Game manager with win/lose conditions
- [x] Proximity-based weather system (4 states: Sunny, Rain, HeavyRain, Storm)
- [x] Weather VFX (rain particles, clouds, lightning, wind)
- [x] Weather audio crossfade (Optimized library only)
- [x] WindZone and wind particles configured per weather state
- [x] WeatherMovementEffect (speed modifiers per weather state)
- [x] Distance-based weather progression (15m/10m/6m thresholds)
- [x] Footstep audio (surface-based: water=splashing, soil=sunny drying effect)
- [x] Splash VFX on wet ground while walking
- [x] AudioManager with ambient crossfade and SFX
- [x] HUD (lives, timer, score, high score, announcements)
- [x] UI flow (welcome -> instructions -> play -> end)
- [x] UI navigation (continue/back between panels, input map switching)
- [x] Interaction prompts and warnings
- [x] Auto-spawner for missing managers (AutoSpawner)

---

## 7. Scanned Objects

| Model | Purpose | ItemType | Prefab |
|-------|---------|----------|--------|
| Bonsay | Plant collectible | Plant | Bonsay.prefab |
| Vase Plant | Plant collectible | Plant | VasePlant.prefab |
| Vase Plant Pot | Plant collectible | Plant | VasePotPlant.prefab |
| Dog Plushie | Toy collectible (general waste) | Toy | DogPlushie.prefab |
| Dog Plushie 2 | Toy collectible (general waste) | Toy | DogPlushie2.prefab |
| Plastic Bottle | Bottle collectible (recyclable) | Bottle | PlasticBottle.prefab |
| Nature Recycle Bin | Recycling bin (plants) | — | NatureRecyclingBin.prefab |
| Stairs | Environment prop | — | Stairs.prefab |

**Total:** 8 scanned models (6 collectibles + 1 bin + 1 prop)

**Photogrammetry Pipeline:**
1. Physical objects scanned using photogrammetry
2. Raw scans cleaned in Blender (mesh cleanup, UV mapping, texture baking)
3. Exported as OBJ with MTL and JPG textures
4. Imported into Unity URP with materials applied

---

## 8. System Architecture

### Script Structure (29 scripts)

**Core (4 scripts):**
- `GameManager.cs` — Singleton, manages game state, lives, timer, win/lose conditions
- `ScoreManager.cs` — Singleton, tracks score, high score persistence
- `AudioManager.cs` — Singleton, ambient crossfade, SFX playback
- `AutoSpawner.cs` — Runtime fallback, auto-creates missing AudioManager/HUDManager

**Player (5 scripts):**
- `PlayerMovement.cs` — WASD movement, sprint, jump, gravity, ground check, speed modifier
- `PlayerLook.cs` — Mouse look, sensitivity, vertical clamping
- `PlayerInteraction.cs` — Raycast interaction, pickup/drop/throw, hold position
- `PlayerFootstepAudio.cs` — Context-based footstep sounds (walking/running/wet)
- `WeatherMovementEffect.cs` — Applies weather-based speed modifiers to PlayerMovement

**Interaction (2 scripts):**
- `PickupItem.cs` — ItemType enum (Plant/Toy/Bottle), score value, physics pickup/drop/throw
- `RecycleBinInteractable.cs` — Bin type acceptance, success/error VFX and SFX

**UI (4 scripts):**
- `UIManager.cs` — Panel state machine with input map switching (welcome/instructions/play/end)
- `HUDManager.cs` — Lives, timer, score, high score, announcements, score popups
- `InteractionPromptUI.cs` — Context-sensitive prompts (E to pick up, Q to drop)
- `WeatherUI.cs` — Weather state display with icon sprites

**Weather (4 scripts):**
- `WeatherFeedbackSystem.cs` — Proximity-based weather controller: tracks held item, uses OverlapSphere to find bins, calculates storm intensity based on distance to nearest wrong bin
- `WeatherState.cs` — Current weather state (Sunny/Rainy/Stormy), SetWeather API, OnWeatherChanged event
- `WeatherEffects.cs` — Orchestrates VFX/audio per state, ambient crossfade, storm overlay, storm intensity scaling
- `WeatherAnchorFollow.cs` — Follows player position for weather particles

**Weather/Effects (8 scripts):**
- `RainEffect.cs` — Rain particle intensity control
- `CloudEffect.cs` — Cloud color and emission rate
- `WindEffect.cs` — WindZone speed and wind particles per weather state
- `SunnyEffect.cs` — Sun light intensity and god rays
- `LightingEffect.cs` — Lightning flash activation
- `LightingFlash.cs` — Random lightning flash coroutine
- `SplashEffect.cs` — Individual splash particle
- `SplashSpawner.cs` — Spawns splash VFX at foot positions

**Weather/Data (2 scripts):**
- `WeatherEffectParameters.cs` — MonoBehaviour holding cloud/wind/rain/special effect values
- `SplashData.cs` — ScriptableObject for configurable splash effects

**Environment (0 scripts):**
- Terrain is pre-built in the Unity scene with trees, vegetation, and rocks placed via the Terrain Editor

### Key Design Patterns
- **Singleton:** GameManager, ScoreManager, AudioManager (DontDestroyOnLoad)
- **Event-Driven:** All systems communicate via C# events (decoupled)
- **Component-Based:** Each feature is a separate MonoBehaviour
- **ScriptableObject:** SplashData for configurable splash effects
- **Player-Behavior-Driven Weather:** WeatherFeedbackSystem responds to what player is holding and proximity to recycling bins

### Weather System Architecture
```
PlayerInteraction -> OnObjectPickedUp / OnObjectDropped
       |
WeatherFeedbackSystem -> tracks heldItem + ItemType
       |
       +-- Physics.OverlapSphere(player, 15m, binLayer) -> find nearby bins
       +-- RecycleBinInteractable.AcceptsItem(ItemType) -> check if bin accepts held item
       +-- Storm intensity = 1 - (distanceToWrongBin / stormActivationRadius)
       |
WeatherState -> SetWeather(Sunny/Rainy/Stormy) -> OnWeatherChanged event
       |
WeatherEffects -> applies VFX/audio per state
       |    SunnyEffect, CloudEffect, RainEffect, LightingEffect
       |    Storm overlay alpha, ambient audio crossfade
       |
WindEffect -> WindZone speed + wind particles (continuous intensity)
       |
WeatherMovementEffect -> PlayerMovement.SetSpeedModifier()
       |
PlayerFootstepAudio -> raycast downward to detect surface layer (Water/Environment/Default)
       |    Water layer -> always wet clips + splash
       |    Environment/Default + rainy/stormy -> wet clips
       |    Environment/Default + sunny + recent water -> wet clips (drying)
       |    Environment/Default + sunny + no recent water -> dry clips
```

**Weather States:**
- **Sunny:** Default state when not holding anything or after correct recycle — speed boost 1.2x
- **Rainy:** Holding any item but no wrong bins in range — slight slowdown 0.75x
- **Stormy:** Holding item near a wrong bin — heavy slowdown 0.45x–0.75x (lerped by intensity)

**Bin Acceptance Matrix:**
| Item | Nature Recycling | Plastic Recycling | General Waste |
|------|:---:|:---:|:---:|
| Plant | Accepted (+20) | Rejected (-45) | Rejected (-20) |
| Bottle | Rejected (-15) | Accepted (+20) | Accepted (+15) |
| Toy | Rejected (-25) | Rejected (-15) | Accepted (+25) |

### Footstep Audio Logic (Surface-Based Detection)
- **Sprinting:** Always plays running clips regardless of weather or surface
- **On Water (Layer 4):** Always plays wet walk clips + splash VFX
- **On Soil/Environment (Layer 6/0) + Rainy/Stormy:** Plays wet walk clips
- **On Soil + Sunny + recently left water:** Plays wet clips (5-second drying timer)
- **On Soil + Sunny + no recent water:** Plays dry walk clips
- **Splash VFX:** Only spawns when walking (not running) on wet surfaces

---

## 9. Asset References

### Audio Assets (19 files)

**Ambient (3):**
- Raining.wav, Sunny.mp3, ThunderRain.wav

**Object Feedback (5):**
- BinCollectionPlants.wav, BinCollectionToys.wav
- CollectItem.wav, DropItem.wav, DropPlasticBottle.mp3

**Player (4):**
- Running.ogg, WalkingInTheRain.mp3, WalkingOnDryLand.mp3
- ThrowItemIntoBin.wav

**SFX (4):**
- Achievement.wav, ErrorSFX.wav, FailureSFX.wav, SuccessSFX.wav

**Source (CC0 from freesound.org):** 19 original audio files with full attribution

### UI Prefabs (8)
- WelcomeCard, ControlsCard, RulesCard, ObjectiveCard
- ScoreCard, ScoreUI, HighScoreCard, GlassCard

### Scripts (29)
- Core: 4 | Player: 5 | Interaction: 2 | UI: 4 | Weather: 4 | Weather/Effects: 8 | Weather/Data: 2

---

## 10. Project Comparison: Current vs Reference Prototype

The current project (CM2121 Resit Assessment - MJSD) is a complete rewrite of the original prototype (`resit-assessment` repository on GitHub). This section documents the key differences and improvements.

### 10.1 Architecture Overview

| Aspect | Reference Prototype | Current Project |
|--------|--------------------|-----------------|
| **Scene setup** | Procedurally generated at Editor open by `GameDemoSceneSetup.cs` (1300+ lines) | Pre-built Unity scene (`Florance.unity`) with manually placed objects |
| **Weather control** | `GameManager` holds weather as a string (`"sunny"`/`"rainy"`), `EnvironmentManager` lerps light/fog/skybox | Dedicated `WeatherFeedbackSystem` with event-driven state machine and proximity detection |
| **Weather states** | 2 active states (sunny, rainy) via string comparison | 3 states (Sunny/Rainy/Stormy) via typed enum |
| **Bin system** | Single bin per zone (9 bins total), no type checking — any item in any bin | 3 separate bins (Nature/Plastic/General) with per-bin acceptance matrix |
| **Score system** | Score fields directly on `GameManager`, hardcoded penalties | Separate `ScoreManager` singleton with events, high score via PlayerPrefs |
| **HUD** | Legacy `UnityEngine.UI.Text` on `HUD.cs` | TextMesh Pro with event-driven updates on `HUDManager.cs` |
| **Input system** | `GameInput` static facade + `EcoInputActions` wrapper | Direct InputActionAsset references, input map switching (Player/UI) |
| **Player controller** | Single `PlayerController.cs` (296 lines) — walk/sprint/crouch/slide/dash/trip/shoot | Split into `PlayerMovement.cs`, `PlayerLook.cs`, `PlayerInteraction.cs` |
| **Model handling** | Runtime `ScannedModelMaterialApplier.cs` loads JPG textures from disk as fallback | Pre-configured URP materials on imported models |

### 10.2 Weather System Comparison

| Feature | Reference | Current |
|---------|-----------|---------|
| **Trigger** | Picking up any item → sunny/rainy binary | Holding item + bin proximity → 3-state with storm intensity |
| **States** | Sunny (plant pickup or default), Rainy (toy/bottle) | Sunny (not holding / correct recycle), Rainy (holding item), Stormy (near wrong bin) |
| **Storm mechanic** | None | Intensity scales 0→1 based on distance to nearest rejecting bin |
| **VFX** | Light color/intensity lerp, fog, skybox swap, rain toggle | Per-state VFX, storm overlay alpha, ambient crossfade |
| **Wind** | `WeatherWindController` lerps WindZone + grass shader properties | `WindEffect` scales wind speed 2→20 + wind particles |
| **Movement** | `GameManager.GetSpeedMultiplier()` returns 1.2x or 0.7x | `WeatherMovementEffect` applies 1.2x / 0.75x / 0.45–0.75x |
| **Footsteps** | Not implemented | Surface-based: raycast detects water/soil, 5s drying timer after leaving water in sun |

### 10.3 Scoring & Game Rules Comparison

| Rule | Reference | Current |
|------|-----------|---------|
| **Plant deposit** | +10 pts, +50% life recovery | +20 pts (Nature only) |
| **Toy deposit** | -20 pts, -0.5 life | -15 to -25 pts (varies by bin) |
| **Bottle deposit** | -5 pts, -0.5 life | -15 to +20 pts (varies by bin) |
| **Lives** | 5, lose 0.5 on wrong bin | 5, lose on wrong recycle |
| **Chain bonus** | +40 for 2+ consecutive plant pickups | +40 for 2+ consecutive correct recycles |
| **Win condition** | All plants binned + score >= 1 + lives > 0 | All plants recycled within 5 minutes |
| **Sorting required** | No — single bin accepts everything | Yes — 3 bins with acceptance matrix |

### 10.4 Player Controller Comparison

| Feature | Reference (`PlayerController`) | Current (`PlayerMovement`) |
|---------|-------------------------------|---------------------------|
| **Walk/sprint** | 5 / 9 | 5 / 8.5 |
| **Jump** | 1.8 height | 1.8 height |
| **Crouch** | Toggle, 2.5 speed | Not implemented |
| **Slide** | Sprint+Ctrl, 12 speed | Not implemented |
| **Dash** | Q key, 16 speed, 0.22s | Not implemented |
| **Trip/fall** | Rocks trip (1.2s) or knock down (2.8s) | Not implemented |
| **Shoot** | LMB destroys small rocks | Not implemented |
| **Look** | Smoothing, FOV lerp (68 to 50 aim), head bob, trip tilt | Sensitivity, vertical clamping |
| **Weather speed** | `GameManager.GetSpeedMultiplier()` | `SetSpeedModifier()` API from `WeatherMovementEffect` |

### 10.5 UI System Comparison

| Feature | Reference (`HUD.cs`) | Current (`HUDManager` + `UIManager`) |
|---------|----------------------|--------------------------------------|
| **Framework** | Legacy UI.Text | TextMesh Pro |
| **Menu flow** | None — game starts immediately | Welcome to Instructions to Playing to End |
| **Input switching** | No | Player/UI action map switching |
| **Back navigation** | No | Escape/Cancel returns to previous panel |
| **Weather display** | Text string | Emoji/icon + text |
| **Cursor** | Dedicated `CursorLockController` | Handled by `UIManager` on transitions |

### 10.6 Summary of Key Improvements

| Category | Improvement |
|----------|-------------|
| **Weather** | Binary sunny/rainy to 3-state proximity system with continuous storm intensity |
| **Sorting** | Single bin to 3-type acceptance matrix (core mechanic) |
| **UI** | Legacy Text to TMP with full menu flow and input switching |
| **Audio** | Single-source swap to dual-source crossfade + context footsteps |
| **Architecture** | Monolithic `GameManager` to event-driven singletons with separation of concerns |

### 10.7 Features from Reference Not Yet in Current

| Feature | Description |
|---------|-------------|
| **Crouch/Slide/Dash** | 3 additional movement mechanics adding variety |
| **Rock trip/fall** | Environmental hazards causing temporary loss of control |
| **Shoot mechanic** | LMB destroys small rocks |
| **Procedural scene generation** | Auto-bootstrap on Editor open, no manual setup |
| **Grass wind shaders** | MaterialPropertyBlock-driven grass animation |
| **Bounds guard** | Player respawn on falling below terrain |
| **Aspect ratio enforcement** | 16:9 letterboxing for ultrawide displays |
| **188 collectibles** | Larger scale with 9-zone grid layout |

---

## Gen AI Acknowledgement

I acknowledge use of opencode from https://opencode.ai to assist with project setup, code scaffolding, bug fixing, documentation generation, and refactoring. I entered prompts between 19-24 July 2026 for: initial project structure setup, script migration and rewriting, lives/timer/scoring system implementation, recycling bin type-checking logic, AudioManager event wiring, HUD/UI updates, proximity-based weather system rewrite, movement effect implementation, footstep audio logic correction, wind zone configuration, and documentation generation. Content was used to accelerate development while maintaining code quality and assessment alignment.

In my partaking, I structured the project design and architecture while following a few YouTube tutorials to boost my efficiency for UI proficiency and Unity overall layout and understanding.

For Weather Effects:
Cloud Effect: https://youtu.be/H0jUEuPENKI?si=TZao6phoIzXmRd9U
Godray effects: https://youtu.be/kbsd6askiCY?si=WcZZTbS5WKoy6b9_
Rain Particles: https://youtu.be/SrWrUN56UWU?si=T3i5mxH4uBzwbJDt
Weather System: https://youtu.be/UNP5wEqLKmM?si=LOLIyeS1slTLvicx

Ideas were closing in for UI improvements but time was not accommodating, so the implementation design isn't as I wished from my original design.

My first scope was to build a fighting, story game having environmental changes as a slow showreel while the fighting continued between two participants.
Second scope was to build a story design where a child looks through secrets and discovers mysteries of a fight that took place, going through environmental changes, history, and storytelling.

Third scope is a nature game where the player collects various items and throws them in their corresponding bins to win.

Acknowledging the difficulties imposed and the strict design requirements that needed an SDG as a connection, I opted for the third option. Ideas were scarce, which is why the design and implementation started late. However, overall I am proud to say that this simple logic may be beneficial as a proposed idea and be considered a commendable project demo designed for the assessment.

AI usage was used as mentioned to assist me and speed up development, helping me draft a clear idea out of the millions I had, to have a clear goal and design scope to opt for. Code has been written partially by me and assisted by AI for debugging and overall restructuring when I didn't like it. UI redesigned from first prop design, and new implementations were added along the way, hoping it may be of clear conscientiousness for the user testing.
