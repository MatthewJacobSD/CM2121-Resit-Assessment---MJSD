# CM2121 ChainFragrance — Game Pitch

**Module:** CM2121 — 3D Reconstructive Techniques
**Project:** ChainFragrance (Unity + photogrammetry scans)
**Student:** Matthew Jacob SD (2506116)
**University:** Robert Gordon University
**SDG focus:** SDG 12 (Responsible Consumption & Production)

---

## Elevator Pitch

The player starts with **0 points** and **5 lives**. Using a first-person controller, they explore a terrain scattered with **photogrammetry-scanned plants, toys, and plastic bottles**, navigating an open environment with dynamic weather.

**Weather reacts to recycling behaviour:**
- **No item held** → Sunny, calm conditions
- **Approaching wrong bin** → Rain begins, then heavy rain, then storm
- **Wrong recycle** → Full storm with wind push away from the bin
- **Correct recycle** → Sunny, calm restored

The goal is to sort all items into the correct bins within **5 minutes**, earning points for correct recycles and losing lives for wrong ones. A **plant chain bonus** rewards consecutive correct plant recycling.

**Controls:** WASD movement, mouse look, E to pick up, Q to drop, throw while aiming.

**Technologies:** Unity 6 URP, New Input System, TextMesh Pro, dual-source audio crossfade, 21 automated tests.

This demo teaches that **respecting the environment keeps nature healthy (sunny)**, while **careless waste choices cause pollution (rainy)** — aligning with UN Sustainable Development Goals for responsible consumption and climate awareness.

---

## Photogrammetry Pipeline

1. **Capture** — Real-world objects scanned using photogrammetry
2. **Clean** — Models cleaned in Blender (remove noise, fix topology)
3. **Export** — OBJ + MTL + textures exported
4. **Import** — Unity imports with URP materials applied

### Objects Scanned

| Object | Type | Purpose |
|--------|------|---------|
| Vase Plant | Plant | Collectible (+10) |
| Vase Pot Plant | Plant | Collectible (+10) |
| Bonsai | Plant | Collectible (+10) |
| Dog Plushie | Toy | Collectible (−20) |
| Dog Plushie 2 | Toy | Collectible (−20) |
| Plastic Bottle | Bottle | Collectible (−5) |
| Recycling Trash Bin | Bin | Goal container |
| Stairs | Prop | Environment decoration |

---

## Audio

Royalty-free clips from Freesound (CC0):
- Pickup, throw, bin deposit (plant / toy / bottle)
- Trip / fall, CRIT bonus, win, lose
- Sunny forest ambience, rainy thunder ambience

---

## Assessment Brief Alignment

| Brief requirement | Status |
|-------------------|--------|
| Photogrammetry objects in Unity | ✅ 8 objects |
| Interactive mechanics | ✅ FPS + pickup/throw + scoring |
| SDG theme | ✅ Eco weather + recycling |
| SFX | ✅ 19 clips |
| VFX | ✅ Weather, lighting, particles |
| Narrative / missions | ✅ Timed plant rescue mission |
| Design doc (10 pp) | 🔲 To be generated |
| User testing (2 pp) | 🔲 To be generated |
| Demo video (2 min) | 🔲 To be recorded |
