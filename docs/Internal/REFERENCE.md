# CM2121 ChainFragrance — Reference (Old Project)

Consolidated from: AUDIT.md, GAMEPLAY_UPDATE.md, DESIGN_UPDATE_AUDIT.md, VISUAL_FIX_AUDIT.md, IMPLEMENTATION_LOG.md, COMPLETION_FINAL.md, PROJECT_HANDOFF.md, PROJECT_AUDIT_CURRENT.md, POSSIBLE_UPDATES.md

Source project: `C:\A\resit-assessment` (Unity 6 URP, 18 commits, v14)

---

## Game Rules

| Rule | Value |
|------|-------|
| Time limit | 5 minutes |
| Lives | 5 max |
| Plant in bin | +10 points, +0.5 life (capped at 5) |
| Toy in bin | −20 points, −0.5 life |
| Bottle in bin | −5 points, −0.5 life |
| Plant chain CRIT | +40 (pick up 2+ plants in a row without toys/bottles) |
| Win | All plants in bin + score ≥ 1 + lives > 0 + time remaining |
| Perfect win | All plants in bin + no toys or bottles in bin |
| Lose | Lives = 0, or time expires before all plants binned, or final score < 1 |

---

## Scoring & Lives

| Event | Points | Lives |
|-------|--------|-------|
| Plant lands in bin | +10 | +50% of current (if not full), capped at 5 |
| Toy lands in bin | −20 | −0.5 |
| Bottle lands in bin | −5 | −0.5 |
| Item misses bin | 0 | 0 |
| Pick up plant | 0 (chain CRIT still possible) | — |
| Pick up toy/bottle | 0 | — |
| Lives reach 0 | Game over | — |

---

## Weather & Movement

| Item Collected | Weather | Movement Effect |
|----------------|---------|-----------------|
| Plant | Sunny | +20% speed boost |
| Toy or Bottle | Rainy | Slow effect |

---

## Controls

| Action | Input |
|--------|-------|
| Move | WASD / Arrow keys |
| Sprint | Left Shift |
| Jump | Space |
| Crouch/Slide | Left Ctrl / C |
| Dash | Q |
| Pickup | Walk into item (trigger) or E (raycast) |
| Aim | RMB hold |
| Throw | LMB |
| Drop | G |
| Free mouse | Esc |
| Re-lock mouse | Click / WASD |

---

## Photogrammetry Assets

| Object | Type | Points | In-game role |
|--------|------|--------|--------------|
| Vase Plant | Plant | +10 | Collectible |
| Vase Pot Plant | Plant | +10 | Collectible |
| Bonsai | Plant | +10 | Collectible |
| Dog Plushie | Toy | −20 | Collectible (penalty) |
| Dog Plushie 2 | Toy | −20 | Collectible (penalty) |
| Plastic Bottle | Bottle | −5 | Collectible (penalty) |
| Recycling Trash Bin | Bin | — | Goal container |
| Stairs | Prop | — | Environment decoration |

Pipeline: RawScans → CleanedScans → Blender (.blend) → Unity (OBJ + MTL + textures)

---

## Camera Settings

| Setting | Value |
|---------|-------|
| FOV (normal) | 68 |
| FOV (aim) | 50 |
| Near clip | 0.15 |
| Far clip | 300 |
| Mouse look | Smooth (Slerp) |
| Head bob | Subtle on walk/sprint |
| Aspect ratio | 16:9 (enforced via CameraAspectEnforcer) |

---

## Visual Fixes (from old project)

| Issue | Fix |
|-------|-----|
| Motion blur streaking | Intensity → 0 |
| Ultrawide stretch | 16:9 letterbox |
| Trees pass-through | Solid capsule/box colliders |
| Bin pass-through | Solid box on body, trigger on BinTrigger child |
| Walk off terrain | Invisible walls at ±94 + PlayerBoundsGuard |
| Flat lighting | Sun 48°/−32°, ambient 1.25, fog 55–150 |
| Shrubs blocking | Collider-free (decoration only) |

---

## Bug Fixes (from old project)

| Issue | Cause | Fix |
|-------|-------|-----|
| Player floating | Spawned at y=1, ground at y=0 | SnapToGround on Start |
| Backward drift | Trip velocity not reset | Zero horizontal when grounded |
| Items not collectible | E + centre raycast only | Added walk-up trigger + GetComponentInParent |
| Plain ground | Flat URP colour | Grass_A albedo + normal, tiled 10× |
| Plain HUD | White text on transparent | Semi-transparent panels |

---

## Environment Layout (v14)

| Setting | Value |
|---------|-------|
| Ground size | 200×200 |
| Player spawn | (0, −35) |
| Bin position | (0, 45) |
| Collectible spacing | 12 m minimum |
| Trees | 28 around perimeter, radius 68–88 |
| Rocks | 5 small + 2 big, 14 m separation |
| Collectibles | 14 (6 plants + 4 toys + 4 bottles) |

---

## Audio (SFX)

19 royalty-free clips from Freesound (CC0):

- **Ambient:** rain/thunder, forest night, spring stream
- **Footsteps:** splashing, stumble, suburban, dirt falling
- **Object interaction:** drop, recycle bin, bottle drop, bottle in trash
- **Game feedback:** fail sound, victory chime, error sound

---

## Outstanding TODOs (from old project)

| Priority | Task | Status |
|----------|------|--------|
| P0 | Regenerate scene | ✅ Done |
| P1 | Record demo video | 🔲 Pending |
| P1 | Personalise design doc | 🔲 Pending |
| P1 | Personalise user testing doc | 🔲 Pending |
| P2 | Standalone build test | 🔲 Pending |
| P2 | TextMeshPro font upgrade | 🔲 Pending |
| P3 | Full Unity Terrain | 🔲 Pending |
| P3 | URP post-processing | 🔲 Pending |

---

## Grade Target Notes

| Criterion | Weight | B Target | A Target |
|-----------|--------|----------|----------|
| Design | 20% | Good description, assets, SFX/VFX, map | Thorough purpose, timeline, picture per asset, alternate list |
| Implementation | 50% | Photogrammetry in level, interactive mechanics, SFX, VFX | All B + polished, multiple scans, strong SDG delivery |
| User Testing | 10% | Tested, bugs resolved, photogrammetry tested | Thorough testing, wide scenarios, clear bug resolution |
| Demonstration | 20% | Clear video: game, assets, mechanics explained | Comprehensive step-by-step, photogrammetry pipeline detailed |

---

## SDG Alignment

| SDG | How the game teaches it |
|-----|-------------------------|
| SDG 12 | Sort plants from toys/bottles; recycling bin as correct disposal |
| SDG 13 | Rainy weather as pollution consequence of wrong waste choices |
| SDG 15 | Rescuing scanned plants; sunny weather as healthy land/ecosystem |
