# CM2121 ChainFragrance — Demo Video Script

**Duration:** 2 minutes max
**Narrator:** Matthew Jacob SD (2506116)
**University:** Robert Gordon University
**Date:** 24 July 2026

---

## Script

### 0:00–0:15 — Introduction

"Hi, I'm Matthew, and this is ChainFragrance, a recycling sorting game built in Unity 6 URP. The game addresses SDG 12: Responsible Consumption and Production by teaching players about proper recycling habits through interactive gameplay."

### 0:15–0:35 — Photogrammetry Pipeline

"The game features 8 photogrammetry-scanned objects. I scanned physical objects using photogrammetry, cleaned the meshes in Blender, and exported them as OBJ files with textures. These include plastic bottles, plant vases, dog plushies, a nature recycle bin, and stairs — all imported into Unity with URP materials."

**Show:** Raw scan -> Blender cleanup -> Unity import -> in-game objects

### 0:35–1:05 — Gameplay Mechanics

"The core loop: explore the environment, pick up scattered items with E, and sort them into three separate recycling bins — one for plants, one for toys, one for bottles. Plants go in the plant bin for +10 points. Toys in the wrong bin cost a life and -20 points. Bottles in the wrong bin cost -5 points. Chain multiple plant recyclings for a +40 bonus. Players have 5 lives and 5 minutes to recycle all plants."

**Show:** Pickup -> identify correct bin -> throw in -> score update -> chain bonus

### 1:05–1:30 — Weather System and Movement Effects

"The weather is proximity-based — it responds to what you're holding and nearby recycling bins. Sunny weather is the default with a speed boost. When holding an item, it shifts to rainy, slowing you down. If you approach a bin that rejects your item, a storm triggers with heavy rain, lightning, wind, and a dark overlay, while your movement is reduced. Wind zones and wind particles also increase with weather intensity. Footstep audio is surface-based — walking on water always plays splashing sounds, and after leaving water in sunny weather, your footsteps gradually dry out over 5 seconds."

**Show:** Walk near items -> weather changes -> pick up wrong item near bin -> storm triggers

### 1:30–1:45 — UI and HUD

"The UI uses a card-based system with TextMesh Pro. The HUD displays lives, a countdown timer that turns red at 30 seconds, current score, high score, and contextual announcements. The welcome screen introduces the game, instructions explain controls with a back option, and the end screen shows results with three tiers: perfect, good, and needs improvement."

**Show:** HUD elements, UI flow, end screen

### 1:45–1:55 — Technical Architecture

"The codebase uses 29 modular scripts. All systems communicate through C# events for clean decoupling. The proximity-based weather uses a threat detection system that monitors held items and nearby recycling bins, adjusting weather, movement speed, and surface-aware footstep audio accordingly."

**Show:** Script folder structure

### 1:55–2:00 — Conclusion

"ChainFragrance demonstrates how photogrammetry and game design can create educational experiences that promote sustainability. Thank you for watching."

---

## Timestamps

| Time | Topic | Visual |
|------|-------|--------|
| 0:00 | Introduction | Title screen / game view |
| 0:15 | Photogrammetry | Scan pipeline, models |
| 0:35 | Gameplay | Pickup, sort, three bins, score |
| 1:05 | Weather & Movement | Proximity detection, speed effects |
| 1:30 | UI/HUD | Cards, HUD elements |
| 1:45 | Technical | Scripts, architecture |
| 1:55 | Conclusion | Final gameplay shot |

---

## Recording Notes

- Use screen recording software (OBS Studio)
- Capture at 1080p, 30fps, MP4 format
- Narrate clearly and concisely
- Keep total duration under 2 minutes (penalty for exceeding)
- Show actual gameplay, not editor views
- Highlight photogrammetry objects prominently
- Show weather changing as you approach non-recyclable items

---

## Gen AI Acknowledgement

I acknowledge use of opencode from https://opencode.ai to assist with script generation and documentation. I entered prompts on 24 July 2026 for: demo video script drafting based on current game features including proximity-based weather and movement effects, timestamp planning, and recording instructions. Content was used to create a structured walkthrough covering all assessment requirements.
