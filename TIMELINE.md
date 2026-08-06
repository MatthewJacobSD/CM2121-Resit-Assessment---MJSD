# CM2121 Eco Rescue FPS — Combined Project Timeline

## Project Overview

- **Module:** CM2121 — 3D Reconstructive Techniques
- **Student:** Matthew Jacob SD
- **SDG:** Goal 12 — Responsible Consumption & Production (Recycling)
- **Engine:** Unity 6 URP (6000.3.18f1)
- **Deadline (Extension):** Thursday 6 August 2026 @ 1pm

---

## Old Project — Prototype (resit-assessment)

**Purpose:** General idea of how the final product would look and play.
**Repository:** `MatthewJacobSD/resit-assessment` on GitHub

| Date | Day | Commits | Work Done |
|------|-----|---------|-----------|
| 3 Jul (Thu) | Day 1 | 3 | Initial check-in, full game implementation (rules, Input System, SFX, scene tooling, Resources folder) |
| 4 Jul (Fri) | Day 2 | 2 | Scene rebuild, Input System fixes, Burst cache, documentation, scene size fix |
| 6 Jul (Sun) | Day 3 | 5 | Polish pass, ground snap, touch pickup, throw/drop, lives, forest env, textures, cursor confinement, layout v7–v8, Input System error fixes |
| 7 Jul (Mon) | Day 4 | 7 | Visual v9 (motion blur, 16:9, collisions, boundaries), mouse sensitivity, tree/rock scaling v10–v11, rock Y boost, environment patch v13 (wind, rain, grass), v14 (3x3 ground field, zone collectibles, announcements) |
| 15 Jul (Tue) | Day 5 | 1 | Script audit documentation for rebuild reference |

**Prototype total:** 5 active days, 18 commits, full game from zero to polished prototype

---

## New Project — Final Submission (CM2121 Resit Assessment - MJSD)

**Purpose:** Final version submitted for assessment.

| Date | Day | Commits | Phase | Work Done | Status |
|------|-----|---------|-------|-----------|--------|
| 15 Jul (Wed) | Day 1 | 2 | Init | Initial check-in, starting project status | Done |
| 16 Jul (Thu) | Day 2 | 1 | Core | Basic player movement, camera, jump functionality | Done |
| 19 Jul (Sun) | Day 3 | 2 | Setup | Folder restructure, migrate audio/models, add scanned models, .gitignore, assessment docs, timeline | Done |
| 20 Jul (Mon) | Day 4 | 2 | Core | Pickup/drop/throw rewrite with smooth follow, layer/tag refactor, single-item hold, UI prompts and warnings, pre-layer backup | Done |
| 21 Jul (Tue) | Day 5 | 1 | Core | Recycling system, score tracking, chain bonuses, UI integration | Done |
| 22 Jul (Wed) | Day 6 | 1 | Core | UI logic integration, UI + Unity assets connected, score logic functional | Done |
| 23 Jul (Thu) | Day 7 | 1 | Polish | Callback work, saving before test and debugs | Done |
| 24 Jul (Fri) | Day 8 | 4 | Polish | Callback 2: folder/UI structure update, proximity-based weather system, WeatherFeedbackSystem rewrite, bug fixes | Done |
| 26 Jul (Sun) | Day 9 | 1 | Polish | Surface-based footstep audio (water/soil detection, 5s drying timer), deleted RockScatterer, trimmed TerrainDemoScene_URP (4GB→2.3MB), deleted Rocks and Boulders 2 (386MB), updated docs | Done |
| 27 Jul (Mon) | Day 10 | 4 | UI | Created ChainFragrance scene for PauseMenu prototyping, built full PauseMenu hierarchy (PausePanel, SettingsPanel, ConfirmationModal, SaveProgressModal), created PauseMenuManager script | Done |
| 28 Jul (Tue) | Day 11 | 5 | UI | Rewrote UIManager.cs with pause support, added PauseGame/ResumeGame to GameManager, created AutoSetupPauseMenu Editor tool, migrated PauseMenu structure into Florance scene | Done |
| 29 Jul (Wed) | Day 12 | 3 | Polish | Documentation update, user testing logs written, final project verification, script audit, scene cleanup | Done |
| 30 Jul (Thu) | Day 13 | — | Submission | Project evolution documentation, final review, project ready for submission | Done |
| 3 Aug (Mon) | Day 14 | — | Resit audit | Full project audit (PROJECT_AUDIT.md), fix pass: colliders/layers, GameManager rewrite, UI rewiring, 21 automated tests, asmdefs | Done |
| 5 Aug (Wed) | Day 15 | — | Hardening | Water + terrain material chain self-contained (demo folder independent), single terrain migration | Done |
| 6 Aug (Thu) | Day 16 | — | Final hardening | Player grounding fix, dual-terrain reconciliation, storm wind-push, gameplay audio remap to Optimized clips, bin-type fix, HUD cleanup, demo folder removal, asset audit, batch validation, final commit + build | Done |

