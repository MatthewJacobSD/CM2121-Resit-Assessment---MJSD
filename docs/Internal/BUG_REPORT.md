# ChainFragrance — Bug & Issue Report

**Date:** 14 August 2026
**Project:** CM2121 Resit Assessment - MJSD
**Unity Version:** 6000.3.18f1

---

## Summary

| Severity | Count |
|----------|-------|
| High (Blocking) | 0 |
| Medium (Non-blocking) | 4 |
| Low (Cosmetic/Minor) | 5 |
| Advisory | 3 |

**Note:** The weather effect lifecycle fixes (uncommitted changes) resolve the most critical issues identified in the 10 August investigation. After these changes are committed, no blocking bugs remain.

---

## Known Issues

### ISSUE-001: Bins are trigger colliders — player walks through

| Field | Value |
|-------|-------|
| **Severity** | Medium |
| **Affected system** | `RecycleBinInteractable.cs`, bin prefabs |
| **Evidence** | `RecycleBinInteractable.Awake()` force-sets `col.isTrigger = true`. CharacterController does not collide with triggers. |
| **Current behaviour** | Player walks through recycling bins. |
| **Expected behaviour** | Bins should be solid physical objects the player cannot walk through. |
| **Blocking** | No |
| **Safe to leave unresolved** | Yes — gameplay still works (items can be thrown in), but immersion is reduced. |
| **Status** | **Fixed in pending commit** — `EnsureSolidCollider()` adds a non-trigger BoxCollider at runtime. |
| **Suggested future fix** | Add a permanent non-trigger BoxCollider in the Unity Editor prefabs instead of relying on runtime code. |

---

### ISSUE-002: Weather VFX GameObjects inactive at scene start

| Field | Value |
|-------|-------|
| **Severity** | Medium |
| **Affected system** | Weather effects (CloudEffect, RainEffect, SunnyEffect, WindEffect, LightingEffect) |
| **Evidence** | All 5 weather effect GOs have `m_IsActive: 0` in scene YAML. |
| **Current behaviour** | Weather VFX never render because `Awake()` never runs on inactive GOs → null references. |
| **Expected behaviour** | Effect GOs activate on demand when their weather state is entered. |
| **Blocking** | No |
| **Safe to leave unresolved** | No — weather VFX are a core feature. |
| **Status** | **Fixed in pending commit** — each effect script now calls `gameObject.SetActive(true)` in its activation method, triggering `Awake()` before accessing cached components. |
| **Suggested future fix** | Set effect GOs active in the Unity Editor scene. |

---

### ISSUE-003: `heavyRainParameters` not assigned (null)

| Field | Value |
|-------|-------|
| **Severity** | Low |
| **Affected system** | `WeatherEffects.cs` |
| **Evidence** | `heavyRainParameters: {fileID: 0}` in scene YAML. |
| **Current behaviour** | Heavy rain falls back to `rainyParameters` via null-coalescing. Heavy rain looks identical to light rain minus intensity. |
| **Expected behaviour** | Heavy rain should have distinct visual parameters (darker clouds, higher emission). |
| **Blocking** | No |
| **Safe to leave unresolved** | Yes — fallback is functional. Heavy rain has higher rain intensity (500 vs 200) which is visually distinguishable. |
| **Suggested future fix** | Create a `WeatherEffectParameters` ScriptableObject with darker cloud color and higher emission for HeavyRain. |

---

### ISSUE-004: `windParticles` not assigned (null)

| Field | Value |
|-------|-------|
| **Severity** | Low |
| **Affected system** | `WindEffect.cs` |
| **Evidence** | `windParticles: {fileID: 0}` in scene YAML. |
| **Current behaviour** | WindZone works (physics wind) but no visible wind streak particles. |
| **Expected behaviour** | Wind particles should be visible during windy/stormy weather. |
| **Blocking** | No |
| **Safe to leave unresolved** | Yes — WindZone provides physics wind. Assessment brief does not explicitly require wind particles. |
| **Suggested future fix** | Assign a wind streak ParticleSystem prefab to the WindEffect component in the Inspector. |

---

### ISSUE-005: WeatherUI not present in scene

| Field | Value |
|-------|-------|
| **Severity** | Low |
| **Affected system** | `WeatherUI.cs` |
| **Evidence** | Script exists but is not placed in the scene hierarchy. |
| **Current behaviour** | No on-screen weather state text/icon indicator. |
| **Expected behaviour** | Weather state shown as icon or emoji on the HUD. |
| **Blocking** | No |
| **Safe to leave unresolved** | Yes — weather VFX are the primary feedback. The new `WeatherStatusUI` (pending commit) provides a text-based weather notification. |
| **Suggested future fix** | Add `WeatherUI` to the scene if emoji-based weather icons are desired. |

---

### ISSUE-006: Items and bins spread 200–430m apart

| Field | Value |
|-------|-------|
| **Severity** | Low |
| **Affected system** | Level layout (scene) |
| **Evidence** | Nearest item to spawn is 24m. All bins are 240+ m from spawn. |
| **Current behaviour** | Player walks long distances with held items before reaching bins. Weather detection (15m radius) only triggers at the very end of the walk. |
| **Expected behaviour** | Weather should engage more frequently during normal gameplay. |
| **Blocking** | No |
| **Safe to leave unresolved** | Yes — this is a design decision, not a bug. |
| **Suggested future fix** | Cluster items closer to bins, or increase weather detection radii. |

---

### ISSUE-007: HUD scrolling not implemented

