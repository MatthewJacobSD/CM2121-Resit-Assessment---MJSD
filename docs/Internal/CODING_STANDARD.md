# CM2121 Eco Rescue FPS — Project Coding Standard

This document defines the coding conventions for the CM2121 Eco Rescue FPS project.
All existing code follows this standard (see the final organisation pass) and any
**new scripts created in the future must follow it** unless there is a strong reason
not to.

## 1. Namespaces

The project intentionally uses the **global namespace** (no `namespace` declarations).
All scripts are referenced by class name throughout scenes and other scripts. Do not
introduce namespaces unless the whole project is migrated at once.

## 2. Using Directives

- Place all `using` directives at the top of the file, before the class declaration.
- Order: `System.*` first, then third-party libraries (e.g. `TMPro`), then
  `UnityEngine.*`, then `UnityEditor.*` (Editor scripts only).
- Remove any using directive that is not actually used.

```csharp
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
```

## 3. Class Layout

Every class follows the same order (omit sections that do not apply):

1. `using` directives
2. XML class `<summary>` and attributes (`[RequireComponent]`, `[Serializable]`, etc.)
3. Constants (`#region Constants`)
4. Serialized fields (`#region Serialized Fields`, grouped with `[Header]`)
5. Private fields (`#region Private Fields`)
6. Public properties (`#region Public Properties`)
7. Events (`#region Events`)
8. Unity lifecycle methods (`#region Unity Lifecycle`)
9. Public methods (`#region Public Methods`)
10. Private methods (`#region Private Methods`)
11. Event callbacks / handlers
12. Utility / helper methods (`#region Utility`)

## 4. `#region` Usage

- Use `#region` blocks in **medium and large scripts** (roughly 120+ lines or scripts
  covering multiple concerns) to allow IDE collapsing.
- **Do not** add regions to small scripts (< ~60 lines) — they add noise without value.
- Standard region names:

| Region name | Contents |
|-------------|----------|
| `Constants` | `const` values and magic-number replacements |
| `Serialized Fields` | All `[SerializeField]` fields |
| `Private Fields` | Non-serialized private fields |
| `Public Properties` | Read-only properties / getters |
| `Events` | C# `event` and Unity `UnityEvent` declarations |
| `Unity Lifecycle` | `Awake`, `OnEnable`, `Start`, `Update`, `LateUpdate`, `OnDisable`, `OnDestroy`, physics/trigger callbacks |
| `Public Methods` | Methods callable from other scripts / Inspector |
| `Private Methods` | Internal logic |
| `Utility` | Small helpers, gizmos, validation |

## 5. Inspector Layout

- Group related serialized fields under `[Header("...")]`.
- Add `[Space]` between major sections only when a header is not enough.
- Add `[Tooltip("...")]` only when the purpose of a field is not obvious from its name.
- Use `[SerializeField, Range(min, max)]` for bounded numeric values.
- Prefer `private` fields with `[SerializeField]` over `public` fields. Expose read-only
  properties instead of public variables.

```csharp
[Header("Movement")]
[SerializeField] private float walkSpeed = 5f;
[SerializeField, Range(0f, 1f)] private float airControl = 0.6f;

[Tooltip("Cached CharacterController reference. Auto-filled in Awake if empty.")]
[SerializeField] private CharacterController characterController;
```

## 6. XML Documentation

- Every **public class** gets a `<summary>` describing its responsibility.
- Every **public method** gets a concise `<summary>` (add `<param>` only when the
  parameter meaning is not obvious).
- Keep summaries short and professional. Do not document private members.
- Never generate documentation for code that is self-evident.

```csharp
/// <summary>
/// Handles pickup, drop and throw interactions for the player.
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    /// <summary>Drops the currently held item with a small forward nudge.</summary>
    public void Drop() { ... }
}
```

## 7. Inline Comments

- Add single-line comments only where they explain **why**, not what.
- Do not comment self-explanatory code.
- Use `//` for comments (not `///`), placed above the line they describe.

