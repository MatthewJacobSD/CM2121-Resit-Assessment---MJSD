# Investigation Report — 10 August 2026

## 1. Weather Diagnosis

### 1.1 Root cause: weather effect GameObjects are inactive

All five weather effect GameObjects are **inactive** (`m_IsActive: 0`) in the scene:

| GO name | fileID | Component(s) | Active? |
|---------|--------|--------------|---------|
| SunlightEffect | 71494726 | SunnyEffect, Light, WeatherEffectParameters | **0** |
| CloudsEffect | 392451092 | CloudEffect, ParticleSystem, WeatherEffectParameters | **0** |
| WindyParams | 342524164 | WindEffect, ParticleSystem, WeatherEffectParameters | **0** |
| RainyParams | 803733999 | RainEffect, ParticleSystem, WeatherEffectParameters, AudioSource | **0** |
| StormyParams | 1185070769 | LightingEffect, LightingFlash, Light, WeatherEffectParameters | **0** |

They have been inactive since at least commit `4bc9bd1` (the earliest commit containing SunlightEffect). **No code activates these GameObjects** — `WeatherEffects.SetWeather` calls `rainEffect.SetActive(true)`, which calls `rainParticles.Play()`, but since the GO is inactive, `Awake()` never ran → `rainParticles` is null → **NullReferenceException**.

### 1.2 NRE chain blocks all feedback

When `EnsureState` transitions the state:

```csharp
// WeatherFeedbackSystem.EnsureState (line 245-253)
weatherState.SetWeather(target);       // state changes to Rainy — BEFORE the NRE
weatherEffects?.SetWeather(target);    // → ApplyImmediateEffects → rainEffect.SetActive(true) → NRE
windEffect?.SetWeatherState(target);   // never reached
cooldownTimer = stormCooldown;          // never set
```

- `weatherState.SetWeather(Rainy)` fires first → `CurrentWeather` = Rainy internally.
- `weatherEffects.SetWeather(Rainy)` → `ApplyImmediateEffects(Rainy)` → `rainEffect.SetActive(true)` → `rainParticles.Play()` → **NRE** (rainParticles null, Awake never ran on inactive GO).
- `PlayAmbientAudio(state)` is called AFTER `ApplyImmediateEffects` inside `SetWeather` — never reached due to NRE → **no rain audio either**.
- No `WeatherUI` in the scene → no on-screen weather state indicator.

**Result:** the user sees zero visual, zero audio, zero text feedback. From the player's perspective, "weather remained Sunny."

### 1.3 Detection logic appears correct

`EvaluateBinProximity` uses `Physics.OverlapSphereNonAlloc` (radius 15m, layer 8). Trigger colliders are included. The bins are on layer 8 (Bin), the collider is a BoxCollider with `isTrigger: 1`. Detection should work IF the player is within 15m of a wrong bin while holding an item AND `GameManager.Instance.IsPlaying` is true.

### 1.4 Level layout prevents weather from triggering during normal play

Item and bin world positions (relative to player spawn at 798.14, 8.24, 243.96):

| Object | World position | Distance from spawn |
|--------|---------------|---------------------|
| Plastic_Bottle_03 | (802.3, 9.5, 220.2) | **24.2m** (nearest item) |
| Plastic_Bottle_01 | (621.4, 8.7, 325.1) | 194.5m |
| WolfPlushie_04 (toy) | (760.5, -0.4, 446.9) | **206.6m** |
| WolfPlushie_03 (toy) | (715.4, -0.4, 451.3) | 223.4m |
| Plastic Recycling bin | (604.4, 13.7, 103.4) | **239.5m** |
| General Waste bin | (604.4, -0.7, 503.4) | **323.9m** |
| Nature Recycling bin | (1004.4, 18.6, 103.4) | **249.8m** |

**Critical observation:** items and bins are 200-430m apart. The closest toy to the nature bin (WolfPlushie_04 → Nature Recycling) is ~422m apart. The weather detection radius is only 15m. During normal gameplay, the player walks hundreds of metres with an item — weather stays Sunny until within 15m of a bin, which only happens at the very end of the walk.

### 1.5 Additional configuration gaps

- **`heavyRainParameters: {fileID: 0}`** (WeatherEffects scene block, line 73994). `GetParametersForState` falls back to `rainyParameters` for HeavyRain, so heavy rain uses the same visual params as light rain minus intensity — no distinct look. Not a crash, but a config oversight.
- **`windParticles: {fileID: 0}`** on WindEffect — wind particles never show. The WindZone itself (serialized, non-null) still works for wind speed.

---

## 2. Bin Collision

### 2.1 Root cause: collider is a trigger

- The bin prefab (`Nature Recycling.prefab`) adds a `BoxCollider` with `m_IsTrigger: 1` (line 121).
- `RecycleBinInteractable.Awake()` also force-sets `col.isTrigger = true` (lines 54-57).
- `CharacterController` does not collide with triggers → player walks through the bin.

