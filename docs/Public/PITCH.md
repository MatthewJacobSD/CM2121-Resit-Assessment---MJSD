# CM2121 Eco Rescue FPS — Game Pitch

**Module:** CM2121 — 3D Reconstructive Techniques
**Project:** Eco Rescue FPS (Unity + photogrammetry scans)
**SDG focus:** SDG 12 (Responsible Consumption), SDG 13 (Climate Action), SDG 15 (Life on Land)

---

## Elevator Pitch

The player starts with **0 points** and **no items collected**. Using a first-person controller, they explore a terrain scattered with **photogrammetry-scanned plants, toys, and plastic bottles**, navigating **randomly placed rocks** that cause a stumble or a full fall depending on rock size.

Collecting items changes the **weather**:
- **Plants** → sunny skies and a **movement speed boost**
- **Toys or bottles** → rainy weather and a **movement slow effect**

The goal is to throw **all plants into a single recycling bin** within **5 minutes**, scoring at least **1 point** to win. Plants award **+10** each. Toys cost **-20** (tracked on the HUD). Bottles cost **-5**. Picking up plants **in sequence** without touching toys or bottles triggers a **+40 CRIT chain bonus**.

The player can **walk, sprint, jump, crouch, slide, and dash** across the level. **Sound effects and visual feedback** (rain particles, weather lighting, UI messages) clarify actions and reinforce the environmental theme.

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
