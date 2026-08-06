# CM2121 Eco Rescue FPS — Game Rules

## Overview

Eco Rescue FPS is a first-person Unity game where the player collects photogrammetry-scanned plants, toys, and bottles, then throws them into a recycling bin within five minutes. Weather and movement speed react to eco-friendly vs polluting choices.

---

## Core Mechanics

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
