# FloranceOverflow — Visibility & Verification Report

Result of the read-only verification pass (Phase 0.5) over `Assets/Scenes/Florance.unity`
and the surrounding assets, **3 August 2026**. The purpose of this report is to
separate **Unity serialisation artefacts** (things that look broken but are
normal) from **genuine problems**, so that fixes target real issues.

**This phase performed no asset modifications.**

---

## 1. Serialisation artefacts vs actual project state

Unity's `.unity` YAML serialises **prefab instances** differently from plain
GameObjects. Three facts, if not understood, produce false "broken project"
conclusions:

1. **Prefab-instance children are not linked through Transform `m_Father`.**
   Each `--- !u!1001` (PrefabInstance) block lists a `m_TransformParent`
   (the transform the instance root hangs under) and a `m_Modifications`
   array describing overridden properties. A text scan that only walks
   `m_Father` links reads the whole environment as "empty" — this is an
   **artefact**, not missing content.
2. **Component fileIDs inside a prefab instance are the *scene-side* ids**, and
   `m_Modifications` entries reference the *prefab-side* ids (`fileID: …,
   guid: <prefab-guid>, type: 3`). Naive GUID checks flag these as "missing".
3. **Inactive GameObjects are fully serialised.** Screens such as Welcome,
   Instruction, HUD, Credits and the pause modals are inactive by design and
   toggled at runtime. Their absence from a visual hierarchy walk is normal.

### What the real scene contains

- 192 GameObjects, 8 root transforms, **36 prefab instances**.
- Instances include: 24 collectibles, 3 bins, `Stairs`, `WaterPlane`,
  probe rigs (`Valley`, `Gulley`, `Lake`, `Global`, `ExtentsLightProbes`,
  `MainLightProbes`, `MorningSun`).
- Environment scaffold: `Environment` (root, world 519/0/2982) → `Terrain`
  and `ScannedItems`/`CollectablesItems`/`Toys`/`Nature`/`Bins`/`ObjectProp`
  containers. `Reflections` and `LightProbes` roots at origin hold the
  probe rigs.

## 2. Prefab instance behaviour

- All 36 instances resolve to real prefab assets on disk (GUID → file check
  passed). None are missing.
- `WaterPlane` instance (root `WaterPlane`, parent = Terrain transform
  1399221564) has local position (745.43, 283.52, −2452.00) → world
  (1264.6, 283.5, 530.0).
- The **8 toy instances and the WaterPlane share the identical world position
  (1264.6, 283.5, 530.0)**. All toy instances carry byte-identical transform
  overrides (169.76085, 271.73416, −2828.3384) — duplicated instances that
  were never repositioned. This is a **genuine scene data problem**: the toys
  are stacked at a single point ~2,400 units outside the play corridor
  (walls at z ≈ 2957–3957) and are unreachable. Resolution: user repositions
  in Unity (this pass does not reposition scene objects).
