# CM2121 Eco Rescue FPS — Combined Project Timeline

## Project Overview

- **Module:** CM2121 — 3D Reconstructive Techniques
- **Student:** Matthew Jacob SD
- **SDG:** Goal 12 — Responsible Consumption & Production (Recycling)
- **Engine:** Unity 6 URP (6000.3.18f1)
- **Deadline:** Friday 24 July 2026 @ 1pm

---

## Old Project — Prototype (resit-assessment)

**Purpose:** General idea of how the final product would look and play.

| Date | Day | Time | Commits | Work Done |
|------|-----|------|---------|-----------|
| 3 Jul | Fri | 15:38–23:08 | 4 | Initial check-in + full game implementation (rules, Input System, SFX, scene tooling) |
| 4 Jul | Sat | 00:03–00:29 | 2 | Scene rebuild, Input System fixes, Burst cache, documentation |
| 6 Jul | Mon | 22:39–23:46 | 4 | Polish pass, lives, forest environment, textures, layout v7–v8 |
| 7 Jul | Tue | 00:09–04:47 | 6 | Visual v9–v14, environment patches, rocks, trees, weather (6 commits in 4.5hrs) |
| 15 Jul | Wed | 14:03 | 1 | Script audit documentation |

**Prototype total:** 4 active days, 18 commits, full game from zero

---

## New Project — Final Submission (CM2121 Resit Assessment - MJSD)

**Purpose:** Final version submitted for assessment.

| Day | Date | Phase | Tasks | Deliverable | Status |
|-----|------|-------|-------|-------------|--------|
| — | 15 Jul (Wed) | Init | Initial check-in, starting status | Project created | ✅ |
| — | 16 Jul (Thu) | Init | Basic player movement, camera, jump | Player controller working | ✅ |
| 1 | 19 Jul (Sun) | Setup | Folder restructure, migrate 3 audio files, .gitignore, commit | Clean project structure | ✅ |
| 2 | 20 Jul (Mon) | Core | Pickup/drop mechanics, scoring system, lives counter, game manager | Core game loop | 🔲 |
| 3 | 21 Jul (Tue) | Core | Recycling bin logic, zone-based sorting, collectible types, win/lose | Playable game | 🔲 |
| 4 | 22 Jul (Wed) | Env | Terrain, trees, rocks, weather VFX, skybox, lighting, audio | Full environment | 🔲 |
| 5 | 23 Jul (Thu) | Polish | HUD, UI cards, announcements, SFX integration, boundaries, cursor lock | Polished game | 🔲 |
| 6 | 24 Jul (Fri) | Final | User testing (solo), generate docs, record demo video, submit by 1pm | **SUBMISSION** | 🔲 |

---

## Day 6 — Submission Day (Fri 24 Jul)

| Time | Task |
|------|------|
| 08:00–09:00 | Final bug fixes, test run-through |
| 09:00–10:00 | User testing (solo) — document results |
| 10:00–11:00 | Generate design doc, user testing doc, demo script |
| 11:00–12:00 | Record demo video (2 min max) |
| 12:00–12:45 | Final review, package submission |
| **13:00** | **DEADLINE** |

---

## Feature Comparison — Old vs New Project

| Feature | Old Project (4 days) | New Project (5 days) | Status |
|---------|---------------------|---------------------|--------|
| Player movement | ✅ Day 1 | ✅ Already done | — |
| Camera look | ✅ Day 1 | ✅ Already done | — |
| Jump/sprint | ✅ Day 1 | ✅ Already done | — |
| Pickup/drop | ✅ Day 1 | 🔲 Day 2 | — |
| Scoring | ✅ Day 1 | 🔲 Day 2 | — |
| Lives system | ✅ Day 3 | 🔲 Day 2 | — |
| Game manager | ✅ Day 1 | 🔲 Day 2 | — |
| Recycling bins | ✅ Day 1 | 🔲 Day 3 | — |
| Zone logic | ✅ Day 4 | 🔲 Day 3 | — |
| Collectibles | ✅ Day 1 | 🔲 Day 3 | — |
| Terrain/trees | ✅ Day 4 | 🔲 Day 4 | — |
| Weather VFX | ✅ Day 4 | 🔲 Day 4 | — |
| Audio | ✅ Day 1 | 🔲 Day 4 | — |
| HUD/UI | ✅ Day 3 | 🔲 Day 5 | — |
| Design doc | ✅ Day 5 | 🔲 Day 6 | — |
| User testing | — | 🔲 Day 6 | — |
| Demo video | — | 🔲 Day 6 | — |

---

## Simplified Scope (Compared to Prototype)

| Aspect | Old Project | New Project |
|--------|-------------|-------------|
| Zone system | 9 zones | 3 zones |
| Prefabs | 64 | 10–15 |
| Scripts | 25 | 8–10 |
| Collectible types | 3 (bottle, plant, toy) | 3 (bottle, plant, toy) |
| Weather | Complex (rain, wind, grass) | Basic (rain + ambient) |
| HUD | Code-based | UI card system (TMP) |

---

## Risk Assessment

| Risk | Impact | Mitigation |
|------|--------|------------|
| Friday morning bugs | High | Test thoroughly Thursday night |
| Doc generation takes too long | Medium | Script pre-written, just run it |
| Demo video recording issues | Medium | Record Thursday night as backup |
| Scope creep | High | Stick to 3 zones, 4 collectible types, basic weather |
| Testing alone | Medium | Systematic checklist, test each feature |