```csharp
// Snap small downward velocities so the controller stays glued to slopes.
if (isGrounded && velocity.y < 0)
    velocity.y = -2f;
```

## 8. Section Separators

Use the `// ----------------------` separator only where a file gains real readability
from it and does **not** use `#region`. Do not stack separators inside files that
already use regions.

```csharp
// ----------------------
// Player Movement
// ----------------------
```

## 9. Naming Conventions

| Item | Convention | Example |
|------|-----------|---------|
| Classes / types | PascalCase | `PlayerMovement` |
| Public methods / properties | PascalCase | `SetSpeedModifier` |
| Private fields | camelCase (no underscore prefix — project style) | `moveInput` |
| Serialized fields | camelCase (no underscore prefix) | `walkSpeed` |
| Constants | PascalCase | `GroundedSnapVelocity` |
| Enums and values | PascalCase | `WeatherState.State.Sunny` |
| Input actions | PascalCase | `moveAction` |

Do not use abbreviations (`rb` → `rigidbody`) and prefer descriptive names. The
project intentionally does **not** use `_camelCase` for private fields; keep the
existing style.

## 10. Formatting

- 4-space indentation (no tabs).
- Opening brace on the same line as the declaration (Allman variant: brace on its own
  line — pick one and keep it consistent; this project uses the **K&R** style with the
  brace on the same line for blocks and methods).
- Single space around binary operators and after commas.
- Remove trailing whitespace and preserve logical blank-line separation.
- Use expression-bodied members (`=>`) for single-line properties and methods.

## 11. Encapsulation

- Keep fields `private` unless there is a real need for `public`.
- Use `[SerializeField]` for Inspector references and configuration.
- Expose read-only properties (`public int Score => score;`).
- Make access modifiers explicit on every member.

## 12. Magic Numbers

Replace repeated or non-obvious literal values with named constants or serialized
fields. Do not rename existing serialized defaults.

```csharp
private const float GroundedSnapVelocity = -2f;
private const float SprintInputThreshold = 0.5f;
```

## 13. Error Handling & Validation

- Null-check `[SerializeField]` references that are optional (`?` access).
- In `Awake`, auto-resolve references that can be fetched from the component or scene.
- Log a `Debug.LogWarning` only when a **required** reference is missing and the
  behaviour would otherwise fail silently.
- Avoid throwing exceptions in gameplay code.

## 14. Performance

- Cache `GetComponent`, `Find*` and `GetComponentsInChildren` results in `Awake`.
- Keep `Update`/`LateUpdate` lightweight; avoid per-frame lookups.
- Remove unused variables, methods and dead code.
- Never call `FindObjectOfType` in `Update`.

## 15. File Organisation

Mirror the existing folder structure — new scripts go in the folder matching their
responsibility:

```
Assets/Scripts/
├── Core/          GameManager, ScoreManager, AudioManager, AutoSpawner
├── Player/        Movement, Look, Interaction, footsteps, weather movement
├── UI/            UIManager, HUD, prompts, pause menu, weather UI
├── Interaction/   PickupItem, RecycleBinInteractable
├── Weather/       WeatherState, effects system, anchor follower
│   ├── Effects/   Per-state effect components
│   └── Data/      ScriptableObject / parameter data holders
└── Editor/        Editor-only tooling (never referenced at runtime)
```

## 16. Reusable Template

A copy-paste template implementing this standard is provided at
`docs/NewScriptTemplate.cs.txt`. Copy it, rename the file and class, and fill in the
sections.

## 17. Scenes / Architecture Notes

- `Florance.unity` is the build target scene.
- Managers are singletons accessed through `X.Instance` (`GameManager`,
  `ScoreManager`, `AudioManager`).
- Systems communicate through C# events (`OnWeatherChanged`, `OnItemRecycled`, etc.).
- Keep the global-namespace convention and event-driven coupling described above.