- `m_Modifications` on two instances reference target fileIDs
  (`7285457759098376123`, `1474031242315731251`) that do **not** exist inside
  the referenced prefab assets (Gulley/Valley/Lake) — these are stale
  overrides that Unity reports as warnings ("There are modifications on this
  GameObject that cannot be applied"). **Non-blocking.**

## 3. Scene hierarchy findings

```
Environment (519, 0, 2982)          ← all level content scaffold
├── Terrain                          (no Terrain component, no geometry)
│   └── WaterPlane [instance]        (MeshCollider + MeshRenderer; world 1264, 283.5, 530)
├── ScannedItems / CollectablesItems
│   ├── Toys / Toys2                 → 8 toy instances (stacked, see §2)
│   ├── Nature                       → 12 plant instances
│   ├── Bottles                      → 4 bottle instances
│   └── Bins                         → 3 bin instances
└── ObjectProp                       → Stairs [instance]
Player (592, 94.2, 2995)
  ├── PlayerCamera                   (local 0,1.5,0; culling mask = all)
  └── GroundCheck                    (local 0,−1,0)
Walls: Eastwall/Southwall/Westwall/Northwall  (built-in plane 10202, scale 1000)
```

**Collider inventory (scene-level, verified):** only 4 wall `BoxCollider`s +
the player's `CapsuleCollider`/`CharacterController`. No `MeshCollider` or
`TerrainCollider` inside the play volume. The only horizontal collider in the
project is the **WaterPlane prefab's** `MeshCollider` (a built-in quad), and its
instance sits at y ≈ 283.5 — *above and off* the play area.

**Conclusion:** the player spawns with **no walkable ground beneath** it. This
is a genuine blocker, not an artefact. Terrain data exists
(`Environment/Terrain/Soil.asset`, user's `New Terrain*.asset`) but no
`Terrain` component is applied in the scene. This pass deliberately does **not**
create placeholder floor geometry; the user is authoring terrain in Unity.

## 4. Rendering findings

- **URP is active.** `ProjectSettings/GraphicsSettings.asset` sets
  `m_CustomRenderPipeline` → `Assets/Settings/PC_RPAsset.asset`
  (guid `4b83569d67af61e458304325a23e5dfd`). The water surface uses the
  included Shader Graph `Environment/Water/Shaders/WaterDepthBased.shadergraph`.
- Scene renderers: 4 walls + Player cube (built-in meshes 10202/10208,
  Default material 2100000). No URP-vs-built-in mismatch on gameplay objects.
- Camera (PlayerCamera, comp 871825670): clear flags 1, culling mask all layers
  (`4294967295`) — nothing is culled.
- Directional light at (0,3,0) with a `MonoBehaviour` sun-rotation script;
  probe rigs (ReflectionProbe/LightProbeGroup) are named after the terrain
  regions they illuminate but carry **no geometry**.

## 5. GUID verification

Scanned every GUID reference in both scenes, resolving against `.meta` files
under `Assets/`, `Packages/`, and `Library/PackageCache/`.

- All 36 prefab sources: **resolved**.
- All project + package scripts (TMP `fe87c0e1…`, uGUI `4e29b1a8…`,
  Input System `e8794d9b…`): **resolved**.
- 4 unresolvable GUIDs, 3 of them benign (see table; `0b6d251b…` was
  subsequently resolved on 5 Aug):
  | GUID | Where | Verdict |
  | --- | --- | --- |
  | `0b6d251b…` | SoilPlane `Volume.m_MaterialTemplate` | **Resolved 5 Aug** — `TerrainLit.mat` copied into `Assets/Environment/Terrain/Materials/` (new guid `8e36950e…`); scene remapped, no longer a demo-folder reference |
  | `241f7368…` | SoilPlane `Volume.m_LightmapParameters` | cosmetic inspector-only ref |
  | `3d2422e4…` | PrefabInstance 7285457759098376123 override target | stale override warning |
  | `c7439120…` | PrefabInstance 1474031242315731251 override target | stale override warning |

No missing scripts, prefabs, or meshes in gameplay-critical paths.

## 6. Environment investigation

| Asset | What it actually is |
| --- | --- |
| `Environment/Terrain/Prefabs/Valley.prefab` | ReflectionProbe rig (no geometry/collider) |
| `Environment/Terrain/Prefabs/Gulley.prefab` | ReflectionProbe rig |
| `Environment/Terrain/Prefabs/Lake.prefab` | ReflectionProbe rig |
| `Environment/Terrain/Prefabs/Global.prefab` | ReflectionProbe rig |
| `Environment/Prefabs/Lighting/MorningSun.prefab` | Light + sun-rotation script |
| `Environment/Prefabs/Lighting/ExtentsLightProbes.prefab` | LightProbeGroup |
| `Environment/Prefabs/Lighting/MainLightProbes.prefab` | LightProbeGroup |
| `Environment/Terrain/Prefabs/WaterPlane.prefab` | MeshFilter+Renderer+MeshCollider (built-in quad), Water material |
| `Environment/Terrain/Soil.asset` | TerrainData (orphaned — no Terrain component uses it) |
| `Models/Prefabs/Prop/Stairs.prefab` + OBJ | Stairs model (scene uses the OBJ import directly) |

**The project contains no terrain geometry mesh.** Valley/Gulley/Lake are
lighting rigs, not geometry. Terrain must be authored in Unity (user task).

## 7. Visibility investigation

- **"Empty parents"** (Terrain, Reflections, LightProbes, ScannedItems
  containers): artefact — children are prefab instances (see §1, §2).
- **Invisible environment**: expected — no terrain geometry exists yet, and
  probe rigs render nothing by themselves.
- **HUD/UI panels**: inactive by design; wired at runtime by UIManager /
  HUDManager (with fallback text creation).
- **Duplicate PauseMenuManager** (comp 1854047160): genuine leftover; code
  (`AutoSetupPauseMenu.FindBestPauseMenuManager`) already prefers the wired
  instance (comp 1037489632). To be disabled/deleted in Phase 3.

## 8. Conclusions

1. The scene is **not broken at the reference level**: everything GUID-resolves;
   the earlier "missing scripts / empty environment" readings were serialisation
   artefacts.
2. The genuine, confirmed blockers are gameplay/content problems, not
   reference problems:
   - no colliders on items/bins (Phase 1),
   - wrong item layers (Phase 1),
   - no walkable ground (user terrain work),
   - score/lives/win logic (Phase 2),
   - UI wiring (Phase 3/4),
   - stacked, unreachable toys (user scene work).
3. No placeholder floor, no scene repositions, and no level-design changes are
   made by the automated passes.