**New project total:** 16 active days, full game with pause menu, weather system, surface audio, and documentation

---

## Submission Deadline

**Original:** Friday 24 July 2026 @ 1pm  
**Extension granted:** Thursday 6 August 2026 @ 1pm

---

## Feature Comparison — Old vs New Project

| Feature | Old Project (Prototype) | New Project (Final) | Status |
|---------|------------------------|---------------------|--------|
| Player movement | Walk/sprint/crouch/slide/dash/trip | Walk/sprint/jump with speed modifier API | Done |
| Camera look | Smoothing, FOV lerp, head bob, trip tilt | Sensitivity, vertical clamping | Done |
| Pickup/drop/throw | Basic | Smooth follow, single-item hold, E/Q/right-click | Done |
| Recycling bins | 9 bins, no type checking | 3 bins with acceptance matrix | Done |
| Scoring | Hardcoded on GameManager | Separate ScoreManager, events, chain bonus | Done |
| Lives | 5, 0.5 loss | 5, full loss | Done |
| Weather | Binary sunny/rainy via string | 3-state (Sunny/Rainy/Stormy) with storm intensity | Done |
| Weather trigger | Pick up item → binary | Proximity-based (OverlapSphere + AcceptsItem) | Done |
| Movement effects | GetSpeedMultiplier() 1.2x/0.7x | SetSpeedModifier() 1.2x/0.75x/0.45–0.75x | Done |
| Footsteps | Not implemented | Surface-based: water=splashing, soil=sunny drying | Done |
| Wind | WeatherWindController lerp | WindEffect: WindZone + particles, continuous intensity | Done |
| VFX | Light/fog/skybox lerp, rain toggle | Per-state VFX, storm overlay, ambient crossfade | Done |
| HUD | Legacy UI.Text | TextMesh Pro, event-driven | Done |
| Menu flow | None (starts immediately) | Welcome→Instructions→Play→End | Done |
| Input system | GameInput static facade | Direct InputActionAsset, map switching | Done |
| Audio | Single-source swap | Dual-source crossfade + context footsteps | Done |
| Terrain | Procedurally generated | Pre-built with trees/vegetation/rocks | Done |
| Photogrammetry | Pre-configured URP materials | Pre-configured URP materials | Done |
| Scripts | 25 | 30 | — |
| Zones | 9 zones | Open world (3 bin types) | — |

---

## Simplified Scope (Compared to Prototype)

| Aspect | Old Project | New Project |
|--------|-------------|-------------|
| Zone system | 9 zones (3x3 grid) | Open world with 3 bin types |
| Collectible types | 3 (bottle, plant, toy) | 3 (bottle, plant, toy) |
| Weather states | 2 (sunny, rainy) | 3 (sunny, rainy, stormy) with intensity |
| Footstep audio | None | Surface-based with drying effect |
| Scripts | 25 | 29 |
| Scene setup | Procedural (GameDemoSceneSetup.cs) | Pre-built Unity scene |
| HUD | Legacy Text | TextMesh Pro |
| Menu flow | None | Full card-based UI flow |

---

## Risk Assessment

| Risk | Impact | Mitigation |
|------|--------|------------|
| Demo video not recorded | High | Submit code + docs first, record video when build is ready |
| Terrain needs Unity Editor work | Medium | Trees/rocks/vegetation already placed in scene |
| User testing solo only | Medium | Systematic checklist, test each feature |
| Doc generation | Low | Script pre-written, just run it |
| Scope creep | High | Stuck to 30 scripts, core mechanics only |
