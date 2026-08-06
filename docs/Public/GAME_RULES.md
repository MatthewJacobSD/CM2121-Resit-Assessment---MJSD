# CM2121 ChainFragrance — Game Rules

## Overview

ChainFragrance is a first-person Unity game where the player collects photogrammetry-scanned plants, toys, and bottles, then throws them into the correct recycling bin within five minutes. Weather and movement speed react dynamically to recycling behaviour.

---

## Core Mechanics

| Rule | Value |
|------|-------|
| Time limit | 5 minutes |
| Lives | 5 max |
| Plant in Nature bin | +20 points |
| Bottle in Plastic bin | +20 points |
| Toy in General Waste | +25 points |
| Wrong recycle | Score penalty + lose 1 life |
| Plant chain bonus | +40 (2+ consecutive correct plant recycles) |
| Win | All category objectives met + time remaining |
| Failure | Lives depleted or score drops below zero |

---

## Bin Acceptance Matrix

| Item | Nature Recycling | Plastic Recycling | General Waste |
|------|-----------------|-------------------|---------------|
| Plant | +20 | -45 | -20 |
| Bottle | -15 | +20 | +15 |
| Toy | -25 | -15 | +25 |

---

## Weather System

| Condition | Weather | Effect |
|-----------|---------|--------|
| No item held | Sunny | Speed 1.2x, no ambient audio |
| Near wrong bin (15m) | Rain | Speed 0.75x, AMB_Rain.wav |
| Closer to wrong bin (10m) | Heavy Rain | Speed 0.6x, AMB_StrongRain.wav |
| Very close to wrong bin (6m) | Storm | Speed 0.45-0.75x, AMB_Storm.wav, wind push |
| Near correct bin (5m) | Calm | Weather calms progressively |
| Correct recycle | Sunny | Immediate return to calm |
| Wrong recycle | Storm | 2s feedback, then calm |
| Lose | Lives = 0, or time expires before all plants binned |

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

## Collectibles

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

---

## Environment

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

## SDG Alignment

| SDG | How the game teaches it |
|-----|-------------------------|
| SDG 12 | Sort plants from toys/bottles; recycling bin as correct disposal |
| SDG 13 | Rainy weather as pollution consequence of wrong waste choices |
| SDG 15 | Rescuing scanned plants; sunny weather as healthy land/ecosystem |
