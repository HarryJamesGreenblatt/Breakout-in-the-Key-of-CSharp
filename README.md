# Breakout

A faithful recreation of Atari's classic *Breakout* (1976) in **Godot 4.5 with C#**, built as a hands-on learning exercise in 2D game development fundamentals and architectural patterns.

**Current Status:** Objective 1.1 complete (MVP playable) — Nystrom's Component Pattern fully implemented (January 5, 2026).

---

## Overview

This project follows a **pattern-driven learning approach** grounded in:
- **Godot 4.5 official documentation** (source of truth for engine behavior)
- **Robert Nystrom's *Game Programming Patterns*** (https://gameprogrammingpatterns.com/) — architectural guidance
- **2D Game Development Curriculum** (Branch A: Arcade Foundations) — design and progression

### Philosophy

Rather than providing complete code, this project **implements features iteratively**, discovering problems that make design patterns valuable. The goal is to understand *why* patterns exist, not just apply them blindly.

---

## What's Implemented

### MVP (Objective 1.1 Complete ✅)

- **Entities:** Paddle (player-controlled), Ball (physics-driven), Brick Grid (8×8, destructible), Walls (boundary)
- **Gameplay:** Ball bounces off walls, paddle, and bricks; bricks destroyed on contact; speed increases (4, 12 hit milestones) working; paddle shrinking working
- **Collision Detection:** Signal-based (`AreaEntered`/`AreaExited` events) instead of polling
- **Components:** PhysicsComponent (ball physics, speed multipliers), GameStateComponent (rules & state), BrickGrid (grid management in Infrastructure/), EntityFactoryUtility (factory in Utilities/)
- **Brick Destruction:** Ball collision → PhysicsComponent.HandleBrickCollision() → brick.Destroy() → BrickDestroyed signal → BrickGrid listener
- **Configuration:** Centralized in `Config.cs`; dynamic brick grid spacing
- **Brick Colors:** Type-safe enum with scoring metadata (Red=7pts, Orange=5pts, Green=3pts, Yellow=1pt)
- **Architecture:** Nystrom's Component Pattern — plain C# components own state & logic; thin entities forward events; pure signal wiring; direct component access

### Not Yet Implemented

- Scoring display UI
- Lives display UI
- Game state machine (Playing, GameOver, Level Complete)
- Level progression

---

## Architecture Notes

### Current Design: Nystrom's Component Pattern

