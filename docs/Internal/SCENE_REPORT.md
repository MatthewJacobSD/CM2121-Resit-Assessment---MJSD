# Florance.unity Scene Report

Snapshot of the scene structure and component wiring in
`Assets/Scenes/Florance.unity`, taken **2 August 2026** ahead of the
CM2121 resit submission (due 6 August 2026).

This record exists so any further change to the scene can be verified against
a known-good baseline. All `fileID`s below are the Unity YAML ids used inside
the scene file and can be looked up with a plain text search.

---

## 1. Scene file facts

| Item | Value |
| --- | --- |
| Path | `Assets/Scenes/Florance.unity` |
| Size on disk | 2,013,505 bytes |
| Last modified | 2 Aug 2026 07:39 |
| Runtime errors | Player `NullReferenceException`s (see section 2) |

---

## 2. Runtime-error fixes already applied (on disk)

Both fixes were applied directly to the scene YAML and are still present in
the file at snapshot time.

1. **Dangling `playerControls` references (4 refs).**
   Pointed at the deleted asset `PlayerControl.inputactions`
   (guid `52db6205...`). Replaced with the current asset
   `Assets/Input/ActionsControl.inputactions`
   (guid `e8794d9b3fba38746937503e7bab5d4d`).
   Locations: lines 13459 (UIManager), 64008 / 64027 / 64053 (player scripts).

2. **`PlayerMovement.groundCheck` was `{fileID: 0}`.**
   Wired to the `GroundCheck` transform `{fileID: 2001586296}`
   (line 64036). This is the cause of jump / ground-detection failures.

No gameplay logic was changed. Serialized field names/types are identical to
the pre-refactor commit `6b371f4`.

---

## 3. Component inventory (project scripts)

### UIManager
- Component fileID: **438008474** (host GameObject `UIManager`, 438008473)
- `welcomePanel`    -> `{fileID: 2128927384}` (WelcomeScreen)
- `instructionPanel`-> `{fileID: 1455053195}` (InstructionScreen)
- `hudPanel`        -> `{fileID: 1049536818}`  (HUDScreen)
- `endPanel`        -> `{fileID: 1700447993}`  (CreditsScreen)
- `pauseMenuPanel`  -> `{fileID: 0}`           (**NOT wired**)
- `playerControls`  -> ActionsControl (`e8794d9b3fba38746937503e7bab5d4d`)

### PauseMenuManager (instance 1 — fully wired)
- Component fileID: **1037489632**
- Host GameObject: `PauseMenuManager` (1037489631)
- `pausePanel`      -> `{fileID: 910353431}` (PausePanel)
- `settingsPanel`   -> `{fileID: 1802253831}` (SettingsPanel)
- `confirmExitPanel`-> `{fileID: 2056683490}` (ConfirmationModal)
- `saveProgressPanel`-> `{fileID: 946659743}` (SaveProgressModal)
- `usernameInput`   -> `{fileID: 0}`           (**NOT wired**)
- `volumeSlider`    -> `{fileID: 0}`           (**NOT wired**)
- `currentScoreText`-> `{fileID: 850433620}`

### PauseMenuManager (instance 2 — duplicate / broken)
- Component fileID: **1854047160**
- Host GameObject: `PauseMenuManager` (1854047158) at `(-640, -186)`,
  parent GameObject 859145149
- All panel refs `{fileID: 0}`; only `usernameInput`
  -> `{fileID: 949799755}` (InputField (TMP)) is set.
- This instance is a leftover and should be **deleted**.

### HUDManager
- Component fileID: **1838788584** (host GameObject `HUDManager`, 1838788583)
- `livesText`, `timerText`, `scoreText`, `highScoreText`,
  `announcementText`, `toysText`, `bottlesText` -> all `{fileID: 0}`
- `collectedText` -> `{fileID: 1957344435}`
- (3 score-popup refs also wired.)

### InteractionPromptUI
- Component fileID: **1261081362** (host GameObject `Player`, 1261081353)
- `promptText` -> `{fileID: 0}`  (**NOT wired**)
- `warningText` -> `{fileID: 0}` (**NOT wired**)

---