| Field | Value |
|-------|-------|
| **Severity** | Low |
| **Affected system** | HUD UI layout |
| **Evidence** | No ScrollRect + Mask + Content setup in the scene Canvas. |
| **Current behaviour** | HUD content may overflow on smaller screens. |
| **Expected behaviour** | HUD should scroll if content exceeds viewport. |
| **Blocking** | No |
| **Safe to leave unresolved** | Yes — reference resolution is 1280x720, most content fits. |
| **Suggested future fix** | Add ScrollRect + Mask to the HUD Canvas in the Unity Editor. |

---

### ISSUE-008: Audio volume slider uses AudioListener.volume (global)

| Field | Value |
|-------|-------|
| **Severity** | Low |
| **Affected system** | `PauseMenuManager.cs` |
| **Evidence** | Volume slider sets `AudioListener.volume` (global master volume). |
| **Current behaviour** | All audio sources affected equally. No per-source ambient/SFX control. |
| **Expected behaviour** | Separate ambient and SFX volume controls. |
| **Blocking** | No |
| **Safe to leave unresolved** | Yes — global volume is functional for the assessment. |
| **Suggested future fix** | Implement separate AudioMixer groups for ambient and SFX. |

---

### ISSUE-009: CRLF line ending warnings in weather effect scripts

| Field | Value |
|-------|-------|
| **Severity** | Advisory |
| **Affected system** | `CloudEffect.cs`, `LightingEffect.cs`, `LightingFlash.cs`, `RainEffect.cs`, `SunnyEffect.cs`, `WindEffect.cs` |
| **Evidence** | Git reports `LF will be replaced by CRLF` on these files. |
| **Current behaviour** | Files have LF endings on disk but Git is configured with `core.autocrlf=true`. |
| **Expected behaviour** | Consistent line endings. |
| **Blocking** | No |
| **Safe to leave unresolved** | Yes — Git handles the conversion transparently. |
| **Suggested future fix** | Ensure `.gitattributes` handles these files correctly (already configured as `text diff=csharp`). |

---

### ISSUE-010: `RawAudio` directory meta tracked despite folder being gitignored

| Field | Value |
|-------|-------|
| **Severity** | Advisory |
| **Affected system** | `.gitignore` |
| **Evidence** | `Assets/Sounds/RawAudio.meta` is listed in `.gitignore` but `Assets/Sounds/RawAudio/` contents (with .meta files) are LFS-tracked. |
| **Current behaviour** | The folder .meta is ignored but the contents are tracked. This works because the .meta files inside are separate entries. |
| **Expected behaviour** | Clean ignore rules. |
| **Blocking** | No |
| **Safe to leave unresolved** | Yes — no functional impact. |
| **Suggested future fix** | None needed — the current setup correctly tracks the audio files while ignoring the raw originals. |

---

### ISSUE-011: TerrainDemoScene directory is empty (orphaned)

| Field | Value |
|-------|-------|
| **Severity** | Advisory |
| **Affected system** | `Assets/Scenes/TerrainDemoScene/` |
| **Evidence** | Empty directory with stale `.meta` file (gitignored). |
| **Current behaviour** | Empty folder visible in Unity Project window. |
| **Expected behaviour** | No orphaned directories. |
| **Blocking** | No |
| **Safe to leave unresolved** | Yes — Unity ignores empty folders. |
| **Suggested future fix** | Delete the empty directory and its .meta in the Unity Editor. |

---

### ISSUE-012: Visual Scripting generated database tracked

| Field | Value |
|-------|-------|
| **Severity** | Advisory |
| **Affected system** | `Assets/Unity.VisualScripting.Generated/` |
| **Evidence** | `.gitignore` excludes `UnitOptions.db` but the parent directory and `.meta` are tracked. |
| **Current behaviour** | Generated directory is tracked. Unity regenerates this file on import. |
| **Expected behaviour** | Generated files not tracked. |
| **Blocking** | No |
| **Safe to leave unresolved** | Yes — Unity regenerates this on open. |
| **Suggested future fix** | Add `Assets/Unity.VisualScripting.Generated/` to `.gitignore`. |

---

## Uncommitted Changes Summary

The following changes exist in the working tree and have NOT been committed:

| File | Change | Purpose |
|------|--------|---------|
| `RecycleBinInteractable.cs` | +30 lines: `EnsureSolidCollider()` method | Adds non-trigger BoxCollider at runtime |
| `CloudEffect.cs` | Lifecycle fix: activates GO on `SetCloudy(true)` | Prevents NRE on inactive GO |
| `RainEffect.cs` | Lifecycle fix: activates GO on `SetActive(true)` | Prevents NRE on inactive GO |
| `SunnyEffect.cs` | Lifecycle fix: activates GO on `SetActive(true)` | Prevents NRE on inactive GO |
| `LightingEffect.cs` | Lifecycle fix: activates GO on `SetActive(true)` | Prevents NRE on inactive GO |
| `LightingFlash.cs` | Added null-check on `flashLight` | Prevents NRE |
| `WindEffect.cs` | Lifecycle fix: activates GO on `SetWindSpeed` | Prevents NRE on inactive GO |
| `PlayerLook.cs` | Sensitivity change to 0.5 | Lower mouse sensitivity |
| `HUDManager.cs` | Score/Best label formatting | Display improvements |
| `PauseMenuManager.cs` | Minor fix | Bug fix |
| `Florance.unity` | Scene changes (+285/-122 lines) | Serialized weather/bin fixes |
| `WeatherStatusUI.cs` | New file (127 lines) | Weather status HUD notification |
| `WeatherStatusUI.cs.meta` | New meta file | Unity asset metadata |
| `INVESTIGATION_REPORT_2026-08-10.md` | New doc | Investigation findings |
| `WEATHER_LIFECYCLE_VALIDATION.md` | New doc | Validation report |

---

*Generated by final audit pass, 14 August 2026.*