**Pattern:** Component (plain C# classes owning state + logic) + Observer (Godot Signals + C# Events)

**Architecture Layers (Organized by Role):**

1. **Components/** (Business Logic) — Own state and logic, emit C# events:
   - `PhysicsComponent` — Ball velocity, position, collision tracking, speed multipliers, bounce logic
   - `GameStateComponent` — Score, lives, hit count, speed/shrink decision logic

2. **Infrastructure/** (World Structure) — Entity collections forming environment:
   - `BrickGrid` — Brick grid management and destruction tracking
   - `Walls` — Stateless boundary nodes

3. **Utilities/** (Helpers) — Factory and lookup functions:
   - `EntityFactoryUtility` — Factory for entity-component pair instantiation
   - `BrickColorUtility` — Color-to-config lookup

4. **Entities/** (Node2D Containers) — Forward component events to Godot signals:
   - `Ball` — Delegates to PhysicsComponent; emits `BallHitPaddle`, `BallOutOfBounds`, `BallHitCeiling` signals
   - `Paddle` — Input handling, movement bounds, `Shrink()` action method
   - `Brick` — Has `Destroy()` method that emits `BrickDestroyed` signal and removes entity

5. **Game/** (Orchestration + Config):
   - `Controller` — Pure signal wiring; zero business logic; instantiates via EntityFactoryUtility
   - `Config` — Centralized constants

**Signal Flow Example:**
```
Ball hits brick → PhysicsComponent.HandleBrickCollision()
                  ├─ bounces ball (velocity.Y = -velocity.Y)
                  └─ calls brick.Destroy() → emits BrickDestroyed signal
                     ↓
                  BrickGrid listens → OnBrickDestroyed()
                  ├─ removes brick from dictionary
                  └─ emits BrickDestroyedWithColor(color)
                     ↓
                  GameStateComponent.OnBrickDestroyed() checks milestones
                  ├─ Emits SpeedIncreaseRequired → Controller wires DIRECTLY to PhysicsComponent.ApplySpeedMultiplier()
                  │  (no indirection through Ball!)
                  └─ Emits PaddleShrinkRequired → Controller wires to Paddle.Shrink()
```

**Rationale:** Per Nystrom's Component pattern, components own state + behavior. Entities are thin containers. Controller is mechanical wiring. Zero state duplication. Each concern has exactly one owner.

---

## Development Workflow

### Build & Run

```bash
# Build the project
dotnet build

# Run in Godot editor
# Scene: main.tscn (auto-loaded)
```

### Key Files (Organized by Folder)

**Game/**
- **Controller.cs** — Pure signal wiring and instantiation via EntityFactoryUtility; zero business logic
- **Config.cs** — All constants; dynamic layout logic

**Components/**
- **PhysicsComponent.cs** — Ball physics, collision tracking, speed multipliers; calls brick.Destroy() on collision
- **GameStateComponent.cs** — Game rules, score, lives, speed/shrink logic; emits events for Controller to wire

**Infrastructure/**
- **BrickGrid.cs** — Brick grid management and destruction tracking; listens to Brick.BrickDestroyed signals
- **Walls.cs** — Boundary walls (positioned outside viewport)

**Utilities/**
- **EntityFactoryUtility.cs** — Factory for entity-component pair instantiation; creates all game entities
- **BrickColorUtility.cs** — Color-to-config lookup (Red/Orange/Green/Yellow)

**Entities/**
- **Ball.cs** — Thin entity; delegates physics to PhysicsComponent; exposes component via GetPhysicsComponent()
- **Paddle.cs** — Input handling, movement, shrink action
- **Brick.cs** — Has Destroy() method that emits BrickDestroyed signal and removes entity from scene

### Git Workflow

Commits follow conventional format:
```
feat: new feature (e.g., "implement brick grid collision")
fix: bug fix
refactor: structural improvement
docs: documentation
```

---

## Objectives Roadmap

### Objective 1.1: Scene Structure & Component Architecture ✅
- Scene hierarchy (Controller → Entities)
- Frame-by-frame update loop
- Component-based architecture with state centralization
- Zero state duplication
- Pure signal wiring
- **Status:** Complete; MVP playable with solid architecture (January 5 refactor)

### Objective 1.2: Paddle Control ✅
- Keyboard input (arrow keys)
- Bounds checking
- Smooth movement

### Objective 1.3: Ball Physics (Basic) ✅
- Constant velocity with bouncing
- Wall collisions
- Paddle collisions
- Brick collisions (recently added)

### Objective 2.1: Scoring & Game State (Next)
- Hit counter and speed increases
- Scoring system (color-based points)
- Lives system
- Level complete detection

### Objective 2.2: Game State Machine (After scoring)
- States: Waiting, Playing, Game Over, Level Complete
- Transitions triggered by game events

---

## Design References

### Canonical Breakout (Original Arcade, 1976)

**Grid:** 8 rows × variable columns (2 rows each color)
**Colors (bottom to top):** Yellow (1pt), Green (3pts), Orange (5pts), Red (7pts)
**Ball Speed Increases:** After 4 hits, 12 hits, and hitting orange/red rows
**Paddle:** Shrinks to 50% after ball breaks through red row
**Scoring:** Max 864 points (2 screens × 432 each)
**Lives:** 3 turns to clear 2 screens

**Reference:** https://en.wikipedia.org/wiki/Breakout_(video_game)#Gameplay

---

## Patterns Applied (Nystrom)

| Pattern | Link | Usage |
|---------|------|-------|
| Update Method | https://gameprogrammingpatterns.com/update-method.html | Entity `_Process()` updates |
| Game Loop | https://gameprogrammingpatterns.com/game-loop.html | Controller drives loop |
| Observer | https://gameprogrammingpatterns.com/observer.html | C# events & Godot Signals |
| Component | https://gameprogrammingpatterns.com/component.html | PhysicsComponent, GameStateComponent (business logic components) |
| Factory | https://gameprogrammingpatterns.com/factory-method.html | EntityFactoryUtility creates entity-component pairs |
| Object Pool | https://gameprogrammingpatterns.com/object-pool.html | Brick grid Dictionary |

---

## Recent Improvements

### Session 2: Full Component Pattern Refinement (January 5, 2026)

**Phase 1-6:** Refactored from "signals only" to true **Nystrom's Component Pattern**:
- Created `PhysicsComponent` (ball physics, collision tracking, speed multipliers)
- Created `GameStateComponent` (game rules, score, lives, speed/shrink logic)
- Created `BrickGridComponent` (grid management, destruction handling)
- Renamed `GameOrchestrator` → `Controller` (accurate naming: dumb wiring)
- Eliminated all state duplication (e.g., shrink logic was in 2 places, now in 1)
- Controller reduced from 100+ lines with decisions to ~50 lines of pure wiring

**Phase 7:** Eliminated unnecessary pass-through method indirection:
- Discovered `Ball.ApplySpeedMultiplier()` was a pass-through to `PhysicsComponent`
- Violated principle: behavior owner (PhysicsComponent) should be wired directly
- Created `EntityComponent.CreateBallWithPhysics()` returning `(Ball, PhysicsComponent)` tuple
- Updated Controller to wire `SpeedIncreaseRequired` directly to `PhysicsComponent.ApplySpeedMultiplier()`
- Removed `Ball.ApplySpeedMultiplier()` pass-through; added `Ball.GetPhysicsComponent()`

**Phase 8:** Reorganized folder structure by architectural role (January 6):
- Reclassified components by role instead of mixing all in one folder
  - **Components/** = business logic (PhysicsComponent, GameStateComponent, future Sound/Rendering/AI)
  - **Infrastructure/** = world structures (Walls, BrickGrid, future LevelLayout/Environment)
  - **Utilities/** = helper functions (EntityFactoryUtility, BrickColorUtility)
  - **Game/** = orchestration (Controller) + config (Config)
- Renamed `EntityComponent` → `EntityFactoryUtility` (factory utility, not component)
- Renamed `BrickGridComponent` → `BrickGrid` (infrastructure, not business logic component)
- Updated all usages to reflect new locations

**Phase 9:** Fixed brick destruction bug:
- Discovered bricks weren't being destroyed on collision
- Root cause: `PhysicsComponent.HandleBrickCollision()` only bounced ball, never destroyed brick
- Added `Brick.Destroy()` method that emits `BrickDestroyed` signal and calls `QueueFree()`
- Updated `HandleBrickCollision()` to call `brick.Destroy()` after bouncing
- Signal flow now works: Brick.Destroy() → BrickGrid listener → emits BrickDestroyedWithColor → GameState rules applied

**Result:**
- ✅ Components own state AND logic (not just data containers)
- ✅ Entities are thin; they forward events only
- ✅ Controller is mechanical signal wiring only (zero state, zero decisions)
- ✅ **No indirection**: all signals wired directly to behavior owners
- ✅ Folder organization reflects architectural intent (Components = logic, Infrastructure = structures, Utilities = helpers)
- ✅ Brick destruction works completely: hit → destroy → signal → rules applied
- ✅ Speed multipliers persist across ball resets
- ✅ Paddle shrinks exactly once (via flag guard)
- ✅ No state polling; pure event-driven
- ✅ **Architecture is solid, complete, and ready for feature development**

---

## Next Session: Objective 2.1 (Scoring & Game State UI)

Architecture is solid and ready for feature addition. No refactoring needed.

### Recommended Tasks
1. Create UI layer (CanvasLayer with labels)
2. Bind `ScoreChanged` event to score label
3. Bind `LivesChanged` event to lives label
4. Implement game-over detection when lives reach 0
5. Test speed increases and paddle shrinking
6. **Time:** 2-3 hours | **Benefit:** Game is playable and testable

**Why architecture is ready:** Components emit all necessary events (ScoreChanged, LivesChanged, BallOutOfBounds). UI can simply listen without any architectural changes.

---

## Resources

- **Godot 4.5 Docs:** https://docs.godotengine.org/en/4.5/
- **Game Programming Patterns:** https://gameprogrammingpatterns.com/
- **2D Game Development Curriculum:** See `.context/A Curriculum for 2D Game Development Mastery.md`
- **Development Log:** See `.context/OBJECTIVE_1_1_DEVLOG.md` (not committed; local reference)

---

## Notes for Contributors (or Future Self)

- **DRY Principle:** Configuration is computation-based. Changing `ViewportWidth` or `GridColumns` in GameConfig auto-scales brick layout.
- **Signal Convention:** Signals emit meaningful names (e.g., `BallHitPaddle`), not generic notifications.
- **No Editor Scenes:** All structure defined in C# code, not Godot editor, to deepen understanding of node relationships.
- **Testing:** Currently manual (play in editor). As complexity grows, consider unit tests for physics logic.

---

## License & Attribution

This project is an educational implementation. Breakout is a trademark of Atari, Inc. This code is for learning purposes.

**Learning Resources Credited:**
- Robert Nystrom (Game Programming Patterns)
- Godot Foundation (Engine & Documentation)
- Atari, Inc. (Original Breakout Design)