## 4. UI hierarchy snapshot (key objects)

```
Canvas
└── PanelUI
    ├── WelcomeScreen       (inactive) 2128927384
    ├── InstructionScreen   (inactive) 1455053195
    ├── HUDScreen           (inactive) 1049536818
    │   ├── ScoreUI                   1870190293
    │   ├── WeatherStatus             405516380
    │   ├── CollectablesUI            1927498211
    │   └── TopLevelNavigation        (RectTransform 1951737042)
    └── CreditsScreen       (inactive) 1700447993
└── PauseMenu              (active)   906992089
    ├── PausePanel          (RT 910353432)
    ├── SettingsPanel                 1802253831
    │   ├── VolumeSlider              1835502012 / Slider comp 1835502014
    │   └── InputField (TMP)          949799754 / comp 949799755
    ├── ConfirmationModal   (inactive) 2056683490
    └── SaveProgressModal   (inactive) 946659743
```

`HUDScreen` (1049536818), `PanelUI`, and the modal screens are **inactive**
by design and toggled at runtime.

### Notes / hazards found during snapshot
- `YesButton` (GameObject 1804455384, Button comp 1804455386) has an
  `m_OnClick` whose `m_Target` points at the **PauseMenuManager script asset**
  (`fileID: 11500000, guid: f438914bf2ac32044af113070ac0bb56`) instead of a
  scene instance, with an empty `m_MethodName`. Broken onClick wiring of this
  kind is what `SetupButtons()` in the editor tool rewires.
- Script guids: PauseMenuManager `f438914bf2ac32044af113070ac0bb56`,
  UIManager `b4c8d9e0f1a2b3c4d5e6f7a8b9c0d1e2`,
  HUDManager `c5d9e0f1a2b3c4d5e6f7a8b9c0d1e2f3`,
  InteractionPromptUI `deb6f6cc9fd83bd4b89b15fb9a8d5eff`.

---

## 5. Known gaps (to be fixed)

| Gap | Owner | Fix |
| --- | --- | --- |
| `UIManager.pauseMenuPanel` null | Tool (now fixed) | `SetupPauseMenuManager()` wires it to PauseMenu GO |
| `PauseMenuManager.usernameInput` null (instance 1) | Tool | auto-wires SettingsPanel InputField (949799755) |
| `PauseMenuManager.volumeSlider` null (instance 1) | Tool | auto-wires SettingsPanel Slider (1835502014) |
| HUD stat text refs null | Tool | `SetupHUDManager()` now creates+wires missing texts under HUDScreen |
| `InteractionPromptUI.promptText/warningText` null | Tool | `SetupInteractionPrompt()` creates/wires under HUDScreen |
| Duplicate broken PauseMenuManager (1854047160) | Manual | delete GameObject 1854047158 |
| Broken button onClick targets | Tool | `SetupButtons()` rewires to best PauseMenuManager |

---

## 6. How to apply the fixes

1. Close Unity **without saving** (so the on-disk scene edits in section 2
   are not overwritten), then reopen Unity and open `Florance.unity`.
2. Run **Tools → Setup UI in Current Scene**.
3. Delete the duplicate `PauseMenuManager` GameObject (1854047158).
4. Verify per section 7, then test play.

The editor tool (`Assets/Scripts/Editor/AutoSetupPauseMenu.cs`) now uses
inactive-safe lookups (`FindTransformIncludingInactive`,
`FindGameObjectIncludingInactive`, inactive-safe `FindChildRecursive`), so it
can find all the inactive screens/modals.

---

## 7. Verification checklist

- [ ] Play: no NullReferenceExceptions on the Player / UIManager / HUD.
- [ ] Player jump works (`groundCheck` 2001586296 wired).
- [ ] Pause opens/closes (UIManager.pauseMenuPanel -> PauseMenu).
- [ ] Settings volume slider wired to surviving PauseMenuManager.
- [ ] Username input wired to surviving PauseMenuManager.
- [ ] HUD lives/timer/score/highscore/announcement texts visible & updating.
- [ ] Interaction prompt + warning text show on proximity events.
- [ ] Only one PauseMenuManager remains in the scene.
