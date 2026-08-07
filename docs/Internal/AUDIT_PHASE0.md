# ChainFragrance — Complete Project Audit (Phase 0)

**Date:** 6 August 2026  
**Auditor:** Automated code analysis  
**Scope:** All runtime scripts, scene configuration, assets, build settings, git state

---

## Architecture Overview

```
GameManager (singleton, DontDestroyOnLoad)
  ├── owns: timer, lives, category progress, win/lose logic
  ├── fires: OnGameStarted, OnGameOver, OnGameWon, OnGameEnded
  ├── calls: ScoreManager.AddScore/Reset, ScoreManager.SaveHighScore

ScoreManager (singleton, NO DontDestroyOnLoad)
  ├── owns: currentScore, highScore (PlayerPrefs)
  ├── fires: OnScoreChanged, OnHighScoreChanged

AudioManager (singleton, NO DontDestroyOnLoad)
  ├── owns: 2 ambient AudioSources + 1 SFX AudioSource
  ├── methods: CrossfadeAmbient, PlaySFX, 5 convenience wrappers
  └── STATUS: COMPLETELY UNWIRED FROM ALL GAME EVENTS

AutoSpawner (safety net)
  └── instantiates missing AudioManager, ScoreManager, HUDManager prefabs

UIManager
  ├── panel flow: Welcome → Instructions → Playing → Ended
  ├── input map switching (Player ↔ UI)
  └── delegates pause to PauseMenuManager

HUDManager (NO singleton)
  ├── subscribes to GameManager + ScoreManager events
  ├── renders: category counts, lives, timer, score, popups, announcements
  └── auto-generates fallback TMP_Text at runtime

PauseMenuManager
  ├── owns: pause/resume (timeScale), settings, exit confirmation
  ├── volume slider → AudioListener.volume (NOT AudioManager)
  └── saves username + volume to PlayerPrefs (volume NOT applied on load)

WeatherState
  ├── holds current weather enum (Sunny/Rainy/Stormy)
  ├── fires OnWeatherChanged event
  └── EVENT IS BYPASSED BY WeatherFeedbackSystem

WeatherFeedbackSystem
  ├── bridges gameplay ↔ weather (proximity-based)
  ├── drives WeatherEffects, WindEffect, WeatherMovementEffect directly
  ├── manages wind push via PlayerMovement.SetWindPush
  └── windPush field NEVER auto-resets to zero

WeatherEffects
  ├── master VFX/audio controller
  ├── transition coroutine (clouds + wind, 2s)
  ├── immediate effects (rain, lightning, overlay, sunny)
  └── crossfade ambient audio via AudioManager

WindEffect, CloudEffect, RainEffect, SunnyEffect, LightingEffect
  └── per-type visual controllers
```

---

## Critical Bugs (Must Fix)

### BUG-01: Audio Completely Unwired
**Severity:** CRITICAL  
**Impact:** Game is silent — no SFX, no ambient, no feedback sounds

`AudioManager` exists as a fully functional service but is called from **ZERO** scripts. All convenience methods (`PlaySuccessSFX`, `PlayErrorSFX`, `PlayPickupSFX`, `PlayDropSFX`, `PlayAchievementSFX`) are orphaned. `CrossfadeAmbient` is never called. The weather system does not crossfade ambient clips.

**Fix:** Wire AudioManager calls to appropriate game events across PlayerInteraction, RecycleBinInteractable, WeatherEffects, and WeatherFeedbackSystem.

### BUG-02: Wind Push Never Resets
**Severity:** HIGH  
**Impact:** Player drifts forever once storm pushes them

`PlayerMovement.windPush` is set via `SetWindPush(Vector3)` but is never cleared internally. If `WeatherFeedbackSystem` stops calling the setter, the last wind velocity persists. The field should decay to zero when there's no active push.

**Fix:** Add automatic decay/damping to windPush in PlayerMovement.Update, or ensure WeatherFeedbackSystem always sets Vector3.zero when clearing.

### BUG-03: Chain Bonus Fires Only Once
**Severity:** MEDIUM  
**Impact:** Player gets bonus only on first streak, never again

`GameManager.HandlePlantChain()` triggers at exactly `consecutivePlants == chainThreshold` (2). Once passed, the condition is never true again for the rest of the game. Should be `consecutivePlants % chainThreshold == 0` or similar modular check.

**Fix:** Change to fire bonus every N consecutive correct plant recycles.

### BUG-04: Volume Never Applied on Load
**Severity:** MEDIUM  
**Impact:** Volume resets to 1.0 every time the game starts

`PauseMenuManager` saves volume to PlayerPrefs but never applies it in `Start()`. The saved value is only loaded when the settings panel is opened.

**Fix:** Call `AudioListener.volume = PlayerPrefs.GetFloat(VolumeKey, 1f)` in PauseMenuManager.Start().

### BUG-05: ScoreManager Missing DontDestroyOnLoad
**Severity:** LOW  
**Impact:** Inconsistent singleton persistence across scene reloads

`GameManager` and `AudioManager` use DontDestroyOnLoad. `ScoreManager` does not. On scene reload, a new ScoreManager is created, but it loads high score from PlayerPrefs which is correct. However, this inconsistency should be resolved.

**Fix:** Add DontDestroyOnLoad to ScoreManager.Awake(), or document the intentional difference.

### BUG-06: SplashSpawner Memory Leak
**Severity:** MEDIUM  
**Impact:** Scene accumulates orphaned splash GameObjects indefinitely

`SplashSpawner.SpawnSplash()` instantiates new objects via `Instantiate` but never destroys them. Each footstep in rain creates a new orphaned GameObject.

**Fix:** Add DestroyAfter delay or implement object pooling.

