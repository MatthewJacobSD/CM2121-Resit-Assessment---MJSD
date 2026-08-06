# CM2121 Eco Rescue FPS — User Testing Report

**Project:** Eco Rescue FPS — SDG 12 Recycling Game  
**Tester:** Matthew Jacob SD  
**Date:** 30 July 2026

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

Although extensive testing and iteration were carried out, progress remained slower than anticipated. I am satisfied that I successfully produced a functional demonstration showcasing the core mechanics of the intended game. However, I am not satisfied with the overall quality of the final design, as it falls short of the original vision.

Development began approximately one month before submission. Given additional development time, the project could have been significantly improved in terms of gameplay polish, environmental design, UI quality and overall presentation.

Despite following tutorials and researching different implementations, I was unable to fully integrate the weather system so that it reacted dynamically to player recycling actions.

The most successful aspect of the project remains the implementation of the player movement and interaction systems.

---

## Known Limitations

| Issue | Impact | Status |
|-------|--------|--------|
| Weather system not triggering | Weather VFX and transitions do not activate | Needs debugging |
| Jump not functional (some tests) | Player unable to jump in certain scenarios | Needs investigation |
| Audio balancing inconsistent | Volume levels vary between clips | Needs adjustment |
| Object placement spread too far | Exploration time exceeds game timer | Needs rebalancing |
| HUD references incomplete in scene | Some text elements shown as fallback | Inspector wiring needed |
| Pause Menu requires Editor setup | Must run Setup UI tool before use | One-time setup |

## Future Improvements & Recommendations

- Resolve weather system triggering and transitions
- Improve environmental readability and player guidance
- Balance audio levels across all clips
- Add additional environmental feedback
- Improve objective guidance for players
- Refine UI animations and transitions
- Increase environmental variety
- Improve recycling feedback (particle effects, sounds)
- Optimise object placement density
- Continue refactoring scripts into a modular architecture
- Perform broader playtesting with additional users
- Optimise overall project performance
- Improve visual polish and gameplay presentation
