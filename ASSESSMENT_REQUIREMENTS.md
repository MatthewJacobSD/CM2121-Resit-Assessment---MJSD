# CM2121 Eco Rescue FPS — Assessment Requirements

## Module Info

- **Module Code:** CM2121
- **Module Title:** 3D Reconstructive Techniques
- **Academic Year:** 2025-2026
- **Semester:** 2
- **Module Coordinator:** Verda Munir
- **Deadline (Extension):** Thursday 6 August 2026 @ 1pm

---

## Assessment Breakdown

| Component | Weight | Word/Page Limit | Status |
|-----------|--------|-----------------|--------|
| Design | 20% | 10 pages (penalty applies) | ✅ docs/DESIGN_DOCUMENT.md |
| Implementation | 50% | — | ✅ 30 scripts, 8 models, full game loop, pause menu, settings |
| User Testing | 10% | 2 pages (penalty applies) | ✅ docs/USER_TESTING.md (templates ready) |
| Demonstration | 20% | 2 min max (penalty applies) | ✅ docs/DEMO_VIDEO_SCRIPT.md |

---

## Assessment Brief Requirements

### What the project must do:

1. **SDG Alignment** — Align with at least one UN Sustainable Development Goal
   - ✅ SDG 12: Responsible Consumption & Production (Recycling)

2. **Photogrammetry/LIDAR Assets** — Incorporate scanned real-world objects
   - ✅ 8 scanned models (Bonsay, Dog Plushie, Plastic Bottle, Recycle Bin, etc.)

3. **Interactive Elements** — Gameplay mechanics that reinforce the SDG theme
   - ✅ Pickup/drop/throw mechanics (E/Q/right-click)
   - ✅ Recycling bin type-checking (plant/toy/bottle bins)
   - ✅ Scoring system with chain bonuses

4. **Narrative/Direction** — Tasks/missions to entice players
   - ✅ Clear objective (recycle all plants within 5 minutes)
   - ✅ Lives system (5 lives, lose on wrong bin)

5. **SFX and VFX** — Audio and visual effects for immersion
   - ✅ 19 audio clips with ambient crossfade and SFX
   - ✅ Weather VFX (rain, clouds, lightning, wind, storm overlay)
   - ✅ Movement effects (sunny speed boost, rainy slowdown, storm heavy slowdown)
   - ✅ WindZone and wind particles per weather state

6. **Proper Crediting** — IP regulations for free resources
   - ✅ Audio from freesound.org (CC0 license)

---

## Design Document Requirements (10 pages max)

| Section | Required Content | Status |
|---------|------------------|--------|
| Contents Page | List of sections | ✅ |
| UN SDG Theme Overview | SDG 12 explanation, recycling importance | ✅ |
| Project Plan | Timeline, milestones, deliverables | ✅ |
| Mood Boards | Visual references, style guide | ✅ |
| Initial Sketches | Environment layout, UI mockups | ✅ |
| Task List | What needs to be built | ✅ |
| Scanned Objects | List of photogrammetry assets with images | ✅ |
| Consent Form | If scanning humans (not applicable) | N/A |
| Asset Images | Images of assets with references | ✅ (in doc) |

---

## Implementation Requirements (50%)

| Requirement | Description | Status |
|-------------|-------------|--------|
| Environment Scan | Multiple objects scanned and brought into Unity | ✅ 8 models |
| Interactive Mechanics | Pickup, sort, recycle gameplay | ✅ Full game loop |
| SFX | Sound effects for actions | ✅ 19 clips wired |
| VFX | Visual effects (weather, particles) | ✅ 4 weather states with proximity detection |
| Movement | Weather-based speed modifiers | ✅ Sunny/rainy/stormy speed effects |
| Narrative/Tasks | Clear objectives for players | ✅ 5min timer, 5 lives, chain bonus |

### Grade Criteria (Implementation)

| Grade | Criteria |
|-------|----------|
| A | All assets created per design doc, excellent populated level, best photogrammetry practices, optimized |
| B | Most assets created per design doc, very good populated level, very good photogrammetry practices |
| C | Some assets not created per design doc, good populated level, good photogrammetry practices |
| D | Most assets not created per design doc, satisfactory level, satisfactory photogrammetry practices |
| F | Complete failure, no trace of photogrammetry reconstruction |

---

## User Testing Requirements (10%)

| Requirement | Description | Status |
|-------------|-------------|--------|
| Test at least 2 users | Document 2 user testing sessions | ✅ Templates ready (24 Jul) |
| Test photogrammetry | Assess proper integration into game | ✅ Included in test plan |
| Document results | Identify and resolve errors/bugs | ✅ USER_TESTING.md |

### Grade Criteria (User Testing)

| Grade | Criteria |
|-------|----------|
| A | All assets tested thoroughly, covers wide range, photogrammetry tested, documents all results |
| B | Most assets tested, covers essential scenarios, some bugs may need addressing |
| C | Some assets not tested, adequate scenarios, more testing could be beneficial |
| D | Most assets not tested, tests should be expanded, photogrammetry barely tested |
| F | Complete failure, no form of testing |

---

## Demonstration Requirements (20%)

| Requirement | Description | Status |
|-------------|-------------|--------|
| Video walkthrough | Narrated (audio/video) walkthrough | ✅ Script ready |
| Functionality | Explain game mechanics | ✅ Script covers all mechanics |
| Technical details | Explain photogrammetry process | ✅ Script includes pipeline |
| Max 2 minutes | Penalty for exceeding | ✅ Script is 2 min |

### Grade Criteria (Demonstration)

| Grade | Criteria |
|-------|----------|
| A | Comprehensive, highly detailed, step-by-step guide, clearly explains game, assets, mechanics, SFX, VFX |
| B | Detailed, step-by-step guide, contains necessary components, efficient explanation |
| C | Basic, general overview, contains necessary components, basic overview |
| D | Vague, limited detail, lack of detail in steps |
| F | Incomplete or does not exist |

---

## Word Limit Statement

- **Design:** 10 pages max (penalty if exceeded by >10%)
- **User Testing:** 2 pages max (penalty if exceeded)
- **Demonstration:** 2 min max (penalty if exceeded)

### Included in word count:
- Main text (Introduction, Literature Review, Methodology, Results, Discussion, Analysis, Conclusions, Recommendations)
- Headings and subheadings
- In-text citations
- Footnotes
- Quotes and quotations
- Tables (mainly text content)

### Excluded from word count:
- Cover/Title Page
- Executive Summary/Abstract
- Contents Page
- List of Abbreviations/Acronyms
- List of Tables/Figures
- Tables (mainly numeric content)
- Figures
- Reference List/Bibliography
- Appendices
- Glossary

---

## Gen AI Usage

- **Category:** Full AI (Gen AI use authorised throughout)
- **Requirement:** Must acknowledge use in submission
- **Template:** "I acknowledge use of [tool] from [url] to [use]. I entered the following prompts on [date]: [prompts] and [description of how content was used]."

---

## Submission Checklist

| Item | Format | Status |
|------|--------|--------|
| Design Document | 10 pages max | ✅ docs/DESIGN_DOCUMENT.md |
| Implementation | Unity project (zip) | ✅ 29 scripts, 8 models, full game |
| User Testing | 2 pages max | ✅ docs/USER_TESTING.md |
| Demo Video | 2 min max | 🔲 Record on 24 Jul |
| Gen AI Acknowledgement | Included in docs | ✅ In all 3 documents |