---

## High-Priority Issues

### ISSUE-01: Weather Transitions Always From Neutral Defaults
`TransitionWeather` coroutine always lerps from `Color.grey` and `DefaultWindSpeed=2`, never from the current values. Storm→Sunny transition jumps from storm values to grey-default then lerps to sunny.

### ISSUE-02: Weather Event System Bypassed
`WeatherFeedbackSystem` calls `SetWeather`/`SetWeatherState` on effects directly instead of through `WeatherState.OnWeatherChanged`. Two parallel control paths can conflict.

### ISSUE-03: Wrong-Bin Miss Not Handled
`OnItemProcessed(false)` in WeatherFeedbackSystem nulls the item and zeroes intensity but does not change weather state, leaving it at Stormy until the next evaluation cycle.

### ISSUE-04: Drop Throw Force Not Reset
`PlayerInteraction.OnDrop` does not reset `currentThrowForce`. If player charges, drops, picks up again and throws, the throw starts from the old charged value.

### ISSUE-05: Volume Slider Conflicts
PauseMenuManager sets `AudioListener.volume` (global) while AudioManager has its own `ambientVolume` and `sfxVolume`. These are separate systems that can conflict.

---

## Moderate Issues

### ISSUE-06: Transition Always From Grey Defaults
Cloud and wind transitions start from hardcoded neutral values, not from current state, causing visual jumps during transitions.

### ISSUE-07: Immediate + Coroutine Mismatch
`ApplyImmediateEffects` sets rain/lighting instantly while the coroutine lerps clouds/wind over 2 seconds, creating visual inconsistency during the transition window.

### ISSUE-08: CloudEffect Stop Has No Fade
`CloudEffect.SetCloudy(false)` calls `Stop()` which abruptly stops particle emission. No fade-out mechanism exists.

### ISSUE-09: WindEffect Active Toggle Hardcoded
`WindEffect.SetActive(bool)` uses hardcoded 12f wind speed, conflicting with state-based or intensity-based speed settings.

### ISSUE-10: WeatherEffectParameters Extends MonoBehaviour
`WeatherEffectParameters` extends `MonoBehaviour` instead of `ScriptableObject`. This means three separate GameObjects must exist for the three weather parameter sets, rather than asset files.

### ISSUE-11: Unused WeatherEffectParameters Fields
`rainEmissionRate`, `lightingActive`, `sunRaysActive` are defined but never read by WeatherEffects.

### ISSUE-12: SplashData Fields Never Used
`startScale`, `endScale`, `mainColor`, `splashSprites` are defined but only `lifetime` and `splashMaterial` are used.

### ISSUE-13: AudioManager DontDestroyOnLoad Missing
AudioManager does not persist across scene reloads. On scene reload, a new AudioManager is created and starts `sunnyClip` from the beginning.

### ISSUE-14: No Null Check on GameManager.Instance
`RecycleBinInteractable.ProcessItem` calls `GameManager.Instance.ReportRecycled` without null-checking `Instance`.

### ISSUE-15: HUD Popup Matching Fragile
`HUDManager.ShowPopup` uses `item.name.Contains()` for substring matching, which can produce false positives.

---

## Strengths

1. **Clean modular architecture** — scripts are well-separated by system (Core, Player, Interaction, UI, Weather).
2. **Event-driven design** — GameManager and ScoreManager expose events for loose coupling with UI.
3. **Defensive programming** — null checks, fallback text generation, AutoSpawner safety net.
4. **Batch validation** — 21 automated tests verify scoring, lives, win/loss conditions.
5. **Self-contained environment** — water/terrain materials moved off demo folder with fresh GUIDs.
6. **Audio assets curated** — Optimized clips are properly organized and LFS-tracked.
7. **Input system** — New Input System with proper action map switching.
8. **Modular weather VFX** — 6 separate effect components with configurable parameters.

---

## File Inventory

| Category | Files | Total Lines |
|----------|-------|-------------|
| Core (GameManager, ScoreManager, AudioManager, AutoSpawner) | 4 | 633 |
| Player (Movement, Look, Interaction, Footstep, WeatherEffect) | 5 | 819 |
| Interaction (PickupItem, RecycleBinInteractable) | 2 | 257 |
| UI (UIManager, HUDManager, PauseMenuManager, WeatherUI, InteractionPromptUI) | 5 | 1,056 |
| Weather (State, Feedback, Effects, AnchorFollow) | 4 | 591 |
| Weather/Data (EffectParameters, SplashData) | 2 | 70 |
| Weather/Effects (Wind, Cloud, Rain, Sunny, Lighting, LightningFlash, Splash, SplashSpawner) | 8 | 391 |
| Editor (AutoSetupPauseMenu, FixGameplayAssets, FixSceneUI, BuildScript) | 4 | ~900 |
| Tests (EditMode, PlayMode) | 4 | ~400 |
| **TOTAL** | **38** | **~5,100** |

---

## Recommendations Priority Order

1. **Wire AudioManager** to game events (BUG-01) — highest impact
2. **Fix wind push decay** (BUG-02) — gameplay-critical
3. **Fix chain bonus logic** (BUG-03) — gameplay-correctness
4. **Apply volume on load** (BUG-04) — user experience
5. **Fix weather transition continuity** (ISSUE-01) — visual quality
6. **Clean up weather event flow** (ISSUE-02) — architecture
7. **Handle wrong-bin weather** (ISSUE-03) — gameplay logic
8. **Reset throw force on drop** (ISSUE-04) — gameplay-correctness
9. **Add splash cleanup** (BUG-06) — memory management
10. **Resolve AudioManager persistence** (ISSUE-13) — architecture