### 2.2 Fix options (Inspector-level)

**Option A (recommended — two-collider approach):**
1. Keep the existing trigger BoxCollider (item detection zone — items pass through and trigger `OnTriggerEnter`).
2. Add a **second non-trigger BoxCollider** on the bin root, sized to the bin body/walls (slightly smaller than the trigger, open top). This blocks the player (CharacterController collides with non-trigger colliders) while items thrown into the opening still enter the trigger.

**Option B (single-collider restructure):**
1. Set the existing BoxCollider to `isTrigger: false` (solid).
2. Add a child GameObject with a trigger BoxCollider positioned at/above the opening for item detection.
3. Items thrown in pass the solid rim and enter the child trigger → `OnTriggerEnter` fires.

**Option C (code change — simplest if Inspector approach doesn't fit the mesh):**
- Remove the `col.isTrigger = true` line from `RecycleBinInteractable.Awake()`.
- Change `OnTriggerEnter` to `OnCollisionEnter` (or add a separate small trigger child for item detection).
- This requires a script change but avoids complex collider geometry.

The existing assessment docs state bins should be solid physical objects. The trigger collider was likely set to enable item pass-through, but it inadvertently made the bin non-physical for the player.

---

## 3. Bin Pinpoint / Navigation System Audit

### 3.1 Existing implementation

`BinDirectionIndicator.cs` is a complete, functional bin-proximity navigation system:

| Aspect | Status | Evidence |
|--------|--------|----------|
| Screen-edge arrow pointing to nearest accepting bin | Functional | Serialized in scene (line ~74225) with `indicatorText`, `edgeMargin 40`, `maxGuideDistance 60` |
| Centre-screen message "This bin is nearby (Xm)" | Functional | `nearbyMessageText`, `nearbyRadius 10`, `nearbyMessage "This bin is nearby"` |
| Null-safe auto-creation of TMP text | Functional | `Start()` creates fallback TMP if text refs null |
| Distance display | Functional | Shows distance in metres |
| Reuses `FindNearestAcceptingBin` / `AcceptsItem` | Functional | No duplicate bin logic |

### 3.2 HUD integration

`HUDManager.cs` renders `Score: {n}` and `Best: {n}` dynamically. Scene initial text: `Score: 0` / `Best: 0`. Single `ScoreManager` singleton. No duplicates.

### 3.3 Recommendation

This system is **functional and complete**. No changes needed. Per user instruction: do NOT delete. Report only.

---

## 4. Score / Best Verification

| Aspect | Status | Evidence |
|--------|--------|----------|
| HUDManager renders `Score: {int}` | Correct | `OnScoreChanged` / `UpdateStats` methods |
| Best label shows `Best: {int}` | Correct | `Best: {n}` format |
| Scene initial TMP text | Correct | `Score: 0` / `Best: 0` |
| ScoreManager singleton | Correct | No duplicates |
| `int` interpolation preserved | Correct | Integer values throughout |

**No issues found.** Committed in `aac35e7`.

---

## 5. Sunny Ambient Audio

| Aspect | Status | Evidence |
|--------|--------|----------|
| `sunnyAmbient: {fileID: 0}` | Correct | Null clip = silence (design intent) |
| `PlayAmbientAudio(Sunny)` | Correct | Routes to `CrossfadeAmbient(null)` → fade to silence |
| Audio clips wired for other states | Correct | `rainyClip`, `stormyClip` assigned on AudioManager |

**No issues found.** Sunny = silence is by design.

---

## 6. Assessment Compliance Audit

### 6.1 Requirements table (from `ASSESSMENT_REQUIREMENTS.md` + `output/CM2121 Resit Assessment Brief (1).docx`)

| Requirement | Brief weight | Status | Evidence | Remaining work |
|-------------|-------------|--------|----------|----------------|
| SDG alignment (SDG12 recycling) | — | **Complete** | Recycling theme throughout | — |
| Photogrammetry/LIDAR (8 scanned models) | — | **Complete** | 8 models: Bonsay, Dog Plushie, Plastic Bottle, Recycle Bin, etc. | — |
| Pickup/drop/throw mechanics | — | **Complete** | `PlayerInteraction.cs` with E/Q/right-click | — |
| Bin type-checking | — | **Partial** | `RecycleBinInteractable` + scoring matrix works. But bins are triggers → player walks through. Weather feedback broken (inactive GOs). | Fix bin colliders; fix weather effect GOs |
| Scoring system with chain bonuses | — | **Complete** | `ScoreManager` + `GameManager.HandlePlantChain()` | — |
| Lives system (5 lives) | — | **Complete** | `GameManager.maxLives = 5` | — |
| Timer (5 minutes) | — | **Complete** | `GameManager.gameDuration = 300` | — |
| SFX (19 audio clips) | — | **Complete** | AudioManager wired with clips | — |
| VFX (weather particles) | — | **BROKEN** | Effect GOs inactive → no rain/clouds/lightning renders | Activate effect GOs in scene |
| Weather state detection | — | **Complete** | `WeatherFeedbackSystem` logic correct | — |
| Movement speed modifiers | — | **Unknown** | `WeatherMovementEffect` wired but GO inactive → may not function | Verify WeatherMovementEffect on player (GO active) |
| WindZone + wind particles | — | **Partial** | WindZone wired but WindyParams GO inactive; `windParticles: {fileID: 0}` | Activate WindyParams GO; assign wind particles |
| Ambient crossfade | — | **Complete** | AudioManager `CrossfadeAmbient` works; clips assigned | — |
| Pause menu | — | **Complete** | `PauseMenuManager` + settings panel | — |
| UI/HUD | — | **Partial** | Score/Best labels present. No WeatherUI in scene. | Add WeatherUI or verify not required |

### 6.2 Broken items (require fix)

1. **Weather VFX — BROKEN** (5 inactive effect GOs)
2. **Bin collision — BROKEN** (trigger collider → player walks through)
3. **heavyRainParameters null** — config gap (falls back to rainy params)
4. **WeatherUI not in scene** — no on-screen weather indicator

### 6.3 Partial items

1. Wind particles never show (`windParticles: {fileID: 0}`)
2. WeatherMovementEffect — on player GO (active), should work if storm triggers

### 6.4 Complete items

Score/Best labels, bin-nearby navigation, scoring matrix, chain bonuses, lives, timer, SFX wiring, pause menu, SDG alignment, photogrammetry assets.

---

## 7. Proposed Changes

### 7.1 Weather effect GO activation (HIGH PRIORITY)

**Fix:** In the scene, set all five weather effect GOs to active:
- SunlightEffect (GO 71494726) → `m_IsActive: 1`
- CloudsEffect (GO 392451092) → `m_IsActive: 1`
- WindyParams (GO 342524164) → `m_IsActive: 1`
- RainyParams (GO 803733999) → `m_IsActive: 1`
- StormyParams (GO 1185070769) → `m_IsActive: 1`

This ensures `Awake()` runs on each effect script, caches component references, and the particle systems can play. The scripts already handle start/stop via `SetActive`/`SetCloudy`/`Play`/`Stop` methods.

**Risk:** If GOs are inactive by design (e.g., for performance), an alternative is to add `gameObject.SetActive(true)` in each effect's `Awake()` or in `WeatherEffects.Start()`. But making them always-active is simpler and aligns with the original architecture (the effect GOs were meant to be active children of WeatherAnchor/EnvironmentalEffects).

### 7.2 Bin collider fix (HIGH PRIORITY)

**Fix:** Add a non-trigger BoxCollider to each bin prefab for physical blocking:
- On each bin root GO, add a second `BoxCollider` with `m_IsTrigger: 0` (solid).
- Size it to the bin body (approximately `m_Size: {x: 1.2, y: 1.8, z: 1.2}`, `m_Center: {x: 0, y: 0.9, z: 0}` — covers the body, leaves the top open).
- Keep the existing trigger BoxCollider for item detection (same size or slightly inset).
- Remove the `col.isTrigger = true` line from `RecycleBinInteractable.Awake()` (the prefab already sets it; the Awake override is redundant and prevents Inspector changes from sticking).

**Alternative (if collider sizing is tricky):** Set the existing collider to non-trigger and add a child trigger at the opening. This is cleaner but requires prefab restructuring.

### 7.3 heavyRainParameters assignment (LOW PRIORITY)

**Fix:** Assign the `heavyRainParameters` on WeatherEffects to a distinct WeatherEffectParameters asset (or clone `rainyParameters` with darker clouds and higher emission). Currently falls back to `rainyParameters` via the null-coalescing in `GetParametersForState`.

### 7.4 WeatherUI (OPTIONAL)

WeatherUI.cs exists but is not in the scene. If the brief requires visible weather state feedback (the brief says "Weather VFX" — the VFX themselves are the feedback, not necessarily a text label), this may not be needed. The VFX fix (7.1) should make weather visually apparent.

### 7.5 Wind particles (OPTIONAL)

`windParticles: {fileID: 0}` on WindEffect — no wind particle effect. Assign a particle system prefab to show wind streaks during windy/stormy weather. Low priority; the WindZone alone provides physics wind.

### 7.6 Item placement review (ADVISORY)

Items are spread 200-940m from spawn, bins are 240-325m from spawn. This means:
- The nearest item to spawn is Plastic_Bottle_03 at 24m.
- All toys are 200+ m from spawn.
- Bins are 240+ m from spawn.
- Weather detection (15m radius) only triggers at the very end of the walk to a bin.

Consider clustering items closer to bins for denser gameplay, or increasing the detection radii if the level scale is intentional. This is a design decision, not a bug.

---

*Report generated 10 August 2026. Runtime verification not available — findings are CONFIRMED against serialized data unless noted as POSSIBLE·UNREPRODUCED.*
