# CM2121 ChainFragrance — User Testing Report

**Project:** ChainFragrance — SDG 12 Recycling Game  
**Tester:** Matthew Jacob SD (2506116)  
**Date:** 6 August 2026 (updated)

---

## Peer Testing Results

### Tester 1: Rose

**Rating:** 6/10

| Aspect | Feedback |
|--------|----------|
| Movement | Works correctly |
| Visual style | Cute and appealing environment |
| Functionality | Project does not feel fully functional |
| Bugs | Movement bugs encountered |
| Clarity | Objective unclear despite instructions being present |
| Audio | Inconsistent and unbalanced |
| Environment | Lacks guidance in guiding the player |

**Summary:** Good effort with potential, but requires refinement in gameplay clarity, audio balancing and overall polish.

---

### Tester 2: McJames

| Aspect | Feedback |
|--------|----------|
| Movement | Works correctly |
| Core mechanics | Understandable and functional |
| Audio design | Feels unusual, though it fits the atmosphere |
| Object placement | Collectable objects positioned too far apart |
| Game timer | Could be increased to improve exploration and completion |

**Summary:** A longer timer and improved object placement would significantly improve gameplay flow.

---

### Tester 3: Gideon

| Aspect | Feedback |
|--------|----------|
| Movement | Works correctly |
| Recycling mechanic | Throwing objects into bins is enjoyable |
| Jump functionality | Not working during testing |
| Weather system | Did not activate during testing |
| Weather potential | Would have made gameplay more engaging if functional |

**Summary:** The recycling mechanic is enjoyable, but the weather feature requires debugging.

---

## Developer Testing Log (Self Evaluation)

### Overall Design

The overall design is acceptable but below the standard originally envisioned. Considering the available equipment and time constraints, this represents the best implementation achievable within the project period.

### Development Challenges

| Challenge | Impact |
|-----------|--------|
| Limited lab access | Entire development on personal laptop due to Unity version compatibility |
| Defining scope around SDG theme | More difficult than expected to scope appropriately |
| Early development | Movement, jumping and interaction progressed smoothly |
| Environmental assets + weather | Significantly increased project complexity |

### Weather System

Although the weather system has largely been implemented, it does not currently operate as intended. Based on testing, the issue appears to be either:

- An Inspector configuration problem
- Missing object references
- An underlying scripting issue preventing weather transitions from triggering correctly

### Reliable Systems

The following systems have remained functional throughout development:

- Player movement
- Sprinting
- Player interaction
- Object collection
- Basic recycling mechanics

### Project Organisation

The folder structure and script architecture were reorganised multiple times to improve maintainability and make project assets easier to locate.

### Assets Used

| Asset | Usage |
|-------|-------|
| Terrain URP Free Asset | Extensively used for terrain textures, soil and water plane |
| Rock Builder Free Asset | Minor use; Terrain asset better suited the intended environment |

### UI Improvements

The Pause Menu and redesigned HUD were introduced to improve gameplay clarity and provide a cleaner user experience. Time limitations prevented these systems from being refined further.

### Overall Reflection

After the final implementation pass, the weather system now reacts dynamically to player recycling behaviour with a 4-state progression (Sunny → Rain → Heavy Rain → Storm). The player grounding issue has been resolved, the bin types are correctly configured, and all gameplay audio uses the curated Optimized library.

The most successful aspects of the project are:
- Player movement and interaction systems
- Dynamic weather system with progressive transitions
- Recycling mechanics with correct acceptance matrix
- Event-driven UI and HUD system

---

## Known Limitations

| Issue | Impact | Status |
|-------|--------|--------|
| HUD scrolling not implemented | Content overflow not handled | Requires Unity Editor setup |
| Audio volume slider uses global AudioListener | Per-source control not available | Minor UX issue |
| 8 toy instances may need repositioning | Some items hard to reach | Editor adjustment |
| Legacy ambient AudioSources on inactive objects | Console warnings only | No gameplay impact |

## Future Improvements & Recommendations

- Add HUD scrolling for content overflow
- Implement per-source audio volume control
- Add post-processing volume for visual polish
- Refine UI animations and transitions
- Perform broader playtesting with additional users
